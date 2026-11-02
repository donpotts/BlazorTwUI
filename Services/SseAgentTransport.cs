namespace BlazorTwUI.Services;

using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using BlazorTwUI.Models;

/// <summary>One server-sent event frame: the optional <c>event:</c> name and the joined <c>data:</c> lines.</summary>
public readonly record struct SseMessage(string? Event, string Data);

/// <summary>Incremental parser for <c>text/event-stream</c> (WHATWG server-sent events).</summary>
public static class SseParser
{
    public static async IAsyncEnumerable<SseMessage> ReadAsync(Stream stream, [EnumeratorCancellation] CancellationToken ct = default)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
        string? eventName = null;
        var data = new StringBuilder();
        var hasData = false;

        while (await reader.ReadLineAsync(ct) is { } line)
        {
            if (line.Length == 0)
            {
                // A blank line dispatches the frame. Frames without data (keep-alives) are dropped.
                if (hasData) yield return new SseMessage(eventName, data.ToString());
                eventName = null;
                data.Clear();
                hasData = false;
                continue;
            }

            if (line[0] == ':') continue; // comment / heartbeat

            var colon = line.IndexOf(':');
            var field = colon < 0 ? line : line[..colon];
            var value = colon < 0 ? "" : line[(colon + 1)..];
            if (value.StartsWith(' ')) value = value[1..];

            switch (field)
            {
                case "event": eventName = value; break;
                case "data":
                    if (hasData) data.Append('\n');
                    data.Append(value);
                    hasData = true;
                    break;
            }
        }

        // A stream that ends without the final blank line still delivers its last frame.
        if (hasData) yield return new SseMessage(eventName, data.ToString());
    }
}

/// <summary>
/// Maps AG-UI wire events (RUN_STARTED, TEXT_MESSAGE_CONTENT, TOOL_CALL_*, STATE_*, RUN_*) onto <see cref="AgUiEvent"/>.
/// Tool calls may carry two extension fields, <c>risk</c> and <c>requiresApproval</c>, which feed the approval gate.
/// </summary>
public sealed class AgUiWireMapper
{
    private readonly Dictionary<string, string> _state = new();

    public IEnumerable<AgUiEvent> Map(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var type = Str(root, "type");

        switch (type)
        {
            case "RUN_STARTED":
                yield return new AgUiEvent(AgUiEventType.RunStarted);
                break;

            case "TEXT_MESSAGE_CONTENT":
                yield return new AgUiEvent(AgUiEventType.TextDelta, Text: Str(root, "delta"));
                break;

            case "TOOL_CALL_START":
                yield return new AgUiEvent(
                    AgUiEventType.ToolCallStart,
                    Str(root, "toolCallId"),
                    Str(root, "toolCallName"),
                    Risk: ParseRisk(Str(root, "risk")),
                    RequiresApproval: root.TryGetProperty("requiresApproval", out var ra) && ra.ValueKind == JsonValueKind.True);
                break;

            case "TOOL_CALL_ARGS":
                yield return new AgUiEvent(AgUiEventType.ToolCallArgs, Str(root, "toolCallId"), Text: Str(root, "delta"));
                break;

            case "TOOL_CALL_END":
                yield return new AgUiEvent(AgUiEventType.ToolCallEnd, Str(root, "toolCallId"));
                break;

            case "TOOL_CALL_RESULT":
                yield return new AgUiEvent(AgUiEventType.ToolCallResult, Str(root, "toolCallId"), Text: Str(root, "content"));
                break;

            case "STATE_SNAPSHOT":
                if (root.TryGetProperty("snapshot", out var snap) && snap.ValueKind == JsonValueKind.Object)
                {
                    var changed = new Dictionary<string, string>();
                    foreach (var p in snap.EnumerateObject())
                        Apply(changed, p.Name, p.Value);
                    if (changed.Count > 0) yield return new AgUiEvent(AgUiEventType.StateDelta, State: changed);
                }
                break;

            case "STATE_DELTA":
                if (root.TryGetProperty("delta", out var ops) && ops.ValueKind == JsonValueKind.Array)
                {
                    var changed = new Dictionary<string, string>();
                    foreach (var op in ops.EnumerateArray())
                    {
                        var kind = Str(op, "op");
                        if (kind is not ("add" or "replace") || !op.TryGetProperty("value", out var value)) continue;
                        // JSON Patch pointer: "/refund.status" -> "refund.status" (RFC 6901 unescape).
                        var key = (Str(op, "path") ?? "").TrimStart('/').Replace("~1", "/").Replace("~0", "~");
                        if (key.Length > 0) Apply(changed, key, value);
                    }
                    if (changed.Count > 0) yield return new AgUiEvent(AgUiEventType.StateDelta, State: changed);
                }
                break;

            case "RUN_FINISHED":
                yield return new AgUiEvent(AgUiEventType.RunFinished);
                break;

            case "RUN_ERROR":
                yield return new AgUiEvent(AgUiEventType.RunError, Text: Str(root, "message"));
                break;

                // TEXT_MESSAGE_START/END, STEP_*, RAW, CUSTOM carry nothing this UI renders.
        }
    }

