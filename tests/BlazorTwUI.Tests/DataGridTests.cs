using BlazorTwUI.Components.UI;
using BlazorTwUI.Services;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Xunit;

namespace BlazorTwUI.Tests;

public class GridDataTests
{
    [Fact]
    public void Aggregates_IgnoreNullsAndNonNumbers_AndCountCountsPresentValues()
    {
        var values = new object?[] { 10, 20.5m, "30", null, "n/a", true, 'x' };
        Assert.Equal(60.5, GridAggregate.Compute(GridSummary.Sum, values));
        Assert.Equal(60.5 / 3, GridAggregate.Compute(GridSummary.Average, values));
        Assert.Equal(10, GridAggregate.Compute(GridSummary.Min, values));
        Assert.Equal(30, GridAggregate.Compute(GridSummary.Max, values));
        Assert.Equal(6, GridAggregate.Compute(GridSummary.Count, values)); // everything that is not null
        Assert.Null(GridAggregate.Compute(GridSummary.None, values));
        Assert.Null(GridAggregate.Compute(GridSummary.Sum, new object?[] { "a", null }));
    }

    [Fact]
    public void Format_UsesTheGivenFormat_OtherwiseTrimsTrailingZeros()
    {
        var us = System.Globalization.CultureInfo.GetCultureInfo("en-US");
        Assert.Equal("1,234", GridAggregate.Format(1234, null, us));
        Assert.Equal("1,234.57", GridAggregate.Format(1234.567, null, us));
        Assert.Equal("$1,235", GridAggregate.Format(1234.567, "C0", us));
    }

    [Theory]
    [InlineData("plain", "plain")]
    [InlineData("a,b", "\"a,b\"")]
    [InlineData("say \"hi\"", "\"say \"\"hi\"\"\"")]
    [InlineData("line1\nline2", "\"line1\nline2\"")]
    [InlineData(null, "")]
    [InlineData("=1+1", "'=1+1")]
    [InlineData("+cmd|' /C calc'!A0", "'+cmd|' /C calc'!A0")] // neutralised with a leading apostrophe
    [InlineData("@SUM(A1)", "'@SUM(A1)")]
    [InlineData("-5", "-5")]       // a real number stays a number
    [InlineData("+3.2", "+3.2")]
    [InlineData("-malware", "'-malware")]
    public void Csv_QuotesWhenNeeded_AndNeutralisesSpreadsheetFormulas(string? input, string expected) =>
        Assert.Equal(expected, GridCsv.Escape(input));

    [Fact]
    public void Csv_Build_WritesHeaderAndRowsWithCrLf()
    {
        var csv = GridCsv.Build(new[] { "Name", "Note" }, new[] { (IReadOnlyList<string?>)new string?[] { "Ann", "a,b" } });
        Assert.Equal("Name,Note\r\nAnn,\"a,b\"\r\n", csv);
    }
}

public class DataGridTests : TestContext
{
    private sealed record Emp(string Name, string Dept, decimal Salary, int Age);

    private sealed class Person
    {
        public string Name { get; set; } = "";
        public string City { get; set; } = "";
    }

    private static List<Emp> Staff() => new()
    {
        new("Zoe", "Sales", 50000m, 41), new("Adam", "Eng", 90000m, 30), new("Mia", "Eng", 110000m, 35),
        new("Liam", "Ops", 60000m, 28), new("Noah", "Sales", 70000m, 52), new("Eve", "Ops", 65000m, 45)
    };

    private sealed record Def(string Title, Func<Emp, object?> Prop, Dictionary<string, object?>? Extra = null);

    private static RenderFragment Cols(params Def[] defs) => b =>
    {
        var seq = 0;
        foreach (var d in defs)
        {
            b.OpenComponent<TwDataGridColumn<Emp>>(seq++);
            b.AddAttribute(seq++, "Title", d.Title);
            b.AddAttribute(seq++, "Property", d.Prop);
            if (d.Extra is not null)
                foreach (var (k, v) in d.Extra) b.AddAttribute(seq++, k, v);
            b.CloseComponent();
        }
    };

