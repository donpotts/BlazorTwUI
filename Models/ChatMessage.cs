namespace BlazorTwUI.Models;

public enum ChatRole
{
    User,
    Assistant,
    System
}

public enum StepStatus
{
    Pending,
    InProgress,
    Completed
}

public sealed record ReasoningStep(string Label, StepStatus Status);

public sealed record ChatAttachment(string Id, string Name, long SizeBytes, string MimeType, string? DataUrl = null);

public sealed class ChatMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ChatRole Role { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public bool IsStreaming { get; set; }
    public bool IsThinking { get; set; }
    public double ThinkingDurationSeconds { get; set; }
    public string? ReasoningSummary { get; set; }
    public List<ReasoningStep>? ReasoningSteps { get; set; }
    public List<string>? SuggestedPrompts { get; set; }
    public List<string>? Sources { get; set; }
    public List<ChatAttachment>? Attachments { get; set; }
    public bool? FeedbackThumb { get; set; } // true = up, false = down, null = unrated
}
