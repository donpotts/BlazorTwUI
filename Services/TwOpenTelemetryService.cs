namespace BlazorTwUI.Services;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using BlazorTwUI.Models;

/// <summary>
/// Native OpenTelemetry (OTel) Instrumentation and ActivitySource Service.
/// Collects high-precision component lifecycle spans, render latencies, WASM heap allocations, and exports W3C OTLP JSON.
/// </summary>
public class TwOpenTelemetryService : IDisposable
{
    private static readonly ActivitySource Tracer = new("BlazorTwUI.Components", "0.7.0");
    private readonly ConcurrentQueue<ComponentTraceSpan> _spans = new();
    private readonly ConcurrentDictionary<string, List<double>> _componentTimings = new();
    private readonly ConcurrentDictionary<string, int> _componentRenderCounts = new();
    private readonly ConcurrentDictionary<string, int> _componentSlowFrames = new();
    private const int MaxSpans = 250;

    public event Action? OnTraceRecorded;
    public event Action? OnMetricsUpdated;

    public bool IsEnabled { get; set; } = true;
    public double CurrentFps { get; private set; } = 60.0;
    public double AverageRenderMs { get; private set; } = 1.4;
    public long HeapMemoryBytes => GC.GetTotalMemory(false);
    public int Gen0Collections => GC.CollectionCount(0);
    public int Gen1Collections => GC.CollectionCount(1);
    public int Gen2Collections => GC.CollectionCount(2);

    public IReadOnlyList<ComponentTraceSpan> RecentSpans => _spans.Reverse().Take(100).ToList();
    public int TotalRecordedCount { get; private set; }

    public IReadOnlyList<ComponentPerformanceSummary> PerformanceSummaries
    {
        get
        {
            var list = new List<ComponentPerformanceSummary>();
            foreach (var kvp in _componentTimings)
            {
                var name = kvp.Key;
                var durations = kvp.Value;
                if (durations.Count == 0) continue;

                lock (durations)
                {
                    var count = _componentRenderCounts.TryGetValue(name, out var c) ? c : durations.Count;
                    var total = durations.Sum();
                    var avg = total / durations.Count;
                    var max = durations.Max();
                    var min = durations.Min();
                    var slow = _componentSlowFrames.TryGetValue(name, out var s) ? s : 0;

                    list.Add(new ComponentPerformanceSummary(
                        name,
                        count,
                        total,
                        avg,
                        max,
                        min,
                        slow,
                        DateTime.UtcNow
                    ));
                }
            }
            return list.OrderByDescending(x => x.TotalDurationMs).ToList();
        }
    }

    /// <summary>
    /// Starts an IDisposable execution span for automatic duration and memory delta tracking.
    /// </summary>
    public IDisposable StartSpan(string componentName, string operation, string category = "Render", Dictionary<string, string>? initialTags = null)
    {
        if (!IsEnabled)
        {
            return NullScope.Instance;
        }

        var tags = initialTags ?? new Dictionary<string, string>();
        tags["component.type"] = componentName;
        tags["operation"] = operation;
        tags["category"] = category;

        var startTimestamp = Stopwatch.GetTimestamp();
        var startMemory = GC.GetTotalMemory(false);
        var startTime = DateTime.UtcNow;

        var activity = Tracer.StartActivity($"{componentName}.{operation}", ActivityKind.Internal);
        var traceId = activity?.TraceId.ToHexString() ?? Guid.NewGuid().ToString("N");
        var spanId = activity?.SpanId.ToHexString() ?? Guid.NewGuid().ToString("N")[..16];
        var parentSpanId = activity?.ParentSpanId.ToHexString();

        return new TelemetryScope(this, componentName, operation, category, traceId, spanId, parentSpanId, startTimestamp, startTime, startMemory, tags, activity);
    }

    /// <summary>
    /// Records an execution span for a Blazor component lifecycle or service operation.
    /// </summary>
    public ComponentTraceSpan RecordSpan(string componentName, string operationName, double durationUs, Dictionary<string, string>? attributes = null, string category = "Render")
    {
        using var activity = Tracer.StartActivity($"{componentName}.{operationName}");
        if (attributes != null && activity != null)
        {
            foreach (var kvp in attributes)
            {
                activity.SetTag(kvp.Key, kvp.Value);
            }
        }

        var durationMs = durationUs / 1000.0;
        var span = new ComponentTraceSpan
        {
            TraceId = activity?.TraceId.ToHexString() ?? Guid.NewGuid().ToString("N"),
            SpanId = activity?.SpanId.ToHexString() ?? Guid.NewGuid().ToString("N")[..16],
            ParentSpanId = activity?.ParentSpanId.ToHexString(),
            ComponentName = componentName,
            OperationName = operationName,
            Category = category,
            DurationMicroseconds = durationUs,
            Timestamp = DateTime.UtcNow,
            Attributes = attributes ?? new Dictionary<string, string>(),
            MemoryAllocatedBytes = GC.GetAllocatedBytesForCurrentThread()
        };

        _spans.Enqueue(span);
        while (_spans.Count > MaxSpans && _spans.TryDequeue(out _)) { }

        // Update component aggregations
        var timings = _componentTimings.GetOrAdd(componentName, _ => new List<double>());
        lock (timings)
        {
            timings.Add(durationMs);
            if (timings.Count > 100) timings.RemoveAt(0);
        }
        _componentRenderCounts.AddOrUpdate(componentName, 1, (_, current) => current + 1);

        if (span.Health == "Critical")
        {
            _componentSlowFrames.AddOrUpdate(componentName, 1, (_, current) => current + 1);
        }

        var allRecent = _spans.TakeLast(30).ToList();
        if (allRecent.Count > 0)
        {
            AverageRenderMs = allRecent.Average(s => s.DurationMs);
        }

        TotalRecordedCount++;
        OnTraceRecorded?.Invoke();
        OnMetricsUpdated?.Invoke();
        return span;
    }

