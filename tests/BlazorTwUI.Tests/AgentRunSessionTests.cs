using BlazorTwUI.Models;
using BlazorTwUI.Services;
using Xunit;

namespace BlazorTwUI.Tests;

public class AgentRunSessionTests
{
    private static AgentRunSession NewSession() =>
        new(new SimulatedAgentTransport { StepDelay = TimeSpan.Zero });

    private static async Task WaitUntil(Func<bool> condition)
    {
        var deadline = DateTime.UtcNow.AddSeconds(5);
        while (!condition())
        {
            if (DateTime.UtcNow > deadline) throw new TimeoutException("Condition not reached.");
            await Task.Delay(5);
        }
    }

    private static ToolCallRecord Refund(AgentRunSession s) =>
        s.Timeline.OfType<ToolCallRecord>().Single(t => t.Name == "issue_refund");

    [Fact]
    public async Task HighRiskCall_BlocksUntilApproved_ThenExecutes()
    {
        var s = NewSession();
        var run = s.RunAsync("refund order 4821");

        await WaitUntil(() => s.Status == AgentRunStatus.AwaitingApproval);
        await Task.Delay(50);

        // Still blocked: no result, no side effect on shared state.
        Assert.False(run.IsCompleted);
        Assert.Null(Refund(s).Result);
        Assert.Equal("proposed", s.State["refund.status"]);

        s.Approve(Refund(s).Id);
        await run;

        Assert.Equal(AgentRunStatus.Finished, s.Status);
        Assert.Equal("issued", s.State["refund.status"]);
        Assert.Contains("issued", Refund(s).Result);
        Assert.Contains(s.Timeline.OfType<ToolCallRecord>(), t => t.Name == "send_email");
    }

    [Fact]
    public async Task Rejection_PreventsRefund_AndReroutesToTicket()
    {
        var s = NewSession();
        var run = s.RunAsync("refund order 4821");
        await WaitUntil(() => s.Status == AgentRunStatus.AwaitingApproval);

        s.Reject(Refund(s).Id, "over policy limit");
        await run;

        Assert.Equal("blocked", s.State["refund.status"]);
        Assert.Equal(ToolCallStatus.Rejected, Refund(s).Status);
        Assert.Contains("over policy limit", Refund(s).Result);
        Assert.DoesNotContain(s.Timeline.OfType<ToolCallRecord>(), t => t.Name == "send_email");
        Assert.Contains(s.Timeline.OfType<ToolCallRecord>(), t => t.Name == "create_ticket");
    }

    [Fact]
    public async Task EditedApprovalArguments_ChangeTheExecutedAmount()
    {
        var s = NewSession();
        var run = s.RunAsync("refund order 4821");
        await WaitUntil(() => s.Status == AgentRunStatus.AwaitingApproval);

        s.Approve(Refund(s).Id, "{\"orderId\":\"4821\",\"amount\":\"120.00\"}");
        await run;

        Assert.Equal("120.00", s.State["refund.amount"]);
        Assert.Contains("$120.00", Refund(s).Result);
        Assert.Contains(s.Audit, a => a.Action == "tool.approve" && a.Detail.Contains("edited arguments"));
    }

    [Fact]
    public async Task InvalidEditedArguments_AreRefused_AndRunStaysBlocked()
    {
        var s = NewSession();
        var run = s.RunAsync("refund order 4821");
        await WaitUntil(() => s.Status == AgentRunStatus.AwaitingApproval);

        Assert.Throws<ArgumentException>(() => s.Approve(Refund(s).Id, "{not json"));
        Assert.Equal(AgentRunStatus.AwaitingApproval, s.Status);

        s.Reject(Refund(s).Id);
        await run;
    }

