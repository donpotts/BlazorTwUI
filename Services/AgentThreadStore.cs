namespace BlazorTwUI.Services;

using System.Text.Json;
using Microsoft.JSInterop;

public sealed record ThreadMessage(string Role, string Text);

public sealed class AgentThread
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..10];
    public string Title { get; set; } = "New thread";
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
    public bool Pinned { get; set; }
    public bool Archived { get; set; }
    public List<ThreadMessage> Messages { get; set; } = new();

    /// <summary>Last message, for the list preview.</summary>
    public string Preview => Messages.Count == 0 ? "" : Messages[^1].Text;
}

public interface IAgentThreadStore
{
    event Action? Changed;
    IReadOnlyList<AgentThread> All { get; }
    AgentThread? Get(string id);
    AgentThread Create(string? title = null);
    void Rename(string id, string title);
    void SetPinned(string id, bool pinned);
    void SetArchived(string id, bool archived);
    void Delete(string id);
    void Append(string id, string role, string text);
    string ExportJson();
    void ImportJson(string json);
}

/// <summary>
/// Conversation threads kept in memory and mirrored to <c>localStorage</c> so they survive a reload.
/// Call <see cref="LoadAsync"/> once on startup; persistence failures (private mode, quota) never break the UI.
/// </summary>
public sealed class AgentThreadStore : IAgentThreadStore
{
    private const string StorageKey = "btui-agent-threads";
    private static readonly JsonSerializerOptions Json = new() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private readonly List<AgentThread> _threads = new();
    private readonly IJSRuntime? _js;
    private bool _loaded;

    public AgentThreadStore(IJSRuntime? js = null) => _js = js;

    public event Action? Changed;

    /// <summary>Pinned first, then most recently updated. Archived threads are included; filter in the UI.</summary>
    public IReadOnlyList<AgentThread> All => _threads
        .OrderByDescending(t => t.Pinned)
        .ThenByDescending(t => t.UpdatedUtc)
        .ToList();

    public AgentThread? Get(string id) => _threads.FirstOrDefault(t => t.Id == id);

    public AgentThread Create(string? title = null)
    {
        var thread = new AgentThread();
        if (!string.IsNullOrWhiteSpace(title)) thread.Title = title.Trim();
        _threads.Add(thread);
        Touch();
        return thread;
    }

    public void Rename(string id, string title)
    {
        if (Get(id) is not { } t || string.IsNullOrWhiteSpace(title)) return;
        t.Title = title.Trim();
        Touch();
    }

    public void SetPinned(string id, bool pinned)
    {
        if (Get(id) is not { } t) return;
        t.Pinned = pinned;
        Touch();
    }

    public void SetArchived(string id, bool archived)
    {
        if (Get(id) is not { } t) return;
        t.Archived = archived;
        Touch();
    }

    public void Delete(string id)
    {
        if (_threads.RemoveAll(t => t.Id == id) > 0) Touch();
    }

    public void Append(string id, string role, string text)
    {
        if (Get(id) is not { } t || string.IsNullOrWhiteSpace(text)) return;
        // The first user message names the thread.
        if (t.Messages.Count == 0 && role == "user" && t.Title == "New thread")
            t.Title = text.Length > 48 ? text[..48].TrimEnd() + "…" : text;
        t.Messages.Add(new ThreadMessage(role, text));
        t.UpdatedUtc = DateTime.UtcNow;
        Touch();
    }

    public string ExportJson() => JsonSerializer.Serialize(_threads, Json);

    public void ImportJson(string json)
    {
        var incoming = JsonSerializer.Deserialize<List<AgentThread>>(json, Json) ?? new();
        foreach (var thread in incoming.Where(i => _threads.All(t => t.Id != i.Id)))
            _threads.Add(thread);
        Touch();
    }

    public async Task LoadAsync()
    {
        if (_loaded || _js is null) return;
        _loaded = true;
        try
        {
            var json = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            if (!string.IsNullOrWhiteSpace(json)) ImportJson(json);
        }
        catch (Exception ex) when (ex is JSException or InvalidOperationException or JsonException or JSDisconnectedException)
        {
            // Storage unavailable or corrupt: start empty.
        }
    }

    private void Touch()
    {
        Changed?.Invoke();
        if (_js is null) return;
        _ = PersistAsync();
    }

    private async Task PersistAsync()
    {
        try { await _js!.InvokeVoidAsync("localStorage.setItem", StorageKey, ExportJson()); }
        catch (Exception ex) when (ex is JSException or InvalidOperationException or JSDisconnectedException) { }
    }
}
