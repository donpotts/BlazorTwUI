using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorTwUI.Models;

public sealed class TwDashboardCard
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string Title { get; set; } = "Dashboard Widget";
    public string? Subtitle { get; set; }
    public string CardType { get; set; } = "kpi"; // kpi, chart, activity, opportunities, notes, custom
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public int W { get; set; } = 4;
    public int H { get; set; } = 4;
    public int? MinW { get; set; } = 2;
    public int? MinH { get; set; } = 2;
    public int? MaxW { get; set; } = 12;
    public int? MaxH { get; set; } = 12;
    public bool IsCollapsed { get; set; } = false;
    public string AccentColor { get; set; } = "indigo"; // indigo, emerald, amber, rose, cyan, purple, brand
    public string? Value { get; set; }
    public string? ChangePercent { get; set; }
    public bool? IsPositiveTrend { get; set; } = true;
    public string? Icon { get; set; }
    public string? NoteText { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TwDashboardChartConfig? ChartConfig { get; set; }

    public Dictionary<string, string> Properties { get; set; } = new();
}

public sealed class TwDashboardChartConfig
{
    public string Type { get; set; } = "line"; // line, bar, doughnut, funnel
    public List<string> Labels { get; set; } = new();
    public List<TwDashboardChartDataset> Datasets { get; set; } = new();
    public string? ValuePrefix { get; set; } = "$";
}

public sealed class TwDashboardChartDataset
{
    public string Label { get; set; } = string.Empty;
    public List<double> Data { get; set; } = new();
    public string? BorderColor { get; set; }
    public string? BackgroundColor { get; set; }
    public List<string>? BackgroundColors { get; set; }
    public double? Tension { get; set; } = 0.35;
    public bool? Fill { get; set; } = true;
}

public sealed class TwCrmKpiSummary
{
    public int NewLeads { get; set; } = 142;
    public decimal TotalRevenue { get; set; } = 284500m;
    public double ConversionRate { get; set; } = 18.4;
    public decimal PipelineValue { get; set; } = 1240000m;
    public int DealsClosed { get; set; } = 38;
    public decimal AvgDealSize { get; set; } = 7480m;
    public int TotalCustomers { get; set; } = 1840;
    public int OpenSupportCases { get; set; } = 12;
}

public sealed class TwCrmRecentActivity
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..6];
    public string Type { get; set; } = "Call"; // Call, Email, Meeting, Note, Task
    public string Subject { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;
}

public sealed class TwCrmOpportunity
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..6];
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Stage { get; set; } = "Proposal";
    public double Probability { get; set; } = 65;
    public string CustomerName { get; set; } = string.Empty;
}
