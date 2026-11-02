using BlazorTwUI.Components.UI;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;

namespace BlazorTwUI.Tests;

public class LayoutPieceTests : TestContext
{
    [Fact]
    public void LoadingPanel_CoversTheRegion_AndMarksItBusy()
    {
        var cut = RenderComponent<TwLoadingPanel>(p => p.Add(c => c.Message, "Saving…").AddChildContent("<p>content</p>"));
        Assert.Empty(cut.FindAll("[data-testid=loading-overlay]"));
        Assert.Equal("false", cut.Find("[data-testid=loading-panel]").GetAttribute("aria-busy"));

        cut.SetParametersAndRender(p => p.Add(c => c.Visible, true));
        Assert.Equal("Saving…", cut.Find("[data-testid=loading-message]").TextContent);
        Assert.Equal("true", cut.Find("[data-testid=loading-panel]").GetAttribute("aria-busy"));
        Assert.True(cut.Find("[data-testid=loading-panel] > div").HasAttribute("inert")); // covered content cannot be tabbed into
    }

    [Fact]
    public void StackLayout_MapsOptionsToClasses_AndClampsTheGap()
    {
        var cut = RenderComponent<TwStackLayout>(p => p.Add(c => c.Horizontal, true).Add(c => c.Gap, 99).Add(c => c.Justify, "between").Add(c => c.Align, "center").Add(c => c.Wrap, true));
        var cls = cut.Find("[data-testid=stack-layout]").GetAttribute("class")!;
        Assert.Contains("flex-row", cls);
        Assert.Contains("gap-8", cls); // 99 clamps to the largest step
        Assert.Contains("justify-between", cls);
        Assert.Contains("items-center", cls);
        Assert.Contains("flex-wrap", cls);
        Assert.Equal("horizontal", cut.Find("[data-testid=stack-layout]").GetAttribute("data-orientation"));
    }

    [Theory]
    [InlineData(1, "grid-cols-1")]
    [InlineData(2, "lg:grid-cols-2")]
    [InlineData(3, "lg:grid-cols-3")]
    [InlineData(9, "lg:grid-cols-4")]
    public void FormLayout_ColumnsClamp(int columns, string expected)
    {
        var cut = RenderComponent<TwFormLayout>(p => p.Add(c => c.Columns, columns));
        Assert.Contains(expected, cut.Find("[data-testid=form-layout]").GetAttribute("class"));
    }

    [Fact]
    public void FormGroup_Collapses_AndExposesItsState()
    {
        var cut = RenderComponent<TwFormGroup>(p => p.Add(c => c.Title, "Billing").Add(c => c.Collapsible, true));
        Assert.Equal("true", cut.Find("[data-testid=form-group-toggle]").GetAttribute("aria-expanded"));

        cut.Find("[data-testid=form-group-toggle]").Click();
        Assert.Equal("false", cut.Find("[data-testid=form-group-toggle]").GetAttribute("aria-expanded"));
        Assert.Equal("false", cut.Find("[data-testid=form-group]").GetAttribute("data-open"));

        var starts = RenderComponent<TwFormGroup>(p => p.Add(c => c.Title, "Advanced").Add(c => c.Collapsible, true).Add(c => c.InitiallyCollapsed, true));
        Assert.Equal("false", starts.Find("[data-testid=form-group]").GetAttribute("data-open"));
    }

    [Fact]
    public void FormItem_LabelsTheField_MarksRequired_AndPrefersTheErrorOverTheHint()
    {
        var cut = RenderComponent<TwFormItem>(p => p
            .Add(c => c.Label, "Email").Add(c => c.FieldId, "email").Add(c => c.Required, true)
            .Add(c => c.Hint, "We never share it").Add(c => c.ColSpan, 2)
            .AddChildContent("<input id=\"email\" />"));

        Assert.Equal("email", cut.Find("label").GetAttribute("for"));
        Assert.Contains("(required)", cut.Find("label").TextContent);
        Assert.Contains("We never share it", cut.Markup);
        Assert.Contains("md:col-span-2", cut.Find("[data-testid=form-item]").GetAttribute("class"));

        cut.SetParametersAndRender(p => p.Add(c => c.Error, "Enter a valid email"));
        Assert.Equal("alert", cut.Find("[data-testid=form-item-error]").GetAttribute("role"));
        Assert.DoesNotContain("We never share it", cut.Markup);
    }
}

