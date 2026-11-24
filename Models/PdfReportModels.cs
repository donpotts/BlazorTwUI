using System;
using System.Collections.Generic;

namespace BlazorTwUI.Models;

public enum PdfReportAlignment
{
    Left,
    Center,
    Right
}

public sealed class PdfReportColor
{
    public double R { get; set; }
    public double G { get; set; }
    public double B { get; set; }

    public PdfReportColor(double r, double g, double b)
    {
        R = Math.Clamp(r, 0.0, 1.0);
        G = Math.Clamp(g, 0.0, 1.0);
        B = Math.Clamp(b, 0.0, 1.0);
    }

    public static PdfReportColor FromRgbBytes(int r, int g, int b) =>
        new(r / 255.0, g / 255.0, b / 255.0);

    public static PdfReportColor FromHex(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex)) return FromAccent(AccentTheme.Indigo);
        var clean = hex.Trim().TrimStart('#');
        if (clean.Length == 3)
            clean = $"{clean[0]}{clean[0]}{clean[1]}{clean[1]}{clean[2]}{clean[2]}";
        if (clean.Length != 6) return FromAccent(AccentTheme.Indigo);

        if (int.TryParse(clean[..2], System.Globalization.NumberStyles.HexNumber, null, out var r) &&
            int.TryParse(clean[2..4], System.Globalization.NumberStyles.HexNumber, null, out var g) &&
            int.TryParse(clean[4..6], System.Globalization.NumberStyles.HexNumber, null, out var b))
        {
            return FromRgbBytes(r, g, b);
        }
        return FromAccent(AccentTheme.Indigo);
    }

    public static PdfReportColor FromAccent(AccentTheme accent) => accent switch
    {
        AccentTheme.Emerald => FromRgbBytes(5, 150, 105),   // #059669
        AccentTheme.Rose => FromRgbBytes(225, 29, 72),      // #e11d48
        AccentTheme.Amber => FromRgbBytes(217, 119, 6),     // #d97706
        AccentTheme.Violet => FromRgbBytes(124, 58, 237),   // #7c3aed
        AccentTheme.Sky => FromRgbBytes(2, 132, 199),       // #0284c7
        AccentTheme.Cyan => FromRgbBytes(8, 145, 178),      // #0891b2
        AccentTheme.Teal => FromRgbBytes(13, 148, 136),     // #0d9488
        AccentTheme.Fuchsia => FromRgbBytes(192, 38, 211),  // #c026d3
        AccentTheme.Pink => FromRgbBytes(219, 39, 119),     // #db2777
        AccentTheme.Orange => FromRgbBytes(234, 88, 12),    // #ea580c
        AccentTheme.Slate => FromRgbBytes(71, 85, 105),     // #475569
        _ => FromRgbBytes(79, 70, 229)                      // Indigo #4f46e5
    };
}

public sealed class PdfReportKpi
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? ChangeText { get; set; }
    public bool? IsPositive { get; set; }
    public string? Subtext { get; set; }
}

public sealed class PdfReportColumn
{
    public string Header { get; set; } = string.Empty;
    public double WidthWeight { get; set; } = 1.0;
    public PdfReportAlignment Alignment { get; set; } = PdfReportAlignment.Left;
    public bool IsBadge { get; set; } = false;
    public string? BadgeTone { get; set; } // "success", "warning", "danger", "neutral"
}

public sealed class PdfReportRow
{
    public List<string> Cells { get; set; } = new();
    public bool IsEmphasized { get; set; } = false;
    public string? RowCategory { get; set; }
}

public sealed class PdfExportOptions
{
    public string Title { get; set; } = "Data Export";
    public string? Subtitle { get; set; }
    public string? OrganizationName { get; set; }
    public string? Tagline { get; set; }
    public AccentTheme Accent { get; set; } = AccentTheme.Indigo;
    public string? CustomAccentHex { get; set; }
    public string? Environment { get; set; }
    public PdfPageSize PageSize { get; set; } = PdfPageSize.Letter;
    public bool Landscape { get; set; } = false;
    public bool AutoCalculateSummaries { get; set; } = true;
    public bool ShowAttestationSeal { get; set; } = false;
    public bool ShowWatermark { get; set; } = false;
    public string? WatermarkText { get; set; }
    public List<PdfReportKpi>? Kpis { get; set; }
}

public sealed class PdfBrandedReportDocument
{
    public string ReportId { get; set; } = $"RPT-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
    public string Title { get; set; } = "Data Report";
    public string? Subtitle { get; set; }
    public string? OrganizationName { get; set; }
    public string? Tagline { get; set; }
    public AccentTheme Accent { get; set; } = AccentTheme.Indigo;
    public string? CustomAccentHex { get; set; }
    public bool IsDarkStyle { get; set; } = false;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public string? Author { get; set; }
    public string? Environment { get; set; }
    public PdfPageSize PageSize { get; set; } = PdfPageSize.Letter;
    public bool Landscape { get; set; } = false;

    // Header KPIs (dynamic summary metrics)
    public List<PdfReportKpi> Kpis { get; set; } = new();

    // Data Table Schema & Records
    public List<PdfReportColumn> Columns { get; set; } = new();
    public List<PdfReportRow> Rows { get; set; } = new();
    public List<string>? SummaryRow { get; set; }

    // Compliance & Integrity
    public bool ShowWatermark { get; set; } = false;
    public string? WatermarkText { get; set; }
    public bool ShowAttestationSeal { get; set; } = false;
    public string? Sha256Seal { get; set; }
}

