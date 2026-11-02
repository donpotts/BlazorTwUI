using Bunit;
using BlazorTwUI.Components.Theme;
using BlazorTwUI.Models;
using BlazorTwUI.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BlazorTwUI.Tests;

public class ThemeContrastTests : TestContext
{
    public ThemeContrastTests()
    {
        // setContrast returns the effective state: true only when 'high' was requested.
        JSInterop.Setup<bool>("blazorTheme.setContrast", inv => (string?)inv.Arguments[0] == "high").SetResult(true);
        JSInterop.Setup<bool>("blazorTheme.setContrast", inv => (string?)inv.Arguments[0] != "high").SetResult(false);
        Services.AddScoped<ThemeService>();
    }

    [Fact]
    public async Task Toggle_FlipsBetweenHighAndStandard_AndTellsTheBrowser()
    {
        var theme = Services.GetRequiredService<ThemeService>();
        Assert.False(theme.IsHighContrastEffective);

        await theme.ToggleHighContrastAsync();
        Assert.True(theme.IsHighContrastEffective);
        Assert.Equal(ContrastMode.High, theme.CurrentContrast);

        await theme.ToggleHighContrastAsync();
        Assert.False(theme.IsHighContrastEffective);
        Assert.Equal(ContrastMode.Standard, theme.CurrentContrast);

        Assert.Equal(new[] { "high", "standard" },
            JSInterop.Invocations["blazorTheme.setContrast"].Select(i => (string)i.Arguments[0]!));
    }

    [Fact]
    public async Task OsContrastChange_IsFollowedOnlyInSystemMode()
    {
        var theme = Services.GetRequiredService<ThemeService>();

        theme.OnSystemContrastChanged(true);
        Assert.True(theme.IsHighContrastEffective); // default mode is System: follows the OS

        await theme.SetContrastAsync(ContrastMode.Standard);
        theme.OnSystemContrastChanged(true);
        Assert.False(theme.IsHighContrastEffective); // an explicit choice wins over the OS
    }

    [Fact]
    public async Task SettingContrast_RaisesOnChange_SoViewsRepaint()
    {
        var theme = Services.GetRequiredService<ThemeService>();
        var raised = 0;
        theme.OnChange += () => raised++;

        await theme.SetContrastAsync(ContrastMode.High);

        Assert.Equal(1, raised);
    }

    [Fact]
    public async Task Switcher_ExposesContrastAsAPressedToggle()
    {
        var cut = RenderComponent<TwThemeSwitcher>();
        var button = cut.Find("[data-testid=contrast-toggle]");
        Assert.Equal("false", button.GetAttribute("aria-pressed"));
        Assert.Equal("High contrast", button.GetAttribute("aria-label"));

        button.Click();

        await cut.InvokeAsync(() => Task.CompletedTask);
        cut.WaitForAssertion(() =>
            Assert.Equal("true", cut.Find("[data-testid=contrast-toggle]").GetAttribute("aria-pressed")));
    }

    [Fact]
    public void Switcher_CanHideTheContrastToggle()
    {
        var cut = RenderComponent<TwThemeSwitcher>(p => p.Add(c => c.ShowContrastToggle, false));
        Assert.Empty(cut.FindAll("[data-testid=contrast-toggle]"));
    }
}