public class ToolbarAndPickerTests : TestContext
{
    private static List<TwToolbar.ToolbarItem> Items() => new()
    {
        new() { Id = "new", Text = "New" },
        new() { Id = "bold", Text = "Bold", Kind = TwToolbar.ToolbarKind.Toggle },
        new() { Kind = TwToolbar.ToolbarKind.Separator },
        new() { Id = "print", Text = "Print" },
        new() { Id = "export", Text = "Export" }
    };

    [Fact]
    public void Toolbar_MovesOverflowItemsIntoAMenu_AndReportsClicksFromBoth()
    {
        var clicked = new List<string>();
        var cut = RenderComponent<TwToolbar>(p => p.Add(c => c.Items, Items()).Add(c => c.MaxVisible, 3)
            .Add(c => c.OnItemClick, EventCallback.Factory.Create<TwToolbar.ToolbarItem>(this, i => clicked.Add(i.Id))));

        Assert.Equal(new[] { "new", "bold" }, cut.FindAll("[data-testid=toolbar-item]").Select(b => b.GetAttribute("data-id")));
        Assert.Empty(cut.FindAll("[data-testid=toolbar-overflow]"));

        cut.Find("[data-testid=toolbar-more]").Click();
        Assert.Equal(new[] { "print", "export" }, cut.FindAll("[data-testid=toolbar-overflow-item]").Select(b => b.GetAttribute("data-id")));
        cut.FindAll("[data-testid=toolbar-overflow-item]")[1].Click();
        cut.FindAll("[data-testid=toolbar-item]")[0].Click();

        Assert.Equal(new[] { "export", "new" }, clicked);
        Assert.Empty(cut.FindAll("[data-testid=toolbar-overflow]")); // the menu closes after a pick
    }

    [Fact]
    public void Toolbar_ToggleButtonsKeepTheirPressedState()
    {
        var cut = RenderComponent<TwToolbar>(p => p.Add(c => c.Items, Items()));
        var bold = () => cut.Find("[data-id=bold]");
        Assert.Equal("false", bold().GetAttribute("aria-pressed"));
        bold().Click();
        Assert.Equal("true", bold().GetAttribute("aria-pressed"));
    }

    [Fact]
    public void Toolbar_ArrowKeysMoveTheSingleTabStop_AndWrap()
    {
        var cut = RenderComponent<TwToolbar>(p => p.Add(c => c.Items, Items()));
        string Stop() => cut.FindAll("[data-testid=toolbar-item]").Single(b => b.GetAttribute("tabindex") == "0").GetAttribute("data-id")!;
        Assert.Equal("new", Stop());

        cut.Find("[role=toolbar]").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        Assert.Equal("bold", Stop());
        cut.Find("[role=toolbar]").KeyDown(new KeyboardEventArgs { Key = "End" });
        Assert.Equal("export", Stop());
        cut.Find("[role=toolbar]").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        Assert.Equal("new", Stop()); // wraps, skipping the separator on the way
    }

    [Fact]
    public void DropDownBox_OpensAndCloses_ByTriggerEscapeAndBackdrop()
    {
        var opens = new List<bool>();
        RenderFragment<TwDropDownBox> content = _ => b => b.AddMarkupContent(0, "<span data-testid=\"inside\">panel</span>");
        var cut = RenderComponent<TwDropDownBox>(p => p.Add(c => c.Placeholder, "Pick a customer")
            .Add(c => c.OpenChanged, EventCallback.Factory.Create<bool>(this, v => opens.Add(v)))
            .Add(c => c.ChildContent, content));

        Assert.Equal("Pick a customer", cut.Find("[data-testid=dropdown-text]").TextContent);
        Assert.Equal("false", cut.Find("[data-testid=dropdown-trigger]").GetAttribute("aria-expanded"));

        cut.Find("[data-testid=dropdown-trigger]").Click();
        Assert.NotNull(cut.Find("[data-testid=inside]"));
        Assert.Equal("true", cut.Find("[data-testid=dropdown-trigger]").GetAttribute("aria-expanded"));

        cut.Find("[data-testid=dropdown-box]").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        Assert.Empty(cut.FindAll("[data-testid=dropdown-panel]"));

        cut.Find("[data-testid=dropdown-trigger]").Click();
        cut.Find("[data-testid=dropdown-backdrop]").Click();
        Assert.Empty(cut.FindAll("[data-testid=dropdown-panel]"));
        Assert.Equal(new[] { true, false, true, false }, opens);
    }

