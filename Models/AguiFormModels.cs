namespace BlazorTwUI.Models;

public enum AguiFieldType
{
    Text,
    TextArea,
    Number,
    Date,
    Select,
    Switch,
    Slider,
    Checkbox,
    RadioGroup,
    Tags
}

public class AguiFormField
{
    public string Id { get; set; } = "";
    public string Label { get; set; } = "";
    public AguiFieldType Type { get; set; } = AguiFieldType.Text;
    public string? Placeholder { get; set; }
    public string? HelperText { get; set; }
    public bool IsRequired { get; set; }
    public List<AguiOptionItem> Options { get; set; } = new();
    public double? Min { get; set; }
    public double? Max { get; set; }
    public double? Step { get; set; }
    public string? DependsOnField { get; set; }
    public object? DependsOnValue { get; set; }
    public double? AiConfidence { get; set; } // 0.0 to 1.0 (e.g. 0.98 = 98%)
    public string? ExtractedSource { get; set; } // e.g. "Auto-detected from SLA document page 3"
    public string? ValidationRegex { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Group { get; set; } = "General";
}

public class AguiOptionItem
{
    public string Value { get; set; } = "";
    public string Label { get; set; } = "";
    public string? Icon { get; set; }
    public string? Description { get; set; }
}

public class AguiFormSchema
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Icon { get; set; } = "📋";
    public string AccentColor { get; set; } = "purple"; // blue, purple, emerald, amber, rose, cyan
    public string SubmitLabel { get; set; } = "Approve & Execute";
    public string CancelLabel { get; set; } = "Reject / Cancel";
    public List<AguiFormField> Fields { get; set; } = new();
    public Dictionary<string, object?> InitialValues { get; set; } = new();
    public double OverallConfidence { get; set; } = 0.95;
    public string RiskLevel { get; set; } = "Low"; // Low, Medium, High
    public string? AgentName { get; set; } = "DevOps Copilot";
    public bool ShowDiffInspector { get; set; } = true;
}

public class AguiFormSubmission
{
    public string FormId { get; set; } = "";
    public Dictionary<string, object?> Values { get; set; } = new();
    public DateTime SubmittedAt { get; set; } = DateTime.Now;
    public bool IsApproved { get; set; } = true;
    public string? UserNotes { get; set; }
}
