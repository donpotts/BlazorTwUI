namespace BlazorTwUI.Services;

using System.Text.Json;
using System.Text.Json.Nodes;

/// <summary>An action fired by a component on an A2UI surface (a button press), with its context resolved against the data model.</summary>
public sealed record A2uiUserAction(string Name, string SurfaceId, string? SourceComponentId, IReadOnlyDictionary<string, string> Context);

/// <summary>
/// Client-side model of one A2UI (v0.9) surface: a flat set of components addressed by id, rendered from the
/// <c>root</c> component, plus a JSON data model that components bind to with <c>{"path":"/a/b"}</c>.
/// Agents stream <c>createSurface</c>, <c>updateComponents</c> and <c>updateDataModel</c> messages; the UI re-renders on each.
/// </summary>
public sealed class A2uiSurface
{
    public const string RootId = "root";

    private readonly Dictionary<string, JsonObject> _components = new();
    private JsonObject _data = new();

    public string SurfaceId { get; private set; } = "default";
    public IReadOnlyDictionary<string, JsonObject> Components => _components;
    public JsonObject Data => _data;
    public bool HasRoot => _components.ContainsKey(RootId);

    public event Action? Changed;
    public event Action<A2uiUserAction>? ActionRaised;

    /// <summary>Applies one protocol message (a single JSON object). Unknown messages are ignored. Returns false on invalid JSON.</summary>
    public bool Apply(string json)
    {
        JsonNode? node;
        try { node = JsonNode.Parse(json); }
        catch (JsonException) { return false; }
        if (node is not JsonObject msg) return false;

        if (msg["createSurface"] is JsonObject create)
        {
            SurfaceId = (string?)create["surfaceId"] ?? SurfaceId;
            _components.Clear();
            _data = new JsonObject();
        }

        if (msg["updateComponents"] is JsonObject update && update["components"] is JsonArray list)
        {
            foreach (var item in list.OfType<JsonObject>())
            {
                if ((string?)item["id"] is { Length: > 0 } id)
                    _components[id] = (JsonObject)item.DeepClone();
            }
        }

        if (msg["updateDataModel"] is JsonObject dm)
        {
            var path = (string?)dm["path"] ?? "/";
            var value = dm["value"]?.DeepClone();
            if (path is "/" or "") _data = value as JsonObject ?? new JsonObject();
            else SetPath(path, value, notify: false);
        }

        Changed?.Invoke();
        return true;
    }

    /// <summary>Resolves a dynamic value: a literal, or <c>{"path":"/x"}</c> looked up in the data model.</summary>
    public string Resolve(JsonNode? value)
    {
        if (value is JsonObject o && o["path"] is { } p) value = Lookup((string?)p ?? "");
        return value switch
        {
            null => "",
            JsonValue v when v.TryGetValue<string>(out var s) => s,
            JsonValue v => v.ToJsonString().Trim('"'),
            _ => value.ToJsonString()
        };
    }

    public bool ResolveBool(JsonNode? value) => bool.TryParse(Resolve(value), out var b) && b;

    public double ResolveNumber(JsonNode? value, double fallback = 0) =>
        double.TryParse(Resolve(value), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : fallback;

    public JsonNode? Lookup(string path)
    {
        JsonNode? cur = _data;
        foreach (var part in Segments(path))
        {
            cur = cur switch
            {
                JsonObject obj => obj[part],
                JsonArray arr when int.TryParse(part, out var i) && i >= 0 && i < arr.Count => arr[i],
                _ => null
            };
            if (cur is null) return null;
        }
        return cur;
    }

    /// <summary>Writes a value into the data model (two-way binding from inputs).</summary>
    public void SetPath(string path, JsonNode? value, bool notify = true)
    {
        var parts = Segments(path).ToList();
        if (parts.Count == 0) return;

        JsonObject cur = _data;
        foreach (var part in parts.Take(parts.Count - 1))
        {
            if (cur[part] is not JsonObject next)
            {
                next = new JsonObject();
                cur[part] = next;
            }
            cur = next;
        }
        cur[parts[^1]] = value;
        if (notify) Changed?.Invoke();
    }

    /// <summary>Fires the action declared on a component, resolving its context bindings first.</summary>
    public void Raise(string componentId)
    {
        if (!_components.TryGetValue(componentId, out var c) || c["action"] is not JsonObject action) return;
        var name = (string?)action["name"] ?? (action["event"] as JsonObject)?["name"]?.GetValue<string>();
        if (string.IsNullOrEmpty(name)) return;

        var context = new Dictionary<string, string>();
        var ctx = action["context"] as JsonObject ?? (action["event"] as JsonObject)?["context"] as JsonObject;
        if (ctx is not null)
            foreach (var (key, v) in ctx) context[key] = Resolve(v);

        ActionRaised?.Invoke(new A2uiUserAction(name, SurfaceId, componentId, context));
    }

    private static IEnumerable<string> Segments(string path) =>
        path.Split('/', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Replace("~1", "/").Replace("~0", "~"));
}
