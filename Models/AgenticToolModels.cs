namespace BlazorTwUI.Models;

public enum ToolSource
{
    FrontendWasm,
    BackendApi
}

public enum ToolExecutionStatus
{
    Pending,
    Executing,
    Completed,
    Failed
}

public class AgentToolDefinition
{
    public string Name { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public ToolSource Source { get; set; } = ToolSource.FrontendWasm;
    public string Description { get; set; } = "";
    public string Icon { get; set; } = "⚡";
    public Dictionary<string, string> ParameterSchemas { get; set; } = new();
}

public class ToolCallExecution
{
    public string CallId { get; set; } = "call_" + Guid.NewGuid().ToString("N")[..8];
    public string ToolName { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public ToolSource Source { get; set; }
    public Dictionary<string, object?> Arguments { get; set; } = new();
    public object? Result { get; set; }
    public double LatencyMs { get; set; }
    public ToolExecutionStatus Status { get; set; } = ToolExecutionStatus.Completed;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class AgenticExecutionTrace
{
    public string Prompt { get; set; } = "";
    public List<ToolCallExecution> ToolCalls { get; set; } = new();
    public AguiWidgetSchema? ResultWidget { get; set; }
    public double TotalExecutionMs { get; set; }
    public string ReasoningSummary { get; set; } = "";
}
