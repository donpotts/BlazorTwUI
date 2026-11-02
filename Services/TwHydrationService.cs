namespace BlazorTwUI.Services;

using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;
using BlazorTwUI.Models;

/// <summary>
/// Universal Zero-Flicker Hydration and State Persistence Service for Blazor .NET 10+ Fullstack Web Apps.
/// Bridges Server Prerendering (Static SSR) and Client Hydration (WASM/Server/Auto)
/// without redundant database roundtrips.
/// </summary>
public class TwHydrationService
{
    private readonly ConcurrentDictionary<string, string> _persistedStore = new();
    private readonly HydrationTelemetry _telemetry = new();

    public event Action? OnTelemetryUpdated;

    public HydrationTelemetry Telemetry => _telemetry;

    /// <summary>
    /// Gets a value from the persisted hydration store if available, or fetches it via the provided loader.
    /// When restoring from prerender state, the loader is NOT executed, saving CPU cycles and database load.
    /// </summary>
    public async Task<HydrationState<T>> GetOrLoadAsync<T>(
        string key, 
        Func<Task<T>> loader, 
        RenderModeKind renderMode = RenderModeKind.InteractiveAuto,
        bool simulatePrerenderDelay = false)
    {
        // 1. Check if state was persisted during prerender
        if (_persistedStore.TryRemove(key, out var json) && !string.IsNullOrEmpty(json))
        {
            try
            {
                var val = JsonSerializer.Deserialize<T>(json);
                _telemetry.ClientRestoredCount++;
                _telemetry.MillisecondsSaved += 240.0; // average simulated network/DB roundtrip saved
                OnTelemetryUpdated?.Invoke();

                return new HydrationState<T>
                {
                    Key = key,
                    Value = val,
                    WasRestoredFromPrerender = true,
                    HydratedAt = DateTime.UtcNow,
                    SourceRenderMode = renderMode
                };
            }
            catch
            {
                // Fall back to loader if deserialization fails
            }
        }

        // 2. Fresh fetch executed (either during initial prerender or without hydration cache)
        if (simulatePrerenderDelay)
        {
            await Task.Delay(240); // Simulate database query delay
        }

        var freshValue = await loader();
        _telemetry.FreshFetchCount++;

        // Automatically persist into store for subsequent client hydration
        var serialized = JsonSerializer.Serialize(freshValue);
        _persistedStore[key] = serialized;
        _telemetry.PrerenderPersistCount++;
        _telemetry.BytesTransferred += serialized.Length;
        OnTelemetryUpdated?.Invoke();

        return new HydrationState<T>
        {
            Key = key,
            Value = freshValue,
            WasRestoredFromPrerender = false,
            PrerenderedAt = DateTime.UtcNow,
            SourceRenderMode = renderMode
        };
    }

    /// <summary>
    /// Explicitly persists a value for client-side restoration.
    /// </summary>
    public void Persist<T>(string key, T value)
    {
        var serialized = JsonSerializer.Serialize(value);
        _persistedStore[key] = serialized;
        _telemetry.PrerenderPersistCount++;
        _telemetry.BytesTransferred += serialized.Length;
        OnTelemetryUpdated?.Invoke();
    }

    /// <summary>
    /// Clears simulated hydration store to re-test cold fetches.
    /// </summary>
    public void Reset()
    {
        _persistedStore.Clear();
        _telemetry.ClientRestoredCount = 0;
        _telemetry.FreshFetchCount = 0;
        _telemetry.PrerenderPersistCount = 0;
        _telemetry.MillisecondsSaved = 0;
        _telemetry.BytesTransferred = 0;
        OnTelemetryUpdated?.Invoke();
    }
}
