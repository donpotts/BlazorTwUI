using System.Text.Json.Nodes;
using BlazorTwUI.Components.UI;
using BlazorTwUI.Models;
using Bunit;
using Xunit;

namespace BlazorTwUI.Tests;

public class EntitySchemaTests : TestContext
{
    private static readonly EntitySchema Products = new("Product", "Products",
        new[]
        {
            new EntityField("Name", "Name", FieldKind.Text, true),
            new EntityField("Price", "Price", FieldKind.Number, true),
            new EntityField("Description", "Description", FieldKind.LongText, false),
            new EntityField("InStock", "In stock", FieldKind.Boolean, false),
        },
        new[]
        {
            new EntityRelation("Category", "Category", RelationKind.One, "Category"),
            new EntityRelation("Tags", "Tags", RelationKind.Many, "Tag"),
        });

    private static readonly EntitySchema Plain = Products with { Relations = Array.Empty<EntityRelation>() };

    private static JsonObject Row(string json) => JsonNode.Parse(json)!.AsObject();

    private static Func<GridDataRequest, Task<GridDataResult<JsonObject>>> Rows(params JsonObject[] rows)
        => _ => Task.FromResult(new GridDataResult<JsonObject>(rows, rows.Length));

    private static IReadOnlyList<(string Id, string Name)> TagOptions(int count)
        => Enumerable.Range(1, count).Select(i => (i.ToString(), $"Tag {i}")).ToList();

