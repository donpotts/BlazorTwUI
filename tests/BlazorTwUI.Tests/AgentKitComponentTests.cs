using BlazorTwUI.Components.UI;
using BlazorTwUI.Models;
using BlazorTwUI.Services;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BlazorTwUI.Tests;

public class ThreadListTests : TestContext
{
    private (AgentThreadStore Store, IRenderedComponent<TwThreadList> Cut, List<string> Selected) Make()
    {
        var store = new AgentThreadStore();
        var selected = new List<string>();
        var cut = RenderComponent<TwThreadList>(p => p
            .Add(c => c.Store, store)
            .Add(c => c.OnSelect, EventCallback.Factory.Create<AgentThread>(this, t => selected.Add(t.Id))));
        return (store, cut, selected);
    }

    [Fact]
    public void New_CreatesAThread_AndSelectsIt()
    {
        var (store, cut, selected) = Make();
        cut.Find("[data-testid=thread-new]").Click();

        Assert.Single(store.All);
        Assert.Single(selected);
        cut.WaitForAssertion(() => Assert.Single(cut.FindAll("[data-testid=thread-item]")));
    }

    [Fact]
    public void Rename_UpdatesTheTitle()
    {
        var (store, cut, _) = Make();
        var t = store.Create("old");
        cut.WaitForAssertion(() => cut.Find("[data-testid=thread-rename]"));
        cut.Find("[data-testid=thread-rename]").Click();
        cut.Find("[data-testid=thread-rename-input]").Input("renamed");
        cut.Find("[data-testid=thread-rename-save]").Click();

        Assert.Equal("renamed", store.Get(t.Id)!.Title);
        cut.WaitForAssertion(() => Assert.Equal("renamed", cut.Find("[data-testid=thread-title]").TextContent));
    }

    [Fact]
    public void PinArchiveDelete_ActOnTheStore()
    {
        var (store, cut, _) = Make();
        var t = store.Create("one");
        cut.WaitForAssertion(() => cut.Find("[data-testid=thread-pin]"));
        cut.Find("[data-testid=thread-pin]").Click();
        Assert.True(store.Get(t.Id)!.Pinned);

        cut.Find("[data-testid=thread-archive]").Click();
        Assert.True(store.Get(t.Id)!.Archived);
        cut.WaitForAssertion(() => Assert.Empty(cut.FindAll("[data-testid=thread-item]"))); // hidden until "Show archived"

        cut.Find("[data-testid=thread-show-archived]").Change(true);
        cut.WaitForAssertion(() => Assert.Single(cut.FindAll("[data-testid=thread-item]")));
        cut.Find("[data-testid=thread-delete]").Click();
        Assert.Null(store.Get(t.Id));
    }

    [Fact]
    public void Search_FiltersByTitleAndMessageText()
    {
        var (store, cut, _) = Make();
        store.Create("Alpha");
        var b = store.Create("Beta");
        store.Append(b.Id, "assistant", "mentions needle here");
        cut.WaitForAssertion(() => Assert.Equal(2, cut.FindAll("[data-testid=thread-item]").Count));

        cut.Find("[data-testid=thread-search]").Input("needle");
        Assert.Equal("Beta", Assert.Single(cut.FindAll("[data-testid=thread-title]")).TextContent);

        cut.Find("[data-testid=thread-search]").Input("zzz");
        Assert.NotNull(cut.Find("[data-testid=thread-empty]"));
    }
}

public class ChatPiecesTests : TestContext
{
    [Fact]
    public void Suggestions_ClickPicksTheText_AndDisabledBlocksIt()
    {
        string? picked = null;
        var cut = RenderComponent<TwChatSuggestions>(p => p
            .Add(c => c.Items, new[] { "One", "Two" })
            .Add(c => c.OnPick, EventCallback.Factory.Create<string>(this, s => picked = s)));
        cut.FindAll("[data-testid=chat-suggestion]")[1].Click();
        Assert.Equal("Two", picked);

        cut.SetParametersAndRender(p => p.Add(c => c.Disabled, true));
        Assert.All(cut.FindAll("[data-testid=chat-suggestion]"), b => Assert.True(b.HasAttribute("disabled")));
    }

    [Fact]
    public void Suggestions_Loading_ShowsPlaceholdersInsteadOfButtons()
    {
        var cut = RenderComponent<TwChatSuggestions>(p => p.Add(c => c.Loading, true).Add(c => c.Items, new[] { "One" }));
        Assert.Empty(cut.FindAll("[data-testid=chat-suggestion]"));
        Assert.Contains("Loading suggestions", cut.Markup);
    }