    [Fact]
    public void DropDownBox_ShowsItsTextOnceSet()
    {
        var cut = RenderComponent<TwDropDownBox>(p => p.Add(c => c.Text, "Acme Corp").Add(c => c.Placeholder, "Pick"));
        Assert.Equal("Acme Corp", cut.Find("[data-testid=dropdown-text]").TextContent);
    }

    [Fact]
    public void ListBox_SearchFilters_SelectAllOnlyAffectsWhatIsShown_AndCountsSelections()
    {
        IReadOnlyList<string> selected = Array.Empty<string>();
        var cut = RenderComponent<TwListBox<string>>(p => p
            .Add(c => c.Items, new[] { "Apple", "Apricot", "Banana", "Cherry" })
            .Add(c => c.SelectedChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, s => selected = s)));

        cut.Find("[data-testid=listbox-search]").Input("ap");
        Assert.Equal(new[] { "Apple", "Apricot" }, cut.FindAll("[data-testid=listbox-item]").Select(i => i.GetAttribute("data-text")));

        cut.Find("[data-testid=listbox-all]").Change(true);
        Assert.Equal(new[] { "Apple", "Apricot" }, selected);
        Assert.Equal("2 of 4 selected", cut.Find("[data-testid=listbox-count]").TextContent);

        cut.Find("[data-testid=listbox-search]").Input("");
        cut.FindAll("[data-testid=listbox-item]").Single(i => i.GetAttribute("data-text") == "Banana").Change(true);
        Assert.Equal(3, selected.Count);

        cut.Find("[data-testid=listbox-search]").Input("zzz");
        Assert.NotNull(cut.Find("[data-testid=listbox-empty]"));
    }

    [Fact]
    public void ListBox_SingleMode_UsesRadiosAndKeepsOneValue()
    {
        IReadOnlyList<int> selected = Array.Empty<int>();
        var cut = RenderComponent<TwListBox<int>>(p => p.Add(c => c.Items, new[] { 1, 2, 3 }).Add(c => c.Multiple, false)
            .Add(c => c.SelectedChanged, EventCallback.Factory.Create<IReadOnlyList<int>>(this, s => selected = s)));

        Assert.All(cut.FindAll("[data-testid=listbox-item]"), i => Assert.Equal("radio", i.GetAttribute("type")));
        Assert.Empty(cut.FindAll("[data-testid=listbox-all]"));
        cut.FindAll("[data-testid=listbox-item]")[0].Change(true);
        cut.FindAll("[data-testid=listbox-item]")[2].Change(true);
        Assert.Equal(new[] { 3 }, selected);
    }
}

public class DataVisualTests : TestContext
{
    [Fact]
    public void BarGauge_ScalesEachValueToItsArc_AndPutsTheNumbersInTheLegend()
    {
        var cut = RenderComponent<TwBarGauge>(p => p.Add(c => c.Unit, "%").Add(c => c.Items, new[]
        {
            new TwBarGauge.GaugeItem("CPU", 75), new TwBarGauge.GaugeItem("Memory", 30), new TwBarGauge.GaugeItem("Over", 250), new TwBarGauge.GaugeItem("Under", -10)
        }));

        var arcs = cut.FindAll("[data-testid=bar-gauge-arc]").ToDictionary(a => a.GetAttribute("data-label")!, a => double.Parse(a.GetAttribute("data-fraction")!, System.Globalization.CultureInfo.InvariantCulture));
        Assert.Equal(0.75, arcs["CPU"]);
        Assert.Equal(0.3, arcs["Memory"]);
        Assert.Equal(1.0, arcs["Over"]);   // clamped to the maximum
        Assert.Equal(0.0, arcs["Under"]);  // clamped to the minimum
        Assert.Contains("CPU", cut.FindAll("[data-testid=bar-gauge-legend]")[0].TextContent);
        Assert.Contains("75%", cut.FindAll("[data-testid=bar-gauge-legend]")[0].TextContent);
        Assert.Contains("CPU 75%", cut.Find("svg").GetAttribute("aria-label"));
    }

