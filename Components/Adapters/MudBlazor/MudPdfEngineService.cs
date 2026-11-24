using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorTwUI.Models;
using BlazorTwUI.Services;

namespace BlazorTwUI.Components.Adapters.MudBlazor;

/// <summary>
/// MudBlazor Integration Service for the MIT-licensed Pure C# PDF Engine.
/// Bridges MudBlazor dialogs, snackbars, and data grids with high-velocity PDF generation.
/// </summary>
public sealed class MudPdfEngineService
{
    /// <summary>
    /// Generates a PDF byte array for immediate binding to an iframe or download helper.
    /// </summary>
    public Task<byte[]> GenerateDocumentBytesAsync(PdfContractDocument doc, bool isFlattened = false)
    {
        byte[] bytes = PdfEngineService.GeneratePdfBytes(doc, isFlattened);
        return Task.FromResult(bytes);
    }

    /// <summary>
    /// Generates Base64 data URL string (e.g. data:application/pdf;base64,...) for direct MudBlazor iframe embedding.
    /// </summary>
    public Task<string> GenerateDataUrlAsync(PdfContractDocument doc, bool isFlattened = false)
    {
        string b64 = PdfEngineService.GeneratePdfBase64(doc, isFlattened);
        return Task.FromResult($"data:application/pdf;base64,{b64}");
    }

    /// <summary>
    /// Flattens form annotations and seals the document for legal archiving.
    /// </summary>
    public Task<PdfContractDocument> FlattenAndSealAsync(PdfContractDocument doc, string? signerIp = null)
    {
        PdfEngineService.SealDocument(doc, signerIp);
        PdfEngineService.FlattenDocument(doc);
        return Task.FromResult(doc);
    }

    /// <summary>
    /// Merges multiple MudBlazor contract or invoice items into a single unified portfolio PDF.
    /// </summary>
    public Task<byte[]> CombinePortfolioAsync(IEnumerable<PdfContractDocument> docs, string portfolioTitle = "Executive Portfolio")
    {
        var combined = PdfEngineService.CombineDocuments(docs, portfolioTitle, applyContinuousBates: true);
        PdfEngineService.FlattenDocument(combined);
        byte[] bytes = PdfEngineService.GeneratePdfBytes(combined, isFlattened: true);
        return Task.FromResult(bytes);
    }
}

