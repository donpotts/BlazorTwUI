namespace BlazorTwUI.Models;

using System;
using System.Collections.Generic;

/// <summary>
/// Layout orientation modes for Master/Detail workspaces.
/// </summary>
public enum MasterDetailLayout
{
    SplitHorizontal,
    SplitVertical,
    Stacked
}

/// <summary>
/// State lifecycle modes for master and detail records.
/// </summary>
public enum MasterDetailMode
{
    View,
    Edit,
    Create
}

/// <summary>
/// Tab identifiers inside the Detail Inspector pane.
/// </summary>
public enum MasterDetailTab
{
    Details,
    ChildRecords,
    Activity
}

/// <summary>
/// Column definition for displaying and editing detail/child rows.
/// </summary>
public class ChildRecordColumn<TChild>
{
    public string Title { get; set; } = "";
    public string PropertyName { get; set; } = "";
    public Func<TChild, object?>? ValueSelector { get; set; }
    public string? Width { get; set; }
    public string? Format { get; set; }
    public bool IsEditable { get; set; }
    public Action<TChild, string>? OnValueChanged { get; set; }

    public ChildRecordColumn() { }

    public ChildRecordColumn(string title, Func<TChild, object?> valueSelector, string? width = null, string? format = null, bool isEditable = false)
    {
        Title = title;
        ValueSelector = valueSelector;
        Width = width;
        Format = format;
        IsEditable = isEditable;
    }
}

/// <summary>
/// Result of master-detail form validation check.
/// </summary>
public class MasterDetailValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// Simple audit entry for Master/Detail activity timelines.
/// </summary>
public class MasterDetailAuditEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Author { get; set; } = "System Operator";
    public string Action { get; set; } = "Updated record";
    public string? Details { get; set; }
    public string BadgeVariant { get; set; } = "info"; // "success", "warning", "info", "danger"
}