    [Fact]
    public void Feedback_TogglesAndClears_WhenPressedAgain_AndAsksWhyAfterThumbsDown()
    {
        var value = TwFeedbackButtons.FeedbackRating.None;
        string? reason = null;
        var cut = RenderComponent<TwFeedbackButtons>(p => p
            .Add(c => c.Value, value)
            .Add(c => c.ValueChanged, EventCallback.Factory.Create<TwFeedbackButtons.FeedbackRating>(this, v => value = v))
            .Add(c => c.OnReason, EventCallback.Factory.Create<string>(this, r => reason = r)));

        cut.Find("[data-testid=feedback-down]").Click();
        Assert.Equal(TwFeedbackButtons.FeedbackRating.Down, value);

        cut.SetParametersAndRender(p => p.Add(c => c.Value, value));
        Assert.Equal("true", cut.Find("[data-testid=feedback-down]").GetAttribute("aria-pressed"));
        cut.Find("[data-testid=feedback-reason]").Change("too slow");
        Assert.Equal("too slow", reason);

        cut.Find("[data-testid=feedback-down]").Click();
        Assert.Equal(TwFeedbackButtons.FeedbackRating.None, value);
    }

    [Fact]
    public void AttachmentQueue_EnforcesCountSizeAndDuplicates_AndRemoves()
    {
        var cut = RenderComponent<TwAttachmentQueue>(p => p.Add(c => c.MaxFiles, 2).Add(c => c.MaxBytes, 1000));
        var queue = cut.Instance;

        Assert.Null(queue.TryAdd(new("a.txt", 10, "text/plain")));
        Assert.Contains("already attached", queue.TryAdd(new("a.txt", 10, "text/plain")));
        Assert.Contains("larger than", queue.TryAdd(new("big.bin", 5000, "x")));
        Assert.Null(queue.TryAdd(new("b.txt", 20, "text/plain")));
        Assert.Contains("at most 2", queue.TryAdd(new("c.txt", 1, "text/plain")));

        cut.Render();
        Assert.Equal(2, cut.FindAll("[data-testid=attachment-item]").Count);
        cut.Find("[data-testid=attachment-remove]").Click();
        Assert.Single(cut.FindAll("[data-testid=attachment-item]"));
    }

    [Theory]
    [InlineData(AgentRunStatus.Running, "working")]
    [InlineData(AgentRunStatus.AwaitingApproval, "waiting for approval")]
    [InlineData(AgentRunStatus.Failed, "failed")]
    public void Orb_ExposesItsStateAsText(AgentRunStatus status, string text)
    {
        var cut = RenderComponent<TwAgentOrb>(p => p.Add(c => c.Name, "Refund Desk").Add(c => c.Status, status));
        Assert.Equal($"Refund Desk: {text}", cut.Find("[data-testid=agent-orb]").GetAttribute("aria-label"));
    }
}

public class AgentControlPlaneTests : TestContext
{
    [Fact]
    public void DryRun_ShowsBeforeAndAfter_PerKind_AndTheEmptyState()
    {
        var changes = new List<TwDryRunChanges.DryRunChange>
        {
            new(TwDryRunChanges.DryRunKind.Update, "Order 4821 refund", "$0.00", "$249.00"),
            new(TwDryRunChanges.DryRunKind.Create, "Ticket ESC-1", After: "opened"),
            new(TwDryRunChanges.DryRunKind.Delete, "Draft email", Before: "queued")
        };
        var cut = RenderComponent<TwDryRunChanges>(p => p.Add(c => c.Changes, changes));

        Assert.Equal("3 changes", cut.Find("[data-testid=dry-run-count]").TextContent);
        var rows = cut.FindAll("[data-testid=dry-run-change]");
        Assert.Contains("$0.00", rows[0].QuerySelector("del")!.TextContent);
        Assert.Contains("$249.00", rows[0].QuerySelector("ins")!.TextContent);
        Assert.Null(rows[1].QuerySelector("del"));
        Assert.Null(rows[2].QuerySelector("ins"));

        cut.SetParametersAndRender(p => p.Add(c => c.Changes, new List<TwDryRunChanges.DryRunChange>()));
        Assert.NotNull(cut.Find("[data-testid=dry-run-none]"));
    }

