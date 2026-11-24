using System;
using BlazorTwUI.Models;

namespace BlazorTwUI.Services;

/// <summary>
/// Backwards-compatible facade for PDF flattening and stream generation.
/// Powered by the high-performance, MIT-licensed pure C# <see cref="PdfEngineService"/>.
/// </summary>
public static class PdfFlattenStreamGenerator
{
    public static byte[] GenerateFlattenedPdfBytes(PdfContractDocument doc)
    {
        return PdfEngineService.GeneratePdfBytes(doc, isFlattened: true);
    }

    public static string GenerateFlattenedPdfBase64(PdfContractDocument doc)
    {
        return PdfEngineService.GeneratePdfBase64(doc, isFlattened: true);
    }
}

