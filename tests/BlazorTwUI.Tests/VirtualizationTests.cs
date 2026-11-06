using BlazorTwUI.Components.UI;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace BlazorTwUI.Tests;

/// <summary>
/// Long lists render only the rows on screen (Blazor's Virtualize). 5,000 records must not become 5,000 DOM rows,
/// and search, sort and selection must still see every record, not only the rendered ones.
/// </summary>
public class VirtualizationTests : TestContext
{
    private sealed record Rec(int No, string Name);

    private static List<Rec> Many(int n) => Enumerable.Range(1, n).Select(i => new Rec(i, $"Item {i:D5}")).ToList();

    private static RenderFragment Cols => b =>
    {
        b.OpenComponent<TwDataGridColumn<Rec>>(0);
        b.AddAttribute(1, "Title", "No");
        b.AddAttribute(2, "Property", (Func<Rec, object?>)(r => r.No));
        b.CloseComponent();
        b.OpenComponent<TwDataGridColumn<Rec>>(3);
        b.AddAttribute(4, "Title", "Name");
        b.AddAttribute(5, "Property", (Func<Rec, object?>)(r => r.Name));
        b.CloseComponent();
    };

    private IRenderedComponent<TwDataGrid<Rec>> Grid(int n, Action<ComponentParameterCollectionBuilder<TwDataGrid<Rec>>>? more = null)
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        return RenderComponent<TwDataGrid<Rec>>(p =>
        {
            p.Add(g => g.Items, Many(n)).Add(g => g.Columns, Cols).Add(g => g.PageSize, 10).Add(g => g.Selectable, true);
            more?.Invoke(p);
        });
    }

    private static string[] Names(IRenderedComponent<TwDataGrid<Rec>> cut) =>
        cut.FindAll("table [data-testid=grid-row]").Select(r => r.QuerySelector("[data-col=Name]")!.TextContent.Trim()).ToArray();

    [Fact]
    public void Grid_with_5000_items_virtualizes_by_default_and_renders_few_rows()
    {
        var cut = Grid(5000);
        var rows = cut.FindAll("table [data-testid=grid-row]").Count;
        Assert.InRange(rows, 1, 79);
        Assert.Empty(cut.FindAll("nav[aria-label=Pagination]"));
        Assert.Equal("5000 records", cut.Find("[data-testid=grid-range]").TextContent.Trim());
        var table = cut.Find("[data-testid=grid-table]");
        Assert.Equal("true", table.GetAttribute("data-virtual"));
        Assert.Equal("5002", table.GetAttribute("aria-rowcount")); // header + filter row + 5000
        Assert.Equal("3", cut.Find("table [data-testid=grid-row]").GetAttribute("aria-rowindex"));
    }

    [Fact]
    public void Grid_virtual_search_and_sort_see_every_record()
    {
        var cut = Grid(5000);
        cut.Find("[data-testid=grid-search]").Input("Item 04999");
        Assert.Equal(new[] { "Item 04999" }, Names(cut));
        Assert.Equal("1 record", cut.Find("[data-testid=grid-range]").TextContent.Trim());

        cut.Find("[data-testid=grid-search]").Input("");
        cut.Find("[data-testid=grid-sort][data-col=No]").Click(); // ascending
        cut.Find("[data-testid=grid-sort][data-col=No]").Click(); // descending
        Assert.Equal("Item 05000", Names(cut)[0]);
    }

    [Fact]
    public void Grid_virtual_select_all_selects_every_filtered_record()
    {
        var cut = Grid(5000);
        cut.Find("[data-testid=grid-select-row]").Change(true);
        Assert.Equal("1 selected", cut.Find("[data-testid=grid-selected-count]").TextContent.Trim());
        cut.Find("[data-testid=grid-select-all]").Change(true);
        Assert.Equal("5000 selected", cut.Find("[data-testid=grid-selected-count]").TextContent.Trim());
    }

    [Fact]
    public void Grid_virtualize_false_keeps_paging()
    {
        var cut = Grid(5000, p => p.Add(g => g.Virtualize, false));
        Assert.Equal(10, cut.FindAll("table [data-testid=grid-row]").Count);
        Assert.NotEmpty(cut.FindAll("nav[aria-label=Pagination]"));
        Assert.Null(cut.Find("[data-testid=grid-table]").GetAttribute("data-virtual"));
    }

    [Fact]
    public void Grid_under_threshold_keeps_paging()
    {
        var cut = Grid(150);
        Assert.Equal(10, cut.FindAll("table [data-testid=grid-row]").Count);
        Assert.Equal("Showing 1 - 10 of 150", cut.Find("[data-testid=grid-range]").TextContent);
    }

    // ---- combobox ----

    private static IReadOnlyList<TwCombobox.ComboboxItem> Options(int n) =>
        Enumerable.Range(1, n).Select(i => new TwCombobox.ComboboxItem(i.ToString(), $"Option {i:D5}")).ToList();

    [Fact]
    public void Combobox_with_5000_options_renders_few_and_search_finds_far_items()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        string? chosen = null;
        var cut = RenderComponent<TwCombobox>(p => p.Add(c => c.Items, Options(5000)).Add(c => c.Label, "Owner")
            .Add(c => c.ValueChanged, EventCallback.Factory.Create<string>(this, v => chosen = v)));
        cut.Find("input").Focus();
        var rendered = cut.FindAll("[role=option]");
        Assert.InRange(rendered.Count, 1, 79);
        Assert.Equal("5000", rendered[0].GetAttribute("aria-setsize"));
        Assert.Equal("1", rendered[0].GetAttribute("aria-posinset"));
        Assert.Equal("listbox", cut.Find("[data-testid=combobox-list]").GetAttribute("role"));

        cut.Find("input").Input("04987");
        var hits = cut.FindAll("[role=option]");
        Assert.Single(hits);
        hits[0].Click();
        Assert.Equal("4987", chosen);
    }

    [Fact]
    public void Combobox_keyboard_reaches_items_beyond_the_rendered_window()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        string? chosen = null;
        var cut = RenderComponent<TwCombobox>(p => p.Add(c => c.Items, Options(5000))
            .Add(c => c.ValueChanged, EventCallback.Factory.Create<string>(this, v => chosen = v)));
        var input = cut.Find("input");
        input.Focus();
        for (var i = 0; i < 150; i++) cut.Find("input").KeyDown("ArrowDown");
        cut.Find("input").KeyDown("End");
        cut.Find("input").KeyDown("Enter");
        Assert.Equal("5000", chosen);
    }

    // ---- pick list (relation picker, many) ----

    private sealed record Opt(string Id, string Name);

    [Fact]
    public void PickList_with_5000_options_renders_few_rows_and_keeps_selection()
    {
        var source = Enumerable.Range(1, 5000).Select(i => new Opt(i.ToString(), $"Author {i:D5}")).ToList();
        var target = new List<Opt>();
        var cut = RenderComponent<TwPickList<Opt>>(p => p.Add(x => x.Source, source).Add(x => x.Target, target)
            .Add(x => x.ItemText, o => o.Name));
        var available = cut.FindAll("[data-testid=picklist-source] [role=option]");
        Assert.InRange(available.Count, 1, 79);
        Assert.Equal("5000", available[0].GetAttribute("aria-setsize"));

        cut.Find("[data-testid=picklist-source-filter]").Input("04321");
        var hit = Assert.Single(cut.FindAll("[data-testid=picklist-source] [role=option]"));
        hit.DoubleClick();
        Assert.Equal("Author 04321", Assert.Single(target).Name);
        cut.Find("[data-testid=picklist-source-filter]").Input("");
        // chosen item is no longer offered, the rest still are
        Assert.Equal("4999", cut.Find("[data-testid=picklist-source] [role=option]").GetAttribute("aria-setsize"));
        Assert.Contains("Author 04321", cut.Find("[data-testid=picklist-target]").TextContent);
    }

    // ---- list box ----

    [Fact]
    public void ListBox_with_5000_items_renders_few_and_select_all_covers_the_filter()
    {
        var items = Enumerable.Range(1, 5000).Select(i => $"Tag {i:D5}").ToList();
        IReadOnlyList<string> selected = Array.Empty<string>();
        var cut = RenderComponent<TwListBox<string>>(p => p.Add(x => x.Items, items).Add(x => x.Label, "Tags")
            .Add(x => x.SelectedChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, s => selected = s)));
        Assert.InRange(cut.FindAll("[data-testid=listbox-item]").Count, 1, 79);
        cut.Find("[data-testid=listbox-search]").Input("Tag 049");
        cut.Find("[data-testid=listbox-all]").Change(true);
        Assert.Equal(100, selected.Count);
    }

    // ---- relation picker on a NEW record (no Record yet) ----

    [Fact]
    public void RelationPicker_for_a_new_record_offers_every_option_and_writes_the_choice()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        IReadOnlyList<string> ids = Array.Empty<string>();
        var options = Enumerable.Range(1, 5000).Select(i => (i.ToString(), $"Supplier {i:D4}")).ToList();
        var cut = RenderComponent<TwRelationPicker>(p => p
            .Add(r => r.Relation, new BlazorTwUI.Models.EntityRelation("supplier", "Supplier", BlazorTwUI.Models.RelationKind.One, "Supplier"))
            .Add(r => r.LoadOptions, _ => Task.FromResult<IReadOnlyList<(string Id, string Name)>>(options))
            .Add(r => r.SelectedIdsChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => ids = v)));
        cut.Find("input").Focus();
        Assert.Equal("5000", cut.Find("[role=option]").GetAttribute("aria-setsize"));
        cut.Find("input").Input("4321");
        cut.Find("[role=option]").Click();
        Assert.Equal(new[] { "4321" }, ids);
    }
}
