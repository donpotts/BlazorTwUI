namespace BlazorTwUI.Services;

using System.Text.Json;
using BlazorTwUI.Models;

/// <summary>
/// Reduces an <see cref="IAgentTransport"/> event stream into UI state and enforces the human controls:
/// high-risk tool calls block until approved or rejected, takeover pauses the run between events,
/// and every decision lands in an append-only audit trail.
/// </summary>
public sealed class AgentRunSession
{
    private static readonly JsonSerializerOptions AuditJson = new() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private readonly IAgentTransport _transport;
    private readonly Dictionary<string, string> _state = new();
    private readonly List<TimelineItem> _timeline = new();
    private readonly List<AuditEntry> _audit = new();
    private readonly HashSet<string> _changedKeys = new();
    private readonly Dictionary<string, TaskCompletionSource<ApprovalDecision>> _pending = new();
    private readonly Dictionary<string, DateTime> _toolStarts = new();

    private CancellationTokenSource? _cts;
    private TaskCompletionSource? _resume;
    private bool _takeoverRequested;
    private AgentMessage? _openMessage;

    public AgentRunSession(IAgentTransport transport) => _transport = transport;

    public event Action? Changed;

    /// <summary>Increments on every change. Views compare it to skip repainting a state they already rendered.</summary>
    public int Version { get; private set; }

    public AgentRunStatus Status { get; private set; } = AgentRunStatus.Idle;
    public string? Error { get; private set; }
    public IReadOnlyList<TimelineItem> Timeline => _timeline;
    public IReadOnlyList<AuditEntry> Audit => _audit;
    public IReadOnlyDictionary<string, string> State => _state;
    public IReadOnlySet<string> RecentlyChangedKeys => _changedKeys;
    public bool TakeoverRequested => _takeoverRequested;

    public bool IsActive => Status is AgentRunStatus.Running or AgentRunStatus.AwaitingApproval or AgentRunStatus.Paused;
    public bool CanEditState => IsActive && (_takeoverRequested || Status == AgentRunStatus.AwaitingApproval);

    public IEnumerable<ToolCallRecord> PendingApprovals =>
        _timeline.OfType<ToolCallRecord>().Where(t => t.Status == ToolCallStatus.AwaitingApproval);

    /// <summary>Runs one agent turn to completion. Completes when the run finishes, fails or is cancelled.</summary>
    public async Task RunAsync(string prompt, CancellationToken ct = default)
    {
        if (IsActive) throw new InvalidOperationException("A run is already in progress.");

        ResetForNewRun();
        _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        _timeline.Add(new AgentMessage { Role = "user", Content = prompt });
        Record(AuditActor.Human, "run.start", prompt);
        SetStatus(AgentRunStatus.Running);

        var context = new AgentRunContext(_state);
        try
        {
            await using var events = _transport.RunAsync(prompt, context, _cts.Token).GetAsyncEnumerator(_cts.Token);
            while (true)
            {
                await WaitIfTakenOverAsync();
                if (!await events.MoveNextAsync()) break;
                await ApplyAsync(events.Current, context);
            }

            CloseMessage();
            Record(AuditActor.System, "run.finish", "Agent run completed.");
            SetStatus(AgentRunStatus.Finished);
        }
        catch (OperationCanceledException)
        {
            CloseMessage();
            FailOpenToolCalls("Cancelled");
            Record(AuditActor.Human, "run.cancel", "Run cancelled by operator.");
            SetStatus(AgentRunStatus.Cancelled);
        }
        catch (Exception ex)
        {
            CloseMessage();
            FailOpenToolCalls(ex.Message);
            Error = ex.Message;
            Record(AuditActor.System, "run.error", ex.Message);
            SetStatus(AgentRunStatus.Failed);
        }
        finally
        {
            _takeoverRequested = false;
            _resume = null;
            _cts?.Dispose();
            _cts = null;
        }
    }

