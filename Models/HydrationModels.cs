namespace BlazorTwUI.Models;

using System;

/// <summary>
/// Represents modern .NET 10+ unified Blazor render modes.
/// </summary>
public enum RenderModeKind
{
    StaticSsr,
    StreamingSsr,
    InteractiveServer,
    InteractiveWebAssembly,
    InteractiveAuto
}

/// <summary>
/// Holds state persisted between server prerendering (SSR) and client hydration.
/// Prevents the common Blazor double-fetch flicker.
/// </summary>
public class HydrationState<T>
{
    public string Key { get; set; } = string.Empty;
    public T? Value { get; set; }
    public DateTime PrerenderedAt { get; set; } = DateTime.UtcNow;
    public DateTime? HydratedAt { get; set; }
    public bool WasRestoredFromPrerender { get; set; }
    public RenderModeKind SourceRenderMode { get; set; } = RenderModeKind.InteractiveAuto;
}

/// <summary>
/// Telemetry metrics for server prerender and client hydration performance.
/// </summary>
public class HydrationTelemetry
{
    public int PrerenderPersistCount { get; set; }
    public int ClientRestoredCount { get; set; }
    public int FreshFetchCount { get; set; }
    public double MillisecondsSaved { get; set; }
    public long BytesTransferred { get; set; }
    public RenderModeKind CurrentMode { get; set; } = RenderModeKind.InteractiveAuto;
}

/// <summary>
/// Telemetry tick or real-time event item consumed by TwStreamingGrid.
/// </summary>
public class StreamingTelemetryItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Symbol { get; set; } = "MSFT";
    public string Service { get; set; } = "OrderRouting";
    public double Price { get; set; } = 420.50;
    public double Volume { get; set; } = 1500;
    public double LatencyMs { get; set; } = 1.2;
    public string Status { get; set; } = "Executed";
    public bool IsNewArrival { get; set; } = true;
}
