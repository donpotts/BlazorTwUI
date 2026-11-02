namespace BlazorTwUI.Models;

/// <summary>AG-UI style event kinds emitted by an agent transport.</summary>
public enum AgUiEventType
{
    RunStarted,
    TextDelta,
    ToolCallStart,
    ToolCallArgs,
    ToolCallEnd,
    ToolCallResult,
    StateDelta,
    RunFinished,
    RunError
}

public enum ToolRisk
{
    Low,
    Medium,
    High
}

public enum AgentRunStatus
{
    Idle,
    Running,
    AwaitingApproval,
    Paused,
    Finished,
    Failed,
    Cancelled
}

public enum ToolCallStatus
{
    Streaming,
    AwaitingApproval,
    Approved,
    Rejected,
    Completed,
    Failed
}

public enum AuditActor
{
    System,
    Agent,
    Human
}

/// <summary>One event on the agent wire. Only the fields relevant to <see cref="Type"/> are populated.</summary>
public sealed record AgUiEvent(
    AgUiEventType Type,
    string? Id = null,
    string? Name = null,
    string? Text = null,
    ToolRisk Risk = ToolRisk.Low,
    bool RequiresApproval = false,
    IReadOnlyDictionary<string, string>? State = null);

public sealed record ApprovalDecision(bool Approved, string? Reason = null, string? EditedArgsJson = null);

/// <summary>Live view the transport gets of the run: human decisions and shared state.</summary>
public sealed class AgentRunContext
{
    private readonly Dictionary<string, ApprovalDecision> _decisions = new();

    public AgentRunContext(IReadOnlyDictionary<string, string> state) => State = state;

    /// <summary>Shared state as it stands right now, including human edits made during takeover.</summary>
    public IReadOnlyDictionary<string, string> State { get; }

    public ApprovalDecision? DecisionFor(string callId) => _decisions.GetValueOrDefault(callId);

    internal void Record(string callId, ApprovalDecision decision) => _decisions[callId] = decision;
}

/// <summary>
/// Source of agent events. The session pulls events lazily, so everything the transport does between
/// two yields (including side effects) is held back while the run is paused or awaiting approval.
/// </summary>
public interface IAgentTransport
{
    IAsyncEnumerable<AgUiEvent> RunAsync(string prompt, AgentRunContext context, CancellationToken ct);
}

public abstract class TimelineItem
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];
}

public sealed class AgentMessage : TimelineItem
{
    public string Role { get; init; } = "assistant";
    public string Content { get; set; } = "";
    public bool IsStreaming { get; set; }
}

public sealed class ToolCallRecord : TimelineItem
{
    public string Name { get; init; } = "";
    public ToolRisk Risk { get; set; }
    public bool RequiresApproval { get; set; }
    public string ArgsJson { get; set; } = "";
    public string? Result { get; set; }
    public ToolCallStatus Status { get; set; } = ToolCallStatus.Streaming;
    public string? DecisionReason { get; set; }
    public DateTime StartedUtc { get; init; } = DateTime.UtcNow;
    public TimeSpan? Duration { get; set; }
}

public sealed record AuditEntry(int Seq, DateTime TimestampUtc, AuditActor Actor, string Action, string Detail);

public sealed record AgentProfile(string Id, string Name, string Role, string Description, string Icon);
