using BlazorTwUI.Models;

namespace BlazorTwUI.Services;

public sealed record ToastMessage(
    Guid Id,
    string Title,
    string? Description,
    SemanticVariant Variant,
    DateTime CreatedAt);

public sealed class ToastService
{
    private readonly List<ToastMessage> _toasts = new();
    public IReadOnlyList<ToastMessage> Toasts => _toasts;

    public event Action? OnChange;

    public void Show(string title, string? description = null, SemanticVariant variant = SemanticVariant.Default, int durationMs = 4000)
    {
        var toast = new ToastMessage(Guid.NewGuid(), title, description, variant, DateTime.UtcNow);
        _toasts.Add(toast);
        OnChange?.Invoke();

        if (durationMs > 0)
        {
            _ = DismissAfterAsync(toast.Id, durationMs);
        }
    }

    public void Dismiss(Guid id)
    {
        if (_toasts.RemoveAll(t => t.Id == id) > 0)
        {
            OnChange?.Invoke();
        }
    }

    private async Task DismissAfterAsync(Guid id, int durationMs)
    {
        await Task.Delay(durationMs);
        Dismiss(id);
    }
}
