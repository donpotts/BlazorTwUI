using System.Net;
using System.Text;
using System.Text.Json;
using BlazorTwUI.Models;
using BlazorTwUI.Services;
using Bunit;
using Xunit;

namespace BlazorTwUI.Tests;

public class SseParserTests
{
    private static async Task<List<SseMessage>> Parse(string text)
    {
        using var ms = new MemoryStream(Encoding.UTF8.GetBytes(text));
        var list = new List<SseMessage>();
        await foreach (var m in SseParser.ReadAsync(ms)) list.Add(m);
        return list;
    }

    [Fact]
    public async Task Frames_AreSplitOnBlankLines_AndKeepEventNames()
    {
        var frames = await Parse("event: run\ndata: {\"a\":1}\n\ndata: two\n\n");

        Assert.Equal(2, frames.Count);
        Assert.Equal("run", frames[0].Event);
        Assert.Equal("{\"a\":1}", frames[0].Data);
        Assert.Null(frames[1].Event);
        Assert.Equal("two", frames[1].Data);
    }

    [Fact]
    public async Task MultipleDataLines_AreJoinedWithNewlines()
    {
        var frames = await Parse("data: line1\ndata: line2\n\n");
        Assert.Equal("line1\nline2", Assert.Single(frames).Data);
    }

    [Fact]
    public async Task CommentsAndKeepAlives_AreIgnored()
    {
        var frames = await Parse(": heartbeat\n\n: another\ndata: real\n\n");
        Assert.Equal("real", Assert.Single(frames).Data);
    }

    [Fact]
    public async Task CrLfLineEndings_AndMissingFinalBlankLine_AreHandled()
    {
        var frames = await Parse("data: x\r\n\r\ndata: last");
        Assert.Equal(new[] { "x", "last" }, frames.Select(f => f.Data));
    }
}

public class AgUiWireMapperTests
{
    private static List<AgUiEvent> Map(AgUiWireMapper m, string json) => m.Map(json).ToList();

    [Fact]
    public void ToolCall_CarriesRiskAndApprovalFlags()
    {
        var evt = Assert.Single(Map(new AgUiWireMapper(),
            "{\"type\":\"TOOL_CALL_START\",\"toolCallId\":\"c1\",\"toolCallName\":\"issue_refund\",\"risk\":\"high\",\"requiresApproval\":true}"));

        Assert.Equal(AgUiEventType.ToolCallStart, evt.Type);
        Assert.Equal("c1", evt.Id);
        Assert.Equal("issue_refund", evt.Name);
        Assert.Equal(ToolRisk.High, evt.Risk);
        Assert.True(evt.RequiresApproval);
    }

    [Fact]
    public void StandardAgUiToolCall_WithoutExtensions_IsLowRiskAndAutoApproved()
    {
        var evt = Assert.Single(Map(new AgUiWireMapper(), "{\"type\":\"TOOL_CALL_START\",\"toolCallId\":\"c\",\"toolCallName\":\"x\"}"));
        Assert.Equal(ToolRisk.Low, evt.Risk);
        Assert.False(evt.RequiresApproval);
    }

    [Fact]
    public void TextArgsAndResult_MapToTheirEvents()
    {
        var m = new AgUiWireMapper();
        Assert.Equal("Hi", Assert.Single(Map(m, "{\"type\":\"TEXT_MESSAGE_CONTENT\",\"messageId\":\"m\",\"delta\":\"Hi\"}")).Text);
        Assert.Equal("{\"a\"", Assert.Single(Map(m, "{\"type\":\"TOOL_CALL_ARGS\",\"toolCallId\":\"c\",\"delta\":\"{\\\"a\\\"\"}")).Text);
        Assert.Equal("done", Assert.Single(Map(m, "{\"type\":\"TOOL_CALL_RESULT\",\"toolCallId\":\"c\",\"content\":\"done\"}")).Text);
    }

    [Fact]
    public void StateSnapshot_ThenDelta_ReportsOnlyWhatChanged()
    {
        var m = new AgUiWireMapper();
        var snap = Assert.Single(Map(m, "{\"type\":\"STATE_SNAPSHOT\",\"snapshot\":{\"refund.status\":\"proposed\",\"order\":4821}}"));
        Assert.Equal("proposed", snap.State!["refund.status"]);
        Assert.Equal("4821", snap.State["order"]);

        var delta = Assert.Single(Map(m,
            "{\"type\":\"STATE_DELTA\",\"delta\":[{\"op\":\"replace\",\"path\":\"/refund.status\",\"value\":\"issued\"},{\"op\":\"replace\",\"path\":\"/order\",\"value\":4821}]}"));
        Assert.Equal("issued", delta.State!["refund.status"]);
        Assert.False(delta.State.ContainsKey("order")); // unchanged value is not re-reported
    }

    [Fact]
    public void RunError_AndRunFinished_Map()
    {
        var m = new AgUiWireMapper();
        Assert.Equal("boom", Assert.Single(Map(m, "{\"type\":\"RUN_ERROR\",\"message\":\"boom\"}")).Text);
        Assert.Equal(AgUiEventType.RunFinished, Assert.Single(Map(m, "{\"type\":\"RUN_FINISHED\"}")).Type);
    }

