namespace BlazorTwUI.Models;

using System;
using System.Collections.Generic;

/// <summary>
/// Modern C# 15 Discriminated Union pattern for type-safe asynchronous UI state machines.
/// Enables compiler-checked exhaustiveness and eliminates untyped null states.
/// </summary>
public abstract record UiState<T>
{
    public sealed record Loading(string? Message = "Loading...", double? ProgressPercent = null) : UiState<T>
    {
        public Loading() : this("Loading...", null) { }
    }

    public sealed record Success(T Data) : UiState<T>;

    public sealed record Empty(string? Title = "No Records Found", string? Description = "No active records match the criteria.") : UiState<T>
    {
        public Empty() : this("No Records Found", "No active records match the criteria.") { }
    }

    public sealed record Failure(string Message, Exception? Exception = null, string? ErrorCode = null, DateTime? Timestamp = null) : UiState<T>
    {
        public DateTime TimestampUtc { get; init; } = Timestamp ?? DateTime.UtcNow;
    }

    public bool IsLoading => this is Loading;
    public bool IsSuccess => this is Success;
    public bool IsEmpty => this is Empty;
    public bool IsFailure => this is Failure;

    public T? DataOrNull => this is Success s ? s.Data : default;
    public T? DataOrDefault => this is Success s ? s.Data : default;

    public TResult Match<TResult>(
        Func<Loading, TResult> onLoading,
        Func<Success, TResult> onSuccess,
        Func<Empty, TResult> onEmpty,
        Func<Failure, TResult> onFailure) => this switch
    {
        Loading l => onLoading(l),
        Success s => onSuccess(s),
        Empty e => onEmpty(e),
        Failure f => onFailure(f),
        _ => throw new InvalidOperationException("Unreachable UI state variant.")
    };

    public async Task<TResult> MatchAsync<TResult>(
        Func<Loading, Task<TResult>> onLoading,
        Func<Success, Task<TResult>> onSuccess,
        Func<Empty, Task<TResult>> onEmpty,
        Func<Failure, Task<TResult>> onFailure) => this switch
    {
        Loading l => await onLoading(l),
        Success s => await onSuccess(s),
        Empty e => await onEmpty(e),
        Failure f => await onFailure(f),
        _ => throw new InvalidOperationException("Unreachable UI state variant.")
    };

    public static UiState<T> CreateLoading(string? msg = null, double? progress = null) => new Loading(msg ?? "Loading...", progress);
    public static UiState<T> CreateSuccess(T data) => new Success(data);
    public static UiState<T> CreateEmpty(string? title = null, string? desc = null) => new Empty(title, desc);
    public static UiState<T> CreateFailure(string msg, Exception? ex = null, string? code = null) => new Failure(msg, ex, code);
}

/// <summary>
/// Functional Result union type for robust operation handling.
/// </summary>
public abstract record Result<T, E>
{
    public sealed record Ok(T Value) : Result<T, E>;
    public sealed record Err(E Error) : Result<T, E>;

    public bool IsOk => this is Ok;
    public bool IsErr => this is Err;

    public T? ValueOrDefault => this is Ok ok ? ok.Value : default;
    public E? ErrorOrDefault => this is Err err ? err.Error : default;

    public TResult Match<TResult>(Func<T, TResult> onOk, Func<E, TResult> onErr) => this switch
    {
        Ok ok => onOk(ok.Value),
        Err err => onErr(err.Error),
        _ => throw new InvalidOperationException("Unreachable result branch.")
    };

    public async Task<TResult> MatchAsync<TResult>(Func<T, Task<TResult>> onOk, Func<E, Task<TResult>> onErr) => this switch
    {
        Ok ok => await onOk(ok.Value),
        Err err => await onErr(err.Error),
        _ => throw new InvalidOperationException("Unreachable result branch.")
    };

    public Result<U, E> Map<U>(Func<T, U> mapper) => this switch
    {
        Ok ok => new Result<U, E>.Ok(mapper(ok.Value)),
        Err err => new Result<U, E>.Err(err.Error),
        _ => throw new InvalidOperationException("Unreachable result branch.")
    };