    /// <summary>Approves a pending tool call, optionally replacing its arguments.</summary>
    public void Approve(string callId, string? editedArgsJson = null)
    {
        if (!_pending.TryGetValue(callId, out var tcs)) return;
        if (editedArgsJson is not null && !IsValidJson(editedArgsJson))
            throw new ArgumentException("Edited arguments are not valid JSON.", nameof(editedArgsJson));

        var edited = editedArgsJson is not null && editedArgsJson != Call(callId)?.ArgsJson;
        Record(AuditActor.Human, "tool.approve", edited ? $"{Call(callId)?.Name}: approved with edited arguments {editedArgsJson}" : $"{Call(callId)?.Name}: approved");
        tcs.TrySetResult(new ApprovalDecision(true, null, edited ? editedArgsJson : null));
    }

    public void Reject(string callId, string? reason = null)
    {
        if (!_pending.TryGetValue(callId, out var tcs)) return;
        Record(AuditActor.Human, "tool.reject", $"{Call(callId)?.Name}: rejected{(string.IsNullOrWhiteSpace(reason) ? "" : $" ({reason})")}");
        tcs.TrySetResult(new ApprovalDecision(false, reason));
    }

    /// <summary>Pauses the agent before its next event so a human can inspect and edit shared state.</summary>
    public void TakeOver()
    {
        if (!IsActive || _takeoverRequested) return;
        _takeoverRequested = true;
        _resume = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Record(AuditActor.Human, "takeover.start", "Operator took control; agent will pause before its next step.");
        Notify();
    }

    public void HandBack()
    {
        if (!_takeoverRequested) return;
        _takeoverRequested = false;
        Record(AuditActor.Human, "takeover.end", "Operator handed control back to the agent.");
        _resume?.TrySetResult();
        Notify();
    }

    public void Cancel() => _cts?.Cancel();

    /// <summary>Human edit of a shared-state value. Only allowed while the human is in control.</summary>
    public void EditState(string key, string value)
    {
        if (!CanEditState) throw new InvalidOperationException("Take over the run before editing shared state.");
        var previous = _state.GetValueOrDefault(key);
        if (previous == value) return;
        _state[key] = value;
        _changedKeys.Add(key);
        Record(AuditActor.Human, "state.edit", $"{key}: \"{previous}\" → \"{value}\"");
    }

    public string ExportAuditJson() => JsonSerializer.Serialize(
        _audit.Select(a => new
        {
            a.Seq,
            timestampUtc = a.TimestampUtc.ToString("O"),
            actor = a.Actor.ToString().ToLowerInvariant(),
            a.Action,
            a.Detail
        }), AuditJson);

    private async Task WaitIfTakenOverAsync()
    {
        if (!_takeoverRequested || _resume is null) return;
        SetStatus(AgentRunStatus.Paused);
        Record(AuditActor.System, "run.paused", "Agent paused; waiting for operator.");
        using var reg = _cts!.Token.Register(() => _resume?.TrySetCanceled());
        await _resume.Task;
        Record(AuditActor.System, "run.resumed", "Agent resumed.");
        SetStatus(AgentRunStatus.Running);
    }

    private async Task ApplyAsync(AgUiEvent e, AgentRunContext context)
    {
        switch (e.Type)
        {
            case AgUiEventType.RunStarted:
                break;

            case AgUiEventType.TextDelta:
                if (_openMessage is null)
                {
                    _openMessage = new AgentMessage { IsStreaming = true };
                    _timeline.Add(_openMessage);
                }
                _openMessage.Content += e.Text;
                Notify();
                break;

            case AgUiEventType.ToolCallStart:
                CloseMessage();
                _timeline.Add(new ToolCallRecord { Id = e.Id!, Name = e.Name ?? "tool", Risk = e.Risk, RequiresApproval = e.RequiresApproval });
                _toolStarts[e.Id!] = DateTime.UtcNow;
                Record(AuditActor.Agent, "tool.propose", $"{e.Name} (risk: {e.Risk})");
                Notify();
                break;

            case AgUiEventType.ToolCallArgs:
                if (Call(e.Id) is { } streaming) streaming.ArgsJson += e.Text;
                Notify();
                break;

            case AgUiEventType.ToolCallEnd:
                await HandleToolEndAsync(e.Id!, context);
                break;

            case AgUiEventType.ToolCallResult:
                if (Call(e.Id) is { } done)
                {
                    done.Result = e.Text;
                    if (done.Status != ToolCallStatus.Rejected) done.Status = ToolCallStatus.Completed;
                    done.Duration = DateTime.UtcNow - _toolStarts.GetValueOrDefault(done.Id, done.StartedUtc);
                    Record(AuditActor.Agent, "tool.result", $"{done.Name}: {e.Text}");
                }
                Notify();
                break;

            case AgUiEventType.StateDelta:
                if (e.State is not null)
                {
                    _changedKeys.Clear();
                    foreach (var (key, value) in e.State)
                    {
                        _state[key] = value;
                        _changedKeys.Add(key);
                    }
                    Record(AuditActor.Agent, "state.update", string.Join(", ", e.State.Select(kv => $"{kv.Key}={kv.Value}")));
                }
                Notify();
                break;

            case AgUiEventType.RunFinished:
                break;

            case AgUiEventType.RunError:
                throw new InvalidOperationException(e.Text ?? "Agent reported an error.");
        }
    }