    private void Apply(Dictionary<string, string> changed, string key, JsonElement value)
    {
        var text = value.ValueKind == JsonValueKind.String ? value.GetString() ?? "" : value.ToString();
        if (_state.TryGetValue(key, out var existing) && existing == text) return;
        _state[key] = text;
        changed[key] = text;
    }

    private static string? Str(JsonElement e, string name) =>
        e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;

    private static ToolRisk ParseRisk(string? risk) => risk?.ToLowerInvariant() switch
    {
        "high" => ToolRisk.High,
        "medium" => ToolRisk.Medium,
        _ => ToolRisk.Low
    };
}

public sealed class SseAgentTransportOptions
{
    /// <summary>Endpoint that accepts the run request and answers with <c>text/event-stream</c>.</summary>
    public string RunUrl { get; set; } = "";

    /// <summary>
    /// Where human approval decisions are posted. The server holds the stream open at a gated tool call until it
    /// receives the decision here. Null means decisions are not reported (read-only agents).
    /// </summary>
    public string? DecisionUrl { get; set; }

    /// <summary>Optional thread to continue; sent so the server can keep conversation context.</summary>
    public string? ThreadId { get; set; }
}

/// <summary>
/// Live AG-UI transport: POSTs the prompt and shared state, reads the SSE stream, and reports approval decisions
/// back so the server can continue the run. Plug it in with DI in place of <see cref="SimulatedAgentTransport"/>.
/// </summary>
public sealed class SseAgentTransport : IAgentTransport
{
    private readonly HttpClient _http;
    private readonly SseAgentTransportOptions _options;

    public SseAgentTransport(HttpClient http, SseAgentTransportOptions options)
    {
        _http = http;
        _options = options;
    }

    public async IAsyncEnumerable<AgUiEvent> RunAsync(string prompt, AgentRunContext context, [EnumeratorCancellation] CancellationToken ct)
    {
        var runId = Guid.NewGuid().ToString("N");
        using var request = new HttpRequestMessage(HttpMethod.Post, _options.RunUrl)
        {
            Content = JsonContent.Create(new
            {
                threadId = _options.ThreadId,
                runId,
                messages = new[] { new { role = "user", content = prompt } },
                state = context.State
            })
        };
        request.Headers.Accept.ParseAdd("text/event-stream");

        using var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Agent endpoint returned {(int)response.StatusCode} {response.ReasonPhrase}.");

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        var mapper = new AgUiWireMapper();

        await foreach (var frame in SseParser.ReadAsync(stream, ct))
        {
            foreach (var evt in mapper.Map(frame.Data))
            {
                yield return evt;

                // The session resumes us only after the gate has been decided, so the decision exists by now.
                if (evt.Type == AgUiEventType.ToolCallEnd && evt.Id is { } callId && context.DecisionFor(callId) is { } decision)
                    await ReportDecisionAsync(runId, callId, decision, ct);
            }
        }
    }

    private async Task ReportDecisionAsync(string runId, string callId, ApprovalDecision decision, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_options.DecisionUrl)) return;

        using var response = await _http.PostAsJsonAsync(_options.DecisionUrl, new
        {
            threadId = _options.ThreadId,
            runId,
            toolCallId = callId,
            approved = decision.Approved,
            reason = decision.Reason,
            editedArgs = decision.EditedArgsJson
        }, ct);
        response.EnsureSuccessStatusCode();
    }
}