    [Fact]
    public void ActivityLog_ListsEntriesInOrder_WithCount()
    {
        var now = DateTime.UtcNow;
        var cut = RenderComponent<TwActivityLog>(p => p.Add(c => c.Entries, new[]
        {
            new TwActivityLog.ActivityEntry(now, "navigate", "Opened portal", "https://x.test"),
            new TwActivityLog.ActivityEntry(now, "click", "Pressed Submit")
        }));
        Assert.Equal("2 events", cut.Find("[data-testid=activity-count]").TextContent);
        Assert.Equal(new[] { "navigate", "click" }, cut.FindAll("[data-testid=activity-entry]").Select(e => e.GetAttribute("data-kind")));
    }

    [Fact]
    public void CommandOutput_MasksSecrets_TruncatesToTheTail_AndShowsExit()
    {
        var output = string.Join('\n', Enumerable.Range(1, 10).Select(i => $"line {i}")) + "\nAuthorization: Bearer abcdef123456";
        var cut = RenderComponent<TwCommandOutput>(p => p
            .Add(c => c.Command, "deploy --password=hunter2")
            .Add(c => c.Output, output).Add(c => c.ExitCode, 2).Add(c => c.MaxLines, 4));

        Assert.DoesNotContain("hunter2", cut.Find("[data-testid=command-line]").TextContent);
        var text = cut.Find("[data-testid=command-text]").TextContent;
        Assert.DoesNotContain("abcdef123456", text);
        Assert.DoesNotContain("line 1\n", text);
        Assert.Contains("line 10", text);
        Assert.Equal("exit 2", cut.Find("[data-testid=command-exit]").TextContent);
        Assert.NotNull(cut.Find("[data-testid=command-truncated]"));
    }

    [Fact]
    public void Handoff_TakeOverThenHandBack_FiresCallbacksInOrder()
    {
        var log = new List<string>();
        var cut = RenderComponent<TwHandoffPanel>(p => p
            .Add(c => c.AgentName, "Refund Desk").Add(c => c.Reason, "CAPTCHA blocks the portal")
            .Add(c => c.OnTakeOver, EventCallback.Factory.Create(this, () => log.Add("take")))
            .Add(c => c.OnHandBack, EventCallback.Factory.Create(this, () => log.Add("back"))));

        Assert.Equal("waiting", cut.Find("[data-testid=handoff-panel]").GetAttribute("data-state"));
        cut.Find("[data-testid=handoff-take]").Click();
        Assert.Equal("driving", cut.Find("[data-testid=handoff-panel]").GetAttribute("data-state"));
        cut.Find("[data-testid=handoff-return]").Click();

        Assert.Equal(new[] { "take", "back" }, log);
    }

    [Fact]
    public void Routines_ToggleChangesState_AndRunNowIsBlockedWhenOff()
    {
        var items = new List<TwRoutinesList.Routine> { new() { Name = "Daily digest", Schedule = "Weekdays 8am", Enabled = true } };
        var ran = 0;
        var cut = RenderComponent<TwRoutinesList>(p => p
            .Add(c => c.Items, items)
            .Add(c => c.OnRunNow, EventCallback.Factory.Create<TwRoutinesList.Routine>(this, _ => ran++)));

        cut.Find("[data-testid=routine-run]").Click();
        Assert.Equal(1, ran);
        cut.Find("[data-testid=routine-toggle]").Click();
        Assert.False(items[0].Enabled);
        Assert.Equal("false", cut.Find("[data-testid=routine-toggle]").GetAttribute("aria-checked"));
        Assert.True(cut.Find("[data-testid=routine-run]").HasAttribute("disabled"));
    }

    [Fact]
    public void Memory_SearchesAndForgets()
    {
        var items = new List<TwMemoryList.Memory>
        {
            new("1", "Prefers email over phone", "Chat on Mar 3", DateTime.UtcNow),
            new("2", "Billing contact is Dana", "Invoice thread", DateTime.UtcNow)
        };
        var forgotten = new List<string>();
        var cut = RenderComponent<TwMemoryList>(p => p
            .Add(c => c.Items, items)
            .Add(c => c.OnForget, EventCallback.Factory.Create<TwMemoryList.Memory>(this, m => forgotten.Add(m.Id))));

        cut.Find("[data-testid=memory-search]").Input("billing");
        Assert.Single(cut.FindAll("[data-testid=memory-item]"));
        Assert.Equal("1 of 2", cut.Find("[data-testid=memory-count]").TextContent);

        cut.Find("[data-testid=memory-forget]").Click();
        Assert.Equal(new[] { "2" }, forgotten);
        cut.Find("[data-testid=memory-search]").Input("");
        Assert.Single(cut.FindAll("[data-testid=memory-item]"));
    }