    public Result<U, E> Bind<U>(Func<T, Result<U, E>> binder) => this switch
    {
        Ok ok => binder(ok.Value),
        Err err => new Result<U, E>.Err(err.Error),
        _ => throw new InvalidOperationException("Unreachable result branch.")
    };

    public static Result<T, E> Success(T value) => new Ok(value);
    public static Result<T, E> Failure(E error) => new Err(error);

    public static implicit operator Result<T, E>(T value) => new Ok(value);
}

/// <summary>
/// Donut Caching model representing a cached SSR/prerendered segment with dynamic hole perforations.
/// </summary>
public class DonutCacheEntry
{
    public string Key { get; set; } = string.Empty;
    public string StaticHtml { get; set; } = string.Empty;
    public DateTime CachedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan Ttl { get; set; } = TimeSpan.FromMinutes(5);
    public List<string> Tags { get; set; } = new();
    public int HitCount { get; set; }
    public int DynamicHoleCount { get; set; }
    public double GenerationDurationMs { get; set; }

    public bool IsExpired => DateTime.UtcNow - CachedAt > Ttl;
}

/// <summary>
/// Donut Cache runtime telemetry metrics.
/// </summary>
public class DonutCacheTelemetry
{
    public int TotalHits { get; set; }
    public int TotalMisses { get; set; }
    public int ActiveEntries { get; set; }
    public int EvictionCount { get; set; }
    public double EstimatedLatencySavedMs { get; set; }
    public double HitRatio => (TotalHits + TotalMisses) == 0 ? 0 : (double)TotalHits / (TotalHits + TotalMisses);
}

/// <summary>
/// Zstandard compression benchmark result.
/// </summary>
public class ZstandardBenchmarkResult
{
    public string PayloadName { get; set; } = string.Empty;
    public long OriginalSizeBytes { get; set; }
    public long CompressedSizeBytes { get; set; }
    public double CompressionRatio => OriginalSizeBytes == 0 ? 0 : (double)CompressedSizeBytes / OriginalSizeBytes;
    public double SavingsPercentage => OriginalSizeBytes == 0 ? 0 : (1.0 - CompressionRatio) * 100.0;
    public double CompressionDurationMs { get; set; }
    public double DecompressionDurationMs { get; set; }
    public double SpeedMBps { get; set; }
    public string CompressionFormat { get; set; } = "Zstandard (v1.5.6 Native)";
}

/// <summary>
/// Post-Quantum Cryptography (PQC) Enclave evaluation model (ML-KEM / HPKE).
/// </summary>
public class PostQuantumEnclaveResult
{
    public string Algorithm { get; set; } = "ML-KEM-768 (NIST FIPS 203)";
    public string KeyId { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string EncapsulatedCiphertextHex { get; set; } = string.Empty;
    public string DecapsulatedSecretHex { get; set; } = string.Empty;
    public string PlaintextPayload { get; set; } = string.Empty;
    public string EncryptedEnvelopeBase64 { get; set; } = string.Empty;
    public string DecryptedPayload { get; set; } = string.Empty;
    public double HandshakeDurationMs { get; set; }
    public bool IsDecryptionVerified { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// OpenTelemetry component trace span tracking lifecycle durations and memory pressure.
/// </summary>
public class ComponentTraceSpan
{
    public string TraceId { get; set; } = Guid.NewGuid().ToString("N");
    public string SpanId { get; set; } = Guid.NewGuid().ToString("N")[..16];
    public string? ParentSpanId { get; set; }
    public string ComponentName { get; set; } = string.Empty;
    public string OperationName { get; set; } = "BuildRenderTree";
    public string Category { get; set; } = "Render";
    public double DurationMicroseconds { get; set; }
    public double DurationMs => DurationMicroseconds / 1000.0;
    public string Health => DurationMs < 16.67 ? "Optimal" : (DurationMs <= 33.33 ? "Warning" : "Critical");
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, string> Attributes { get; set; } = new();
    public long MemoryAllocatedBytes { get; set; }
}

/// <summary>
/// Aggregated performance summary for an individual Blazor component.
/// </summary>
public sealed record ComponentPerformanceSummary(
    string ComponentName,
    int RenderCount,
    double TotalDurationMs,
    double AvgDurationMs,
    double MaxDurationMs,
    double MinDurationMs,
    int SlowFrameCount,
    DateTime LastRendered
);
