using System.Text.RegularExpressions;
using BlazorTwUI.Components.UI;
using BlazorTwUI.Models;
using Bunit;
using Xunit;

namespace BlazorTwUI.Tests;

public class LottieTests : TestContext
{
    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "BlazorTwUI.csproj"))) dir = dir.Parent;
        return dir?.FullName ?? throw new InvalidOperationException("Repo root not found.");
    }

    private static object? Prop(object o, string name) => o.GetType().GetProperty(name)?.GetValue(o);

    private sealed class Calls(BunitJSModuleInterop module)
    {
        public IReadOnlyList<JSRuntimeInvocation> Invocations => module.Invocations["create"];
    }

    private (BunitJSModuleInterop Module, Calls Create) Setup()
    {
        var module = JSInterop.SetupModule("./js/tw-lottie.js");
        module.SetupModule("create", _ => true);
        module.SetupVoid("destroy", _ => true).SetVoidResult();
        module.SetupVoid("play", _ => true).SetVoidResult();
        return (module, new Calls(module));
    }

    [Fact]
    public void Lottie_passes_its_parameters_and_reduced_motion_poster_by_default()
    {
        var (_, create) = Setup();
        var cut = RenderComponent<TwLottie>(p => p.Add(x => x.Src, "animations/success-check.lottie").Add(x => x.Loop, true).Add(x => x.Speed, 1.5).Add(x => x.Width, "96px"));
        var opts = create.Invocations.Single().Arguments[1]!;
        Assert.Equal("animations/success-check.lottie", Prop(opts, "src"));
        Assert.Equal(true, Prop(opts, "loop"));
        Assert.Equal(true, Prop(opts, "autoplay"));
        Assert.Equal(1.5, Prop(opts, "speed"));
        Assert.Equal("poster", Prop(opts, "reducedMotion"));
        Assert.Equal("poster", Prop(opts, "posterMarker"));
        Assert.Contains("width:96px;height:96px", cut.Find("[data-testid=lottie]").GetAttribute("style"));
    }

    [Fact]
    public void Lottie_is_hidden_from_screen_readers_unless_labelled()
    {
        Setup();
        var plain = RenderComponent<TwLottie>(p => p.Add(x => x.Src, "a.lottie"));
        Assert.Equal("true", plain.Find("[data-testid=lottie]").GetAttribute("aria-hidden"));
        Assert.Null(plain.Find("[data-testid=lottie]").GetAttribute("role"));

        var labelled = RenderComponent<TwLottie>(p => p.Add(x => x.Src, "a.lottie").Add(x => x.AriaLabel, "Loading orders"));
        var el = labelled.Find("[data-testid=lottie]");
        Assert.Equal("img", el.GetAttribute("role"));
        Assert.Equal("Loading orders", el.GetAttribute("aria-label"));
        Assert.Null(el.GetAttribute("aria-hidden"));
    }

    [Fact]
    public async Task Lottie_complete_raises_OnComplete()
    {
        Setup();
        var done = 0;
        var cut = RenderComponent<TwLottie>(p => p.Add(x => x.Src, "a.lottie").Add(x => x.OnComplete, () => done++));
        await cut.Instance.OnLottieComplete();
        Assert.Equal(1, done);
    }

    [Theory]
    [InlineData(CelebrationKind.Success, "success-check.lottie", "Done", "poster")]
    [InlineData(CelebrationKind.LevelUp, "level-up-star.lottie", "Level up!", "poster")]
    [InlineData(CelebrationKind.Confetti, "confetti-burst.lottie", "Well done!", "hide")]
    [InlineData(CelebrationKind.Badge, "badge-unlock-shine.lottie", "Badge unlocked", "poster")]
    [InlineData(CelebrationKind.Streak, "streak-flame.lottie", "Streak kept going", "poster")]
    public void Celebration_maps_kind_to_sample_plays_once_and_announces(CelebrationKind kind, string file, string text, string reduced)
    {
        var (_, create) = Setup();
        var cut = RenderComponent<TwCelebration>(p => p.Add(x => x.Kind, kind));
        var opts = create.Invocations.Single().Arguments[1]!;
        Assert.Equal("animations/" + file, Prop(opts, "src"));
        Assert.Equal(false, Prop(opts, "loop"));
        Assert.Equal(reduced, Prop(opts, "reducedMotion"));
        var msg = cut.Find("[data-testid=celebration-message]");
        Assert.Equal("polite", msg.GetAttribute("aria-live"));
        Assert.Equal(text, msg.TextContent.Trim());
        Assert.True(File.Exists(Path.Combine(RepoRoot(), "wwwroot", "animations", file)), file + " is bundled");
    }

    [Fact]
    public void Player_script_holds_the_poster_under_reduced_motion_and_never_uses_a_cdn()
    {
        var js = File.ReadAllText(Path.Combine(RepoRoot(), "wwwroot", "js", "tw-lottie.js"));
        Assert.Contains("prefers-reduced-motion: reduce", js);
        Assert.Matches(new Regex(@"autoplay:\s*!!opts\.autoplay\s*&&\s*!still\(\)"), js);
        Assert.Contains("setWasmUrl", js);
        Assert.Contains("marker.start", js); // dotlottie-web 0.81 markers are { name, start, end }: reading .time alone held frame 0 (blank for 4 of 10 samples)
        Assert.DoesNotContain("cdn.jsdelivr", js);
        Assert.DoesNotContain("unpkg.com", js);
        Assert.True(File.Exists(Path.Combine(RepoRoot(), "wwwroot", "lib", "dotlottie", "dotlottie-player.wasm")));
        Assert.Contains("0.81.0", File.ReadAllText(Path.Combine(RepoRoot(), "wwwroot", "lib", "dotlottie", "dotlottie-web.js")));
    }

    [Fact]
    public void Notices_name_dotlottie_web_with_its_mit_licence_and_the_samples_provenance()
    {
        var notices = File.ReadAllText(Path.Combine(RepoRoot(), "THIRD_PARTY_NOTICES.md"));
        var start = notices.IndexOf("## dotlottie-web", StringComparison.Ordinal);
        Assert.True(start >= 0, "THIRD_PARTY_NOTICES.md needs a '## dotlottie-web' section");
        var section = notices[start..];
        Assert.Contains("0.81.0", section);
        Assert.Contains("MIT License", section);
        Assert.Contains("Copyright (c) 2023 LottieFiles.com", section);
        Assert.Contains("authored by Ask2App", notices);
    }
}
