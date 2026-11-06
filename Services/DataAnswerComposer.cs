using System.Globalization;
using System.Text.Json.Nodes;
using BlazorTwUI.Models;

namespace BlazorTwUI.Services;

/// <summary>How an executor should read rows for a plan.</summary>
public enum FetchMode
{
    /// <summary>The first <see cref="AnswerPlan.Take"/> rows plus the matching count (a count or a search).</summary>
    Sample,
    /// <summary>Rows that HAVE the field, sorted by it, the first <see cref="AnswerPlan.Take"/> (top N, lowest, highest).</summary>
    Sorted,
    /// <summary>Every matching row, up to <see cref="DataAnswerComposer.RowCap"/> (totals, averages, breakdowns).</summary>
    All,
}

/// <summary>
/// Turns a plan and the rows an executor brought back into one sentence, the rows behind it and a "How I got this" line.
/// Every figure is computed HERE from <see cref="AnswerResult.Rows"/> and <see cref="AnswerResult.TotalMatched"/>; nothing
/// is estimated, and when fewer rows were read than matched the line says so.
/// </summary>
public static class DataAnswerComposer
{
    /// <summary>Most rows read for a total, an average or a breakdown.</summary>
    public const int RowCap = 2000;

    /// <summary>Rows shown under an answer.</summary>
    public const int TableRows = 10;

    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    public static FetchMode FetchOf(AnswerPlan plan) => plan.Intent switch
    {
        AnswerIntent.TopN => FetchMode.Sorted,
        AnswerIntent.Aggregate when plan.Aggregate is AggregateKind.Min or AggregateKind.Max => FetchMode.Sorted,
        AnswerIntent.Aggregate or AnswerIntent.Breakdown => FetchMode.All,
        _ => FetchMode.Sample,
    };

    public static DataAnswer Unknown(AnswerPlan plan) => new()
    {
        Sentence = plan.Message ?? "I could not answer that.",
        IsUnknown = true,
        Suggestions = plan.Suggestions,
    };

    public static DataAnswer Compose(AnswerPlan plan, AnswerResult result)
    {
        if (plan.IsUnknown || plan.Schema is null) return Unknown(plan);
        var s = plan.Schema;
        var plural = s.Plural.ToLowerInvariant();
        var filters = FiltersText(plan);
        var total = result.TotalMatched;
        var rows = result.Rows;
        var ident = s.Fields.FirstOrDefault();

        switch (plan.Intent)
        {
            case AnswerIntent.Count:
            {
                var shown = rows.Take(Math.Min(plan.Take, TableRows)).ToList();
                var cols = TableColumns(s, plan, null, rows);
                return new DataAnswer
                {
                    Sentence = total == 0 ? $"There are no {plural}{filters}." : $"There {(total == 1 ? "is" : "are")} {Count(total)} {Noun(s, total)}{filters}.",
                    Figure = Count(total),
                    Columns = cols.Select(c => c.Title).ToList(),
                    Table = shown.Select(r => Cells(r, cols)).ToList(),
                    How = $"Counted {Count(total)} {Noun(s, total)}{filters}" + (shown.Count > 0 ? $"; the table shows {(shown.Count < total ? $"the first {shown.Count}" : $"all {shown.Count}")}" : ""),
                };
            }

            case AnswerIntent.Find:
            {
                var shown = rows.Take(Math.Min(plan.Take, TableRows)).ToList();
                var cols = TableColumns(s, plan, null, rows);
                return new DataAnswer
                {
                    Sentence = total == 0 ? $"No {plural}{filters} match “{plan.Search}”." : $"{Count(total)} {Noun(s, total)}{filters} {(total == 1 ? "matches" : "match")} “{plan.Search}”.",
                    Figure = Count(total),
                    Columns = cols.Select(c => c.Title).ToList(),
                    Table = shown.Select(r => Cells(r, cols)).ToList(),
                    How = $"Searched {plural} for “{plan.Search}” in their own text and their related names (the same search as the list): {Count(total)} found"
                          + (shown.Count > 0 ? $"; showing {(shown.Count < total ? $"the first {shown.Count}" : $"all {shown.Count}")}" : ""),
                };
            }

            case AnswerIntent.TopN:
            {
                var f = plan.Field!;
                var ranked = rows.Where(r => Number(r, f) is not null).Take(plan.Take).ToList();
                var cols = new List<(string Title, Func<JsonObject, string> Cell)> { ("#", _ => "") };
                if (ident is not null && ident != f) cols.Add((ident.Label, r => EntityRecord.Text(EntityRecord.Get(r, ident.Name))));
                cols.Add((f.Label, r => Format(Number(r, f), f)));
                var table = ranked.Select((r, i) => (IReadOnlyList<string>)cols.Select((c, ci) => ci == 0 ? (i + 1).ToString(Inv) : c.Cell(r)).ToList()).ToList();
                var order = plan.Descending ? "highest first" : "lowest first";
                var lead = ranked.FirstOrDefault();
                return new DataAnswer
                {
                    Sentence = ranked.Count == 0
                        ? $"No {plural}{filters} have a {f.Label.ToLowerInvariant()}."
                        : ranked.Count == 1
                            ? $"The {(plan.Descending ? "highest" : "lowest")} {f.Label.ToLowerInvariant()} is {Format(Number(lead!, f), f)} ({Name(lead!, s)})."
                            : $"The top {ranked.Count} {plural} by {f.Label.ToLowerInvariant()}, {order}: {Name(lead!, s)} leads with {Format(Number(lead!, f), f)}.",
                    Figure = lead is null ? null : Format(Number(lead, f), f),
                    Columns = cols.Select(c => c.Title).ToList(),
                    Table = table,
                    How = $"Counted {Count(total)} {Noun(s, total)}{filters} that have a {f.Label.ToLowerInvariant()}; sorted by {f.Label.ToLowerInvariant()}, {order}; showing the top {ranked.Count}",
                };
            }

            case AnswerIntent.Aggregate:
                return Aggregate(plan, result, s, filters);

            case AnswerIntent.Breakdown:
                return Breakdown(plan, result, s, filters);
        }

        return Unknown(plan);
    }

