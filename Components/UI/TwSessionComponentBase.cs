namespace BlazorTwUI.Components.UI;

using BlazorTwUI.Services;
using Microsoft.AspNetCore.Components;

/// <summary>Base for components that render an <see cref="AgentRunSession"/> and re-render when it changes.</summary>
public abstract class TwSessionComponentBase : ComponentBase, IDisposable
{
    private AgentRunSession? _subscribed;
    private bool _renderQueued;
    private int _renderedVersion = -1;

    [Parameter, EditorRequired] public AgentRunSession Session { get; set; } = default!;
    [Parameter] public string? Class { get; set; }

    protected override void OnParametersSet()
    {
        if (ReferenceEquals(_subscribed, Session)) return;
        if (_subscribed is not null) _subscribed.Changed -= OnSessionChanged;
        _subscribed = Session;
        if (_subscribed is not null) _subscribed.Changed += OnSessionChanged;
    }

    // A single agent step can raise several notifications back to back. Queue one render for the burst so
    // the UI paints the final state once instead of repainting (and re-keying event handlers) per notification.
    private void OnSessionChanged()
    {
        if (_renderQueued) return;
        _renderQueued = true;
        _ = InvokeAsync(() =>
        {
            _renderQueued = false;
            // A parent render may already have painted this version; repainting would only re-key handlers.
            if (_renderedVersion != Session.Version) StateHasChanged();
        });
    }

    protected override void OnAfterRender(bool firstRender) => _renderedVersion = Session.Version;

    public virtual void Dispose()
    {
        if (_subscribed is not null) _subscribed.Changed -= OnSessionChanged;
    }
}
