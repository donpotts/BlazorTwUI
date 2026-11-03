using Bunit;
using BlazorTwUI.Components.UI;
using BlazorTwUI.Models;
using BlazorTwUI.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BlazorTwUI.Tests;

public class AgentConsoleTests : TestContext
{
    public AgentConsoleTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IAgentTransport>(new SimulatedAgentTransport { StepDelay = TimeSpan.FromMilliseconds(20) });
    }

    private IRenderedComponent<TwAgentConsole> Render()
    {
        var cut = RenderComponent<TwAgentConsole>();
        cut.Find("[data-testid=run]").Click();
        return cut;
    }

    [Fact]
    public void RunButton_IsDisabled_WhileRunStarts_AndPromptIsLocked()
    {
        var cut = Render();
        cut.WaitForAssertion(() => Assert.True(cut.Find("[data-testid=prompt]").HasAttribute("disabled")));
        Assert.True(cut.Find("[data-testid=run]").HasAttribute("disabled"));
        cut.Find("[data-testid=cancel]").Click();
    }

    [Fact]
    public void HighRiskCall_ShowsApprovalGate_AndApproveCompletesTheRefund()
    {
        var cut = Render();

        cut.WaitForElement("[data-testid=approval-gate]", TimeSpan.FromSeconds(5));
        Assert.Equal("Waiting for your approval", cut.Find("[data-testid=run-status]").TextContent);
        Assert.Equal("proposed", StateValue(cut, "refund.status"));

        cut.Find("[data-testid=approve]").Click();

        cut.WaitForAssertion(() => Assert.Equal("Finished", cut.Find("[data-testid=run-status]").TextContent), TimeSpan.FromSeconds(5));
        Assert.Empty(cut.FindAll("[data-testid=approval-gate]"));
        Assert.Equal("issued", StateValue(cut, "refund.status"));
        Assert.Contains(cut.FindAll("[data-testid=tool-call]"), e => e.GetAttribute("data-tool") == "send_email");
    }

    [Fact]
    public void Reject_WithReason_ReroutesAgent_AndRecordsHumanDecisionInAudit()
    {
        var cut = Render();
        cut.WaitForElement("[data-testid=approval-gate]", TimeSpan.FromSeconds(5));

        cut.Find("input[id^=gate-reason-]").Input("over limit");
        cut.Find("[data-testid=reject]").Click();

        cut.WaitForAssertion(() => Assert.Equal("Finished", cut.Find("[data-testid=run-status]").TextContent), TimeSpan.FromSeconds(5));
        Assert.Equal("blocked", StateValue(cut, "refund.status"));
        Assert.DoesNotContain(cut.FindAll("[data-testid=tool-call]"), e => e.GetAttribute("data-tool") == "send_email");
        Assert.Contains(cut.FindAll("[data-testid=audit-row]"), r => r.GetAttribute("data-action") == "tool.reject" && r.TextContent.Contains("over limit"));
    }

    [Fact]
    public void InvalidJson_ShowsError_AndKeepsAgentBlocked()
    {
        var cut = Render();
        cut.WaitForElement("[data-testid=approval-gate]", TimeSpan.FromSeconds(5));

        cut.Find("textarea[id^=gate-args-]").Input("{oops");
        cut.Find("[data-testid=approve]").Click();

        Assert.Contains("valid JSON", cut.Find("[role=alert]").TextContent);
        Assert.Single(cut.FindAll("[data-testid=approval-gate]"));
        Assert.Equal("Waiting for your approval", cut.Find("[data-testid=run-status]").TextContent);
        cut.Find("[data-testid=cancel]").Click();
    }

    [Fact]
    public void StateInspector_IsReadOnlyUntilTakeover_ThenEditsAreWrittenToState()
    {
        var cut = Render();
        cut.WaitForElement("[data-testid=approval-gate]", TimeSpan.FromSeconds(5));

        // At the gate the human is already in control, so the inspector is editable.
        var session = cut.Instance.Session;
        cut.Find("[data-testid=state-row][data-key='refund.amount'] input").Change("60.00");

        cut.WaitForAssertion(() => Assert.Equal("60.00", session.State["refund.amount"]));
        cut.WaitForAssertion(() => Assert.Contains(cut.FindAll("[data-testid=audit-row]"), r => r.GetAttribute("data-action") == "state.edit"));
        cut.Find("[data-testid=cancel]").Click();
    }

    [Fact]
    public void AuditFilter_NarrowsRowsToTheChosenActor()
    {
        var cut = Render();
        cut.WaitForElement("[data-testid=approval-gate]", TimeSpan.FromSeconds(5));
        cut.Find("[data-testid=reject]").Click();
        cut.WaitForAssertion(() => Assert.Equal("Finished", cut.Find("[data-testid=run-status]").TextContent), TimeSpan.FromSeconds(5));

        cut.WaitForAssertion(() => Assert.Contains(cut.FindAll("[data-testid=audit-row]"), r => r.GetAttribute("data-action") == "run.finish"));
        var all = cut.FindAll("[data-testid=audit-row]").Count;

        cut.Find("[data-testid=audit-filter]").Change("Human");

        cut.WaitForAssertion(() =>
        {
            var rows = cut.FindAll("[data-testid=audit-row]");
            Assert.True(rows.Count > 0 && rows.Count < all, $"rows={rows.Count} all={all}");
            Assert.All(rows, r => Assert.Equal("Human", r.GetAttribute("data-actor")));
        });
    }

    [Fact]
    public void AuditExport_InvokesDownloadWithTheJsonTrail()
    {
        var cut = Render();
        cut.WaitForElement("[data-testid=approval-gate]", TimeSpan.FromSeconds(5));
        cut.Find("[data-testid=reject]").Click();
        cut.WaitForAssertion(() => Assert.Equal("Finished", cut.Find("[data-testid=run-status]").TextContent), TimeSpan.FromSeconds(5));

        // Wait for the audit panel itself to show the final event, so we click the button it will keep.
        cut.WaitForAssertion(() => Assert.Contains(cut.FindAll("[data-testid=audit-row]"), r => r.GetAttribute("data-action") == "run.finish"));
        cut.Find("[data-testid=audit-export]").Click();

        var call = JSInterop.Invocations["pdfHelper.downloadText"].Single();
        Assert.EndsWith(".json", (string)call.Arguments[0]!);
        Assert.Equal("application/json", call.Arguments[1]);
        Assert.Contains("tool.reject", (string)call.Arguments[2]!);
    }

    [Fact]
    public void ApprovalGate_UsesAccessibleNamesForItsControls()
    {
        var cut = Render();
        cut.WaitForElement("[data-testid=approval-gate]", TimeSpan.FromSeconds(5));

        var gate = cut.Find("[data-testid=approval-gate]");
        var labelledBy = gate.GetAttribute("aria-labelledby")!;
        Assert.Contains("issue_refund", cut.Find($"#{labelledBy}").TextContent);
        foreach (var field in cut.FindAll("[data-testid=approval-gate] textarea, [data-testid=approval-gate] input"))
            Assert.NotEmpty(cut.FindAll($"label[for='{field.Id}']"));
        cut.Find("[data-testid=cancel]").Click();
    }

    private static string StateValue(IRenderedComponent<TwAgentConsole> cut, string key)
    {
        var row = cut.Find($"[data-testid=state-row][data-key='{key}']");
        var input = row.QuerySelector("input");
        return input?.GetAttribute("value") ?? row.QuerySelector("[data-testid=state-value]")!.TextContent;
    }
}