    private static DataAnswer Aggregate(AnswerPlan plan, AnswerResult result, EntitySchema s, string filters)
    {
        var f = plan.Field!;
        var label = f.Label.ToLowerInvariant();
        var plural = s.Plural.ToLowerInvariant();
        var rows = result.Rows;
        var total = result.TotalMatched;
        var ident = s.Fields.FirstOrDefault();
        var cols = new List<(string Title, Func<JsonObject, string> Cell)>();
        if (ident is not null && ident != f) cols.Add((ident.Label, r => EntityRecord.Text(EntityRecord.Get(r, ident.Name))));
        cols.Add((f.Label, r => Format(Number(r, f), f)));

        if (plan.Aggregate is AggregateKind.Min or AggregateKind.Max)
        {
            var valued = rows.Where(r => Number(r, f) is not null).ToList();
            var best = plan.Aggregate == AggregateKind.Max
                ? valued.OrderByDescending(r => Number(r, f)).FirstOrDefault()
                : valued.OrderBy(r => Number(r, f)).FirstOrDefault();
            var word = plan.Aggregate == AggregateKind.Max ? "highest" : "lowest";
            return new DataAnswer
            {
                Sentence = best is null ? $"No {plural}{filters} have a {label}." : $"The {word} {label} among {plural}{filters} is {Format(Number(best, f), f)} ({Name(best, s)}).",
                Figure = best is null ? null : Format(Number(best, f), f),
                Columns = cols.Select(c => c.Title).ToList(),
                Table = best is null ? Array.Empty<IReadOnlyList<string>>() : new[] { Cells(best, cols) },
                How = $"Sorted the {Count(total)} {Noun(s, total)}{filters} that have a {label} by {label}, {(plan.Aggregate == AggregateKind.Max ? "highest" : "lowest")} first, and took the first one",
            };
        }

        var values = rows.Select(r => Number(r, f)).ToList();
        var present = values.OfType<decimal>().ToList();
        var missing = values.Count - present.Count;
        var cap = Cap(rows.Count, total);
        var shown = rows.Take(TableRows).ToList();
        var verb = plan.Aggregate == AggregateKind.Average ? "Averaged" : "Added up";
        var how = $"{verb} the {label} of {Count(present.Count)} {Noun(s, present.Count)}{filters}"
                  + (missing > 0 ? $"; {Count(missing)} had no {label} and {(missing == 1 ? "was" : "were")} left out" : "")
                  + cap
                  + (shown.Count > 0 ? $"; the table shows {(shown.Count < rows.Count ? $"the first {shown.Count}" : $"all {shown.Count}")}" : "");

        if (present.Count == 0)
        {
            return new DataAnswer
            {
                Sentence = total == 0 ? $"There are no {plural}{filters}, so there is nothing to {(plan.Aggregate == AggregateKind.Average ? "average" : "add up")}." : $"None of the {plural}{filters} have a {label}.",
                How = how,
            };
        }

        var figure = plan.Aggregate == AggregateKind.Average ? present.Sum() / present.Count : present.Sum();
        var shownFigure = Format(figure, f, average: plan.Aggregate == AggregateKind.Average);
        return new DataAnswer
        {
            Sentence = plan.Aggregate == AggregateKind.Average
                ? $"The average {label} of {plural}{filters} is {shownFigure}."
                : $"The {label} of {plural}{filters} adds up to {shownFigure}.",
            Figure = shownFigure,
            Columns = cols.Select(c => c.Title).ToList(),
            Table = shown.Select(r => Cells(r, cols)).ToList(),
            How = how,
        };
    }