    [Fact]
    public void RangeSelector_KeepsStartBeforeEnd_AndReportsBothEnds()
    {
        int start = -1, end = -1;
        var cut = RenderComponent<TwRangeSelector>(p => p
            .Add(c => c.Values, new double[] { 3, 5, 2, 8, 6, 9 }).Add(c => c.Labels, new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun" })
            .Add(c => c.Start, 1).Add(c => c.End, 4)
            .Add(c => c.StartChanged, EventCallback.Factory.Create<int>(this, v => start = v))
            .Add(c => c.EndChanged, EventCallback.Factory.Create<int>(this, v => end = v)));

        Assert.Equal("Feb", cut.Find("[data-testid=range-start-label]").TextContent);
        Assert.Equal("4 of 6 points selected", cut.Find("[data-testid=range-summary]").TextContent);

        cut.Find("[data-testid=range-start]").Input("5");   // past the end: stops at it
        Assert.Equal(4, start);
        cut.Find("[data-testid=range-end]").Input("0");     // before the start: stops at it
        Assert.Equal(4, end);
        cut.Find("[data-testid=range-end]").Input("5");
        Assert.Equal(5, end);
        Assert.Equal("Jun", cut.Find("[data-testid=range-end-label]").TextContent);
    }

    private sealed record Node(string Id, string? Parent, string Name, int Size);

    private static List<Node> Nodes() => new()
    {
        new("root", null, "Company", 0), new("eng", "root", "Engineering", 40), new("ops", "root", "Operations", 12),
        new("fe", "eng", "Frontend", 15), new("be", "eng", "Backend", 25), new("orphan", "ghost", "Orphan", 1)
    };

    private IRenderedComponent<TwTreeList<Node>> Tree(bool expanded = false, Action<string?>? onSelect = null) =>
        RenderComponent<TwTreeList<Node>>(p => p
            .Add(c => c.Items, Nodes()).Add(c => c.KeySelector, n => n.Id).Add(c => c.ParentSelector, n => n.Parent)
            .Add(c => c.InitiallyExpanded, expanded)
            .Add(c => c.Columns, new[] { new TwTreeList<Node>.Column("Name", n => n.Name), new TwTreeList<Node>.Column("Size", n => n.Size.ToString()) })
            .Add(c => c.SelectedKeyChanged, EventCallback.Factory.Create<string?>(this, k => onSelect?.Invoke(k))));

    private static string[] Keys(IRenderedComponent<TwTreeList<Node>> cut) => cut.FindAll("[data-testid=tree-row]").Select(r => r.GetAttribute("data-key")!).ToArray();

    [Fact]
    public void TreeList_ShowsRoots_ExpandsLevelByLevel_AndTreatsMissingParentsAsRoots()
    {
        var cut = Tree();
        Assert.Equal(new[] { "root", "orphan" }, Keys(cut));

        cut.Find("[data-key=root] [data-testid=tree-toggle]").Click();
        Assert.Equal(new[] { "root", "eng", "ops", "orphan" }, Keys(cut));
        Assert.Equal("true", cut.Find("[data-key=root]").GetAttribute("aria-expanded"));
        Assert.Equal("2", cut.Find("[data-key=eng]").GetAttribute("aria-level"));

        cut.Find("[data-key=eng] [data-testid=tree-toggle]").Click();
        Assert.Equal(new[] { "root", "eng", "fe", "be", "ops", "orphan" }, Keys(cut));
        Assert.Equal("2", cut.Find("[data-key=fe]").GetAttribute("data-depth"));
    }

    [Fact]
    public void TreeList_ExpandAllCollapseAll_AndKeyboard()
    {
        var cut = Tree();
        cut.Find("[data-testid=tree-expand-all]").Click();
        Assert.Equal(6, Keys(cut).Length);
        cut.Find("[data-testid=tree-collapse-all]").Click();
        Assert.Equal(2, Keys(cut).Length);

        cut.Find("[data-key=root]").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        Assert.Equal("true", cut.Find("[data-key=root]").GetAttribute("aria-expanded"));
        cut.Find("[data-key=root]").KeyDown(new KeyboardEventArgs { Key = "ArrowLeft" });
        Assert.Equal("false", cut.Find("[data-key=root]").GetAttribute("aria-expanded"));
    }

    [Fact]
    public void TreeList_SortsWithinEachLevel_NumbersNumerically_AndSelectsRows()
    {
        string? selected = null;
        var cut = Tree(expanded: true, onSelect: k => selected = k);
        Assert.Equal(new[] { "root", "eng", "fe", "be", "ops", "orphan" }, Keys(cut)); // source order

        cut.Find("[data-testid=tree-sort][data-col=Size]").Click();          // ascending, within each level only
        Assert.Equal(new[] { "root", "ops", "eng", "fe", "be", "orphan" }, Keys(cut));
        Assert.Equal("ascending", cut.Find("th:nth-child(2)").GetAttribute("aria-sort"));

        cut.Find("[data-testid=tree-sort][data-col=Size]").Click();          // descending
        Assert.Equal(new[] { "orphan", "root", "eng", "be", "fe", "ops" }, Keys(cut));

        cut.Find("[data-key=fe]").Click();
        Assert.Equal("fe", selected);
        Assert.Equal("true", cut.Find("[data-key=fe]").GetAttribute("aria-selected"));
    }
}

public class SchedulerAndGanttTests : TestContext
{
    private static readonly DateOnly Wed = new(2026, 3, 11); // week of Mon 9 to Sun 15 March

