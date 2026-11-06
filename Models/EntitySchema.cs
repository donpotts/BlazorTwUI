using System.Text.Json;
using System.Text.Json.Nodes;

namespace BlazorTwUI.Models;

/// <summary>How a scalar field is shown and edited. <see cref="Decimal"/> is money or a measure: always two decimal places.</summary>
public enum FieldKind { Text, LongText, Number, Date, Boolean, Decimal }

/// <summary><see cref="One"/> = to-one (single picker), <see cref="Many"/> = to-many (multi picker).</summary>
public enum RelationKind { One, Many }

/// <summary>A scalar column of an entity. <paramref name="Name"/> is the JSON member; <paramref name="Label"/> is what people read.</summary>
public record EntityField(string Name, string Label, FieldKind Kind, bool Required);

/// <summary>
/// A link to another entity. A record READS it as an object (<c>category</c>) or an array of objects (<c>tags</c>)
/// and WRITES it as ids (<see cref="IdField"/>: <c>categoryId</c> / <c>tagIds</c>).
/// <paramref name="TargetEntity"/> is the key passed to the options loader.
/// </summary>
public record EntityRelation(string Name, string Label, RelationKind Kind, string TargetEntity)
{
    /// <summary>JSON member the ids are written to. Defaults to <c>{name}Id</c> for One and <c>{singular name}Ids</c> for Many.</summary>
    public string IdField { get; init; } = EntityRecord.DefaultIdField(Name, Kind);
}

/// <summary>Describes one entity: its fields and relations, with the singular and plural names people read.</summary>
public record EntitySchema(string Entity, string Plural, IReadOnlyList<EntityField> Fields, IReadOnlyList<EntityRelation> Relations)
{
    /// <summary>Display order: the first two fields, then every relation, then the remaining fields.</summary>
    public IReadOnlyList<object> DisplayOrder()
    {
        var order = new List<object>();
        order.AddRange(Fields.Take(2));
        order.AddRange(Relations);
        order.AddRange(Fields.Skip(2));
        return order;
    }
}

/// <summary>Reads and writes <see cref="JsonObject"/> rows against a schema.</summary>
public static class EntityRecord
{
    private static readonly string[] DisplayKeys = { "name", "title", "displayName", "fullName", "label", "description", "code" };

    /// <summary>A member by name: exact first, then case-insensitive (servers send <c>name</c> for a field named <c>Name</c>).</summary>
    public static JsonNode? Get(JsonObject? obj, string name)
    {
        if (obj is null) return null;
        if (obj.TryGetPropertyValue(name, out var exact)) return exact;
        foreach (var kv in obj)
            if (string.Equals(kv.Key, name, StringComparison.OrdinalIgnoreCase)) return kv.Value;
        return null;
    }

    /// <summary>Sets a member, keeping the casing of an existing key, else camelCase (what System.Text.Json web defaults send).</summary>
    public static void Set(JsonObject obj, string name, JsonNode? value)
    {
        var key = obj.FirstOrDefault(kv => string.Equals(kv.Key, name, StringComparison.OrdinalIgnoreCase)).Key
                  ?? JsonNamingPolicy.CamelCase.ConvertName(name);
        obj[key] = value;
    }

    /// <summary>Removes every member whose key matches <paramref name="name"/> case-insensitively.</summary>
    public static void Remove(JsonObject obj, string name)
    {
        foreach (var key in obj.Select(kv => kv.Key).Where(k => string.Equals(k, name, StringComparison.OrdinalIgnoreCase)).ToList())
            obj.Remove(key);
    }

    public static string Text(JsonNode? node) => node switch
    {
        null => "",
        JsonValue v when v.TryGetValue<string>(out var s) => s,
        JsonValue v when v.TryGetValue<bool>(out var b) => b ? "Yes" : "No",
        JsonValue v => v.ToJsonString(),
        _ => node.ToJsonString(),
    };

    /// <summary>Display text for a related record; never a bare id. A record with no text reads "Untitled #id".</summary>
    public static string DisplayName(JsonObject? related)
    {
        if (related is null) return "";
        foreach (var key in DisplayKeys)
        {
            var t = Text(Get(related, key)).Trim();
            if (t.Length > 0) return t;
        }
        return Fallback(Text(Get(related, "id")));
    }

    /// <summary>The readable stand-in for an option or record that has no display text.</summary>
    public static string Fallback(string id) => string.IsNullOrWhiteSpace(id) ? "Untitled" : $"Untitled #{id}";

    /// <summary>The related records of a row as (id, name) pairs, in record order.</summary>
    public static IReadOnlyList<(string Id, string Name)> Related(JsonObject? row, EntityRelation relation)
    {
        var result = new List<(string, string)>();
        var node = Get(row, relation.Name);
        IEnumerable<JsonNode?> items = node switch
        {
            JsonArray a => a,
            JsonObject o => new[] { o },
            _ => Array.Empty<JsonNode?>(),
        };
        foreach (var item in items)
            if (item is JsonObject o) result.Add((Text(Get(o, "id")), DisplayName(o)));

        if (result.Count == 0)
        {
            // Only ids came back (no expanded objects): still show something readable.
            var ids = Get(row, relation.IdField);
            IEnumerable<JsonNode?> idNodes = ids is JsonArray arr ? arr : ids is null ? Array.Empty<JsonNode?>() : new[] { ids };
            foreach (var id in idNodes)
            {
                var s = Text(id);
                if (s.Length > 0) result.Add((s, Fallback(s)));
            }
        }
        return result;
    }

    /// <summary>The ids currently linked on a row.</summary>
    public static IReadOnlyList<string> RelatedIds(JsonObject? row, EntityRelation relation)
        => Related(row, relation).Select(r => r.Id).Where(id => id.Length > 0).ToList();

    /// <summary>An id as JSON: a number when the text is a plain integer, else a string (GUIDs, codes).</summary>
    public static JsonNode IdNode(string id)
        => long.TryParse(id, out var n) && n.ToString() == id ? JsonValue.Create(n) : JsonValue.Create(id);

    public static string DefaultIdField(string name, RelationKind kind)
    {
        var camel = JsonNamingPolicy.CamelCase.ConvertName(name);
        return kind == RelationKind.One ? camel + "Id" : Singular(camel) + "Ids";
    }

    private static string Singular(string word)
    {
        if (word.EndsWith("ies", StringComparison.OrdinalIgnoreCase) && word.Length > 3) return word[..^3] + "y";
        foreach (var suffix in new[] { "sses", "xes", "ches", "shes" })
            if (word.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) return word[..^2];
        if (word.EndsWith('s') && !word.EndsWith("ss", StringComparison.OrdinalIgnoreCase)) return word[..^1];
        return word;
    }
}