    private async Task HandleToolEndAsync(string callId, AgentRunContext context)
    {
        var call = Call(callId);
        if (call is null) return;

        if (!call.RequiresApproval)
        {
            call.Status = ToolCallStatus.Approved;
            Record(AuditActor.System, "tool.auto-approve", $"{call.Name}: policy allows {call.Risk} risk without review");
            Notify();
            return;
        }

        var tcs = new TaskCompletionSource<ApprovalDecision>(TaskCreationOptions.RunContinuationsAsynchronously);
        _pending[callId] = tcs;
        // Record first, flip status last: the approval gate should appear on the final notification,
        // not render twice and re-key its event handlers under a user who is about to click.
        Record(AuditActor.System, "tool.await-approval", $"{call.Name} is blocked until a human decides.");
        call.Status = ToolCallStatus.AwaitingApproval;
        SetStatus(AgentRunStatus.AwaitingApproval);

        ApprovalDecision decision;
        using (_cts!.Token.Register(() => tcs.TrySetCanceled()))
        {
            try { decision = await tcs.Task; }
            finally { _pending.Remove(callId); }
        }

        context.Record(callId, decision);
        if (decision.Approved)
        {
            if (decision.EditedArgsJson is not null) call.ArgsJson = decision.EditedArgsJson;
            call.Status = ToolCallStatus.Approved;
        }
        else
        {
            call.Status = ToolCallStatus.Rejected;
            call.DecisionReason = decision.Reason;
        }

        SetStatus(AgentRunStatus.Running);
    }

    private ToolCallRecord? Call(string? id) => _timeline.OfType<ToolCallRecord>().FirstOrDefault(t => t.Id == id);

    private void CloseMessage()
    {
        if (_openMessage is null) return;
        _openMessage.IsStreaming = false;
        _openMessage = null;
    }

    private void FailOpenToolCalls(string reason)
    {
        foreach (var call in _timeline.OfType<ToolCallRecord>().Where(t => t.Status is ToolCallStatus.Streaming or ToolCallStatus.AwaitingApproval or ToolCallStatus.Approved))
        {
            call.Status = ToolCallStatus.Failed;
            call.Result ??= reason;
        }
    }

    private void ResetForNewRun()
    {
        _timeline.Clear();
        _audit.Clear();
        _state.Clear();
        _changedKeys.Clear();
        _pending.Clear();
        _toolStarts.Clear();
        _openMessage = null;
        _takeoverRequested = false;
        Error = null;
    }

    private void Record(AuditActor actor, string action, string detail)
    {
        _audit.Add(new AuditEntry(_audit.Count + 1, DateTime.UtcNow, actor, action, detail));
        Notify();
    }

    private void SetStatus(AgentRunStatus status)
    {
        Status = status;
        Notify();
    }

    private void Notify()
    {
        Version++;
        Changed?.Invoke();
    }

    private static bool IsValidJson(string json)
    {
        try { using var _ = JsonDocument.Parse(json); return true; }
        catch (JsonException) { return false; }
    }
}