    private static DataAnswer Breakdown(AnswerPlan plan, AnswerResult result, EntitySchema s, string filters)
    {
        var rows = result.Rows;
        var total = result.TotalMatched;
        var plural = s.Plural.ToLowerInvariant();
        var groups = new Dictionary<string, List<JsonObject>>(StringComparer.Ordinal);
        var order = new List<string>();
        string by;
        string noneLabel;
        Func<JsonObject, IEnumerable<string>> keysOf;

        if (plan.GroupByRelation is { } rel)
        {
            by = Singular(rel.Label.ToLowerInvariant());
            noneLabel = $"No {by}";
            keysOf = r => EntityRecord.Related(r, rel).Select(x => x.Name).Distinct();
        }
        else if (plan.GroupByField is { Kind: FieldKind.Boolean } flag)
        {
            by = flag.Label.ToLowerInvariant();
            noneLabel = "Not recorded";
            keysOf = r => EntityRecord.Get(r, flag.Name) is JsonValue v && v.TryGetValue<bool>(out var b)
                ? new[] { b ? flag.Label : "Not " + flag.Label.ToLowerInvariant() }
                : Array.Empty<string>();
        }
        else
        {
            var field = plan.GroupByField!;
            by = field.Label.ToLowerInvariant();
            noneLabel = $"No {by}";
            keysOf = r => EntityRecord.Text(EntityRecord.Get(r, field.Name)) is { Length: > 0 } t ? new[] { t } : Array.Empty<string>();
        }

        foreach (var r in rows)
        {
            var keys = keysOf(r).ToList();
            if (keys.Count == 0) keys.Add(noneLabel);
            foreach (var k in keys)
            {
                if (!groups.TryGetValue(k, out var list)) { groups[k] = list = []; order.Add(k); }
                list.Add(r);
            }
        }

        var measureField = plan.Field;
        // A group with no values has no average (null), never 0; it is listed last and never "the largest".
        decimal? Measure(List<JsonObject> list)
        {
            var values = plan.Aggregate == AggregateKind.Count ? new List<decimal>() : list.Select(r => Number(r, measureField!)).OfType<decimal>().ToList();
            return plan.Aggregate switch
            {
                AggregateKind.Sum => values.Count == 0 ? null : values.Sum(),
                AggregateKind.Average => values.Count == 0 ? null : values.Average(),
                _ => list.Count,
            };
        }

        var ranked = order.Select(k => (Key: k, Rows: groups[k], Value: Measure(groups[k])))
            .OrderByDescending(g => g.Value.HasValue).ThenByDescending(g => g.Value).ThenBy(g => g.Key, StringComparer.OrdinalIgnoreCase).ToList();

        var measureTitle = plan.Aggregate switch
        {
            AggregateKind.Sum => $"Total {measureField!.Label.ToLowerInvariant()}",
            AggregateKind.Average => $"Average {measureField!.Label.ToLowerInvariant()}",
            _ => s.Plural,
        };
        string Show(decimal? v) => v is null ? "no value" : plan.Aggregate == AggregateKind.Count ? Count((int)v.Value) : Format(v, measureField!, average: plan.Aggregate == AggregateKind.Average);

        var columns = new List<string> { plan.GroupByRelation is not null ? Singular(plan.GroupByRelation.Label) : plan.GroupByField!.Label, measureTitle };
        if (plan.Aggregate != AggregateKind.Count) columns.Add(s.Plural);
        var table = ranked.Select(g => (IReadOnlyList<string>)(plan.Aggregate == AggregateKind.Count
            ? new[] { g.Key, Show(g.Value) }
            : new[] { g.Key, Show(g.Value), Count(g.Rows.Count) })).ToList();

        var many = plan.GroupByRelation?.Kind == RelationKind.Many;
        var none = groups.TryGetValue(noneLabel, out var n) ? n.Count : 0;
        var top = ranked.FirstOrDefault(g => g.Value.HasValue);
        string sentence;
        if (rows.Count == 0) sentence = $"There are no {plural}{filters} to group.";
        else if (plan.GroupByField is { Kind: FieldKind.Boolean } bf && plan.Aggregate == AggregateKind.Count)
        {
            var yes = groups.TryGetValue(bf.Label, out var y) ? y.Count : 0;
            var no = groups.TryGetValue("Not " + bf.Label.ToLowerInvariant(), out var x) ? x.Count : 0;
            sentence = $"Of {Count(rows.Count)} {Noun(s, rows.Count)}{filters}, {Count(yes)} {(yes == 1 ? "is" : "are")} {bf.Label.ToLowerInvariant()} and {Count(no)} {(no == 1 ? "is" : "are")} not.";
        }
        else if (top.Key is null)
        {
            sentence = $"{s.Plural}{filters} by {by}: {Count(ranked.Count)} {(ranked.Count == 1 ? "group" : "groups")}, none with a {measureField?.Label.ToLowerInvariant() ?? "value"}.";
        }
        else
        {
            sentence = $"{s.Plural}{filters} by {by}: {Count(ranked.Count)} {(ranked.Count == 1 ? "group" : "groups")}; the largest is {top.Key} with {Show(top.Value)}.";
        }

        var how = $"Grouped {Count(rows.Count)} {Noun(s, rows.Count)}{filters} by {by} and "
                  + (plan.Aggregate switch
                  {
                      AggregateKind.Sum => $"added up the {measureField!.Label.ToLowerInvariant()} in each group",
                      AggregateKind.Average => $"averaged the {measureField!.Label.ToLowerInvariant()} in each group",
                      _ => "counted each group",
                  })
                  + (many ? $"; a {Singular(s.Entity.ToLowerInvariant())} with several {plan.GroupByRelation!.Label.ToLowerInvariant()} counts once in each" : "")
                  + (none > 0 ? $"; {Count(none)} had no {by}" : "")
                  + Cap(rows.Count, total);

        return new DataAnswer
        {
            Sentence = sentence,
            Figure = top.Key is null ? null : Show(top.Value),
            Columns = columns,
            Table = table,
            How = how,
        };
    }

