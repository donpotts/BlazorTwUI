namespace BlazorTwUI.Services;

using System.Runtime.CompilerServices;
using System.Text.Json;
using BlazorTwUI.Models;

/// <summary>
/// Offline demo agent: a refund-desk assistant. Its behaviour genuinely depends on the human controls:
/// the refund only happens if a reviewer approves it, the amount comes from shared state (which an
/// operator can edit while in control) or from edited approval arguments, and a rejection reroutes
/// the agent to open a ticket instead. Swap in a real AG-UI transport via DI to drive a live agent.
/// </summary>
public sealed class SimulatedAgentTransport : IAgentTransport
{
    /// <summary>Pause between emitted events. Set to <see cref="TimeSpan.Zero"/> in tests.</summary>
    public TimeSpan StepDelay { get; set; } = TimeSpan.FromMilliseconds(450);

    public async IAsyncEnumerable<AgUiEvent> RunAsync(string prompt, AgentRunContext context, [EnumeratorCancellation] CancellationToken ct)
    {
        yield return new AgUiEvent(AgUiEventType.RunStarted);

        foreach (var chunk in Chunks("Looking up the order and checking it against the refund policy. "))
        {
            await Pause(ct);
            yield return new AgUiEvent(AgUiEventType.TextDelta, Text: chunk);
        }

        // 1. Low-risk read; runs without review.
        await Pause(ct);
        yield return new AgUiEvent(AgUiEventType.ToolCallStart, "call_lookup", "lookup_order");
        foreach (var chunk in Chunks("{\"orderId\":\"4821\"}"))
            yield return new AgUiEvent(AgUiEventType.ToolCallArgs, "call_lookup", Text: chunk);
        yield return new AgUiEvent(AgUiEventType.ToolCallEnd, "call_lookup");
        await Pause(ct);
        yield return new AgUiEvent(AgUiEventType.ToolCallResult, "call_lookup", Text: "Order 4821: Walnut desk, $249.00, delivered 12 days ago, customer reports a cracked leg.");
        yield return new AgUiEvent(AgUiEventType.StateDelta, State: new Dictionary<string, string>
        {
            ["order.id"] = "4821",
            ["order.total"] = "249.00",
            ["refund.amount"] = "249.00",
            ["refund.status"] = "proposed"
        });

        // 2. High-risk write; blocked until a human decides.
        await Pause(ct);
        yield return new AgUiEvent(AgUiEventType.TextDelta, Text: "Damage is covered. I will issue the refund once it is approved. ");
        await Pause(ct);
        yield return new AgUiEvent(AgUiEventType.ToolCallStart, "call_refund", "issue_refund", Risk: ToolRisk.High, RequiresApproval: true);
        var proposedAmount = context.State.GetValueOrDefault("refund.amount", "249.00");
        foreach (var chunk in Chunks($"{{\"orderId\":\"4821\",\"amount\":\"{proposedAmount}\"}}"))
            yield return new AgUiEvent(AgUiEventType.ToolCallArgs, "call_refund", Text: chunk);
        yield return new AgUiEvent(AgUiEventType.ToolCallEnd, "call_refund");

        var decision = context.DecisionFor("call_refund");
        if (decision is { Approved: true })
        {
            var amount = ReadAmount(decision.EditedArgsJson) ?? proposedAmount;
            await Pause(ct);
            yield return new AgUiEvent(AgUiEventType.ToolCallResult, "call_refund", Text: $"Refund of ${amount} issued to the original payment method.");
            yield return new AgUiEvent(AgUiEventType.StateDelta, State: new Dictionary<string, string>
            {
                ["refund.status"] = "issued",
                ["refund.amount"] = amount
            });

            await Pause(ct);
            yield return new AgUiEvent(AgUiEventType.ToolCallStart, "call_email", "send_email", Risk: ToolRisk.Medium);
            foreach (var chunk in Chunks($"{{\"to\":\"customer\",\"template\":\"refund_confirmation\",\"amount\":\"{amount}\"}}"))
                yield return new AgUiEvent(AgUiEventType.ToolCallArgs, "call_email", Text: chunk);
            yield return new AgUiEvent(AgUiEventType.ToolCallEnd, "call_email");
            await Pause(ct);
            yield return new AgUiEvent(AgUiEventType.ToolCallResult, "call_email", Text: "Confirmation email queued.");
            yield return new AgUiEvent(AgUiEventType.TextDelta, Text: $"Done. ${amount} has been refunded and the customer has been notified.");
        }
        else
        {
            var why = string.IsNullOrWhiteSpace(decision?.Reason) ? "no reason given" : decision!.Reason!;
            await Pause(ct);
            yield return new AgUiEvent(AgUiEventType.ToolCallResult, "call_refund", Text: $"Not executed. Rejected by reviewer ({why}).");
            yield return new AgUiEvent(AgUiEventType.StateDelta, State: new Dictionary<string, string> { ["refund.status"] = "blocked" });

            await Pause(ct);
            yield return new AgUiEvent(AgUiEventType.ToolCallStart, "call_ticket", "create_ticket", Risk: ToolRisk.Low);
            foreach (var chunk in Chunks($"{{\"queue\":\"escalations\",\"note\":\"Refund rejected: {why}\"}}"))
                yield return new AgUiEvent(AgUiEventType.ToolCallArgs, "call_ticket", Text: chunk);
            yield return new AgUiEvent(AgUiEventType.ToolCallEnd, "call_ticket");
            await Pause(ct);
            yield return new AgUiEvent(AgUiEventType.ToolCallResult, "call_ticket", Text: "Ticket ESC-1042 opened for a human agent.");
            yield return new AgUiEvent(AgUiEventType.TextDelta, Text: "Understood. I did not issue the refund and escalated to a human agent instead.");
        }

        yield return new AgUiEvent(AgUiEventType.RunFinished);
    }

    private Task Pause(CancellationToken ct) =>
        StepDelay > TimeSpan.Zero ? Task.Delay(StepDelay, ct) : Task.CompletedTask;

    private static IEnumerable<string> Chunks(string text, int size = 12)
    {
        for (var i = 0; i < text.Length; i += size)
            yield return text.Substring(i, Math.Min(size, text.Length - i));
    }

    private static string? ReadAmount(string? json)
    {
        if (json is null) return null;
        try
        {
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetProperty("amount", out var a) ? a.ToString() : null;
        }
        catch (JsonException) { return null; }
    }
}
