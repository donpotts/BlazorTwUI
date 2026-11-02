namespace BlazorTwUI.Services;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorTwUI.Models;

/// <summary>
/// Enterprise Donut Caching Service for Blazor SSR and Interactive hybrid models.
/// Enables edge-cached static shells while dynamically perforating personalized donut holes.
/// </summary>
public class TwDonutCacheService
{
    private readonly ConcurrentDictionary<string, DonutCacheEntry> _cache = new();
    private readonly DonutCacheTelemetry _telemetry = new();

    public event Action? OnCacheStateChanged;

    public DonutCacheTelemetry Telemetry => _telemetry;
    public IReadOnlyCollection<DonutCacheEntry> Entries => _cache.Values.ToList();

    /// <summary>
    /// Gets an existing cached donut entry or evaluates the factory generator.
    /// </summary>
    public async Task<(DonutCacheEntry Entry, bool WasCacheHit)> GetOrCreateAsync(
        string key,
        Func<Task<string>> htmlFactory,
        TimeSpan? ttl = null,
        IEnumerable<string>? tags = null,
        int holeCount = 1)
    {
        var targetTtl = ttl ?? TimeSpan.FromMinutes(5);
        var targetTags = tags?.ToList() ?? new List<string> { "default" };

        if (_cache.TryGetValue(key, out var existing) && !existing.IsExpired)
        {
            existing.HitCount++;
            _telemetry.TotalHits++;
            _telemetry.EstimatedLatencySavedMs += Math.Max(12.0, existing.GenerationDurationMs);
            OnCacheStateChanged?.Invoke();
            return (existing, true);
        }

        // Cache Miss: Generate
        _telemetry.TotalMisses++;
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var html = await htmlFactory();
        sw.Stop();

        var entry = new DonutCacheEntry
        {
            Key = key,
            StaticHtml = html,
            CachedAt = DateTime.UtcNow,
            Ttl = targetTtl,
            Tags = targetTags,
            HitCount = 0,
            DynamicHoleCount = holeCount,
            GenerationDurationMs = sw.Elapsed.TotalMilliseconds
        };

        _cache[key] = entry;
        _telemetry.ActiveEntries = _cache.Count;
        OnCacheStateChanged?.Invoke();

        return (entry, false);
    }

    /// <summary>
    /// Programmatically invalidates cache entries matching a tag (tag-based eviction).
    /// </summary>
    public int InvalidateByTag(string tag)
    {
        var keysToRemove = _cache.Where(kvp => kvp.Value.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                                 .Select(kvp => kvp.Key)
                                 .ToList();

        int removed = 0;
        foreach (var k in keysToRemove)
        {
            if (_cache.TryRemove(k, out _))
            {
                removed++;
                _telemetry.EvictionCount++;
            }
        }

        _telemetry.ActiveEntries = _cache.Count;
        OnCacheStateChanged?.Invoke();
        return removed;
    }

    /// <summary>
    /// Clears all cached donut entries.
    /// </summary>
    public void ClearAll()
    {
        _telemetry.EvictionCount += _cache.Count;
        _cache.Clear();
        _telemetry.ActiveEntries = 0;
        OnCacheStateChanged?.Invoke();
    }
}