    [Fact]
    public void Grants_ChangeLevelAndRevoke()
    {
        var items = new List<TwGrantList.Grant> { new() { Tool = "Gmail", Level = TwGrantList.GrantLevel.Ask }, new() { Tool = "Calendar" } };
        var changed = new List<string>();
        var revoked = new List<string>();
        var cut = RenderComponent<TwGrantList>(p => p
            .Add(c => c.Items, items)
            .Add(c => c.OnChanged, EventCallback.Factory.Create<TwGrantList.Grant>(this, g => changed.Add($"{g.Tool}:{g.Level}")))
            .Add(c => c.OnRevoked, EventCallback.Factory.Create<TwGrantList.Grant>(this, g => revoked.Add(g.Tool))));

        cut.FindAll("[data-testid=grant-level]")[0].Change("Write");
        Assert.Equal(new[] { "Gmail:Write" }, changed);

        cut.FindAll("[data-testid=grant-revoke]")[1].Click();
        Assert.Equal(new[] { "Calendar" }, revoked);
        Assert.Single(cut.FindAll("[data-testid=grant]"));
    }

    [Fact]
    public void Questionnaire_WalksThroughQuestions_AndSubmitsAnswers()
    {
        IReadOnlyDictionary<string, IReadOnlyList<string>>? result = null;
        var qs = new[]
        {
            new TwQuestionnaire.Question { Id = "q1", Prompt = "Which account?", Options = { "Checking", "Savings" } },
            new TwQuestionnaire.Question { Id = "q2", Prompt = "Notify via?", Options = { "Email", "SMS" }, Multiple = true, AllowOther = true }
        };
        var cut = RenderComponent<TwQuestionnaire>(p => p
            .Add(c => c.Questions, qs)
            .Add(c => c.OnSubmit, EventCallback.Factory.Create<IReadOnlyDictionary<string, IReadOnlyList<string>>>(this, r => result = r)));

        Assert.Equal("Question 1 of 2", cut.Find("[data-testid=questionnaire-progress]").TextContent);
        Assert.True(cut.Find("[data-testid=questionnaire-next]").HasAttribute("disabled")); // nothing chosen yet
        cut.Find("[data-option=Savings]").Change(true);
        cut.Find("[data-testid=questionnaire-next]").Click();

        Assert.Equal("Question 2 of 2", cut.Find("[data-testid=questionnaire-progress]").TextContent);
        cut.Find("[data-option=Email]").Change(true);
        cut.Find("[data-testid=questionnaire-other]").Input("Push");
        cut.Find("[data-testid=questionnaire-next]").Click();

        Assert.NotNull(cut.Find("[data-testid=questionnaire-done]"));
        Assert.Equal(new[] { "Savings" }, result!["q1"]);
        Assert.Equal(new[] { "Email", "Push" }, result["q2"].OrderBy(x => x));
    }

    [Fact]
    public void RedactedValue_HidesCredentialsUntilRevealed()
    {
        var cut = RenderComponent<TwRedactedValue>(p => p.Add(c => c.Value, "{\"user\":\"dana\",\"password\":\"s3cret!\"}"));
        var shown = cut.Find("[data-testid=redacted-text]").TextContent;
        Assert.DoesNotContain("s3cret!", shown);
        Assert.Contains("dana", shown);
        Assert.Equal("1 masked", cut.Find("[data-testid=redacted-count]").TextContent);

        cut.Find("[data-testid=redacted-toggle]").Click();
        Assert.Contains("s3cret!", cut.Find("[data-testid=redacted-text]").TextContent);
    }

    [Fact]
    public void LiveScreen_ReportsClicksOnlyWhileYouControlIt()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        JSInterop.Setup<double[]>("twAgentKit.elementSize", _ => true).SetResult(new[] { 200d, 100d });
        var clicks = new List<(double X, double Y)>();
        var controlling = false;
        var cut = RenderComponent<TwLiveScreen>(p => p
            .Add(c => c.Url, "https://portal.test")
            .Add(c => c.Controlling, controlling)
            .Add(c => c.ControllingChanged, EventCallback.Factory.Create<bool>(this, v => controlling = v))
            .Add(c => c.OnScreenClick, EventCallback.Factory.Create<(double X, double Y)>(this, c => clicks.Add(c))));

