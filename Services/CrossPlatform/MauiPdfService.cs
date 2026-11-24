using System;
using System.IO;
using System.Threading.Tasks;
using BlazorTwUI.Models;
using BlazorTwUI.Services;

namespace BlazorTwUI.Services.CrossPlatform;

/// <summary>
/// Cross-platform .NET MAUI & Blazor Hybrid service.
/// Generates standard PDF streams, writes to native device storage, and opens in native mobile viewers.
/// 100% Free Distribution under MIT License.
/// </summary>
public sealed class MauiPdfService
{
    /// <summary>
    /// Generates PDF bytes directly on the client device without server latency.
    /// </summary>
    public byte[] GenerateBytes(PdfContractDocument doc, bool isFlattened = false)
    {
        return PdfEngineService.GeneratePdfBytes(doc, isFlattened);
    }

    /// <summary>
    /// Writes PDF bytes to a target directory (such as native AppDataDirectory or CacheDirectory).
    /// </summary>
    public async Task<string> SaveToLocalDirectoryAsync(PdfContractDocument doc, string directoryPath, bool isFlattened = false)
    {
        Directory.CreateDirectory(directoryPath);
        string filename = $"{doc.DocumentId}_{(isFlattened ? "Flattened" : "Contract")}.pdf";
        string fullPath = Path.Combine(directoryPath, filename);

        byte[] bytes = PdfEngineService.GeneratePdfBytes(doc, isFlattened);
        await File.WriteAllBytesAsync(fullPath, bytes);

        return fullPath;
    }

    /// <summary>
    /// Flattens, cryptographically seals, and saves an immutable execution copy.
    /// </summary>
    public async Task<string> SealAndSaveImmutableCopyAsync(PdfContractDocument doc, string directoryPath, string? deviceIp = null)
    {
        PdfEngineService.SealDocument(doc, deviceIp);
        PdfEngineService.FlattenDocument(doc);
        return await SaveToLocalDirectoryAsync(doc, directoryPath, isFlattened: true);
    }
}