    // ------------------------------------------------------------------ helpers

    /// <summary>
    /// The conditions in words: " whose brand is Northpeak", " tagged Waterproof and Ultralight" (several records of one
    /// to-many link are ALL required, so they are said together), " that are in stock".
    /// </summary>
    public static string FiltersText(AnswerPlan plan)
    {
        var parts = new List<string>();
        foreach (var group in plan.Filters.Where(f => f.Relation is not null).GroupBy(f => f.Relation!))
        {
            var names = group.Select(f => f.RecordName).ToList();
            var joined = names.Count == 1 ? names[0] : string.Join(", ", names.Take(names.Count - 1)) + " and " + names[^1];
            var rel = group.Key;
            var label = rel.Label.ToLowerInvariant();
            parts.Add(rel.Kind == RelationKind.One ? $"whose {label} is {joined}"
                : DataQuestionParser.Singular(label) == "tag" ? $"tagged {joined}"
                : names.Count == 1 ? $"with {joined} among their {label}" : $"with {joined} all among their {label}");
        }

        parts.AddRange(plan.Filters.Where(f => f.Relation is null).Select(f => f.Describe()));
        return parts.Count == 0 ? "" : " " + string.Join(" and ", parts);
    }

    private static string Cap(int read, int total) =>
        read < total ? $"; based on the first {Count(read)} of {Count(total)} rows" : "";