    private static List<TwScheduler.Appointment> Appts() => new()
    {
        new() { Id = "a", Title = "Standup", Start = new DateTime(2026, 3, 11, 9, 0, 0), End = new DateTime(2026, 3, 11, 9, 30, 0) },
        new() { Id = "b", Title = "Design review", Start = new DateTime(2026, 3, 11, 9, 0, 0), End = new DateTime(2026, 3, 11, 10, 0, 0), Color = "emerald" },
        new() { Id = "c", Title = "Lunch", Start = new DateTime(2026, 3, 12, 12, 0, 0), End = new DateTime(2026, 3, 12, 13, 0, 0) }
    };

    private IRenderedComponent<TwScheduler> Sched(List<TwScheduler.Appointment>? items = null, TwScheduler.SchedulerView view = TwScheduler.SchedulerView.Week, Action<DateTime>? slot = null, Action<TwScheduler.Appointment>? moved = null) =>
        RenderComponent<TwScheduler>(p => p
            .Add(c => c.Appointments, items ?? Appts()).Add(c => c.Date, Wed).Add(c => c.View, view).Add(c => c.TodayOverride, Wed)
            .Add(c => c.CultureOverride, System.Globalization.CultureInfo.InvariantCulture)
            .Add(c => c.OnSlotClick, EventCallback.Factory.Create<DateTime>(this, d => slot?.Invoke(d)))
            .Add(c => c.OnAppointmentMoved, EventCallback.Factory.Create<TwScheduler.Appointment>(this, a => moved?.Invoke(a))));

    [Fact]
    public void Scheduler_WeekView_ShowsSevenColumns_Title_AndNavigates()
    {
        var cut = Sched();
        Assert.Equal(7, cut.FindAll("[data-testid=sch-col]").Count);
        Assert.Equal("2026-03-09", cut.FindAll("[data-testid=sch-col]")[0].GetAttribute("data-date")); // Monday first
        Assert.Equal("Mar 9 – Mar 15, 2026", cut.Find("[data-testid=sch-title]").TextContent);

        cut.Find("[data-testid=sch-next]").Click();
        Assert.Equal("Mar 16 – Mar 22, 2026", cut.Find("[data-testid=sch-title]").TextContent);
        cut.Find("[data-testid=sch-today]").Click();
        Assert.Equal("Mar 9 – Mar 15, 2026", cut.Find("[data-testid=sch-title]").TextContent);
    }

    [Fact]
    public void Scheduler_SlotClick_ReportsTheExactTime()
    {
        DateTime? clicked = null;
        var cut = Sched(slot: d => clicked = d);
        cut.Find("[data-testid=sch-slot][data-start='2026-03-12T10:30']").Click();
        Assert.Equal(new DateTime(2026, 3, 12, 10, 30, 0), clicked);
    }

    [Fact]
    public void Scheduler_OverlappingAppointments_ShareTheColumn_InSeparateLanes()
    {
        var cut = Sched();
        var wed = cut.Find("[data-testid=sch-col][data-date='2026-03-11']");
        var boxes = wed.QuerySelectorAll("div.absolute").Select(d => d.GetAttribute("style")!).ToList();
        Assert.Equal(2, boxes.Count);
        Assert.Contains(boxes, s => s.Contains("left:0%") && s.Contains("width:50%"));
        Assert.Contains(boxes, s => s.Contains("left:50%") && s.Contains("width:50%"));

        var thu = cut.Find("[data-testid=sch-col][data-date='2026-03-12']").QuerySelector("div.absolute")!.GetAttribute("style")!;
        Assert.Contains("width:100%", thu); // alone: full width
        Assert.Contains("top:256px", thu);  // 12:00 is four hours after 08:00: 8 half-hour slots x 32px
    }