    [Fact]
    public async Task Takeover_PausesAgent_AndHumanStateEditFlowsIntoTheNextAction()
    {
        var s = new AgentRunSession(new SimulatedAgentTransport { StepDelay = TimeSpan.FromMilliseconds(25) });
        var run = s.RunAsync("refund order 4821");

        // The lookup delta lands first; the refund is proposed several steps later.
        await WaitUntil(() => s.State.ContainsKey("refund.amount"));
        s.TakeOver();
        await WaitUntil(() => s.Status == AgentRunStatus.Paused);

        var countWhilePaused = s.Timeline.Count;
        await Task.Delay(150);
        Assert.Equal(countWhilePaused, s.Timeline.Count); // agent produced nothing while paused
        Assert.DoesNotContain(s.Timeline.OfType<ToolCallRecord>(), t => t.Name == "issue_refund");

        s.EditState("refund.amount", "75.00");
        s.HandBack();
        await WaitUntil(() => s.Status == AgentRunStatus.AwaitingApproval);
        Assert.Contains("75.00", Refund(s).ArgsJson);

        s.Approve(Refund(s).Id);
        await run;
        Assert.Equal(AgentRunStatus.Finished, s.Status);
        Assert.Equal("75.00", s.State["refund.amount"]);
    }

    [Fact]
    public async Task StateEdits_AreRejected_WhenHumanIsNotInControl()
    {
        var s = NewSession();
        var run = s.RunAsync("refund order 4821");
        await WaitUntil(() => s.Status == AgentRunStatus.AwaitingApproval);
        s.HandBack(); // not taken over: no-op

        // Awaiting approval counts as human control, so editing is allowed here.
        s.EditState("refund.amount", "10.00");
        s.Reject(Refund(s).Id);
        await run;

        Assert.Throws<InvalidOperationException>(() => s.EditState("refund.amount", "1.00")); // run is over
    }

    [Fact]
    public async Task Cancel_StopsTheRun_AndFailsTheBlockedCall()
    {
        var s = NewSession();
        var run = s.RunAsync("refund order 4821");
        await WaitUntil(() => s.Status == AgentRunStatus.AwaitingApproval);

        s.Cancel();
        await run;

        Assert.Equal(AgentRunStatus.Cancelled, s.Status);
        Assert.Equal(ToolCallStatus.Failed, Refund(s).Status);
        Assert.Equal("proposed", s.State["refund.status"]);
    }

    [Fact]
    public async Task AuditTrail_RecordsActorsInOrder_AndExportsJson()
    {
        var s = NewSession();
        var run = s.RunAsync("refund order 4821");
        await WaitUntil(() => s.Status == AgentRunStatus.AwaitingApproval);
        s.Reject(Refund(s).Id, "fraud check");
        await run;

        Assert.Equal(Enumerable.Range(1, s.Audit.Count), s.Audit.Select(a => a.Seq));
        Assert.Equal("run.start", s.Audit[0].Action);
        Assert.Contains(s.Audit, a => a is { Actor: AuditActor.Human, Action: "tool.reject" });
        Assert.Contains(s.Audit, a => a is { Actor: AuditActor.Agent, Action: "tool.propose" });

        var json = s.ExportAuditJson();
        Assert.Contains("\"action\": \"tool.reject\"", json);
        Assert.Contains("fraud check", json);
    }

    [Fact]
    public async Task SecondRun_IsRefusedWhileActive_AndStateResetsBetweenRuns()
    {
        var s = NewSession();
        var run = s.RunAsync("first");
        await WaitUntil(() => s.Status == AgentRunStatus.AwaitingApproval);
        await Assert.ThrowsAsync<InvalidOperationException>(() => s.RunAsync("second"));

        s.Reject(Refund(s).Id);
        await run;
        await s.RunAsync2Safe();
    }
}

internal static class SessionTestExtensions
{
    public static async Task RunAsync2Safe(this AgentRunSession s)
    {
        var run = s.RunAsync("again");
        var deadline = DateTime.UtcNow.AddSeconds(5);
        while (s.Status != AgentRunStatus.AwaitingApproval && DateTime.UtcNow < deadline) await Task.Delay(5);
        Assert.Equal("proposed", s.State["refund.status"]); // fresh run, not leftover "blocked"
        s.Cancel();
        await run;
    }
}