    private static List<(string Title, Func<JsonObject, string> Cell)> TableColumns(EntitySchema s, AnswerPlan plan, EntityField? extra, IReadOnlyList<JsonObject>? rows = null)
    {
        var cols = new List<(string Title, Func<JsonObject, string> Cell)>();
        var ident = s.Fields.FirstOrDefault();
        if (ident is not null) cols.Add((ident.Label, r => EntityRecord.Text(EntityRecord.Get(r, ident.Name))));
        foreach (var rel in s.Relations.Where(r => r.Kind == RelationKind.One).Take(2)
                     .Concat(plan.Filters.Select(f => f.Relation).OfType<EntityRelation>()).Distinct().Take(3))
        {
            cols.Add((rel.Label, r => string.Join(", ", EntityRecord.Related(r, rel).Select(x => x.Name))));
        }

        if (plan.Intent == AnswerIntent.Find)
        {
            // The to-many link the searched text was found in ("tagged ultralight" -> Tags), else the first one.
            var term = (plan.Search ?? "").Trim();
            var many = s.Relations.Where(r => r.Kind == RelationKind.Many && !cols.Any(c => c.Title == r.Label)).ToList();
            var hit = many.FirstOrDefault(rel => rows is not null && term.Length > 0 && rows.Any(row =>
                EntityRecord.Related(row, rel).Any(x => x.Name.Contains(term, StringComparison.OrdinalIgnoreCase)))) ?? many.FirstOrDefault();
            if (hit is not null) cols.Add((hit.Label, r => string.Join(", ", EntityRecord.Related(r, hit).Select(x => x.Name))));
        }

        foreach (var f in plan.Filters.Select(f => f.Field).OfType<EntityField>())
            cols.Add((f.Label, r => EntityRecord.Text(EntityRecord.Get(r, f.Name))));
        if (extra is not null) cols.Add((extra.Label, r => Format(Number(r, extra), extra)));
        return cols;
    }

    private static IReadOnlyList<string> Cells(JsonObject row, List<(string Title, Func<JsonObject, string> Cell)> cols) =>
        cols.Select(c => c.Cell(row)).ToList();

    private static string Name(JsonObject row, EntitySchema s)
    {
        var ident = s.Fields.FirstOrDefault(f => f.Kind == FieldKind.Text) ?? s.Fields.FirstOrDefault();
        var t = ident is null ? "" : EntityRecord.Text(EntityRecord.Get(row, ident.Name)).Trim();
        return t.Length > 0 ? t : EntityRecord.DisplayName(row);
    }

    private static string Noun(EntitySchema s, int n) => n == 1 ? s.Entity.ToLowerInvariant() : s.Plural.ToLowerInvariant();

    private static string Singular(string label) => DataQuestionParser.Singular(label.Trim());

    private static string Count(int n) => n.ToString("N0", Inv);

    /// <summary>A number from a row: a JSON number, or text holding one.</summary>
    public static decimal? Number(JsonObject row, EntityField f)
    {
        var node = EntityRecord.Get(row, f.Name);
        if (node is not JsonValue v) return null;
        if (v.TryGetValue<decimal>(out var d)) return d;
        if (v.TryGetValue<double>(out var dbl) && !double.IsNaN(dbl) && !double.IsInfinity(dbl)) return (decimal)dbl;
        if (v.TryGetValue<long>(out var l)) return l;
        if (v.TryGetValue<string>(out var s) && decimal.TryParse(s, NumberStyles.Number, Inv, out var parsed)) return parsed;
        return null;
    }

    /// <summary>Money and measures with two decimals; whole numbers as they are (a year stays "1974"); an average keeps two places.</summary>
    public static string Format(decimal? value, EntityField f, bool average = false)
    {
        if (value is null) return "";
        var v = value.Value;
        if (f.Kind == FieldKind.Decimal || average) return Math.Round(v, 2, MidpointRounding.AwayFromZero).ToString(average && f.Kind != FieldKind.Decimal ? "0.##" : "0.00", Inv);
        return v == Math.Truncate(v) ? v.ToString("0", Inv) : v.ToString("0.##", Inv);
    }
}