    [Fact]
    public void Scheduler_DragAndDrop_MovesTheAppointment_KeepingItsLength()
    {
        TwScheduler.Appointment? moved = null;
        var items = Appts();
        var cut = Sched(items, moved: a => moved = a);

        cut.Find("[data-testid=sch-appt][data-id=b]").DragStart();
        cut.Find("[data-testid=sch-slot][data-start='2026-03-13T14:00']").Drop();

        Assert.Same(items[1], moved);
        Assert.Equal(new DateTime(2026, 3, 13, 14, 0, 0), moved!.Start);
        Assert.Equal(new DateTime(2026, 3, 13, 15, 0, 0), moved.End); // still one hour
    }

    [Fact]
    public void Scheduler_KeyboardMove_AltArrowsShiftBySlotOrDay()
    {
        var items = Appts();
        var moves = new List<DateTime>();
        var cut = Sched(items, moved: a => moves.Add(a.Start));

        cut.Find("[data-testid=sch-appt][data-id=c]").KeyDown(new KeyboardEventArgs { Key = "ArrowDown", AltKey = true });
        cut.Find("[data-testid=sch-appt][data-id=c]").KeyDown(new KeyboardEventArgs { Key = "ArrowRight", AltKey = true });
        cut.Find("[data-testid=sch-appt][data-id=c]").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" }); // no Alt: ignored

        Assert.Equal(new[] { new DateTime(2026, 3, 12, 12, 30, 0), new DateTime(2026, 3, 13, 12, 30, 0) }, moves);
        Assert.Equal(new DateTime(2026, 3, 13, 13, 30, 0), items[2].End);
    }

    [Fact]
    public void Scheduler_MonthView_ListsDays_ShowsMoreLink_AndOpensTheDay()
    {
        var items = Appts();
        for (var i = 0; i < 3; i++) items.Add(new() { Title = "Extra " + i, Start = new DateTime(2026, 3, 11, 15 + i, 0, 0), End = new DateTime(2026, 3, 11, 15 + i, 30, 0) });
        var cut = Sched(items, TwScheduler.SchedulerView.Month);

        Assert.Equal("March 2026", cut.Find("[data-testid=sch-title]").TextContent);
        Assert.Equal(42, cut.FindAll("[data-testid=sch-cell]").Count); // March 2026 starts on a Sunday, so six Monday-first weeks
        var cell = cut.Find("[data-testid=sch-cell][data-date='2026-03-11']");
        Assert.Equal(3, cell.QuerySelectorAll("[data-testid=sch-appt]").Length);
        Assert.Equal("+2 more", cut.Find("[data-testid=sch-more]").TextContent);

        cut.Find("[data-testid=sch-more]").Click();
        Assert.Equal("day", cut.Find("[data-testid=scheduler]").GetAttribute("data-view"));
        Assert.Contains("Wednesday", cut.Find("[data-testid=sch-title]").TextContent);
    }

    private static List<TwGantt.GanttTask> Tasks() => new()
    {
        new() { Id = "design", Name = "Design", Start = new(2026, 3, 2), End = new(2026, 3, 6), Progress = 1 },
        new() { Id = "build", Name = "Build", Start = new(2026, 3, 9), End = new(2026, 3, 20), Progress = 0.4, DependsOn = { "design" } },
        new() { Id = "ship", Name = "Ship", Start = new(2026, 3, 18), End = new(2026, 3, 19), DependsOn = { "build" } }
    };

    private IRenderedComponent<TwGantt> Gantt(DateOnly? today = null) =>
        RenderComponent<TwGantt>(p => p.Add(c => c.Tasks, Tasks()).Add(c => c.DayWidth, 20).Add(c => c.TodayOverride, today ?? new DateOnly(2026, 3, 10))
            .Add(c => c.CultureOverride, System.Globalization.CultureInfo.InvariantCulture));

