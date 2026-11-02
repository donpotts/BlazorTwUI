namespace BlazorTwUI.Models;

public record ComponentParameterDoc(
    string Name,
    string Type,
    string DefaultValue,
    string Description
);

public class ComponentDocItem
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RazorSnippet { get; set; } = string.Empty;
    public List<ComponentParameterDoc> Parameters { get; set; } = new();
}
