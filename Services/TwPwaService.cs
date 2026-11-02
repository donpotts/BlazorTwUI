namespace BlazorTwUI.Services;

using Microsoft.JSInterop;

public class PwaStateDto
{
    public bool CanInstall { get; set; }
    public bool IsInstalled { get; set; }
}

public class TwPwaService : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private DotNetObjectReference<TwPwaService>? _dotNetRef;
    private bool _isInitialized = false;

    public bool CanInstall { get; private set; } = false;
    public bool IsInstalled { get; private set; } = false;

    public event Action? OnStateChanged;

    public TwPwaService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        try
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            var state = await _js.InvokeAsync<PwaStateDto>("blazorPwa.init", _dotNetRef);
            if (state != null)
            {
                CanInstall = state.CanInstall;
                IsInstalled = state.IsInstalled;
            }
            _isInitialized = true;
            OnStateChanged?.Invoke();
        }
        catch
        {
            // Fallback for non-browser or test runners
        }
    }

    public async Task<string> PromptInstallAsync()
    {
        try
        {
            var outcome = await _js.InvokeAsync<string>("blazorPwa.promptInstall");
            if (outcome == "accepted")
            {
                CanInstall = false;
                IsInstalled = true;
                OnStateChanged?.Invoke();
            }
            return outcome;
        }
        catch (Exception ex)
        {
            return $"error: {ex.Message}";
        }
    }

    [JSInvokable]
    public void OnPwaStateChanged(PwaStateDto state)
    {
        CanInstall = state.CanInstall;
        IsInstalled = state.IsInstalled;
        OnStateChanged?.Invoke();
    }

    public async ValueTask DisposeAsync()
    {
        _dotNetRef?.Dispose();
        await ValueTask.CompletedTask;
    }
}
