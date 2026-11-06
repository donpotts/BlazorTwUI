using System.Text.Json.Nodes;
using BlazorTwUI.Models;

namespace BlazorTwUI.Services;

/// <summary>A related entity as OData names it: the navigation property and the key of the target.</summary>
public sealed record ODataRelationMap(string Nav, string TargetKeyOData);

/// <summary>
/// How one entity is queried over OData. <see cref="FieldOData"/> maps a field's wire name (the schema's
/// <see cref="EntityField.Name"/>) to the name $filter/$orderby need; <see cref="Relations"/> does the same for relations.
/// <see cref="SearchFilter"/> is the list's own search, with <c>{q}</c> where the text goes.
/// </summary>
public sealed record ODataEntityMap(
    string Path,
    string KeyOData,
    string? Expand,
    string? SearchFilter,
    IReadOnlyDictionary<string, string> FieldOData,
    IReadOnlyDictionary<string, ODataRelationMap> Relations);

/// <summary>
/// Runs an <see cref="AnswerPlan"/> against an OData endpoint with as few GET requests as the plan allows: ONE for a count,
/// a search, a top N or a lowest/highest; pages of <see cref="PageSize"/> up to <see cref="DataAnswerComposer.RowCap"/> rows
/// for a total, an average or a breakdown. It only ever reads.
/// </summary>
public static class ODataAnswerQuery
{
    public const int PageSize = 500;

    /// <summary>The $filter for a plan ("" when there is none).</summary>
    public static string Filter(AnswerPlan plan, ODataEntityMap map)
    {
        var parts = new List<string>();
        foreach (var f in plan.Filters)
        {
            if (f.Relation is { } rel && f.RecordId is { } id)
            {
                if (!map.Relations.TryGetValue(rel.Name, out var r))
                    throw new InvalidOperationException($"There is no query mapping for {rel.Label}.");
                parts.Add(rel.Kind == RelationKind.One
                    ? $"{r.Nav}/{r.TargetKeyOData} eq {Literal(id)}"
                    : $"{r.Nav}/any(x: x/{r.TargetKeyOData} eq {Literal(id)})");
            }
            else if (f.Field is { } field)
            {
                parts.Add($"{FieldName(map, field)} eq {(f.Flag ? "true" : "false")}");
            }
        }

        if (plan.Intent == AnswerIntent.Find)
        {
            if (string.IsNullOrEmpty(map.SearchFilter))
                throw new InvalidOperationException($"{plan.Schema?.Plural ?? "These records"} have no text to search.");
            var term = (plan.Search ?? "").Trim().ToLowerInvariant().Replace("'", "''");
            parts.Add(map.SearchFilter.Replace("{q}", term));
        }

        if (DataAnswerComposer.FetchOf(plan) == FetchMode.Sorted && plan.Field is { } sorted)
            parts.Add($"{FieldName(map, sorted)} ne null");

        return parts.Count switch
        {
            0 => "",
            1 => parts[0],
            _ => string.Join(" and ", parts.Select(p => $"({p})")),
        };
    }

    /// <summary>The relative URL of one request (the same shape as a list page: <c>odata/{path}?$count=true&amp;...</c>).</summary>
    public static string Url(AnswerPlan plan, ODataEntityMap map, int skip, int top)
    {
        var query = new List<string> { "$count=true", $"$skip={skip}", $"$top={top}" };
        if (!string.IsNullOrEmpty(map.Expand)) query.Add("$expand=" + Uri.EscapeDataString(map.Expand));
        var filter = Filter(plan, map);
        if (filter.Length > 0) query.Add("$filter=" + Uri.EscapeDataString(filter));
        var order = DataAnswerComposer.FetchOf(plan) == FetchMode.Sorted && plan.Field is { } f
            ? $"{FieldName(map, f)}{(plan.Descending ? " desc" : "")},{map.KeyOData}"
            : map.KeyOData;
        query.Add("$orderby=" + Uri.EscapeDataString(order));
        return $"odata/{map.Path}?{string.Join('&', query)}";
    }

    /// <summary>
    /// Runs the plan. <paramref name="get"/> performs ONE GET of a relative URL and returns its rows and @odata.count.
    /// </summary>
    public static async Task<AnswerResult> RunAsync(AnswerPlan plan, ODataEntityMap map,
        Func<string, Task<(IReadOnlyList<JsonObject> Rows, int Total)>> get)
    {
        var urls = new List<string>();
        if (DataAnswerComposer.FetchOf(plan) != FetchMode.All)
        {
            var url = Url(plan, map, 0, Math.Max(1, plan.Take));
            urls.Add(url);
            var (rows, total) = await get(url);
            return new AnswerResult(rows, total) { Requests = urls };
        }

        var all = new List<JsonObject>();
        var matched = 0;
        for (var skip = 0; skip < DataAnswerComposer.RowCap; skip += PageSize)
        {
            var url = Url(plan, map, skip, Math.Min(PageSize, DataAnswerComposer.RowCap - skip));
            urls.Add(url);
            var (rows, total) = await get(url);
            matched = total;
            all.AddRange(rows);
            if (rows.Count == 0 || all.Count >= total) break;
        }

        return new AnswerResult(all, matched) { Requests = urls };
    }

    private static string FieldName(ODataEntityMap map, EntityField f) =>
        map.FieldOData.TryGetValue(f.Name, out var name) ? name
            : throw new InvalidOperationException($"There is no query mapping for {f.Label}.");

    private static string Literal(string id) =>
        long.TryParse(id, out var n) && n.ToString() == id ? id
        : Guid.TryParse(id, out _) ? id
        : "'" + id.Replace("'", "''") + "'";
}