        cut.Find("[data-testid=screen-surface]").Click(new Microsoft.AspNetCore.Components.Web.MouseEventArgs { OffsetX = 100, OffsetY = 50 });
        Assert.Empty(clicks); // agent is working; human clicks are ignored
        Assert.Equal("Agent is working", cut.Find("[data-testid=screen-mode]").TextContent.Trim());

        cut.Find("[data-testid=screen-toggle]").Click();
        Assert.True(controlling);
        cut.SetParametersAndRender(p => p.Add(c => c.Controlling, true));
        cut.Find("[data-testid=screen-surface]").Click(new Microsoft.AspNetCore.Components.Web.MouseEventArgs { OffsetX = 100, OffsetY = 50 });
        cut.WaitForAssertion(() => Assert.Equal((0.5, 0.5), Assert.Single(clicks)));
    }

    [Fact]
    public void Waveform_MapsLevelsOntoBars_AndFlattensWhenInactive()
    {
        var levels = new float[] { 1f, 0.5f };
        var active = RenderComponent<TwLiveWaveform>(p => p.Add(c => c.Levels, levels).Add(c => c.Active, true).Add(c => c.Bars, 4));
        var heights = active.FindAll("[data-testid=waveform-bar]").Select(b => b.GetAttribute("style")).ToList();
        Assert.Equal(4, heights.Count);
        Assert.Contains("height:100%", heights[0]);
        Assert.Contains("height:50%", heights[3]);

        var idle = RenderComponent<TwLiveWaveform>(p => p.Add(c => c.Levels, levels).Add(c => c.Bars, 4));
        Assert.All(idle.FindAll("[data-testid=waveform-bar]"), b => Assert.Contains("height:8%", b.GetAttribute("style")));
    }
}

public class InputControlTests : TestContext
{
    [Fact]
    public void Spinner_AnnouncesItsLabel()
    {
        var cut = RenderComponent<TwSpinner>(p => p.Add(c => c.Label, "Saving"));
        Assert.Equal("status", cut.Find("[data-testid=spinner]").GetAttribute("role"));
        Assert.Contains("Saving", cut.Markup);
    }

    [Fact]
    public void ButtonGroup_RendersChildrenInAnAccessibleGroup()
    {
        var cut = RenderComponent<TwButtonGroup>(p => p
            .Add(c => c.Label, "View")
            .AddChildContent("<button>Day</button><button>Week</button>"));
        var group = cut.Find("[data-testid=button-group]");
        Assert.Equal("group", group.GetAttribute("role"));
        Assert.Equal("View", group.GetAttribute("aria-label"));
        Assert.Equal(2, group.QuerySelectorAll("button").Length);
    }

    [Fact]
    public void RadioGroup_SelectsOneValue_AndSkipsDisabledOptions()
    {
        string? value = null;
        var cut = RenderComponent<TwRadioGroup<string>>(p => p
            .Add(c => c.Legend, "Plan")
            .Add(c => c.Items, new[]
            {
                new TwRadioGroup<string>.Option("free", "Free"),
                new TwRadioGroup<string>.Option("pro", "Pro", "For teams"),
                new TwRadioGroup<string>.Option("ent", "Enterprise", Disabled: true)
            })
            .Add(c => c.ValueChanged, EventCallback.Factory.Create<string>(this, v => value = v)));

        cut.FindAll("input[type=radio]")[1].Change(true);
        Assert.Equal("pro", value);
        Assert.True(cut.FindAll("input[type=radio]")[2].HasAttribute("disabled"));
    }

    [Fact]
    public void Calendar_PicksADate_RespectsMinMax_AndNavigatesMonths()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        DateOnly? picked = null;
        var cut = RenderComponent<TwCalendar>(p => p
            .Add(c => c.Value, new DateOnly(2026, 3, 10))
            .Add(c => c.Min, new DateOnly(2026, 3, 5))
            .Add(c => c.Max, new DateOnly(2026, 3, 20))
            .Add(c => c.TodayOverride, new DateOnly(2026, 3, 10))
            .Add(c => c.CultureOverride, System.Globalization.CultureInfo.InvariantCulture)
            .Add(c => c.ValueChanged, EventCallback.Factory.Create<DateOnly?>(this, d => picked = d)));