    [Fact]
    public void UnrenderedEventTypes_ProduceNothing()
    {
        Assert.Empty(Map(new AgUiWireMapper(), "{\"type\":\"TEXT_MESSAGE_START\",\"messageId\":\"m\"}"));
        Assert.Empty(Map(new AgUiWireMapper(), "{\"type\":\"STEP_STARTED\",\"stepName\":\"s\"}"));
    }
}

public class SseAgentTransportTests
{
    private sealed class FakeServer : HttpMessageHandler
    {
        public List<(string Url, string Body)> Posts { get; } = new();
        public string Stream { get; init; } = "";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var body = request.Content is null ? "" : await request.Content.ReadAsStringAsync(ct);
            Posts.Add((request.RequestUri!.AbsolutePath, body));
            if (request.RequestUri.AbsolutePath == "/run")
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(Stream, Encoding.UTF8, "text/event-stream") };
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
    }

    private static string Sse(params string[] json) => string.Concat(json.Select(j => $"data: {j}\n\n"));

    private const string GatedRun =
        "{\"type\":\"RUN_STARTED\"}," +
        "{\"type\":\"TEXT_MESSAGE_CONTENT\",\"delta\":\"Working. \"}," +
        "{\"type\":\"TOOL_CALL_START\",\"toolCallId\":\"c1\",\"toolCallName\":\"issue_refund\",\"risk\":\"high\",\"requiresApproval\":true}," +
        "{\"type\":\"TOOL_CALL_ARGS\",\"toolCallId\":\"c1\",\"delta\":\"{\\\"amount\\\":\\\"249.00\\\"}\"}," +
        "{\"type\":\"TOOL_CALL_END\",\"toolCallId\":\"c1\"}," +
        "{\"type\":\"TOOL_CALL_RESULT\",\"toolCallId\":\"c1\",\"content\":\"refunded\"}," +
        "{\"type\":\"RUN_FINISHED\"}";

    private static string[] Frames(string joined) =>
        joined.Replace("},{", "}\u0001{").Split('\u0001');

    [Fact]
    public async Task GatedRun_StopsAtTheGate_ThenReportsTheEditedDecisionToTheServer()
    {
        var server = new FakeServer { Stream = Sse(Frames(GatedRun)) };
        var transport = new SseAgentTransport(new HttpClient(server) { BaseAddress = new Uri("http://agent.test") },
            new SseAgentTransportOptions { RunUrl = "/run", DecisionUrl = "/decision", ThreadId = "t1" });
        var session = new AgentRunSession(transport);

        var run = session.RunAsync("refund order 4821");
        SpinWait.SpinUntil(() => session.PendingApprovals.Any(), TimeSpan.FromSeconds(5));
        Assert.Equal(AgentRunStatus.AwaitingApproval, session.Status);
        Assert.DoesNotContain(server.Posts, p => p.Url == "/decision"); // nothing reported while blocked

        session.Approve("c1", "{\"amount\":\"120.00\"}");
        await run;

        Assert.Equal(AgentRunStatus.Finished, session.Status);
        var decision = Assert.Single(server.Posts, p => p.Url == "/decision");
        using var doc = JsonDocument.Parse(decision.Body);
        Assert.True(doc.RootElement.GetProperty("approved").GetBoolean());
        Assert.Equal("c1", doc.RootElement.GetProperty("toolCallId").GetString());
        Assert.Equal("{\"amount\":\"120.00\"}", doc.RootElement.GetProperty("editedArgs").GetString());
        Assert.Equal("t1", doc.RootElement.GetProperty("threadId").GetString());

        var runBody = JsonDocument.Parse(server.Posts[0].Body).RootElement;
        Assert.Equal("refund order 4821", runBody.GetProperty("messages")[0].GetProperty("content").GetString());
    }

    [Fact]
    public async Task Rejection_IsReportedWithItsReason()
    {
        var server = new FakeServer { Stream = Sse(Frames(GatedRun)) };
        var transport = new SseAgentTransport(new HttpClient(server) { BaseAddress = new Uri("http://agent.test") },
            new SseAgentTransportOptions { RunUrl = "/run", DecisionUrl = "/decision" });
        var session = new AgentRunSession(transport);

        var run = session.RunAsync("go");
        SpinWait.SpinUntil(() => session.PendingApprovals.Any(), TimeSpan.FromSeconds(5));
        session.Reject("c1", "over policy limit");
        await run;

        using var doc = JsonDocument.Parse(Assert.Single(server.Posts, p => p.Url == "/decision").Body);
        Assert.False(doc.RootElement.GetProperty("approved").GetBoolean());
        Assert.Equal("over policy limit", doc.RootElement.GetProperty("reason").GetString());
    }

    [Fact]
    public async Task NonSuccessResponse_FailsTheRunWithAReadableError()
    {
        var handler = new DelegateHandler(_ => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
        var session = new AgentRunSession(new SseAgentTransport(new HttpClient(handler) { BaseAddress = new Uri("http://agent.test") },
            new SseAgentTransportOptions { RunUrl = "/run" }));

        await session.RunAsync("go");

        Assert.Equal(AgentRunStatus.Failed, session.Status);
        Assert.Contains("503", session.Error);
    }

    private sealed class DelegateHandler(Func<HttpRequestMessage, HttpResponseMessage> f) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) => Task.FromResult(f(request));
    }
}