    [Fact]
    public void Gantt_PositionsBarsFromTheTimeline_AndFillsProgress()
    {
        var cut = Gantt();
        // The timeline starts two days before the first task (2 March), so Design starts at 2 days x 20px.
        var design = cut.Find("[data-testid=gantt-bar][data-id=design]").GetAttribute("style")!;
        Assert.Contains("left:40px", design);
        Assert.Contains("width:100px", design); // five days
        var build = cut.Find("[data-testid=gantt-bar][data-id=build]");
        Assert.Contains("left:180px", build.GetAttribute("style"));
        Assert.Contains("width:240px", build.GetAttribute("style")); // twelve days
        Assert.Contains("width:40%", build.QuerySelector("[data-testid=gantt-progress]")!.GetAttribute("style"));
        Assert.Contains("40 percent complete", build.GetAttribute("aria-label"));
    }

    [Fact]
    public void Gantt_DrawsDependencyArrows_AndFlagsAnOverlapAsAConflict()
    {
        var cut = Gantt();
        var arrows = cut.FindAll("[data-testid=gantt-arrow]");
        Assert.Equal(2, arrows.Count);
        Assert.Equal("false", arrows.Single(a => a.GetAttribute("data-to") == "build").GetAttribute("data-conflict"));
        Assert.Equal("true", arrows.Single(a => a.GetAttribute("data-to") == "ship").GetAttribute("data-conflict")); // ship starts the 18th, build ends the 20th

        var rows = cut.FindAll("[data-testid=gantt-row]");
        Assert.Empty(rows[0].QuerySelectorAll("[data-testid=gantt-conflict]"));
        Assert.Single(rows[2].QuerySelectorAll("[data-testid=gantt-conflict]"));
    }

    [Fact]
    public void Gantt_ShowsTodayOnlyInsideTheRange_AndReportsClicks()
    {
        Assert.Single(Gantt().FindAll("[data-testid=gantt-today]"));
        Assert.Empty(Gantt(new DateOnly(2027, 1, 1)).FindAll("[data-testid=gantt-today]"));

        string? clicked = null;
        var cut = RenderComponent<TwGantt>(p => p.Add(c => c.Tasks, Tasks())
            .Add(c => c.OnTaskClick, EventCallback.Factory.Create<TwGantt.GanttTask>(this, t => clicked = t.Id)));
        cut.Find("[data-testid=gantt-bar][data-id=ship]").Click();
        Assert.Equal("ship", clicked);
        Assert.Equal("true", cut.Find("[data-testid=gantt-bar][data-id=ship]").GetAttribute("aria-pressed"));
    }
}

public class WindowAndEditorTests : TestContext
{
    public WindowAndEditorTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Window_Closes_FromTheButtonAndEscape_AndReportsVisibility()
    {
        var states = new List<bool>();
        var cut = RenderComponent<TwWindow>(p => p.Add(c => c.Title, "Inspector").AddChildContent("<p>body</p>")
            .Add(c => c.VisibleChanged, EventCallback.Factory.Create<bool>(this, v => states.Add(v))));
        Assert.Equal("Inspector", cut.Find("[data-testid=window]").GetAttribute("aria-label"));
        Assert.Equal("false", cut.Find("[data-testid=window]").GetAttribute("aria-modal"));

        cut.Find("[data-testid=window-close]").Click();
        Assert.Empty(cut.FindAll("[data-testid=window]"));

        cut.SetParametersAndRender(p => p.Add(c => c.Visible, true));
        cut.Find("[data-testid=window]").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        Assert.Empty(cut.FindAll("[data-testid=window]"));
        Assert.Equal(new[] { false, false }, states);
    }

    [Fact]
    public void Window_ArrowKeysMove_ShiftMovesFarther_CtrlResizes_AndPositionIsReported()
    {
        var moves = new List<(int X, int Y)>();
        var cut = RenderComponent<TwWindow>(p => p.Add(c => c.Title, "W").Add(c => c.X, 100).Add(c => c.Y, 100).Add(c => c.Width, 400).Add(c => c.Height, 300)
            .Add(c => c.OnMoved, EventCallback.Factory.Create<(int X, int Y)>(this, m => moves.Add(m))));
        var win = () => cut.Find("[data-testid=window]");

        win().KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        win().KeyDown(new KeyboardEventArgs { Key = "ArrowDown", ShiftKey = true });
        Assert.Contains("left:110px", win().GetAttribute("style"));
        Assert.Contains("top:150px", win().GetAttribute("style"));
        Assert.Equal(new[] { (110, 100), (110, 150) }, moves);

        win().KeyDown(new KeyboardEventArgs { Key = "ArrowRight", CtrlKey = true });
        Assert.Contains("width:410px", win().GetAttribute("style"));
        Assert.Equal(2, moves.Count); // resizing is not a move
    }