    private static Def[] StdCols(Dictionary<string, object?>? salary = null, Dictionary<string, object?>? name = null) => new[]
    {
        new Def("Name", e => e.Name, name), new Def("Dept", e => e.Dept),
        new Def("Salary", e => e.Salary, salary), new Def("Age", e => e.Age)
    };

    private IRenderedComponent<TwDataGrid<Emp>> Grid(Action<ComponentParameterCollectionBuilder<TwDataGrid<Emp>>>? more = null, Def[]? cols = null, List<Emp>? data = null, int pageSize = 10)
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        return RenderComponent<TwDataGrid<Emp>>(p =>
        {
            p.Add(g => g.Items, data ?? Staff()).Add(g => g.Columns, Cols(cols ?? StdCols())).Add(g => g.PageSize, pageSize);
            more?.Invoke(p);
        });
    }

    private static string[] Names(IRenderedComponent<TwDataGrid<Emp>> cut) =>
        cut.FindAll("[data-testid=grid-row]").Select(r => r.QuerySelector("[data-col=Name]")!.TextContent.Trim()).ToArray();

    [Fact]
    public void Renders_Rows_Headers_Caption_AndRangeText()
    {
        var cut = Grid(p => p.Add(g => g.Label, "Staff"), pageSize: 4);
        Assert.Equal(4, cut.FindAll("[data-testid=grid-row]").Count);
        Assert.Equal("Showing 1 - 4 of 6", cut.Find("[data-testid=grid-range]").TextContent);
        Assert.Equal("Staff", cut.Find("caption").TextContent);
        Assert.All(cut.FindAll("thead th[data-col]"), th => Assert.Equal("col", th.GetAttribute("scope")));
        cut.Find("[data-testid=grid-next]").Click();
        Assert.Equal("Showing 5 - 6 of 6", cut.Find("[data-testid=grid-range]").TextContent);
    }

    [Fact]
    public void Sorting_IsAButton_CyclesAscDescNone_AndExposesAriaSort()
    {
        var cut = Grid();
        string Sort(string col) => cut.Find($"th[data-col={col}]").GetAttribute("aria-sort")!;
        Assert.Equal("none", Sort("Name"));

        cut.Find("[data-testid=grid-sort][data-col=Name]").Click();
        Assert.Equal("ascending", Sort("Name"));
        Assert.Equal(new[] { "Adam", "Eve", "Liam", "Mia", "Noah", "Zoe" }, Names(cut));

        cut.Find("[data-testid=grid-sort][data-col=Name]").Click();
        Assert.Equal("descending", Sort("Name"));
        Assert.Equal("Zoe", Names(cut)[0]);

        cut.Find("[data-testid=grid-sort][data-col=Name]").Click();
        Assert.Equal("none", Sort("Name"));
        Assert.Equal("Zoe", Names(cut)[0]); // back to source order
    }

    [Fact]
    public void Sorting_ShiftClickAddsASecondarySort_WithPriorityNumbers_AndNumbersSortNumerically()
    {
        var cut = Grid();
        cut.Find("[data-testid=grid-sort][data-col=Dept]").Click();
        cut.Find("[data-testid=grid-sort][data-col=Salary]").Click(new MouseEventArgs { ShiftKey = true });
        cut.Find("[data-testid=grid-sort][data-col=Salary]").Click(new MouseEventArgs { ShiftKey = true }); // salary descending inside each dept

        Assert.Equal(new[] { "Mia", "Adam", "Eve", "Liam", "Noah", "Zoe" }, Names(cut));
        Assert.Contains("2", cut.Find("[data-testid=grid-sort][data-col=Salary]").TextContent);
        Assert.Equal("descending", cut.Find("th[data-col=Salary]").GetAttribute("aria-sort"));
    }

    [Fact]
    public void Search_ColumnFilter_AndReset_NarrowTheRows()
    {
        var cut = Grid();
        cut.Find("[data-testid=grid-search]").Input("eng");
        Assert.Equal(new[] { "Adam", "Mia" }, Names(cut).OrderBy(n => n));

        cut.Find("[data-testid=grid-search]").Input("");
        cut.Find("[data-testid=grid-col-filter][data-col=Name]").Input("o");
        Assert.Equal(new[] { "Noah", "Zoe" }, Names(cut).OrderBy(n => n));

        cut.Find("[data-testid=grid-reset]").Click();
        Assert.Equal(6, Names(cut).Length);
    }

    [Fact]
    public void Footer_SummarisesEveryFilteredRow_NotJustThePage_AndFollowsTheFilter()
    {
        var cut = Grid(null, StdCols(salary: new() { ["Summary"] = GridSummary.Sum, ["SummaryFormat"] = "N0" }).Concat(new[]
        {
            new Def("Avg age", e => e.Age, new() { ["Summary"] = GridSummary.Average, ["SummaryFormat"] = "0.0" })
        }).ToArray(), pageSize: 2);

        Assert.Equal(2, cut.FindAll("[data-testid=grid-row]").Count);
        Assert.Equal("Sum 445,000", cut.Find("[data-testid=grid-summary][data-col=Salary]").TextContent.Trim()); // all six, not the two on this page
        Assert.Equal("Avg 38.5", cut.Find("[data-testid=grid-summary][data-col='Avg age']").TextContent.Trim()); // (41+30+35+28+52+45)/6 = 38.5

        cut.Find("[data-testid=grid-search]").Input("eng");
        Assert.Equal("Sum 200,000", cut.Find("[data-testid=grid-summary][data-col=Salary]").TextContent.Trim());
        Assert.Equal("Totals", cut.Find("[data-testid=grid-summary][data-col=Name]").TextContent.Trim());
    }

    [Fact]
    public void Grouping_AddsHeadersWithCountsAndSummaries_AndCollapses()
    {
        var cut = Grid(p => p.Add(g => g.AllowGrouping, true), StdCols(salary: new() { ["Summary"] = GridSummary.Sum, ["SummaryFormat"] = "N0" }));
        cut.Find("[data-testid=grid-group-by]").Change("Dept");

        var headers = cut.FindAll("[data-testid=grid-group]");
        Assert.Equal(new[] { "Eng", "Ops", "Sales" }, headers.Select(h => h.GetAttribute("data-group")));
        Assert.Contains("(2)", headers[0].TextContent);
        Assert.Contains("Sum 200,000", headers[0].QuerySelector("[data-testid=grid-group-summary]")!.TextContent); // Adam + Mia

        cut.FindAll("[data-testid=grid-group-toggle]")[0].Click();
        Assert.Equal("true", cut.FindAll("[data-testid=grid-group]")[0].GetAttribute("data-collapsed"));
        Assert.Equal(4, cut.FindAll("[data-testid=grid-row]").Count); // Eng's two rows are hidden
        Assert.Equal("false", cut.FindAll("[data-testid=grid-group-toggle]")[0].GetAttribute("aria-expanded"));

        cut.Find("[data-testid=grid-group-by]").Change("");
        Assert.Empty(cut.FindAll("[data-testid=grid-group]"));
    }

    [Fact]
    public void MasterDetail_ExpandsARowBeneathIt_WithMatchingAriaWiring()
    {
        var cut = Grid(p => p.Add(g => g.DetailTemplate, (Emp e) => (RenderFragment)(b => b.AddMarkupContent(0, $"<span data-testid=\"detail-body\">Notes for {e.Name}</span>"))));
        Assert.Empty(cut.FindAll("[data-testid=grid-detail]"));

        var toggle = cut.FindAll("[data-testid=grid-detail-toggle]")[1];
        Assert.Equal("false", toggle.GetAttribute("aria-expanded"));
        var controls = toggle.GetAttribute("aria-controls");
        toggle.Click();

        Assert.Equal("Notes for Adam", cut.Find("[data-testid=detail-body]").TextContent);
        Assert.Equal(controls, cut.Find("[data-testid=grid-detail]").GetAttribute("id"));
        Assert.Equal("true", cut.FindAll("[data-testid=grid-detail-toggle]")[1].GetAttribute("aria-expanded"));

        cut.FindAll("[data-testid=grid-detail-toggle]")[1].Click();
        Assert.Empty(cut.FindAll("[data-testid=grid-detail]"));
    }

    private IRenderedComponent<TwDataGrid<Person>> PeopleGrid(List<Person> people, List<Person> saved, Func<string, string?>? validate = null)
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        RenderFragment cols = b =>
        {
            b.OpenComponent<TwDataGridColumn<Person>>(0);
            b.AddAttribute(1, "Title", "Name");
            b.AddAttribute(2, "Property", (Func<Person, object?>)(p => p.Name));
            b.AddAttribute(3, "Editable", true);
            b.AddAttribute(4, "EditValue", (Action<Person, string>)((p, v) => p.Name = v));
            if (validate is not null) b.AddAttribute(5, "Validate", validate);
            b.CloseComponent();
            b.OpenComponent<TwDataGridColumn<Person>>(10);
            b.AddAttribute(11, "Title", "City");
            b.AddAttribute(12, "Property", (Func<Person, object?>)(p => p.City));
            b.AddAttribute(13, "Editable", true);
            b.AddAttribute(14, "EditValue", (Action<Person, string>)((p, v) => p.City = v));
            b.CloseComponent();
        };
        return RenderComponent<TwDataGrid<Person>>(p => p.Add(g => g.Items, people).Add(g => g.Columns, cols)
            .Add(g => g.RowSaved, EventCallback.Factory.Create<Person>(this, x => saved.Add(x))));
    }

    [Fact]
    public void InlineEdit_StartsOneRowAtATime_AppliesValuesOnSave_AndNotifies()
    {
        var people = new List<Person> { new() { Name = "Ann", City = "Oslo" }, new() { Name = "Bob", City = "Rome" } };
        var saved = new List<Person>();
        var cut = PeopleGrid(people, saved);

        cut.FindAll("[data-testid=grid-edit]")[0].Click();
        Assert.Equal(2, cut.FindAll("[data-testid=grid-edit-input]").Count);
        Assert.True(cut.FindAll("[data-testid=grid-edit]")[0].HasAttribute("disabled")); // the other row cannot start editing meanwhile

        cut.Find("[data-testid=grid-edit-input][data-col=Name]").Input("Anna");
        cut.Find("[data-testid=grid-edit-input][data-col=City]").Input("Bergen");
        Assert.Equal("Ann", people[0].Name); // nothing changes until Save
        cut.Find("[data-testid=grid-save]").Click();

        Assert.Equal(("Anna", "Bergen"), (people[0].Name, people[0].City));
        Assert.Same(people[0], Assert.Single(saved));
        Assert.Empty(cut.FindAll("[data-testid=grid-edit-input]"));
    }

    [Fact]
    public void InlineEdit_CancelDiscards_EnterSaves_EscapeCancels()
    {
        var people = new List<Person> { new() { Name = "Ann", City = "Oslo" } };
        var saved = new List<Person>();
        var cut = PeopleGrid(people, saved);

        cut.Find("[data-testid=grid-edit]").Click();
        cut.Find("[data-testid=grid-edit-input][data-col=Name]").Input("Changed");
        cut.Find("[data-testid=grid-cancel]").Click();
        Assert.Equal("Ann", people[0].Name);

        cut.Find("[data-testid=grid-edit]").Click();
        cut.Find("[data-testid=grid-edit-input][data-col=Name]").Input("Zed");
        cut.Find("[data-testid=grid-edit-input][data-col=Name]").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        Assert.Equal("Ann", people[0].Name);
        Assert.Empty(saved);

        cut.Find("[data-testid=grid-edit]").Click();
        cut.Find("[data-testid=grid-edit-input][data-col=Name]").Input("Zed");
        cut.Find("[data-testid=grid-edit-input][data-col=Name]").KeyDown(new KeyboardEventArgs { Key = "Enter" });
        Assert.Equal("Zed", people[0].Name);
        Assert.Single(saved);
    }

    [Fact]
    public void InlineEdit_Validation_BlocksTheSave_AndExplainsWhy()
    {
        var people = new List<Person> { new() { Name = "Ann", City = "Oslo" } };
        var saved = new List<Person>();
        var cut = PeopleGrid(people, saved, validate: v => string.IsNullOrWhiteSpace(v) ? "Name is required" : null);

        cut.Find("[data-testid=grid-edit]").Click();
        cut.Find("[data-testid=grid-edit-input][data-col=Name]").Input("");
        cut.Find("[data-testid=grid-save]").Click();

        Assert.Equal("Name is required", cut.Find("[data-testid=grid-edit-error]").TextContent);
        Assert.Equal("true", cut.Find("[data-testid=grid-edit-input][data-col=Name]").GetAttribute("aria-invalid"));
        Assert.Equal("alert", cut.Find("[data-testid=grid-edit-error]").GetAttribute("role"));
        Assert.Equal("Ann", people[0].Name);
        Assert.Empty(saved);
        Assert.NotNull(cut.Find("[data-testid=grid-save]")); // still editing

        cut.Find("[data-testid=grid-edit-input][data-col=Name]").Input("Ann B");
        cut.Find("[data-testid=grid-save]").Click();
        Assert.Equal("Ann B", people[0].Name);
        Assert.Empty(cut.FindAll("[data-testid=grid-edit-error]"));
    }

    [Fact]
    public void ColumnChooser_HidesColumns_ButNeverTheLastOne_OrANonHideableOne()
    {
        var cols = new[]
        {
            new Def("Name", e => e.Name, new() { ["Hideable"] = false }), new Def("Dept", e => e.Dept), new Def("Age", e => e.Age)
        };
        var cut = Grid(p => p.Add(g => g.AllowColumnChooser, true), cols);
        cut.Find("[data-testid=grid-columns-button]").Click();
        Assert.True(cut.Find("[data-testid=grid-column-toggle][data-col=Name]").HasAttribute("disabled"));

        cut.Find("[data-testid=grid-column-toggle][data-col=Dept]").Change(false);
        Assert.Empty(cut.FindAll("th[data-col=Dept]"));
        Assert.Empty(cut.FindAll("[data-testid=grid-row] [data-col=Dept]"));

        cut.Find("[data-testid=grid-column-toggle][data-col=Age]").Change(false);
        // Only Name (not hideable) remains, and it cannot be the one that disappears.
        Assert.Single(cut.FindAll("th[data-col]"));
        Assert.True(cut.Find("[data-testid=grid-column-toggle][data-col=Name]").HasAttribute("disabled"));

        cut.Find("[data-testid=grid-column-toggle][data-col=Dept]").Change(true);
        Assert.Equal(2, cut.FindAll("th[data-col]").Count);
    }

    [Fact]
    public void Export_ContainsAllFilteredRows_VisibleColumnsOnly_AndNeutralisesFormulas()
    {
        var data = Staff();
        data.Add(new("=HYPERLINK(\"http://evil\")", "Ops", 1m, 20));
        var cut = Grid(p => p.Add(g => g.AllowExport, true).Add(g => g.ExportFileName, "staff.csv"), data: data, pageSize: 2);
        cut.Find("[data-testid=grid-search]").Input("ops");

        var csv = cut.Instance.BuildCsv();
        var lines = csv.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal("Name,Dept,Salary,Age", lines[0]);
        Assert.Equal(4, lines.Length); // header + Liam, Eve and the formula row: all pages, not just the visible two
        Assert.Contains("Liam,Ops,60000,28", lines);
        Assert.Contains(lines, l => l.StartsWith("\"'=HYPERLINK(\"\"http://evil\"\")\""));

        cut.Find("[data-testid=grid-export]").Click();
        var call = JSInterop.Invocations.Single(i => i.Identifier == "pdfHelper.downloadText");
        Assert.Equal("staff.csv", call.Arguments[0]);
        Assert.StartsWith("﻿Name,Dept", (string)call.Arguments[2]!);
    }

    [Fact]
    public void State_IsSavedToLocalStorage_AndRestoredOnTheNextVisit()
    {
        var cut = Grid(p => p.Add(g => g.StateKey, "staff").Add(g => g.AllowColumnChooser, true).Add(g => g.AllowGrouping, true));
        cut.Find("[data-testid=grid-sort][data-col=Age]").Click();
        cut.Find("[data-testid=grid-search]").Input("o");
        cut.Find("[data-testid=grid-group-by]").Change("Dept");
        cut.Find("[data-testid=grid-columns-button]").Click();
        cut.Find("[data-testid=grid-column-toggle][data-col=Salary]").Change(false);

        var json = (string)JSInterop.Invocations.Last(i => i.Identifier == "localStorage.setItem").Arguments[1]!;
        Assert.Contains("\"Group\":\"Dept\"", json);
        Assert.Contains("Salary", json);
        Assert.Contains("\"Search\":\"o\"", json);

        // A new grid reads that state back.
        JSInterop.Setup<string?>("localStorage.getItem", "btui-grid:staff").SetResult(json);
        var next = Grid(p => p.Add(g => g.StateKey, "staff").Add(g => g.AllowGrouping, true));
        next.WaitForAssertion(() =>
        {
            Assert.Empty(next.FindAll("th[data-col=Salary]"));
            Assert.Equal("ascending", next.Find("th[data-col=Age]").GetAttribute("aria-sort"));
            Assert.NotEmpty(next.FindAll("[data-testid=grid-group]"));
            Assert.Equal("o", next.Find("[data-testid=grid-search]").GetAttribute("value"));
        });
    }

    [Fact]
    public void ConditionalFormatting_AppliesRowAndCellClasses_AndFrozenAndMaxHeightAreHonoured()
    {
        var cols = new[]
        {
            new Def("Name", e => e.Name, new() { ["Frozen"] = true }),
            new Def("Salary", e => e.Salary, new() { ["CellClassSelector"] = (Func<Emp, string?>)(e => e.Salary >= 100000m ? "text-emerald-700 font-bold" : null) })
        };
        var cut = Grid(p => p.Add(g => g.RowClassSelector, (Func<Emp, string?>)(e => e.Age > 50 ? "bg-amber-50" : null)).Add(g => g.MaxHeight, "20rem"), cols);

        Assert.Contains("bg-amber-50", cut.FindAll("[data-testid=grid-row]").Single(r => r.TextContent.Contains("Noah")).GetAttribute("class"));
        Assert.DoesNotContain("bg-amber-50", cut.FindAll("[data-testid=grid-row]").Single(r => r.TextContent.Contains("Zoe")).GetAttribute("class"));
        Assert.Contains("font-bold", cut.FindAll("[data-testid=grid-row]").Single(r => r.TextContent.Contains("Mia")).QuerySelector("[data-col=Salary]")!.GetAttribute("class"));
        Assert.Contains("sticky", cut.Find("th[data-col=Name]").GetAttribute("class"));
        Assert.Contains("max-height:20rem", cut.Find("[data-testid=grid-table]").ParentElement!.GetAttribute("style"));
        Assert.Contains("sticky", cut.Find("thead").GetAttribute("class"));
    }

    [Fact]
    public void Selection_SelectAllOnPage_CountsAndClears_WithLabelledCheckboxes()
    {
        var selection = new List<int>();
        var cut = Grid(p => p.Add(g => g.Selectable, true)
            .Add(g => g.SelectionChanged, EventCallback.Factory.Create<IReadOnlyList<Emp>>(this, s => selection.Add(s.Count))), pageSize: 4);

        Assert.Equal("Select all rows on this page", cut.Find("[data-testid=grid-select-all]").GetAttribute("aria-label"));
        Assert.Equal("Select row 1", cut.FindAll("[data-testid=grid-select-row]")[0].GetAttribute("aria-label"));
        cut.Find("[data-testid=grid-select-all]").Change(true);
        Assert.Equal("4 selected", cut.Find("[data-testid=grid-selected-count]").TextContent);
        Assert.Equal("true", cut.FindAll("[data-testid=grid-row]")[0].GetAttribute("aria-selected"));

        cut.FindAll("[data-testid=grid-select-row]")[0].Change(false);
        Assert.Equal("3 selected", cut.Find("[data-testid=grid-selected-count]").TextContent);
        Assert.Equal(new[] { 4, 3 }, selection);
    }

    [Fact]
    public void EmptyResults_ShowTheMessage_AndOfferAReset()
    {
        var cut = Grid();
        cut.Find("[data-testid=grid-search]").Input("zzzz");
        Assert.NotNull(cut.Find("[data-testid=grid-empty]"));
        cut.Find("[data-testid=grid-empty] button").Click();
        Assert.Equal(6, Names(cut).Length);
    }
}