        Assert.Equal("March 2026", cut.Find("[data-testid=cal-title]").TextContent);
        Assert.True(cut.Find("[data-date='2026-03-04']").HasAttribute("disabled"));
        Assert.True(cut.Find("[data-date='2026-03-21']").HasAttribute("disabled"));
        Assert.Equal("date", cut.Find("[data-date='2026-03-10']").GetAttribute("aria-current"));

        cut.Find("[data-date='2026-03-15']").Click();
        Assert.Equal(new DateOnly(2026, 3, 15), picked);

        cut.Find("[data-testid=cal-next]").Click();
        Assert.Equal("April 2026", cut.Find("[data-testid=cal-title]").TextContent);
    }

    [Fact]
    public void Calendar_ArrowKeysMoveTheRovingTabStop()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var cut = RenderComponent<TwCalendar>(p => p
            .Add(c => c.Value, new DateOnly(2026, 3, 10))
            .Add(c => c.CultureOverride, System.Globalization.CultureInfo.InvariantCulture));
        Assert.Equal("0", cut.Find("[data-date='2026-03-10']").GetAttribute("tabindex"));

        cut.Find("[data-testid=calendar]").KeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowRight" });
        cut.Find("[data-testid=calendar]").KeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowDown" });

        Assert.Equal("0", cut.Find("[data-date='2026-03-18']").GetAttribute("tabindex"));
        Assert.Equal("-1", cut.Find("[data-date='2026-03-10']").GetAttribute("tabindex"));

        cut.Find("[data-testid=calendar]").KeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "PageDown" });
        Assert.Equal("April 2026", cut.Find("[data-testid=cal-title]").TextContent);
        Assert.Equal("0", cut.Find("[data-date='2026-04-18']").GetAttribute("tabindex"));
    }

    [Fact]
    public void TimePicker_ConvertsTwelveHourSelectionsTo24HourTime()
    {
        TimeOnly? value = null;
        var cut = RenderComponent<TwTimePicker>(p => p
            .Add(c => c.ValueChanged, EventCallback.Factory.Create<TimeOnly?>(this, v => value = v)));

        cut.Find("[data-testid=time-hour]").Change("3");
        cut.Find("[data-testid=time-minute]").Change("45");
        Assert.Equal(new TimeOnly(3, 45), value);

        cut.SetParametersAndRender(p => p.Add(c => c.Value, value));
        cut.Find("[data-testid=time-period]").Change("PM");
        Assert.Equal(new TimeOnly(15, 45), value);

        cut.SetParametersAndRender(p => p.Add(c => c.Value, value));
        cut.Find("[data-testid=time-hour]").Change("12"); // 12 PM is noon, not midnight
        Assert.Equal(new TimeOnly(12, 45), value);
    }

    [Fact]
    public void TimePicker_24Hour_HasNoPeriod_AndStepControlsMinutes()
    {
        var cut = RenderComponent<TwTimePicker>(p => p.Add(c => c.Use24Hour, true).Add(c => c.MinuteStep, 15).Add(c => c.Value, new TimeOnly(18, 30)));
        Assert.Empty(cut.FindAll("[data-testid=time-period]"));
        Assert.Equal(new[] { "00", "15", "30", "45" }, cut.Find("[data-testid=time-minute]").QuerySelectorAll("option").Select(o => o.TextContent));
        Assert.Equal(24, cut.Find("[data-testid=time-hour]").QuerySelectorAll("option").Length);
    }

    [Fact]
    public void TextArea_CountsDown_WarnsNearTheLimit_AndShowsErrors()
    {
        string? value = null;
        var cut = RenderComponent<TwTextArea>(p => p
            .Add(c => c.Label, "Notes").Add(c => c.MaxLength, 20)
            .Add(c => c.ValueChanged, EventCallback.Factory.Create<string?>(this, v => value = v)));
        Assert.Equal("20 left", cut.Find("[data-testid=textarea-count]").TextContent);

        cut.Find("[data-testid=textarea-input]").Input("0123456789012");
        Assert.Equal("0123456789012", value);
        Assert.Equal("7 left", cut.Find("[data-testid=textarea-count]").TextContent);

        cut.SetParametersAndRender(p => p.Add(c => c.Error, "Too vague"));
        Assert.Equal("true", cut.Find("[data-testid=textarea-input]").GetAttribute("aria-invalid"));
        Assert.Equal("Too vague", cut.Find("[data-testid=textarea-error]").TextContent);
    }

    [Fact]
    public void TextArea_AutoGrow_StaysWithinMinAndMaxRows()
    {
        var cut = RenderComponent<TwTextArea>(p => p.Add(c => c.AutoGrow, true).Add(c => c.MinRows, 2).Add(c => c.MaxRows, 5).Add(c => c.Value, "a"));
        Assert.Equal("2", cut.Find("[data-testid=textarea-input]").GetAttribute("rows"));
        cut.SetParametersAndRender(p => p.Add(c => c.Value, string.Join('\n', Enumerable.Repeat("x", 30))));
        Assert.Equal("5", cut.Find("[data-testid=textarea-input]").GetAttribute("rows"));
    }
}