    [Fact]
    public void Window_MinimizeHidesTheBody_MaximizeFillsTheScreen_AndLastTouchedStaysOnTop()
    {
        var a = RenderComponent<TwWindow>(p => p.Add(c => c.Title, "A"));
        var b = RenderComponent<TwWindow>(p => p.Add(c => c.Title, "B"));
        int Z(IRenderedComponent<TwWindow> w) => int.Parse(System.Text.RegularExpressions.Regex.Match(w.Find("[data-testid=window]").GetAttribute("style")!, @"z-index:(\d+)").Groups[1].Value);
        Assert.True(Z(b) > Z(a));
        a.Find("[data-testid=window]").PointerDown();
        Assert.True(Z(a) > Z(b)); // touching A raises it above B

        a.Find("[data-testid=window-min]").Click();
        Assert.Equal("minimized", a.Find("[data-testid=window]").GetAttribute("data-state"));
        Assert.Empty(a.FindAll("[data-testid=window-body]"));

        a.Find("[data-testid=window-min]").Click();
        a.Find("[data-testid=window-max]").Click();
        Assert.Equal("maximized", a.Find("[data-testid=window]").GetAttribute("data-state"));
        Assert.Contains("inset:0", a.Find("[data-testid=window]").GetAttribute("style"));
    }

    [Fact]
    public void HtmlEditor_AttachesToTheSurface_WithTheInitialValue_AndToolbarButtonsSendCommands()
    {
        var cut = RenderComponent<TwHtmlEditor>(p => p.Add(c => c.Value, "<p>Hello</p>"));
        var attach = JSInterop.Invocations.Single(i => i.Identifier == "twEnterprise.attachEditor");
        Assert.Equal("<p>Hello</p>", attach.Arguments[2]);

        cut.Find("[data-cmd=bold]").Click();
        cut.Find("[data-cmd=formatBlockh2]").Click();
        var cmds = JSInterop.Invocations.Where(i => i.Identifier == "twEnterprise.editorCommand").Select(i => $"{i.Arguments[1]}:{i.Arguments[2]}").ToList();
        Assert.Equal(new[] { "bold:", "formatBlock:h2" }, cmds);
    }

    [Fact]
    public void HtmlEditor_Link_OnlyAcceptsSafeAddresses()
    {
        var cut = RenderComponent<TwHtmlEditor>();
        cut.Find("[data-testid=editor-link-toggle]").Click();

        cut.Find("[data-testid=editor-url]").Input("javascript:alert(1)");
        cut.Find("[data-testid=editor-link-apply]").Click();
        Assert.NotNull(cut.Find("[data-testid=editor-link-error]"));
        Assert.DoesNotContain(JSInterop.Invocations, i => i.Identifier == "twEnterprise.editorCommand" && (string?)i.Arguments[1] == "createLink");

        cut.Find("[data-testid=editor-url]").Input("https://example.com/docs");
        cut.Find("[data-testid=editor-link-apply]").Click();
        var link = JSInterop.Invocations.Single(i => i.Identifier == "twEnterprise.editorCommand" && (string?)i.Arguments[1] == "createLink");
        Assert.Equal("https://example.com/docs", link.Arguments[2]);
        Assert.Empty(cut.FindAll("[data-testid=editor-url]")); // the link box closes after applying
    }

    [Theory]
    [InlineData("https://example.com", true)]
    [InlineData("http://example.com/a?b=c", true)]
    [InlineData("mailto:team@example.com", true)]
    [InlineData("javascript:alert(1)", false)]
    [InlineData("JaVaScRiPt:alert(1)", false)]
    [InlineData("data:text/html;base64,PHNjcmlwdD4=", false)]
    [InlineData("/relative/path", false)]
    [InlineData("//evil.example.com", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void HtmlEditor_IsSafeUrl(string? url, bool expected) => Assert.Equal(expected, TwHtmlEditor.IsSafeUrl(url));
}
