using System.Text.RegularExpressions;
using BlazorTwUI.Components.UI;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BlazorTwUI.Tests;

/// <summary>Confetti honours prefers-reduced-motion; the rewards centre has no text under 12px.</summary>
public class MotionAndLegibilityTests : TestContext
{
    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "BlazorTwUI.csproj"))) dir = dir.Parent;
        return dir?.FullName ?? throw new InvalidOperationException("Repo root not found.");
    }

    private static object? Prop(object o, string name) => o.GetType().GetProperty(name)?.GetValue(o);

    [Fact]
    public async Task Confetti_asks_the_engine_to_respect_reduced_motion_by_default()
    {
        var fire = JSInterop.SetupVoid("blazorCelebration.fire", _ => true);
        fire.SetVoidResult();
        var cut = RenderComponent<TwConfetti>();
        await cut.InvokeAsync(() => cut.Instance.FireAsync());
        var opts = fire.Invocations.Single().Arguments[1]!;
        Assert.Equal(true, Prop(opts, "respectReducedMotion"));
        Assert.Equal("true", cut.Find("canvas").GetAttribute("aria-hidden"));
    }

    [Fact]
    public void Celebration_engine_checks_prefers_reduced_motion_and_stops_after_one_frame()
    {
        var js = File.ReadAllText(Path.Combine(RepoRoot(), "wwwroot", "js", "theme.js"));
        var start = js.IndexOf("window.blazorCelebration = {", StringComparison.Ordinal);
        var end = js.IndexOf("window.blazorConfetti", start, StringComparison.Ordinal);
        var engine = js[start..end];
        Assert.Contains("prefers-reduced-motion: reduce", engine);
        // the still path must return before the next requestAnimationFrame is scheduled
        var stillAt = engine.IndexOf("if (still) {", StringComparison.Ordinal);
        var rafAt = engine.LastIndexOf("animationFrameId = requestAnimationFrame(animate)", StringComparison.Ordinal);
        Assert.True(stillAt > 0 && stillAt < rafAt, "the reduced-motion branch must come before the animation loop is re-armed");
    }

    [Fact]
    public void RewardsCenter_has_no_text_under_12px()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton<BlazorTwUI.Services.ToastService>();
        var cut = RenderComponent<TwRewardsCenter>();
        var tiny = Regex.Matches(cut.Markup, @"text-\[(\d+(?:\.\d+)?)px\]").Select(m => double.Parse(m.Groups[1].Value)).Where(px => px < 12).ToList();
        Assert.True(tiny.Count == 0, "Text under 12px: " + string.Join(", ", tiny));
    }
}