    /// <summary>
    /// Updates current frames per second measurement.
    /// </summary>
    public void UpdateFps(double fps)
    {
        CurrentFps = Math.Clamp(fps, 0, 240);
        OnMetricsUpdated?.Invoke();
    }

    /// <summary>
    /// Clears recorded trace history.
    /// </summary>
    public void Clear()
    {
        while (_spans.TryDequeue(out _)) { }
        _componentTimings.Clear();
        _componentRenderCounts.Clear();
        _componentSlowFrames.Clear();
        OnTraceRecorded?.Invoke();
        OnMetricsUpdated?.Invoke();
    }

    /// <summary>
    /// Generates simulated burst of component lifecycle spans across fullstack architecture.
    /// </summary>
    public void GenerateSimulatedBurst()
    {
        var components = new[] { "TwDashboard", "TwPdfFormFiller", "TwUnionStudio", "TwStreamingGrid", "TwDonutCache", "TwHydratedView" };
        var operations = new[] { "BuildRenderTree", "OnParametersSetAsync", "StateHasChanged", "ComputeFlattenedPdf", "DiscriminatedMatch", "HydrateFromCache" };
        var rand = new Random();

        for (int i = 0; i < 6; i++)
        {
            var comp = components[i % components.Length];
            var op = operations[i % operations.Length];
            double us = rand.Next(150, 4200) + rand.NextDouble();
            RecordSpan(comp, op, us, new Dictionary<string, string>
            {
                ["render.mode"] = "InteractiveAuto",
                ["otel.status"] = "OK",
                ["w3c.sampled"] = "01",
                ["dotnet.async_v2"] = "true"
            });
        }
    }

    /// <summary>
    /// Exports all recorded spans in standard W3C OTLP JSON format.
    /// </summary>
    public string ExportOtlpJson()
    {
        var spansList = _spans.ToList();
        var payload = new
        {
            resourceSpans = new[]
            {
                new
                {
                    resource = new
                    {
                        attributes = new[]
                        {
                            new { key = "service.name", value = new { stringValue = "BlazorTwUI" } },
                            new { key = "service.version", value = new { stringValue = "0.7.0" } },
                            new { key = "telemetry.sdk.language", value = new { stringValue = "dotnet" } },
                            new { key = "host.runtime", value = new { stringValue = ".NET 10+ WebAssembly" } }
                        }
                    },
                    scopeSpans = new[]
                    {
                        new
                        {
                            scope = new { name = "BlazorTwUI.Components", version = "0.7.0" },
                            spans = spansList.Select(s => new
                            {
                                traceId = s.TraceId,
                                spanId = s.SpanId,
                                parentSpanId = s.ParentSpanId ?? string.Empty,
                                name = $"{s.ComponentName}.{s.OperationName}",
                                kind = 1, // INTERNAL
                                startTimeUnixNano = new DateTimeOffset(s.Timestamp).ToUnixTimeMilliseconds() * 1_000_000,
                                endTimeUnixNano = (new DateTimeOffset(s.Timestamp).ToUnixTimeMilliseconds() + (long)s.DurationMs) * 1_000_000,
                                attributes = s.Attributes.Select(kv => new
                                {
                                    key = kv.Key,
                                    value = new { stringValue = kv.Value }
                                }).ToArray(),
                                status = new
                                {
                                    code = s.Health == "Critical" ? 2 : 1
                                }
                            }).ToArray()
                        }
                    }
                }
            }
        };

        return JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
    }

    public void Dispose()
    {
        Tracer.Dispose();
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();
        public void Dispose() { }
    }

    private sealed class TelemetryScope : IDisposable
    {
        private readonly TwOpenTelemetryService _service;
        private readonly string _componentName;
        private readonly string _operation;
        private readonly string _category;
        private readonly string _traceId;
        private readonly string _spanId;
        private readonly string? _parentSpanId;
        private readonly long _startTimestamp;
        private readonly DateTime _startTime;
        private readonly long _startMemory;
        private readonly Dictionary<string, string> _tags;
        private readonly Activity? _activity;
        private bool _disposed;

        public TelemetryScope(
            TwOpenTelemetryService service,
            string componentName,
            string operation,
            string category,
            string traceId,
            string spanId,
            string? parentSpanId,
            long startTimestamp,
            DateTime startTime,
            long startMemory,
            Dictionary<string, string> tags,
            Activity? activity)
        {
            _service = service;
            _componentName = componentName;
            _operation = operation;
            _category = category;
            _traceId = traceId;
            _spanId = spanId;
            _parentSpanId = parentSpanId;
            _startTimestamp = startTimestamp;
            _startTime = startTime;
            _startMemory = startMemory;
            _tags = tags;
            _activity = activity;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            var endTimestamp = Stopwatch.GetTimestamp();
            var durationMs = (double)(endTimestamp - _startTimestamp) * 1000.0 / Stopwatch.Frequency;
            var durationUs = durationMs * 1000.0;
            var memoryDelta = Math.Max(0, GC.GetTotalMemory(false) - _startMemory);

            _activity?.SetEndTime(DateTime.UtcNow);
            _activity?.Dispose();

            _tags["memory.delta_bytes"] = memoryDelta.ToString();

            _service.RecordSpan(
                _componentName,
                _operation,
                durationUs,
                _tags,
                _category
            );
        }
    }
}
