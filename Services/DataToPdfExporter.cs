using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BlazorTwUI.Models;

namespace BlazorTwUI.Services;

public sealed class DataPdfColumnDefinition
{
    public string Title { get; set; } = string.Empty;
    public PdfReportAlignment Alignment { get; set; } = PdfReportAlignment.Left;
    public double WidthWeight { get; set; } = 1.0;
    public bool IsBadge { get; set; } = false;
    public string? BadgeTone { get; set; }
}

/// <summary>
/// Fully dynamic, generic Data-to-PDF export engine.
/// Zero hardcoding: dynamically extracts schema, properties, types, alignments,
/// column weights, formatted cells, and totals from any IEnumerable&lt;T&gt; or UI grid.
/// </summary>
public static class DataToPdfExporter
{
    /// <summary>
    /// Dynamically exports any typed collection to a styled PDF document using reflection.
    /// Extracts property headers, data types, alignment, cell values, and column summaries automatically.
    /// </summary>
    public static byte[] ExportFromCollection<T>(IEnumerable<T> data, PdfExportOptions? options = null)
    {
        options ??= new PdfExportOptions();
        var items = data?.ToList() ?? new List<T>();
        var type = typeof(T);

        // 1. Discover public readable instance properties
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && !p.GetIndexParameters().Any())
            .Where(p => !Attribute.IsDefined(p, typeof(System.Text.Json.Serialization.JsonIgnoreAttribute)))
            .ToList();

        var columns = new List<PdfReportColumn>();
        var propDescriptors = new List<(PropertyInfo Property, Type UnderlyingType, bool IsNumeric, bool IsDate, bool IsBool, bool IsEnum)>();

        foreach (var p in properties)
        {
            var uType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
            bool isNumeric = IsNumericType(uType);
            bool isDate = IsDateType(uType);
            bool isBool = uType == typeof(bool);
            bool isEnum = uType.IsEnum;

            string header = GetDisplayName(p);
            var alignment = isNumeric ? PdfReportAlignment.Right : (isBool ? PdfReportAlignment.Center : PdfReportAlignment.Left);

            columns.Add(new PdfReportColumn
            {
                Header = header,
                Alignment = alignment,
                IsBadge = isBool || isEnum,
                BadgeTone = isBool ? "success" : "neutral",
                WidthWeight = 1.0 // Will be refined below based on content lengths
            });

            propDescriptors.Add((p, uType, isNumeric, isDate, isBool, isEnum));
        }

        // 2. Extract cell values and compute max string lengths for dynamic column weighting
        var maxLenPerCol = new int[columns.Count];
        for (int i = 0; i < columns.Count; i++)
        {
            maxLenPerCol[i] = Math.Max(4, columns[i].Header.Length);
        }

        var rows = new List<PdfReportRow>();
        var numericTotals = new decimal?[columns.Count];

        foreach (var item in items)
        {
            var cellValues = new List<string>();
            for (int i = 0; i < propDescriptors.Count; i++)
            {
                var desc = propDescriptors[i];
                object? val = item != null ? desc.Property.GetValue(item) : null;
                string formatted = FormatValue(val, desc.UnderlyingType, desc.IsNumeric, desc.IsDate, desc.IsBool);

                cellValues.Add(formatted);
                maxLenPerCol[i] = Math.Max(maxLenPerCol[i], formatted.Length);

                if (desc.IsNumeric && options.AutoCalculateSummaries && val != null)
                {
                    try
                    {
                        decimal d = Convert.ToDecimal(val, CultureInfo.InvariantCulture);
                        numericTotals[i] = (numericTotals[i] ?? 0m) + d;
                    }
                    catch { }
                }
            }
            rows.Add(new PdfReportRow { Cells = cellValues });
        }

        // 3. Assign dynamic proportional width weights based on sampled content lengths
        for (int i = 0; i < columns.Count; i++)
        {
            // Weight bounded between 0.8 and 4.0 for pleasant aesthetics
            columns[i].WidthWeight = Math.Clamp(maxLenPerCol[i] / 8.0, 0.8, 4.0);
        }

        // 4. Generate dynamic summary row if any numeric columns were aggregated
        List<string>? summaryRow = null;
        if (options.AutoCalculateSummaries)
        {
            summaryRow = new List<string>();
            for (int i = 0; i < columns.Count; i++)
            {
                var desc = propDescriptors[i];
                if (i == 0)
                {
                    summaryRow.Add($"TOTALS ({items.Count})");
                }
                else if (desc.IsNumeric && numericTotals[i].HasValue && !desc.Property.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
                {
                    summaryRow.Add(FormatNumericTotal(numericTotals[i]!.Value, desc.UnderlyingType));
                }
                else
                {
                    summaryRow.Add(string.Empty);
                }
            }
        }

        // 5. Build dynamic report document
        var doc = new PdfBrandedReportDocument
        {
            Title = options.Title,
            Subtitle = options.Subtitle,
            OrganizationName = options.OrganizationName,
            Tagline = options.Tagline,
            Accent = options.Accent,
            CustomAccentHex = options.CustomAccentHex,
            Environment = options.Environment,
            PageSize = options.PageSize,
            Landscape = options.Landscape || columns.Count > 5, // Auto-landscape for wide grids!
            Columns = columns,
            Rows = rows,
            SummaryRow = summaryRow,
            ShowAttestationSeal = options.ShowAttestationSeal,
            ShowWatermark = options.ShowWatermark,
            WatermarkText = options.WatermarkText,
            Kpis = options.Kpis ?? new List<PdfReportKpi>()
        };

        return PdfReportService.GenerateReportPdf(doc);
    }

