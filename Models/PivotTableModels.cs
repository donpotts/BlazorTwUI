namespace BlazorTwUI.Models;

using System;
using System.Collections.Generic;

/// <summary>
/// Mathematical and aggregation operations supported by TwPivotTable.
/// </summary>
public enum PivotAggregator
{
    Sum,
    Average,
    Count,
    DistinctCount,
    Min,
    Max,
    Product
}

/// <summary>
/// Classification of a field within an OLAP pivot cube.
/// </summary>
public enum PivotFieldType
{
    Dimension,
    Measure
}

/// <summary>
/// Definition for a field participating in pivot analysis.
/// </summary>
public class PivotFieldDefinition<TItem>
{
    public string FieldName { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public PivotFieldType FieldType { get; set; } = PivotFieldType.Dimension;
    public Func<TItem, object?>? ValueGetter { get; set; }
    public PivotAggregator DefaultAggregator { get; set; } = PivotAggregator.Sum;
    public string? FormatString { get; set; }

    public PivotFieldDefinition() { }

    public PivotFieldDefinition(string fieldName, string displayName, PivotFieldType fieldType = PivotFieldType.Dimension, Func<TItem, object?>? getter = null, string? format = null)
    {
        FieldName = fieldName;
        DisplayName = displayName;
        FieldType = fieldType;
        ValueGetter = getter;
        FormatString = format;
    }
}

/// <summary>
/// Configuration of active measures/metrics in the pivot matrix.
/// </summary>
public class PivotMeasureConfig
{
    public string FieldName { get; set; } = "";
    public string? DisplayName { get; set; }
    public PivotAggregator Aggregator { get; set; } = PivotAggregator.Sum;
    public string FormatString { get; set; } = "C0";

    public string Title => !string.IsNullOrWhiteSpace(DisplayName) ? DisplayName : $"{Aggregator}({FieldName})";
}

/// <summary>
/// State container controlling dimensions, measures, filters, and rendering options.
/// </summary>
public class PivotConfig
{
    public List<string> RowFields { get; set; } = new();
    public List<string> ColumnFields { get; set; } = new();
    public List<PivotMeasureConfig> Measures { get; set; } = new();
    public Dictionary<string, HashSet<string>> Filters { get; set; } = new();

    public bool ShowGrandTotals { get; set; } = true;
    public bool ShowSubTotals { get; set; } = true;
    public bool EnableHeatmap { get; set; } = false;
    public string HeatmapColor { get; set; } = "indigo"; // "indigo", "emerald", "amber", "rose", "sky"
}

/// <summary>
/// Evaluated aggregate cell at the intersection of row and column keys.
/// </summary>
public class PivotCell<TItem>
{
    public string RowKey { get; set; } = "";
    public string ColKey { get; set; } = "";
    public string MeasureField { get; set; } = "";
    public decimal Value { get; set; }
    public string FormattedValue { get; set; } = "—";
    public List<TItem> SourceRecords { get; set; } = new();
    public double NormalizedIntensity { get; set; } // 0.0 to 1.0
}

/// <summary>
/// Event payload emitted when a user inspects or drills down into an aggregate cell.
/// </summary>
public class PivotCellDrillDown<TItem>
{
    public string RowKey { get; set; } = "";
    public string ColKey { get; set; } = "";
    public string MeasureTitle { get; set; } = "";
    public decimal AggregateValue { get; set; }
    public List<TItem> UnderlyingRecords { get; set; } = new();
}
