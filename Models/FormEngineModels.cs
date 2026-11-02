namespace BlazorTwUI.Models;

public enum FormFieldType
{
    Text,
    Email,
    Password,
    Number,
    Textarea,
    Select,
    MultiSelect,
    Checkbox,
    Toggle,
    Date,
    Time,
    Slider,
    Rating,
    Currency,
    Tags,
    Radio
}

public class FormFieldDefinition
{
    public string Name { get; set; } = "";
    public string Label { get; set; } = "";
    public string? Placeholder { get; set; }
    public string? HelperText { get; set; }
    public FormFieldType Type { get; set; } = FormFieldType.Text;
    public object? DefaultValue { get; set; }
    public bool Required { get; set; }
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public double? Min { get; set; }
    public double? Max { get; set; }
    public double? Step { get; set; }
    public List<string>? Options { get; set; }
    public string? ColumnSpan { get; set; } = "col-span-1"; // col-span-1, col-span-2, col-span-full
    public string? Prefix { get; set; }
    public string? Suffix { get; set; }
}

public class FormSchema
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public string Icon { get; set; } = "📝";
    public string SubmitButtonText { get; set; } = "Submit Form";
    public List<FormFieldDefinition> Fields { get; set; } = new();
    public int Columns { get; set; } = 2; // 1 or 2 or 3
}

public class FormSubmissionResult
{
    public string FormId { get; set; } = "";
    public Dictionary<string, object?> Values { get; set; } = new();
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public bool IsValid { get; set; } = true;
}