public class ProtocolSurfaceTests : TestContext
{
    private const string Form = """
        {"createSurface":{"surfaceId":"refund"}}
        """;

    private static A2uiSurface RefundSurface()
    {
        var s = new A2uiSurface();
        Assert.True(s.Apply("""{"createSurface":{"surfaceId":"refund"}}"""));
        Assert.True(s.Apply("""
            {"updateComponents":{"surfaceId":"refund","components":[
              {"id":"root","component":"Card","children":["col"]},
              {"id":"col","component":"Column","children":["title","amount","notify","go"]},
              {"id":"title","component":"Text","variant":"h3","text":"Confirm refund"},
              {"id":"amount","component":"TextField","label":"Amount","value":{"path":"/refund/amount"}},
              {"id":"notify","component":"CheckBox","label":"Email the customer","value":{"path":"/refund/notify"}},
              {"id":"go","component":"Button","label":"Send","action":{"name":"submit_refund","context":{"amount":{"path":"/refund/amount"},"notify":{"path":"/refund/notify"}}}}
            ]}}
            """));
        Assert.True(s.Apply("""{"updateDataModel":{"surfaceId":"refund","path":"/refund","value":{"amount":"249.00","notify":true}}}"""));
        return s;
    }

    [Fact]
    public void Surface_ResolvesPathBindings_AndFiresActionsWithResolvedContext()
    {
        var s = RefundSurface();
        A2uiUserAction? action = null;
        s.ActionRaised += a => action = a;

        Assert.Equal("249.00", s.Resolve(s.Components["amount"]["value"]));
        s.SetPath("/refund/amount", System.Text.Json.Nodes.JsonValue.Create("120.00"));
        s.Raise("go");

        Assert.Equal("submit_refund", action!.Name);
        Assert.Equal("refund", action.SurfaceId);
        Assert.Equal("120.00", action.Context["amount"]);
        Assert.Equal("true", action.Context["notify"]);
    }

    [Fact]
    public void Surface_IgnoresInvalidJson_AndStaysEmptyUntilRootArrives()
    {
        var s = new A2uiSurface();
        Assert.False(s.Apply("{not json"));
        Assert.False(s.HasRoot);
    }

    [Fact]
    public void Component_RendersTheTree_TwoWayBindsInputs_AndRaisesTheButtonAction()
    {
        var s = RefundSurface();
        A2uiUserAction? action = null;
        var cut = RenderComponent<TwA2uiSurface>(p => p
            .Add(c => c.Surface, s)
            .Add(c => c.OnAction, EventCallback.Factory.Create<A2uiUserAction>(this, a => action = a)));

        Assert.Equal("Confirm refund", cut.Find("[data-a2ui=Text]").TextContent);
        Assert.Equal("249.00", cut.Find("[data-a2ui=TextField] input").GetAttribute("value"));

        cut.Find("[data-a2ui=TextField] input").Change("75.00");
        cut.Find("[data-a2ui=CheckBox] input").Change(false);
        cut.Find("[data-a2ui=Button]").Click();

        cut.WaitForAssertion(() => Assert.NotNull(action));
        Assert.Equal("75.00", action!.Context["amount"]);
        Assert.Equal("false", action.Context["notify"]);
    }

    [Fact]
    public void Component_UpdatesLive_WhenTheAgentStreamsMoreComponents()
    {
        var s = new A2uiSurface();
        var cut = RenderComponent<TwA2uiSurface>(p => p.Add(c => c.Surface, s));
        Assert.NotNull(cut.Find("[data-testid=a2ui-waiting]"));

        s.Apply("""{"updateComponents":{"surfaceId":"default","components":[{"id":"root","component":"Text","text":"Hello"}]}}""");
        cut.WaitForAssertion(() => Assert.Equal("Hello", cut.Find("[data-a2ui=Text]").TextContent));

        s.Apply("""{"updateComponents":{"surfaceId":"default","components":[{"id":"root","component":"Text","text":"Updated"}]}}""");
        cut.WaitForAssertion(() => Assert.Equal("Updated", cut.Find("[data-a2ui=Text]").TextContent));
    }

