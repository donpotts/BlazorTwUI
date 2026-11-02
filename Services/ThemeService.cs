using BlazorTwUI.Models;
using Microsoft.JSInterop;

namespace BlazorTwUI.Services;

public sealed class ThemeService : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private DotNetObjectReference<ThemeService>? _dotNetRef;
    private bool _initialized;

    public ThemeMode CurrentMode { get; private set; } = ThemeMode.System;
    public AccentTheme CurrentAccent { get; private set; } = AccentTheme.Indigo;
    public string? CustomHex { get; private set; }
    public bool IsDarkEffective { get; private set; }
    public ContrastMode CurrentContrast { get; private set; } = ContrastMode.System;
    public bool IsHighContrastEffective { get; private set; }
    public bool IsCustomAccent => !string.IsNullOrEmpty(CustomHex);

    public event Action? OnChange;

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;

        try
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            var state = await _jsRuntime.InvokeAsync<ThemeInitState>("blazorTheme.init", _dotNetRef);

            CurrentMode = state.Mode?.ToLowerInvariant() switch
            {
                "light" => ThemeMode.Light,
                "dark" => ThemeMode.Dark,
                _ => ThemeMode.System
            };

            CustomHex = state.CustomHex;
            if (!string.IsNullOrEmpty(state.CustomHex))
            {
                // Custom hex active
                CurrentAccent = AccentTheme.Indigo;
            }
            else
            {
                CurrentAccent = state.Accent?.ToLowerInvariant() switch
                {
                    "emerald" => AccentTheme.Emerald,
                    "rose" => AccentTheme.Rose,
                    "amber" => AccentTheme.Amber,
                    "violet" => AccentTheme.Violet,
                    "sky" => AccentTheme.Sky,
                    "cyan" => AccentTheme.Cyan,
                    "teal" => AccentTheme.Teal,
                    "fuchsia" => AccentTheme.Fuchsia,
                    "pink" => AccentTheme.Pink,
                    "orange" => AccentTheme.Orange,
                    "slate" => AccentTheme.Slate,
                    _ => AccentTheme.Indigo
                };
            }

            IsDarkEffective = state.IsDark;
            CurrentContrast = state.Contrast?.ToLowerInvariant() switch
            {
                "high" => ContrastMode.High,
                "standard" => ContrastMode.Standard,
                _ => ContrastMode.System
            };
            IsHighContrastEffective = state.IsHighContrast;
        }
        catch (Exception)
        {
            // Prerender or unmounted fallback
            CurrentMode = ThemeMode.System;
            CurrentAccent = AccentTheme.Indigo;
            IsDarkEffective = false;
        }

        _initialized = true;
        NotifyStateChanged();
    }

    public async Task SetModeAsync(ThemeMode mode)
    {
        CurrentMode = mode;
        var modeStr = mode.ToString().ToLowerInvariant();
        try
        {
            IsDarkEffective = await _jsRuntime.InvokeAsync<bool>("blazorTheme.setMode", modeStr);
        }
        catch (Exception)
        {
            IsDarkEffective = mode == ThemeMode.Dark;
        }
        NotifyStateChanged();
    }

    public async Task CycleModeAsync()
    {
        var next = CurrentMode switch
        {
            ThemeMode.Light => ThemeMode.Dark,
            ThemeMode.Dark => ThemeMode.System,
            _ => ThemeMode.Light
        };
        await SetModeAsync(next);
    }

    public async Task SetAccentAsync(AccentTheme accent)
    {
        CurrentAccent = accent;
        CustomHex = null;
        var accentStr = accent.ToString().ToLowerInvariant();
        try
        {
            await _jsRuntime.InvokeVoidAsync("blazorTheme.setAccent", accentStr);
        }
        catch (Exception)
        {
            // Interop fallback
        }
        NotifyStateChanged();
    }

    public async Task SetCustomAccentAsync(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex)) return;
        CustomHex = hex;
        try
        {
            await _jsRuntime.InvokeVoidAsync("blazorTheme.setCustomAccent", hex);
        }
        catch (Exception)
        {
            // Interop fallback
        }
        NotifyStateChanged();
    }

    public async Task SetContrastAsync(ContrastMode mode)
    {
        CurrentContrast = mode;
        try
        {
            IsHighContrastEffective = await _jsRuntime.InvokeAsync<bool>("blazorTheme.setContrast", mode.ToString().ToLowerInvariant());
        }
        catch (Exception)
        {
            IsHighContrastEffective = mode == ContrastMode.High;
        }
        NotifyStateChanged();
    }

    /// <summary>Toggles between forced high contrast and forced standard contrast.</summary>
    public Task ToggleHighContrastAsync() =>
        SetContrastAsync(IsHighContrastEffective ? ContrastMode.Standard : ContrastMode.High);

    [JSInvokable]
    public void OnSystemContrastChanged(bool isHigh)
    {
        if (CurrentContrast == ContrastMode.System)
        {
            IsHighContrastEffective = isHigh;
            NotifyStateChanged();
        }
    }

    [JSInvokable]
    public void OnSystemThemeChanged(bool isDark)
    {
        if (CurrentMode == ThemeMode.System)
        {
            IsDarkEffective = isDark;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("blazorTheme.dispose");
        }
        catch (JSDisconnectedException)
        {
            // Ignore when circuit/runtime disconnects
        }
        _dotNetRef?.Dispose();
    }

    private sealed class ThemeInitState
    {
        public string? Mode { get; set; }
        public string? Accent { get; set; }
        public string? CustomHex { get; set; }
        public bool IsDark { get; set; }
        public string? Contrast { get; set; }
        public bool IsHighContrast { get; set; }
    }
}