public class AgentThreadStoreTests : TestContext
{
    [Fact]
    public void FirstUserMessage_NamesTheThread_AndUpdatesPreview()
    {
        var store = new AgentThreadStore();
        var t = store.Create();
        store.Append(t.Id, "user", "Refund order 4821 please");
        store.Append(t.Id, "assistant", "Done.");

        Assert.Equal("Refund order 4821 please", t.Title);
        Assert.Equal("Done.", t.Preview);
        Assert.Equal(2, t.Messages.Count);
    }

    [Fact]
    public void Pinned_SortFirst_ThenByRecency()
    {
        var store = new AgentThreadStore();
        var a = store.Create("a");
        var b = store.Create("b");
        store.Append(a.Id, "user", "x");
        store.SetPinned(b.Id, true);

        Assert.Equal(new[] { "b", "a" }, store.All.Select(t => t.Title));
    }

    [Fact]
    public void RenameArchiveDelete_Work_AndBlankTitleIsRejected()
    {
        var store = new AgentThreadStore();
        var t = store.Create("old");
        store.Rename(t.Id, "  ");
        Assert.Equal("old", t.Title);
        store.Rename(t.Id, "new");
        store.SetArchived(t.Id, true);
        Assert.True(store.Get(t.Id)!.Archived);
        store.Delete(t.Id);
        Assert.Null(store.Get(t.Id));
    }

    [Fact]
    public void ExportThenImport_RoundTrips_WithoutDuplicates()
    {
        var a = new AgentThreadStore();
        var t = a.Create();
        a.Append(t.Id, "user", "hello");

        var b = new AgentThreadStore();
        b.ImportJson(a.ExportJson());
        b.ImportJson(a.ExportJson());

        var copy = Assert.Single(b.All);
        Assert.Equal(t.Id, copy.Id);
        Assert.Equal("hello", copy.Messages[0].Text);
    }

    [Fact]
    public async Task Changes_AreMirroredToLocalStorage_AndReloaded()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var store = new AgentThreadStore(JSInterop.JSRuntime);
        var t = store.Create();
        store.Append(t.Id, "user", "persist me");

        var writes = JSInterop.Invocations.Where(i => i.Identifier == "localStorage.setItem").ToList();
        Assert.NotEmpty(writes);
        Assert.Contains("persist me", writes.Last().Arguments[1]!.ToString());

        var stored = writes.Last().Arguments[1]!.ToString();
        JSInterop.Setup<string?>("localStorage.getItem", "btui-agent-threads").SetResult(stored);
        var fresh = new AgentThreadStore(JSInterop.JSRuntime);
        await fresh.LoadAsync();
        Assert.Equal("persist me", Assert.Single(fresh.All).Messages[0].Text);
    }
}

public class RedactionServiceTests
{
    [Theory]
    [InlineData("password", true)]
    [InlineData("accessToken", true)]
    [InlineData("access_token", true)]
    [InlineData("Authorization", true)]
    [InlineData("client_secret", true)]
    [InlineData("apiKey", true)]
    [InlineData("passwordExpiresAt", false)]
    [InlineData("credentialId", false)]
    [InlineData("secretary", false)]
    [InlineData("orderId", false)]
    public void CredentialKeys_AreRecognised_ButMetadataIsNot(string key, bool expected) =>
        Assert.Equal(expected, RedactionService.IsCredentialKey(key));

    [Fact]
    public void Json_MasksCredentialValues_Recursively_AndKeepsTheRest()
    {
        var json = RedactionService.RedactJson("{\"orderId\":\"4821\",\"auth\":{\"accessToken\":\"abc123\",\"passwordHint\":\"dog\"},\"items\":[{\"apiKey\":\"k\"}]}");
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal("4821", root.GetProperty("orderId").GetString());
        Assert.Equal(RedactionService.Mask, root.GetProperty("auth").GetProperty("accessToken").GetString());
        Assert.Equal("dog", root.GetProperty("auth").GetProperty("passwordHint").GetString()); // hint is metadata
        Assert.Equal(RedactionService.Mask, root.GetProperty("items")[0].GetProperty("apiKey").GetString());
    }

    [Fact]
    public void Text_MasksPairsUrlUserinfoAndBearerTokens()
    {
        var text = RedactionService.RedactText("curl https://admin:hunter2@db.local/x -H 'Authorization: Bearer eyJhbGciOi1234' --data password=letmein&name=bob");

        Assert.DoesNotContain("hunter2", text);
        Assert.DoesNotContain("eyJhbGciOi1234", text);
        Assert.DoesNotContain("letmein", text);
        Assert.Contains("name=bob", text);
    }
}
