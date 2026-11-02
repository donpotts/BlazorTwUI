namespace BlazorTwUI.Services;

using Microsoft.JSInterop;

public enum TwSoundEffect
{
    Confetti,
    Fireworks,
    FloatingReactions,
    Coins,
    Stars,
    Streamers,
    VictoryFanfare,
    PartyHorn
}

/// <summary>
/// Procedural Sound FX Service using 100% royalty-free Web Audio API synthesis.
/// Freely distributable anywhere with zero copyright or asset licensing restrictions.
/// </summary>
public class TwSoundService
{
    private readonly IJSRuntime _js;

    public TwSoundService(IJSRuntime js)
    {
        _js = js;
    }

    public async ValueTask PlayAsync(TwSoundEffect effect, double volume = 1.0)
    {
        var soundName = effect switch
        {
            TwSoundEffect.Fireworks => "fireworks",
            TwSoundEffect.FloatingReactions => "emojis",
            TwSoundEffect.Coins => "coins",
            TwSoundEffect.Stars => "stars",
            TwSoundEffect.Streamers => "streamers",
            TwSoundEffect.VictoryFanfare => "fanfare",
            TwSoundEffect.PartyHorn => "partyhorn",
            _ => "confetti"
        };

        try
        {
            await _js.InvokeVoidAsync("blazorSoundFx.play", soundName, volume);
        }
        catch
        { }
    }

    public async ValueTask PlayPopAsync(double volume = 0.5) => await PlayAsync(TwSoundEffect.FloatingReactions, volume);

    public async ValueTask PlayChimeAsync(double volume = 0.8) => await PlayAsync(TwSoundEffect.VictoryFanfare, volume);

    public async ValueTask SetMutedAsync(bool muted)
    {
        try
        {
            await _js.InvokeVoidAsync("blazorSoundFx.setMuted", muted);
        }
        catch
        { }
    }

    public async ValueTask SetVolumeAsync(double volume)
    {
        try
        {
            await _js.InvokeVoidAsync("blazorSoundFx.setVolume", volume);
        }
        catch
        { }
    }
}
