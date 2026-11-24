using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using BlazorTwUI.Models;

namespace BlazorTwUI.Services;

/// <summary>
/// Cross-platform API Request Handler for the MIT-licensed PDF Engine.
/// Decoupled from transport, enabling use in ASP.NET Core Minimal APIs, 
/// Blazor WebAssembly in-memory handlers, .NET MAUI local services, and console runners.
/// </summary>
public static class PdfEngineApiHandler
{
    public static PdfGenerateResponse HandleGenerate(PdfGenerateRequest request)
    {
        var doc = request.Document ?? PdfEngineService.CreateMasterServicesAgreement();

        if (request.ApplyWatermark && !string.IsNullOrWhiteSpace(request.WatermarkText))
        {
            doc.Watermark = new PdfWatermarkOptions
            {
                Enabled = true,
                Text = request.WatermarkText
            };
        }

        if (request.ApplyBates)
        {
            doc.BatesNumbering = new PdfBatesOptions
            {
                Enabled = true,
                Prefix = request.BatesPrefix ?? "DOC-"
            };
        }

        if (request.Flatten)
        {
            PdfEngineService.FlattenDocument(doc);
        }

        byte[] pdfBytes = PdfEngineService.GeneratePdfBytes(doc, request.Flatten);

        return new PdfGenerateResponse
        {
            DocumentId = doc.DocumentId,
            Filename = $"{doc.DocumentId}_{(request.Flatten ? "Flattened" : "Instrument")}.pdf",
            FileSizeBytes = pdfBytes.Length,
            PageCount = Math.Max(1, doc.Pages.Count),
            Base64Data = request.ReturnBase64 ? Convert.ToBase64String(pdfBytes) : null,
            Sha256Fingerprint = doc.Sha256Fingerprint,
            IsFlattened = doc.IsFlattened,
            IsSealed = doc.IsSealed
        };
    }

    public static PdfGenerateResponse HandleFlatten(PdfFlattenRequest request)
    {
        var doc = request.Document ?? PdfEngineService.CreateMasterServicesAgreement();
        PdfEngineService.FlattenDocument(doc);

        byte[] pdfBytes = PdfEngineService.GeneratePdfBytes(doc, isFlattened: true);

        return new PdfGenerateResponse
        {
            DocumentId = doc.DocumentId,
            Filename = $"{doc.DocumentId}_Flattened_StaticStream.pdf",
            FileSizeBytes = pdfBytes.Length,
            PageCount = Math.Max(1, doc.Pages.Count),
            Base64Data = request.ReturnBase64 ? Convert.ToBase64String(pdfBytes) : null,
            Sha256Fingerprint = doc.Sha256Fingerprint,
            IsFlattened = true,
            IsSealed = doc.IsSealed
        };
    }

    public static PdfGenerateResponse HandleCombine(PdfCombineRequest request)
    {
        var combinedDoc = PdfEngineService.CombineDocuments(
            request.Documents,
            title: request.Title,
            applyContinuousBates: request.ApplyContinuousBates,
            batesPrefix: request.BatesPrefix
        );

        if (request.FlattenAll)
        {
            PdfEngineService.FlattenDocument(combinedDoc);
        }

        byte[] pdfBytes = PdfEngineService.GeneratePdfBytes(combinedDoc, request.FlattenAll);

        return new PdfGenerateResponse
        {
            DocumentId = combinedDoc.DocumentId,
            Filename = $"{combinedDoc.DocumentId}_Portfolio.pdf",
            FileSizeBytes = pdfBytes.Length,
            PageCount = combinedDoc.Pages.Count,
            Base64Data = request.ReturnBase64 ? Convert.ToBase64String(pdfBytes) : null,
            Sha256Fingerprint = combinedDoc.Sha256Fingerprint,
            IsFlattened = combinedDoc.IsFlattened,
            IsSealed = combinedDoc.IsSealed
        };
    }

    public static PdfDocumentInfoResponse HandleInfo(PdfContractDocument doc)
    {
        return new PdfDocumentInfoResponse
        {
            DocumentId = doc.DocumentId,
            Title = doc.Title,
            PageCount = Math.Max(1, doc.Pages.Count),
            FieldCount = doc.Fields.Count,
            RequiredFieldsRemaining = doc.Fields.Count(f => f.IsRequired && !f.IsFilled),
            IsFlattened = doc.IsFlattened,
            IsSealed = doc.IsSealed,
            Status = doc.Status,
            Sha256Fingerprint = doc.Sha256Fingerprint,
            CertificateId = doc.CertificateId
        };
    }

    public static PdfGenerateResponse HandleReportGenerate(PdfBrandedReportDocument reportDoc, bool returnBase64 = true)
    {
        byte[] pdfBytes = PdfReportService.GenerateReportPdf(reportDoc);
        return new PdfGenerateResponse
        {
            DocumentId = reportDoc.ReportId,
            Filename = $"{reportDoc.ReportId}_{reportDoc.Title.Replace(" ", "_")}.pdf",
            FileSizeBytes = pdfBytes.Length,
            PageCount = 1,
            Base64Data = returnBase64 ? Convert.ToBase64String(pdfBytes) : null,
            Sha256Fingerprint = reportDoc.Sha256Seal,
            IsFlattened = true,
            IsSealed = reportDoc.ShowAttestationSeal
        };
    }

    public static PdfContractDocument HandleTemplate(string templateType)
    {
        return templateType.ToLowerInvariant() switch
        {
            "invoice" or "commercial_invoice" => PdfEngineService.CreateCommercialInvoice("INV-2026-8801", "Acme Corporation", 18500.00m),
            "nda" or "confidentiality" => PdfEngineService.CreateNonDisclosureAgreement("Apex Cloud Solutions", "Client Partner Inc."),
            "cert" or "certificate" => PdfEngineService.CreateCertificateOfCompletion("Jane M. Doe", "Enterprise Cloud Architecture", "National Standards Institute"),
            _ => PdfEngineService.CreateMasterServicesAgreement()
        };
    }

    public static object GetCapabilities()
    {
        return new
        {
            Engine = "BlazorTwUI Pure C# PDF Engine",
            Version = "1.0.0",
            License = "MIT (Free Distribution)",
            Dependencies = "Zero Third-Party SDKs",
            PdfStandard = "ISO 32000-1 / PDF 1.4",
            SupportedFonts = new[] { "Helvetica", "Helvetica-Bold", "Times-Roman", "Times-BoldItalic", "Courier" },
            SupportedOperations = new[]
            {
                "doc_generate",
                "doc_flatten",
                "doc_combine",
                "doc_watermark",
                "doc_bates_numbering",
                "doc_cryptographic_seal",
                "doc_inspect_info",
                "doc_template_factory"
            },
            SupportedFieldTypes = new[] { "SignHere", "InitialHere", "DateTimeStamp", "Text", "Checkbox", "Dropdown" },
            PlatformCompatibility = new[]
            {
                ".NET Minimal APIs (ASP.NET Core)",
                "Blazor WebAssembly (Zero-Latency Browser)",
                "React & Next.js (REST/TypeScript)",
                "React Native & Expo (Mobile)",
                ".NET MAUI & Blazor Hybrid (iOS, Android, Windows, macOS)",
                "MudBlazor (Enterprise Blazor Dialogs)"
            }
        };
    }
}