    private IRenderedComponent<TwEntityGrid> Grid(EntitySchema schema, params JsonObject[] rows)
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        return RenderComponent<TwEntityGrid>(p => p.Add(g => g.Schema, schema).Add(g => g.Load, Rows(rows)));
    }

    [Fact]
    public void Related_columns_follow_first_two_fields()
    {
        var cut = Grid(Products, Row("""{"id":1,"name":"Tent","price":389.0,"description":"Two person","inStock":true,"category":{"id":2,"name":"Shelter"},"tags":[{"id":1,"name":"Ultralight"},{"id":3,"name":"Waterproof"}]}"""));

        var headers = cut.FindAll("th[data-col]").Select(th => th.GetAttribute("data-col")).ToList();
        // Long text (Description) starts hidden: see Long_text_starts_hidden_but_can_be_shown.
        Assert.Equal(new[] { "Name", "Price", "Category", "Tags", "In stock" }, headers);

        var cells = cut.FindAll("[data-testid=grid-row] td").Select(td => td.TextContent.Trim()).ToList();
        Assert.Equal("Tent", cells[0]);
        Assert.Equal("Shelter", cells[2]);
        Assert.Equal("Ultralight, Waterproof", cells[3]);
        Assert.Equal("Yes", cells[4]);
    }

    [Fact]
    public void Long_text_starts_hidden_but_can_be_shown()
    {
        // A generated Products grid at 1366-1440px with a sidebar put Tags under the pinned Actions column: ten columns at
        // a 10rem floor each. Long text is the column that costs most and says least in a list, so it starts hidden.
        var cut = Grid(Products, Row("""{"id":1,"name":"Tent","price":389.0,"description":"Two person","inStock":true}"""));
        Assert.DoesNotContain(cut.FindAll("th[data-col]"), th => th.GetAttribute("data-col") == "Description");

        cut.Find("[data-testid=grid-columns-button]").Click();
        var toggle = cut.Find("[data-testid=grid-column-toggle][data-col=Description]");
        Assert.False(toggle.HasAttribute("checked"));
        toggle.Change(true);
        Assert.Contains(cut.FindAll("th[data-col]"), th => th.GetAttribute("data-col") == "Description");
    }

    [Fact]
    public void Entity_grid_is_dense_and_floors_only_the_identifying_and_related_columns()
    {
        var cut = Grid(Products, Row("""{"id":1,"name":"Tent","price":389.0,"inStock":true,"category":{"id":2,"name":"Shelter"}}"""));
        string Style(string col) => cut.Find($"th[data-col='{col}']").GetAttribute("style") ?? "";

        Assert.Contains("min-width: 8rem", Style("Name"));
        Assert.Contains("min-width: 8rem", Style("Category"));
        Assert.Contains("min-width: 8rem", Style("Tags"));
        Assert.DoesNotContain("min-width", Style("Price"));
        Assert.DoesNotContain("min-width", Style("In stock"));
        Assert.Contains("px-3 py-3", cut.Find("th[data-col='Name']").GetAttribute("class"));
        Assert.DoesNotContain("w-44", cut.Markup);
    }

    [Fact]
    public void Pick_list_has_no_text_under_12px()
    {
        // Reels and Showcase images refuse text under 12px; the counts and "n selected" used 10px and 11px.
        var source = File.ReadAllText(Path.Combine(FindKitRoot(), "Components", "UI", "TwPickList.razor"));
        Assert.DoesNotContain("text-[10px]", source);
        Assert.DoesNotContain("text-[11px]", source);
    }

    private static string FindKitRoot()
    {
        for (var d = new DirectoryInfo(AppContext.BaseDirectory); d is not null; d = d.Parent)
            if (File.Exists(Path.Combine(d.FullName, "BlazorTwUI.csproj"))) return d.FullName;
        throw new DirectoryNotFoundException("BlazorTwUI.csproj not found above " + AppContext.BaseDirectory);
    }

    [Fact]
    public void Many_picker_is_prefilled_from_record()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        JsonObject? saved = null;
        var record = Row("""{"id":9,"name":"Tent","price":389,"category":{"id":2,"name":"Shelter"},"tags":[{"id":1,"name":"Tag 1"},{"id":3,"name":"Tag 3"}]}""");
        var cut = RenderComponent<TwEntityForm>(p => p
            .Add(f => f.Schema, Products)
            .Add(f => f.Record, record)
            .Add(f => f.LoadOptions, target => Task.FromResult(target == "Tag" ? TagOptions(5)
                : (IReadOnlyList<(string, string)>)new[] { ("1", "Packs"), ("2", "Shelter") }))
            .Add(f => f.OnSave, (JsonObject o) => saved = o));

        var many = cut.Find("[data-testid=relation-picker][data-relation=Tags]");
        Assert.Equal("Tag 1, Tag 3", many.QuerySelector("[data-testid=relation-selected]")!.TextContent.Trim().Replace("Selected: ", ""));
        var one = cut.Find("[data-testid=relation-picker][data-relation=Category] input");
        Assert.Equal("Shelter", one.GetAttribute("value"));

        cut.Find("[data-testid=entity-form]").Submit();

        Assert.NotNull(saved);
        // Related records READ as objects but WRITE as ids: a PUT without the ids clears the links.
        Assert.Equal(new long[] { 1, 3 }, saved!["tagIds"]!.AsArray().Select(n => n!.GetValue<long>()).ToArray());
        Assert.Equal(2L, saved["categoryId"]!.GetValue<long>());
        Assert.Equal(9, saved["id"]!.GetValue<int>());
    }

    [Fact]
    public void Picker_requests_all_options_not_first_page()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var asked = new List<string>();
        var relation = Products.Relations[1];
        var cut = RenderComponent<TwRelationPicker>(p => p
            .Add(r => r.Relation, relation)
            .Add(r => r.Record, Row("""{"tags":[{"id":250,"name":"Tag 250"}]}"""))
            .Add(r => r.LoadOptions, t => { asked.Add(t); return Task.FromResult(TagOptions(250)); }));

        Assert.Equal(new[] { "Tag" }, asked);
        // Every option is available (not a first page of 10 or 50): 249 offered + the one already chosen.
        // Long lists are virtualized, so the DOM holds only the rows on screen; aria-setsize and the filter prove the rest is there.
        var offered = cut.Find("[data-testid=picklist-source] [role=option]");
        Assert.Equal("249", offered.GetAttribute("aria-setsize"));
        Assert.True(cut.FindAll("[data-testid=relation-option]").Count < 80);
        Assert.Contains("250", cut.Find("[data-testid=picklist-target]").InnerHtml);
        cut.Find("[data-testid=picklist-source-filter]").Input("Tag 1");
        var ids = cut.FindAll("[data-testid=picklist-source] [data-testid=relation-option]").Select(e => e.GetAttribute("data-id")).ToList();
        Assert.Contains("1", ids);
    }

    [Fact]
    public void Missing_display_text_shows_fallback_not_raw_id()
    {
        var cut = Grid(Products, Row("""{"name":"Tent","price":1,"category":{"id":7},"tags":[{"id":4,"name":"  "}]}"""));
        var cells = cut.FindAll("[data-testid=grid-row] td").Select(td => td.TextContent.Trim()).ToList();
        Assert.Equal("Untitled #7", cells[2]);
        Assert.Equal("Untitled #4", cells[3]);

        JSInterop.Mode = JSRuntimeMode.Loose;
        var picker = RenderComponent<TwRelationPicker>(p => p
            .Add(r => r.Relation, Products.Relations[0])
            .Add(r => r.Record, Row("""{"category":{"id":7}}"""))
            .Add(r => r.LoadOptions, _ => Task.FromResult((IReadOnlyList<(string, string)>)new[] { ("7", ""), ("8", "Packs") })));
        Assert.Equal("Untitled #7", picker.Find("input").GetAttribute("value"));
    }

    private static Task<IReadOnlyList<(string Id, string Name)>> Options(string target)
        => Task.FromResult(target == "Tag" ? TagOptions(5)
            : (IReadOnlyList<(string, string)>)new[] { ("1", "Packs"), ("2", "Shelter") });

    private JsonObject? SaveForm(EntitySchema schema, JsonObject record, Action<IRenderedComponent<TwEntityForm>>? edit = null)
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        JsonObject? saved = null;
        var cut = RenderComponent<TwEntityForm>(p => p
            .Add(f => f.Schema, schema).Add(f => f.Record, record)
            .Add(f => f.LoadOptions, Options)
            .Add(f => f.OnSave, (JsonObject o) => saved = o));
        edit?.Invoke(cut);
        cut.Find("[data-testid=entity-form]").Submit();
        return saved;
    }

    [Fact]
    public void Saved_payload_has_ids_and_no_related_objects()
    {
        var saved = SaveForm(Products, Row("""{"id":9,"name":"Tent","price":389,"Category":{"id":2,"name":"Shelter"},"tags":[{"id":1,"name":"Tag 1"}]}"""))!;
        Assert.False(saved.ContainsKey("tags"));
        Assert.DoesNotContain(saved, kv => string.Equals(kv.Key, "category", StringComparison.OrdinalIgnoreCase));
        Assert.Equal(new long[] { 1 }, saved["tagIds"]!.AsArray().Select(n => n!.GetValue<long>()).ToArray());
        Assert.Equal(2L, saved["categoryId"]!.GetValue<long>());
        Assert.Equal("Tent", saved["name"]!.GetValue<string>());
    }

    [Fact]
    public void Cleared_to_one_sends_null_id_and_no_object()
    {
        var saved = SaveForm(Products, Row("""{"id":9,"name":"Tent","price":389,"category":{"id":2,"name":"Shelter"},"categoryId":2,"tags":[]}"""),
            cut => cut.Find("[data-relation=Category] button[aria-label='Clear selection']").Click())!;
        Assert.True(saved.ContainsKey("categoryId"));
        Assert.Null(saved["categoryId"]);
        Assert.False(saved.ContainsKey("category"));
    }

    private static readonly EntitySchema Trips = new("Trip", "Trips",
        new[] { new EntityField("Title", "Title", FieldKind.Text, true), new EntityField("StartsAt", "Starts at", FieldKind.Date, false) },
        Array.Empty<EntityRelation>());

    [Fact]
    public void Date_with_time_round_trips_when_unchanged()
    {
        var saved = SaveForm(Trips, Row("""{"title":"Ridge","startsAt":"2026-03-04T15:30:00+02:00"}"""))!;
        Assert.Equal("2026-03-04T15:30:00+02:00", saved["startsAt"]!.GetValue<string>());

        // Changed: the new date keeps the original time of day and offset.
        var changed = SaveForm(Trips, Row("""{"title":"Ridge","startsAt":"2026-03-04T15:30:00+02:00"}"""),
            cut => cut.Find("[data-field=StartsAt] input").Input("2026-05-06"))!;
        Assert.Equal("2026-05-06T15:30:00+02:00", changed["startsAt"]!.GetValue<string>());

        // A date that never had a time stays a plain date.
        var plain = SaveForm(Trips, Row("""{"title":"Ridge","startsAt":"2026-03-04"}"""),
            cut => cut.Find("[data-field=StartsAt] input").Input("2026-05-06"))!;
        Assert.Equal("2026-05-06", plain["startsAt"]!.GetValue<string>());
    }

    [Fact]
    public void Schema_without_relations_renders_no_picker()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var calls = 0;
        var cut = RenderComponent<TwEntityForm>(p => p
            .Add(f => f.Schema, Plain)
            .Add(f => f.LoadOptions, _ => { calls++; return Task.FromResult((IReadOnlyList<(string, string)>)Array.Empty<(string, string)>()); }));

        Assert.Empty(cut.FindAll("[data-testid=relation-picker]"));
        Assert.Equal(0, calls);
        // The form still renders every field by its schema label.
        Assert.Equal(4, cut.FindAll("[data-testid=entity-field]").Count);
        Assert.Contains("In stock", cut.Markup);
        Assert.DoesNotContain("InStock<", cut.Markup);
    }
    private static readonly EntitySchema Gear = new("Item", "Items",
        new[] { new EntityField("Name", "Name", FieldKind.Text, true), new EntityField("Price", "Price", FieldKind.Decimal, false) },
        Array.Empty<EntityRelation>());

    [Fact]
    public void Decimal_cell_prints_two_places()
    {
        var cut = Grid(Gear,
            Row("""{"id":1,"name":"Stove","price":15}"""),
            Row("""{"id":2,"name":"Tent","price":15.5}"""),
            Row("""{"id":3,"name":"Cabin","price":1234.5}"""));

        var prices = cut.FindAll("[data-testid=grid-row]").Select(r => r.QuerySelectorAll("td")[1].TextContent.Trim()).ToList();
        Assert.Equal(new[] { "15.00", "15.50", "1234.50" }, prices);
    }

    [Fact]
    public void Decimal_field_round_trips_in_form()
    {
        var unchanged = SaveForm(Gear, Row("""{"id":1,"name":"Tent","price":15.5}"""))!;
        Assert.Equal(15.5m, unchanged["price"]!.GetValue<decimal>());

        var changed = SaveForm(Gear, Row("""{"id":1,"name":"Tent","price":15.5}"""),
            cut => cut.Find("[data-field=Price] input").Input("12.75"))!;
        Assert.Equal(12.75m, changed["price"]!.GetValue<decimal>());
        Assert.Equal("number", RenderComponent<TwEntityForm>(p => p.Add(f => f.Schema, Gear).Add(f => f.LoadOptions, Options))
            .Find("[data-field=Price] input").GetAttribute("type"));

        // Text that is not a number is refused, not sent.
        Assert.Null(SaveForm(Gear, Row("""{"id":1,"name":"Tent","price":15.5}"""),
            cut => cut.Find("[data-field=Price] input").Input("twelve")));
    }
}
