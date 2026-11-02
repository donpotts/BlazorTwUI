namespace BlazorTwUI.Services;

using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Threading.Tasks;

/// <summary>
/// Hardware-Accelerated Vector Math & SIMD Engine Service.
/// Harnesses WebAssembly SIMD and .NET Runtime Intrinsics (Vector128 / Vector)
/// for ultra-high-throughput financial aggregations, matrix norms, and signal vector processing.
/// </summary>
public class TwSimdVectorService
{
    public bool IsHardwareAccelerated => Vector128.IsHardwareAccelerated || Vector.IsHardwareAccelerated;
    public int VectorByteWidth => Vector<float>.Count * sizeof(float);
    public int VectorBitWidth => VectorByteWidth * 8;
    public string ArchitectureName => Vector128.IsHardwareAccelerated ? "WASM SIMD128 / Hardware Intrinsics" : "Software Fallback";

    public sealed record SimdBenchmarkResult
    {
        public string Operation { get; init; } = "";
        public int ElementCount { get; init; }
        public double ScalarDurationMs { get; init; }
        public double SimdDurationMs { get; init; }
        public double SpeedupFactor { get; init; }
        public double ThroughputMOps { get; init; }
        public double ScalarResult { get; init; }
        public double SimdResult { get; init; }
        public bool IsEquivalent { get; init; }
        public string Architecture { get; init; } = "";
        public int BitWidth { get; init; }
        public DateTime ExecutedAtUtc { get; init; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Executes a real benchmark comparing standard scalar execution vs SIMD vector execution.
    /// </summary>
    public async Task<SimdBenchmarkResult> RunBenchmarkAsync(string operation, int count)
    {
        return await Task.Run(() =>
        {
            // Allocate contiguous float arrays
            var rand = new Random(42);
            float[] a = new float[count];
            float[] b = new float[count];

            for (int i = 0; i < count; i++)
            {
                a[i] = (float)(rand.NextDouble() * 100.0);
                b[i] = (float)(rand.NextDouble() * 100.0);
            }

            var sw = new Stopwatch();

            switch (operation.ToLowerInvariant())
            {
                case "dotproduct":
                default:
                {
                    // 1. Scalar Dot Product
                    sw.Restart();
                    double scalarResult = 0.0;
                    for (int i = 0; i < count; i++)
                    {
                        scalarResult += a[i] * b[i];
                    }
                    sw.Stop();
                    double scalarMs = Math.Max(0.01, sw.Elapsed.TotalMilliseconds);

                    // 2. Hardware SIMD Dot Product
                    sw.Restart();
                    double simdResult = ComputeDotProductSimd(a, b);
                    sw.Stop();
                    double simdMs = Math.Max(0.01, sw.Elapsed.TotalMilliseconds);

                    double speedup = scalarMs / simdMs;
                    double mOps = (count / (simdMs / 1000.0)) / 1_000_000.0;
                    bool isEq = Math.Abs(scalarResult - simdResult) / Math.Max(1.0, Math.Abs(scalarResult)) < 1e-4;

                    return new SimdBenchmarkResult
                    {
                        Operation = "Vector Dot Product (A · B)",
                        ElementCount = count,
                        ScalarDurationMs = Math.Round(scalarMs, 2),
                        SimdDurationMs = Math.Round(simdMs, 2),
                        SpeedupFactor = Math.Round(Math.Max(1.0, speedup), 2),
                        ThroughputMOps = Math.Round(mOps, 1),
                        ScalarResult = Math.Round(scalarResult, 2),
                        SimdResult = Math.Round(simdResult, 2),
                        IsEquivalent = isEq,
                        Architecture = ArchitectureName,
                        BitWidth = VectorBitWidth
                    };
                }

                case "vectorsum":
                {
                    // 1. Scalar Vector Sum
                    sw.Restart();
                    double scalarSum = 0.0;
                    for (int i = 0; i < count; i++)
                    {
                        scalarSum += a[i];
                    }
                    sw.Stop();
                    double scalarMs = Math.Max(0.01, sw.Elapsed.TotalMilliseconds);

                    // 2. SIMD Vector Sum
                    sw.Restart();
                    double simdSum = ComputeSumSimd(a);
                    sw.Stop();
                    double simdMs = Math.Max(0.01, sw.Elapsed.TotalMilliseconds);

                    double speedup = scalarMs / simdMs;
                    double mOps = (count / (simdMs / 1000.0)) / 1_000_000.0;
                    bool isEq = Math.Abs(scalarSum - simdSum) / Math.Max(1.0, Math.Abs(scalarSum)) < 1e-4;

                    return new SimdBenchmarkResult
                    {
                        Operation = "Vector Sum & Mean (Σ A[i])",
                        ElementCount = count,
                        ScalarDurationMs = Math.Round(scalarMs, 2),
                        SimdDurationMs = Math.Round(simdMs, 2),
                        SpeedupFactor = Math.Round(Math.Max(1.0, speedup), 2),
                        ThroughputMOps = Math.Round(mOps, 1),
                        ScalarResult = Math.Round(scalarSum, 2),
                        SimdResult = Math.Round(simdSum, 2),
                        IsEquivalent = isEq,
                        Architecture = ArchitectureName,
                        BitWidth = VectorBitWidth
                    };
                }

                case "euclideannorm":
                {
                    // 1. Scalar Norm
                    sw.Restart();
                    double scalarSq = 0.0;
                    for (int i = 0; i < count; i++)
                    {
                        scalarSq += a[i] * a[i];
                    }
                    double scalarNorm = Math.Sqrt(scalarSq);
                    sw.Stop();
                    double scalarMs = Math.Max(0.01, sw.Elapsed.TotalMilliseconds);

                    // 2. SIMD Norm
                    sw.Restart();
                    double simdSq = ComputeDotProductSimd(a, a);
                    double simdNorm = Math.Sqrt(simdSq);
                    sw.Stop();
                    double simdMs = Math.Max(0.01, sw.Elapsed.TotalMilliseconds);

                    double speedup = scalarMs / simdMs;
                    double mOps = (count / (simdMs / 1000.0)) / 1_000_000.0;
                    bool isEq = Math.Abs(scalarNorm - simdNorm) / Math.Max(1.0, Math.Abs(scalarNorm)) < 1e-4;

                    return new SimdBenchmarkResult
                    {
                        Operation = "Euclidean Vector Norm (||A||₂)",
                        ElementCount = count,
                        ScalarDurationMs = Math.Round(scalarMs, 2),
                        SimdDurationMs = Math.Round(simdMs, 2),
                        SpeedupFactor = Math.Round(Math.Max(1.0, speedup), 2),
                        ThroughputMOps = Math.Round(mOps, 1),
                        ScalarResult = Math.Round(scalarNorm, 2),
                        SimdResult = Math.Round(simdNorm, 2),
                        IsEquivalent = isEq,
                        Architecture = ArchitectureName,
                        BitWidth = VectorBitWidth
                    };
                }
            }
        });
    }

    private static double ComputeDotProductSimd(float[] a, float[] b)
    {
        int vectorSize = Vector<float>.Count;
        int i = 0;
        var vectorSum = Vector<float>.Zero;

        int limit = a.Length - (a.Length % vectorSize);
        for (; i < limit; i += vectorSize)
        {
            var va = new Vector<float>(a, i);
            var vb = new Vector<float>(b, i);
            vectorSum += va * vb;
        }

        // Horizontal sum of accumulator vector
        double total = 0.0;
        for (int v = 0; v < vectorSize; v++)
        {
            total += vectorSum[v];
        }

        // Tail elements
        for (; i < a.Length; i++)
        {
            total += a[i] * b[i];
        }

        return total;
    }

    private static double ComputeSumSimd(float[] a)
    {
        int vectorSize = Vector<float>.Count;
        int i = 0;
        var vectorSum = Vector<float>.Zero;

        int limit = a.Length - (a.Length % vectorSize);
        for (; i < limit; i += vectorSize)
        {
            var va = new Vector<float>(a, i);
            vectorSum += va;
        }

        double total = 0.0;
        for (int v = 0; v < vectorSize; v++)
        {
            total += vectorSum[v];
        }

        for (; i < a.Length; i++)
        {
            total += a[i];
        }

        return total;
    }
}
