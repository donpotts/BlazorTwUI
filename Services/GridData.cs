namespace BlazorTwUI.Services;

using System.Globalization;
using System.Text;

/// <summary>How a column is summarised in the footer and in group headers.</summary>
public enum GridSummary
{
    None,
    Count,
    Sum,
    Average,
    Min,
    Max
}

/// <summary>Aggregates for the data grid. Non-numeric and null values are ignored (Count counts non-null values).</summary>
public static class GridAggregate
{
    public static double? Compute(GridSummary kind, IEnumerable<object?> values)
    {
        if (kind == GridSummary.None) return null;
        var present = values.Where(v => v is not null).ToList();
        if (kind == GridSummary.Count) return present.Count;

        var numbers = new List<double>();
        foreach (var v in present)
        {
            if (TryNumber(v, out var d)) numbers.Add(d);
        }
        if (numbers.Count == 0) return null;

        return kind switch
        {
            GridSummary.Sum => numbers.Sum(),
            GridSummary.Average => numbers.Average(),
            GridSummary.Min => numbers.Min(),
            GridSummary.Max => numbers.Max(),
            _ => null
        };
    }

    public static bool TryNumber(object? value, out double number)
    {
        number = 0;
        switch (value)
        {
            case null: return false;
            case bool: return false;
            case string s: return double.TryParse(s, NumberStyles.Float, CultureInfo.CurrentCulture, out number);
            case IConvertible c when value is not DateTime and not char:
                try { number = c.ToDouble(CultureInfo.InvariantCulture); return true; }
                catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException) { return false; }
            default: return false;
        }
    }

    public static string Label(GridSummary kind) => kind switch
    {
        GridSummary.Count => "Count",
        GridSummary.Sum => "Sum",
        GridSummary.Average => "Avg",
        GridSummary.Min => "Min",
        GridSummary.Max => "Max",
        _ => ""
    };

    /// <summary>Formats a summary value; without a format, whole numbers have no decimals and others show up to two.</summary>
    public static string Format(double value, string? format, IFormatProvider? provider = null) =>
        string.IsNullOrEmpty(format) ? value.ToString("#,##0.##", provider ?? CultureInfo.CurrentCulture) : value.ToString(format, provider ?? CultureInfo.CurrentCulture);
}

/// <summary>CSV export that is safe to open in a spreadsheet.</summary>
public static class GridCsv
{
    /// <summary>
    /// Quotes a field when needed and neutralises spreadsheet formulas: a cell that starts with =, +, -, @, tab or carriage return
    /// is prefixed with an apostrophe so it is read as text, not executed.
    /// </summary>
    public static string Escape(string? field)
    {
        var s = field ?? "";
        if (s.Length > 0 && s[0] is '=' or '+' or '-' or '@' or '\t' or '\r' && !LooksLikeNumber(s)) s = "'" + s;
        return s.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0 || s.StartsWith(' ') || s.EndsWith(' ')
            ? "\"" + s.Replace("\"", "\"\"") + "\""
            : s;
    }

    // "-5" or "+3.2" are numbers, not formulas, and should stay numeric in the sheet.
    private static bool LooksLikeNumber(string s) =>
        s[0] is '-' or '+' && double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out _);

    public static string Build(IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string?>> rows)
    {
        var sb = new StringBuilder();
        sb.Append(string.Join(',', headers.Select(Escape))).Append("\r\n");
        foreach (var row in rows)
            sb.Append(string.Join(',', row.Select(Escape))).Append("\r\n");
        return sb.ToString();
    }
}
