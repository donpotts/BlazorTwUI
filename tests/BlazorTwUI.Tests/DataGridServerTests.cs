using BlazorTwUI.Components.UI;
using BlazorTwUI.Models;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace BlazorTwUI.Tests;

public class DataGridServerTests : TestContext
{
    private sealed record Row(string Name);

    private static RenderFragment Cols(string? sortKey = null) => b =>
    {
        b.OpenComponent<TwDataGridColumn<Row>>(0);
        b.AddAttribute(1, "Title", "Name");
        if (sortKey is not null) b.AddAttribute(3, "SortKey", sortKey);
        b.AddAttribute(2, "Property", (Func<Row, object?>)(r => r.Name));
        b.CloseComponent();
    };

    private IRenderedComponent<TwDataGrid<Row>> Grid(Func<GridDataRequest, Task<GridDataResult<Row>>> provider, int pageSize = 5)
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        return RenderComponent<TwDataGrid<Row>>(p => p
            .Add(g => g.ServerData, provider).Add(g => g.Columns, Cols())
            .Add(g => g.PageSize, pageSize).Add(g => g.SearchDebounceMs, 0));
    }

    [Fact]
    public void ServerData_receives_search_text_and_page()
    {
        var calls = new List<GridDataRequest>();
        var cut = Grid(r => { calls.Add(r); return Task.FromResult(new GridDataResult<Row>(new[] { new Row("A") }, 40)); });
        Assert.Equal(new GridDataRequest(0, 5, null, null, false), calls[0]);

        cut.Find("[data-testid=grid-search]").Input("tolkien");
        Assert.Equal(new GridDataRequest(0, 5, "tolkien", null, false), calls[^1]);

        cut.Find("[data-testid=grid-next]").Click();
        Assert.Equal(new GridDataRequest(5, 5, "tolkien", null, false), calls[^1]);

        cut.Find("[data-testid=grid-sort][data-col=Name]").Click();
        Assert.Equal("Name", calls[^1].SortBy);
        Assert.False(calls[^1].Descending);
    }

    [Fact]
    public void ServerData_hides_column_filter_row_and_toggle()
    {
        var cut = Grid(_ => Task.FromResult(new GridDataResult<Row>(new[] { new Row("A") }, 1)));
        Assert.Empty(cut.FindAll("[data-testid=grid-filter-toggle]"));
        Assert.Empty(cut.FindAll("[data-testid=grid-filter-row]"));
        Assert.Empty(cut.FindAll("[data-testid=grid-col-filter]"));
    }

    [Fact]
    public void Items_mode_still_shows_column_filters()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var cut = RenderComponent<TwDataGrid<Row>>(p => p.Add(g => g.Items, new[] { new Row("A") }).Add(g => g.Columns, Cols()));
        Assert.Single(cut.FindAll("[data-testid=grid-filter-toggle]"));
        Assert.Single(cut.FindAll("[data-testid=grid-filter-row]"));
    }

    [Fact]
    public void Sort_sends_SortKey_when_set_else_Title()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var calls = new List<GridDataRequest>();
        var cut = RenderComponent<TwDataGrid<Row>>(p => p
            .Add(g => g.ServerData, r => { calls.Add(r); return Task.FromResult(new GridDataResult<Row>(Array.Empty<Row>(), 0)); })
            .Add(g => g.Columns, Cols("name_key")).Add(g => g.SearchDebounceMs, 0));
        cut.Find("[data-testid=grid-sort][data-col=Name]").Click();
        Assert.Equal("name_key", calls[^1].SortBy);
    }

    [Fact]
    public async Task RefreshAsync_recalls_provider_with_same_request()
    {
        var calls = new List<GridDataRequest>();
        var cut = Grid(r => { calls.Add(r); return Task.FromResult(new GridDataResult<Row>(Array.Empty<Row>(), 0)); });
        cut.Find("[data-testid=grid-search]").Input("abc");
        var n = calls.Count;
        await cut.InvokeAsync(() => cut.Instance.RefreshAsync());
        Assert.Equal(n + 1, calls.Count);
        Assert.Equal(calls[n - 1], calls[n]);
    }

    [Fact]
    public void Footer_shows_TotalCount_not_page_length()
    {
        var cut = Grid(_ => Task.FromResult(new GridDataResult<Row>(new[] { new Row("A"), new Row("B") }, 40)));
        Assert.Contains("of 40", cut.Find("[data-testid=grid-range]").TextContent);
        Assert.Contains("of 8", cut.Find("[data-testid=grid-page-label]").TextContent);
    }

    [Fact]
    public void Empty_search_shows_no_match_message()
    {
        var cut = Grid(r => Task.FromResult(new GridDataResult<Row>(r.Search is null ? new[] { new Row("A") } : Array.Empty<Row>(), r.Search is null ? 1 : 0)));
        cut.Find("[data-testid=grid-search]").Input("zzz");
        var text = cut.Find("[data-testid=grid-empty]").TextContent;
        Assert.Contains("zzz", text);
        Assert.Contains("match", text, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Search_with_quote_and_percent_reaches_provider_verbatim()
    {
        string? seen = null;
        var cut = Grid(r => { seen = r.Search; return Task.FromResult(new GridDataResult<Row>(Array.Empty<Row>(), 0)); });
        var term = "O'Brien 100% \"é\"";
        cut.Find("[data-testid=grid-search]").Input(term);
        Assert.Equal(term, seen);
    }
}