    [Fact]
    public void ChoicePicker_SingleAndMultiple_WriteBackToTheDataModel()
    {
        var s = new A2uiSurface();
        s.Apply("""
            {"updateComponents":{"surfaceId":"d","components":[
              {"id":"root","component":"Column","children":["one","many"]},
              {"id":"one","component":"ChoicePicker","label":"Size","options":[{"label":"S","value":"s"},{"label":"L","value":"l"}],"value":{"path":"/size"}},
              {"id":"many","component":"ChoicePicker","variant":"multipleSelection","label":"Extras","options":[{"label":"Gift","value":"gift"},{"label":"Card","value":"card"}],"value":{"path":"/extras"}}
            ]}}
            """);
        var cut = RenderComponent<TwA2uiSurface>(p => p.Add(c => c.Surface, s));

        cut.Find("[data-id=one] [data-option=l]").Change(true);
        Assert.Equal("l", s.Resolve(new System.Text.Json.Nodes.JsonObject { ["path"] = "/size" }));

        cut.Find("[data-id=many] [data-option=gift]").Change(true);
        cut.Find("[data-id=many] [data-option=card]").Change(true);
        var extras = s.Lookup("/extras") as System.Text.Json.Nodes.JsonArray;
        Assert.Equal(new[] { "card", "gift" }, extras!.Select(x => (string)x!).OrderBy(x => x));
    }

    [Fact]
    public void McpAppFrame_NeverGrantsSameOrigin_AndLocksDownNetworkExceptValidatedDomains()
    {
        Assert.DoesNotContain("allow-same-origin", TwMcpAppFrame.SandboxFlags);

        var doc = TwMcpAppFrame.BuildDocument("<p>hi</p>", new[] { "https://cdn.example.com", "evil.com; script-src *", "*.trusted.org" });
        Assert.Contains("connect-src 'none'", doc);
        Assert.Contains("https://cdn.example.com", doc);
        Assert.Contains("*.trusted.org", doc);
        Assert.DoesNotContain("evil.com", doc); // a value that could end the policy early is dropped
        Assert.Contains("<p>hi</p>", doc);

        JSInterop.Mode = JSRuntimeMode.Loose;
        var cut = RenderComponent<TwMcpAppFrame>(p => p.Add(c => c.Html, "<p>x</p>").Add(c => c.Title, "Chart"));
        var frame = cut.Find("[data-testid=mcp-frame]");
        Assert.Equal("allow-scripts allow-forms", frame.GetAttribute("sandbox"));
        Assert.Equal("no-referrer", frame.GetAttribute("referrerpolicy"));
    }

    [Fact]
    public void Console_WithThreads_SavesTheRun_AndReopensItFromTheList()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<IAgentTransport>(new SimulatedAgentTransport { StepDelay = TimeSpan.Zero });
        Services.AddSingleton<AgentThreadStore>();
        Services.AddSingleton<IAgentThreadStore>(sp => sp.GetRequiredService<AgentThreadStore>());
        var store = Services.GetRequiredService<AgentThreadStore>();

        var cut = RenderComponent<TwAgentConsole>(p => p.Add(c => c.ShowThreads, true));
        cut.Find("[data-testid=run]").Click();
        cut.WaitForElement("[data-testid=approval-gate]", TimeSpan.FromSeconds(5));
        cut.Find("[data-testid=approve]").Click();
        cut.WaitForAssertion(() => Assert.Equal("Finished", cut.Find("[data-testid=run-status]").TextContent), TimeSpan.FromSeconds(5));

        cut.WaitForAssertion(() =>
        {
            var thread = Assert.Single(store.All);
            Assert.Equal(2, thread.Messages.Count);
            Assert.Equal("assistant", thread.Messages[1].Role);
        });

        // Pick the thread again: the timeline is cleared and the saved transcript is shown as history.
        cut.Find("[data-testid=thread-select]").Click();
        cut.WaitForAssertion(() =>
        {
            Assert.Equal(2, cut.FindAll("[data-testid=thread-history]").Count);
            Assert.Empty(cut.FindAll("[data-testid=tool-call]"));
        });
    }
}
