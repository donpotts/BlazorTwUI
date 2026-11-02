namespace BlazorTwUI.Models;

public class SmartPasteMapping
{
    public string FieldName { get; set; } = "";
    public string ExtractedValue { get; set; } = "";
    public double Confidence { get; set; } = 0.95; // 0.0 to 1.0
}

public class SmartSuggestion
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Text { get; set; } = "";
    public string Category { get; set; } = "General";
}

public enum SmartTone
{
    Professional,
    Casual,
    Concise,
    Executive,
    Technical
}

public class SemanticSearchResult<T>
{
    public T Item { get; set; } = default!;
    public double RelevanceScore { get; set; } // 0.0 to 1.0
    public string MatchedReason { get; set; } = "";
    public List<string> HighlightedKeywords { get; set; } = new();
}

public class AguiWidgetSchema
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public string WidgetType { get; set; } = "card"; // card, metric, alert, progress, table, action-grid
    public string Icon { get; set; } = "⚡";
    public string AccentColor { get; set; } = "blue"; // blue, emerald, amber, purple, rose, cyan
    public Dictionary<string, object?> Properties { get; set; } = new();
    public List<AguiActionItem> Actions { get; set; } = new();
    public List<AguiMetricItem> Metrics { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}

public class AguiActionItem
{
    public string Id { get; set; } = "";
    public string Label { get; set; } = "";
    public string Variant { get; set; } = "primary";
    public string? Icon { get; set; }
}

public class AguiMetricItem
{
    public string Label { get; set; } = "";
    public string Value { get; set; } = "";
    public string? Delta { get; set; }
    public bool IsPositive { get; set; } = true;
}

public class AguiGenerationRequest
{
    public string Prompt { get; set; } = "";
    public string TargetDomain { get; set; } = "General";
    public string ComplexityLevel { get; set; } = "Standard"; // Compact, Standard, Detailed
}
