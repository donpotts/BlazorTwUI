// ------------------------------------------------------------------------------------------------
// <copyright file="PdfMinimalApiSample.cs" company="BlazorTwUI">
//   Free Distribution under MIT License.
// </copyright>
// <summary>
//   Production-grade .NET Minimal API endpoint definitions for ASP.NET Core with Scalar OpenAPI.
//   Plugs directly into any WebApplication / IEndpointRouteBuilder with:
//       app.MapPdfEndpoints();
//       app.MapScalarApiReference();
// </summary>
// ------------------------------------------------------------------------------------------------

/*
===================================================================================================
 ASP.NET Core Minimal API Setup with Native OpenAPI & Scalar (Scalar.AspNetCore)
===================================================================================================

In your server csproj, add:
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.0.0" />
    <PackageReference Include="Scalar.AspNetCore" Version="2.0.*" />

In Program.cs:

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Scalar.AspNetCore;
using BlazorTwUI.Models;
using BlazorTwUI.Services;
using BlazorTwUI.Server;

var builder = WebApplication.CreateBuilder(args);

// 1. Native .NET OpenAPI Generation
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "Enterprise PDF Engine Minimal API";
        document.Info.Version = "v1";
        document.Info.Description = "High-performance cross-platform PDF generation, form flattening, watermarking, Bates sequencing, and cryptographic ESIGN sealing.";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// 2. Scalar Interactive OpenAPI Client (/scalar/v1)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Enterprise PDF Engine — Scalar API Reference")
            .WithTheme(ScalarTheme.Moon)
            .WithDarkMode(true)
            .WithSidebar(true)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

// 3. Register PDF Engine Endpoints
app.MapPdfEndpoints("/api/pdf");

app.Run();

===================================================================================================
 Endpoint Extension Implementation
===================================================================================================

namespace BlazorTwUI.Server;

public static class PdfMinimalApiExtensions
{
    public static IEndpointRouteBuilder MapPdfEndpoints(this IEndpointRouteBuilder app, string prefix = "/api/pdf")
    {
        var group = app.MapGroup(prefix)
            .WithTags("PDF Engine");

        // Health & Capabilities
        group.MapGet("/health", () => Results.Ok(PdfEngineApiHandler.GetCapabilities()))
            .WithName("GetPdfEngineCapabilities")
            .WithSummary("Returns supported features, fonts, and platform compatibility.");

        // Templates Factory
        group.MapGet("/templates/{type}", (string type) =>
        {
            var doc = PdfEngineApiHandler.HandleTemplate(type);
            return Results.Ok(doc);
        })
        .WithName("GetPdfTemplate")
        .WithSummary("Retrieves a pre-built document template (msa, invoice, nda, certificate).");

        // Generate PDF
        group.MapPost("/generate", (PdfGenerateRequest req) =>
        {
            var res = PdfEngineApiHandler.HandleGenerate(req);
            if (!req.ReturnBase64)
            {
                byte[] raw = PdfEngineService.GeneratePdfBytes(req.Document, req.Flatten);
                return Results.File(raw, "application/pdf", res.Filename);
            }
            return Results.Ok(res);
        })
        .WithName("GeneratePdf")
        .WithSummary("Generates binary or base64 PDF stream from contract document model.");

        // Flatten & Stamp Form Fields
        group.MapPost("/flatten", (PdfFlattenRequest req) =>
        {
            var res = PdfEngineApiHandler.HandleFlatten(req);
            if (!req.ReturnBase64)
            {
                byte[] raw = PdfEngineService.GeneratePdfBytes(req.Document, isFlattened: true);
                return Results.File(raw, "application/pdf", res.Filename);
            }
            return Results.Ok(res);
        })
        .WithName("FlattenPdf")
        .WithSummary("Purges AcroForms and burns form inputs into immutable static vector content.");

        // Combine / Merge Multiple Documents
        group.MapPost("/combine", (PdfCombineRequest req) =>
        {
            var res = PdfEngineApiHandler.HandleCombine(req);
            if (!req.ReturnBase64)
            {
                var combinedDoc = PdfEngineService.CombineDocuments(req.Documents, req.Title, req.ApplyContinuousBates, req.BatesPrefix);
                byte[] raw = PdfEngineService.GeneratePdfBytes(combinedDoc, req.FlattenAll);
                return Results.File(raw, "application/pdf", res.Filename);
            }
            return Results.Ok(res);
        })
        .WithName("CombinePdfs")
        .WithSummary("Combines multiple PDF documents into a unified multi-page portfolio.");

        // Branded Data Report Generation
        group.MapPost("/report", (PdfBrandedReportDocument reportDoc) =>
        {
            byte[] raw = PdfReportService.GenerateReportPdf(reportDoc);
            return Results.File(raw, "application/pdf", $"{reportDoc.ReportId}.pdf");
        })
        .WithName("GenerateBrandedReport")
        .WithSummary("Generates publication-ready PDF report matching app theme accent, KPI cards, and DataGrid records.");

        // Cryptographic Attestation & Sealing
        group.MapPost("/seal", (PdfContractDocument doc, HttpContext http) =>
        {
            string ip = http.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            PdfEngineService.SealDocument(doc, ip);
            return Results.Ok(PdfEngineApiHandler.HandleInfo(doc));
        })
        .WithName("SealPdf")
        .WithSummary("Applies SHA-256 tamper-evident digest and issues digital execution certificate.");

        // Inspect Document Metadata
        group.MapPost("/info", (PdfContractDocument doc) =>
        {
            return Results.Ok(PdfEngineApiHandler.HandleInfo(doc));
        })
        .WithName("InspectPdfInfo")
        .WithSummary("Returns field counts, completion status, and cryptographic hashes.");

        return app;
    }
}
*/