    /// <summary>
    /// Exports raw grid column definitions, row matrix, and optional footer summaries.
    /// Used directly by TwDataGrid or TwDataTable to export whatever columns and rows are currently active.
    /// </summary>
    public static byte[] ExportGrid(
        IReadOnlyList<DataPdfColumnDefinition> columns,
        IReadOnlyList<IReadOnlyList<string>> rows,
        IReadOnlyList<string>? summaries = null,
        PdfExportOptions? options = null)
    {
        options ??= new PdfExportOptions();

        var reportCols = new List<PdfReportColumn>();
        var maxLens = new int[columns.Count];

        for (int i = 0; i < columns.Count; i++)
        {
            maxLens[i] = Math.Max(4, columns[i].Title.Length);
        }

        foreach (var r in rows)
        {
            for (int i = 0; i < Math.Min(columns.Count, r.Count); i++)
            {
                maxLens[i] = Math.Max(maxLens[i], r[i]?.Length ?? 0);
            }
        }

        for (int i = 0; i < columns.Count; i++)
        {
            var c = columns[i];
            reportCols.Add(new PdfReportColumn
            {
                Header = c.Title,
                Alignment = c.Alignment,
                WidthWeight = c.WidthWeight > 0 ? c.WidthWeight : Math.Clamp(maxLens[i] / 8.0, 0.8, 4.0),
                IsBadge = c.IsBadge,
                BadgeTone = c.BadgeTone
            });
        }

        var reportRows = rows.Select(r => new PdfReportRow { Cells = r.ToList() }).ToList();

        var doc = new PdfBrandedReportDocument
        {
            Title = options.Title,
            Subtitle = options.Subtitle,
            OrganizationName = options.OrganizationName,
            Tagline = options.Tagline,
            Accent = options.Accent,
            CustomAccentHex = options.CustomAccentHex,
            Environment = options.Environment,
            PageSize = options.PageSize,
            Landscape = options.Landscape || reportCols.Count > 5,
            Columns = reportCols,
            Rows = reportRows,
            SummaryRow = summaries?.ToList(),
            ShowAttestationSeal = options.ShowAttestationSeal,
            ShowWatermark = options.ShowWatermark,
            WatermarkText = options.WatermarkText,
            Kpis = options.Kpis ?? new List<PdfReportKpi>()
        };

        return PdfReportService.GenerateReportPdf(doc);
    }

    // -------------------------------------------------------------------------
    // DYNAMIC REFLECTION HELPERS
    // -------------------------------------------------------------------------

    private static string GetDisplayName(PropertyInfo prop)
    {
        var displayAttr = prop.GetCustomAttribute<DisplayAttribute>();
        if (!string.IsNullOrWhiteSpace(displayAttr?.Name))
            return displayAttr.Name;

        var displayNameAttr = prop.GetCustomAttribute<DisplayNameAttribute>();
        if (!string.IsNullOrWhiteSpace(displayNameAttr?.DisplayName))
            return displayNameAttr.DisplayName;

        // PascalCase to spaced words: "TotalAmount" -> "Total Amount"
        return SplitPascalCase(prop.Name);
    }

    private static string SplitPascalCase(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        return Regex.Replace(text, "([a-z])([A-Z])", "$1 $2");
    }

    private static bool IsNumericType(Type t) =>
        t == typeof(decimal) || t == typeof(double) || t == typeof(float) ||
        t == typeof(int) || t == typeof(long) || t == typeof(short) ||
        t == typeof(byte) || t == typeof(uint) || t == typeof(ulong);

    private static bool IsDateType(Type t) =>
        t == typeof(DateTime) || t == typeof(DateTimeOffset) || t == typeof(DateOnly);

    private static string FormatValue(object? value, Type type, bool isNumeric, bool isDate, bool isBool)
    {
        if (value == null) return string.Empty;

        if (isBool)
        {
            return (bool)value ? "Yes" : "No";
        }

        if (isDate)
        {
            if (value is DateTime dt)
                return dt.TimeOfDay == TimeSpan.Zero ? dt.ToString("yyyy-MM-dd") : dt.ToString("yyyy-MM-dd HH:mm");
            if (value is DateTimeOffset dto)
                return dto.ToString("yyyy-MM-dd HH:mm");
            if (value is DateOnly d)
                return d.ToString("yyyy-MM-dd");
        }

        if (isNumeric)
        {
            if (value is decimal m) return m.ToString("N2", CultureInfo.CurrentCulture);
            if (value is double d) return d.ToString("N2", CultureInfo.CurrentCulture);
            if (value is float f) return f.ToString("N2", CultureInfo.CurrentCulture);
            return Convert.ToString(value, CultureInfo.CurrentCulture) ?? string.Empty;
        }

        return Convert.ToString(value, CultureInfo.CurrentCulture) ?? string.Empty;
    }

    private static string FormatNumericTotal(decimal total, Type underlyingType)
    {
        if (underlyingType == typeof(decimal) || underlyingType == typeof(double) || underlyingType == typeof(float))
        {
            return total.ToString("N2", CultureInfo.CurrentCulture);
        }
        return total.ToString("N0", CultureInfo.CurrentCulture);
    }
}

