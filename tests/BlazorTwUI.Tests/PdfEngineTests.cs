using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BlazorTwUI.Models;
using BlazorTwUI.Services;
using Xunit;

namespace BlazorTwUI.Tests;

public class PdfEngineTests
{
    [Fact]
    public void GeneratePdf_CreatesValidPdf14BinaryStream()
    {
        var doc = PdfEngineService.CreateMasterServicesAgreement();
        byte[] pdfBytes = PdfEngineService.GeneratePdfBytes(doc, isFlattened: false);

        Assert.NotNull(pdfBytes);
        Assert.True(pdfBytes.Length > 500, "PDF byte length should be substantial");

        string asciiPrefix = Encoding.ASCII.GetString(pdfBytes, 0, 8);
        Assert.StartsWith("%PDF-1.4", asciiPrefix);

        string fullContent = Encoding.ASCII.GetString(pdfBytes);
        Assert.Contains("xref", fullContent);
        Assert.Contains("trailer", fullContent);
        Assert.Contains("%%EOF", fullContent);
        Assert.Contains("/Type /Catalog", fullContent);
        Assert.Contains("/Type /Pages", fullContent);
    }

    [Fact]
    public void FlattenDocument_PurgesAcroForms_AndMarksFlattened()
    {
        var doc = PdfEngineService.CreateMasterServicesAgreement();
        Assert.False(doc.IsFlattened);

        PdfEngineService.FlattenDocument(doc);

        Assert.True(doc.IsFlattened);
        Assert.True(doc.IsReadOnly);
        Assert.NotNull(doc.FlattenedAtUtc);
        Assert.False(string.IsNullOrWhiteSpace(doc.FlattenedHash));
        Assert.StartsWith("STRM-FLAT-", doc.FlattenedContentStreamId);

        byte[] flattenedBytes = PdfEngineService.GeneratePdfBytes(doc, isFlattened: true);
        string content = Encoding.ASCII.GetString(flattenedBytes);
        Assert.Contains("OFFICIALLY FLATTENED", content);
        Assert.Contains("ISO 32000-1 STATIC STREAM", content);
    }

    [Fact]
    public void ApplyWatermark_EmbedsMatrixTransformation()
    {
        var doc = PdfEngineService.CreateMasterServicesAgreement();
        doc.Watermark = new PdfWatermarkOptions
        {
            Enabled = true,
            Text = "STRICTLY CONFIDENTIAL",
            RotationDegrees = 45.0,
            FontSize = 48.0
        };

        byte[] pdfBytes = PdfEngineService.GeneratePdfBytes(doc);
        string content = Encoding.ASCII.GetString(pdfBytes);

        Assert.Contains("(STRICTLY CONFIDENTIAL) Tj", content);
        Assert.Contains("cm", content); // Matrix transformation operator
        Assert.Contains("/ExtGState << /GSWatermark", content);
        Assert.Contains("/BM /Multiply", content);
        Assert.Contains("/GSWatermark gs", content);
    }

    [Fact]
    public void ApplyBatesNumbering_FormatsConsecutiveSequences()
    {
        var doc = PdfEngineService.CreateMasterServicesAgreement();
        doc.BatesNumbering = new PdfBatesOptions
        {
            Enabled = true,
            Prefix = "LEGAL-REF-",
            StartNumber = 100,
            Digits = 6,
            Position = BatesPosition.BottomRight
        };

        Assert.Equal("LEGAL-REF-000100", doc.BatesNumbering.FormatNumber(0));
        Assert.Equal("LEGAL-REF-000101", doc.BatesNumbering.FormatNumber(1));

        byte[] pdfBytes = PdfEngineService.GeneratePdfBytes(doc);
        string content = Encoding.ASCII.GetString(pdfBytes);

        Assert.Contains("LEGAL-REF-000100", content);
        Assert.Contains("LEGAL-REF-000101", content);
    }

    [Fact]
    public void CombineDocuments_MergesMultiplePdfsIntoUnifiedPortfolio()
    {
        var doc1 = PdfEngineService.CreateCommercialInvoice("INV-001", "Client A", 12000m);
        var doc2 = PdfEngineService.CreateNonDisclosureAgreement("Party A", "Party B");

        int expectedPages = doc1.Pages.Count + doc2.Pages.Count;

        var combined = PdfEngineService.CombineDocuments(
            new[] { doc1, doc2 },
            title: "Combined M&A Exhibits",
            applyContinuousBates: true,
            batesPrefix: "EXHIBIT-"
        );

        Assert.Equal(expectedPages, combined.Pages.Count);
        Assert.Equal("Combined M&A Exhibits", combined.Title);
        Assert.True(combined.BatesNumbering.Enabled);

        byte[] bytes = PdfEngineService.GeneratePdfBytes(combined);
        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 1000);

        string content = Encoding.ASCII.GetString(bytes);
        Assert.Contains("EXHIBIT-000001", content);
        Assert.Contains($"Count {expectedPages}", content);
    }

    [Fact]
    public void SealDocument_ComputesSha256DigestAndAssignsCertificate()
    {
        var doc = PdfEngineService.CreateMasterServicesAgreement();
        Assert.False(doc.IsSealed);

        PdfEngineService.SealDocument(doc, "10.0.0.42");

        Assert.True(doc.IsSealed);
        Assert.True(doc.IsReadOnly);
        Assert.Equal(64, doc.Sha256Fingerprint.Length); // 64 hex characters for SHA-256
        Assert.StartsWith("CERT-", doc.CertificateId);
        Assert.Contains("-ESIGN-", doc.CertificateId);
        Assert.Equal("10.0.0.42", doc.SignerIp);
    }

    [Fact]
    public void TemplateFactory_GeneratesProductionReadyModels()
    {
        var msa = PdfEngineApiHandler.HandleTemplate("msa");
        var invoice = PdfEngineApiHandler.HandleTemplate("invoice");
        var nda = PdfEngineApiHandler.HandleTemplate("nda");
        var cert = PdfEngineApiHandler.HandleTemplate("cert");

        Assert.Equal("Master Services & Cloud Architecture Agreement", msa.Title);
        Assert.Contains("COMMERCIAL INVOICE", invoice.Title);
        Assert.Contains("Mutual Non-Disclosure", nda.Title);
        Assert.Contains("Certificate of Professional Mastery", cert.Title);

        Assert.NotEmpty(msa.Fields);
        Assert.NotEmpty(invoice.Fields);
        Assert.NotEmpty(nda.Fields);
        Assert.True(cert.IsSealed);
    }

    [Fact]
    public void ApiHandler_GenerateAndFlatten_ReturnsCorrectResponse()
    {
        var req = new PdfGenerateRequest
        {
            Document = PdfEngineService.CreateMasterServicesAgreement(),
            Flatten = true,
            ReturnBase64 = true
        };

        var res = PdfEngineApiHandler.HandleGenerate(req);

        Assert.NotNull(res);
        Assert.True(res.FileSizeBytes > 500);
        Assert.False(string.IsNullOrWhiteSpace(res.Base64Data));
        Assert.True(res.IsFlattened);
        Assert.Equal(2, res.PageCount);
    }

    [Fact]
    public void ApiHandler_GetCapabilities_ExposesSupportedOperations()
    {
        var caps = PdfEngineApiHandler.GetCapabilities();
        Assert.NotNull(caps);

        string json = System.Text.Json.JsonSerializer.Serialize(caps);
        Assert.Contains("BlazorTwUI Pure C# PDF Engine", json);
        Assert.Contains("MIT (Free Distribution)", json);
        Assert.Contains("doc_generate", json);
        Assert.Contains("doc_flatten", json);
        Assert.Contains("doc_combine", json);
        Assert.Contains(".NET Minimal APIs", json);
    }

    [Fact]
    public void GenerateReportPdf_CreatesBrandedPdf_WithAccentColorsAndKpis()
    {
        var doc = new PdfBrandedReportDocument
        {
            Title = "Q4 Enterprise Financial & Operational Report",
            Subtitle = "Exported from BlazorTwUI DataGrid",
            OrganizationName = "Acme Technologies Global",
            Accent = AccentTheme.Emerald,
            ShowAttestationSeal = true,
            Kpis = new List<PdfReportKpi>
            {
                new() { Label = "Total Revenue", Value = "$1,248,500", ChangeText = "+14.8%", IsPositive = true },
                new() { Label = "Active Subscriptions", Value = "8,420", ChangeText = "+6.2%", IsPositive = true },
                new() { Label = "Gross Margin", Value = "78.4%", ChangeText = "+1.1%", IsPositive = true },
                new() { Label = "Pending Invoices", Value = "14", ChangeText = "-3", IsPositive = false }
            },
            Columns = new List<PdfReportColumn>
            {
                new() { Header = "Invoice ID", WidthWeight = 1.2 },
                new() { Header = "Client Name", WidthWeight = 2.0 },
                new() { Header = "Tier", WidthWeight = 1.0, IsBadge = true, BadgeTone = "neutral" },
                new() { Header = "Status", WidthWeight = 1.2, IsBadge = true, BadgeTone = "success" },
                new() { Header = "Amount", WidthWeight = 1.5, Alignment = PdfReportAlignment.Right }
            },
            Rows = new List<PdfReportRow>
            {
                new() { Cells = new() { "INV-8021", "Contoso Corp", "Enterprise", "Paid", "$42,500.00" } },
                new() { Cells = new() { "INV-8022", "Fabrikam Ltd", "Pro", "Paid", "$18,200.00" } },
                new() { Cells = new() { "INV-8023", "Tailspin Toys", "Enterprise", "Pending", "$31,900.00" } },
                new() { Cells = new() { "INV-8024", "Northwind Traders", "Starter", "Paid", "$4,800.00" } },
                new() { Cells = new() { "INV-8025", "Woodgrove Bank", "Enterprise", "Paid", "$85,000.00" } }
            },
            SummaryRow = new() { "TOTALS (5 Invoices)", "", "", "4 Paid", "$182,400.00" }
        };

        byte[] pdfBytes = PdfReportService.GenerateReportPdf(doc);

        Assert.NotNull(pdfBytes);
        Assert.True(pdfBytes.Length > 800, "Branded PDF should contain significant byte stream");

        string asciiPrefix = Encoding.ASCII.GetString(pdfBytes, 0, 8);
        Assert.StartsWith("%PDF-1.4", asciiPrefix);

        string content = Encoding.ASCII.GetString(pdfBytes);
        Assert.Contains("xref", content);
        Assert.Contains("%%EOF", content);
        Assert.Contains("/Type /Catalog", content);
        Assert.Contains("Q4 Enterprise Financial", content);
        Assert.Contains("Total Revenue", content);
        Assert.Contains("SHA-256 SEAL", content);
    }

    [Fact]
    public void GenerateReportPdf_SupportsMultiPagePagination_ForLargeDataGrids()
    {
        var doc = new PdfBrandedReportDocument
        {
            Title = "High Volume Transaction Log",
            Accent = AccentTheme.Violet,
            Columns = new List<PdfReportColumn>
            {
                new() { Header = "Index", WidthWeight = 1.0 },
                new() { Header = "Timestamp", WidthWeight = 2.0 },
                new() { Header = "Status", WidthWeight = 1.2, IsBadge = true },
                new() { Header = "Amount", WidthWeight = 1.5, Alignment = PdfReportAlignment.Right }
            }
        };

        for (int i = 1; i <= 60; i++)
        {
            doc.Rows.Add(new PdfReportRow
            {
                Cells = new() { $"TX-{i:D4}", DateTime.UtcNow.AddMinutes(-i).ToString("yyyy-MM-dd HH:mm"), (i % 5 == 0 ? "Pending" : "Completed"), $"${(i * 123.45):F2}" }
            });
        }

        byte[] pdfBytes = PdfReportService.GenerateReportPdf(doc);
        Assert.NotNull(pdfBytes);

        string content = Encoding.ASCII.GetString(pdfBytes);
        // Multi-page document should have count > 1
        Assert.Contains("/Count ", content);
        Assert.Contains("Page 1 of", content);
        Assert.Contains("Page 2 of", content);
    }

    private sealed class DynamicTestOrder
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsShipped { get; set; }
    }

    [Fact]
    public void ExportFromCollection_GenericReflection_WithZeroHardcoding()
    {
        // Arbitrary generic dataset without pre-defined PDF models
        var orders = new List<DynamicTestOrder>
        {
            new() { OrderId = 101, CustomerName = "Global Logistics", OrderDate = new DateTime(2026, 11, 1), TotalAmount = 4500.50m, IsShipped = true },
            new() { OrderId = 102, CustomerName = "Pacific Systems", OrderDate = new DateTime(2026, 11, 2), TotalAmount = 1200.00m, IsShipped = false },
            new() { OrderId = 103, CustomerName = "Alpine Engineering", OrderDate = new DateTime(2026, 11, 3), TotalAmount = 8350.75m, IsShipped = true }
        };

        var options = new PdfExportOptions
        {
            Title = "Orders Ledger",
            Subtitle = "Generic reflection-based export",
            Accent = AccentTheme.Sky,
            AutoCalculateSummaries = true
        };

        byte[] pdfBytes = DataToPdfExporter.ExportFromCollection(orders, options);

        Assert.NotNull(pdfBytes);
        Assert.True(pdfBytes.Length > 800);

        string content = Encoding.ASCII.GetString(pdfBytes);
        // Confirms reflection split PascalCase property names into proper headers
        Assert.Contains("Order Id", content);
        Assert.Contains("Customer Name", content);
        Assert.Contains("Order Date", content);
        Assert.Contains("Total Amount", content);
        Assert.Contains("Is Shipped", content);

        // Confirms values and automated totals
        Assert.Contains("Global Logistics", content);
        Assert.Contains("TOTALS", content);
        Assert.Contains("%PDF-1.4", content);
    }

    [Fact]
    public void ExportGrid_DynamicColumnsAndRows_GeneratesValidPdf()
    {
        var cols = new List<DataPdfColumnDefinition>
        {
            new() { Title = "Project Code", Alignment = PdfReportAlignment.Left },
            new() { Title = "Progress", Alignment = PdfReportAlignment.Center, IsBadge = true, BadgeTone = "success" },
            new() { Title = "Budget", Alignment = PdfReportAlignment.Right }
        };

        var rows = new List<IReadOnlyList<string>>
        {
            new List<string> { "PRJ-ALPHA", "Complete", "$50,000" },
            new List<string> { "PRJ-BETA", "Active", "$75,000" }
        };

        var summaries = new List<string> { "TOTALS (2)", "", "$125,000" };

        byte[] pdfBytes = DataToPdfExporter.ExportGrid(cols, rows, summaries, new PdfExportOptions
        {
            Title = "Engineering Projects Summary",
            Accent = AccentTheme.Teal
        });

        Assert.NotNull(pdfBytes);
        string content = Encoding.ASCII.GetString(pdfBytes);
        Assert.Contains("Project Code", content);
        Assert.Contains("Engineering Projects Summary", content);
        Assert.Contains("PRJ-ALPHA", content);
        Assert.Contains("$125,000", content);
    }

    [Fact]
    public void GenerateAllSamplePdfsToDisk_ForPrinting()
    {
        string outDir = @"C:\repos\BlazorTwUI\docs\sample_pdfs";
        Directory.CreateDirectory(outDir);

        // 1. Master Services Agreement
        var msa = PdfEngineService.CreateMasterServicesAgreement();
        PdfEngineService.FlattenDocument(msa);
        File.WriteAllBytes(Path.Combine(outDir, "01_MasterServicesAgreement.pdf"), PdfEngineService.GeneratePdfBytes(msa, isFlattened: true));

        // 2. Commercial Invoice
        var invoice = PdfEngineService.CreateCommercialInvoice("INV-2026-9401", "Acme Global Industries", 24500.00m);
        PdfEngineService.FlattenDocument(invoice);
        File.WriteAllBytes(Path.Combine(outDir, "02_CommercialInvoice.pdf"), PdfEngineService.GeneratePdfBytes(invoice, isFlattened: true));

        // 3. Mutual NDA with Watermark & Bates
        var nda = PdfEngineService.CreateNonDisclosureAgreement("Apex Cloud Technologies", "Frontier Data Corp");
        nda.Watermark = new PdfWatermarkOptions { Enabled = true, Text = "CONFIDENTIAL", Opacity = 0.15, RotationDegrees = 45 };
        nda.BatesNumbering = new PdfBatesOptions { Enabled = true, Prefix = "NDA-", StartNumber = 1, Digits = 6 };
        PdfEngineService.FlattenDocument(nda);
        File.WriteAllBytes(Path.Combine(outDir, "03_MutualNDA_Watermark_Bates.pdf"), PdfEngineService.GeneratePdfBytes(nda, isFlattened: true));

        // 4. Certificate of Completion
        var cert = PdfEngineService.CreateCertificateOfCompletion("Jane M. Doe", "Enterprise Distributed Systems", "Global Software Accreditation Board");
        File.WriteAllBytes(Path.Combine(outDir, "04_MasteryCertificate.pdf"), PdfEngineService.GeneratePdfBytes(cert, isFlattened: true));

        // 5. Branded DataGrid Report
        var orders = new List<DynamicTestOrder>
        {
            new() { OrderId = 101, CustomerName = "Global Logistics", OrderDate = new DateTime(2026, 11, 1), TotalAmount = 4500.50m, IsShipped = true },
            new() { OrderId = 102, CustomerName = "Pacific Systems", OrderDate = new DateTime(2026, 11, 2), TotalAmount = 1200.00m, IsShipped = false },
            new() { OrderId = 103, CustomerName = "Alpine Engineering", OrderDate = new DateTime(2026, 11, 3), TotalAmount = 8350.75m, IsShipped = true },
            new() { OrderId = 104, CustomerName = "Summit Interactive", OrderDate = new DateTime(2026, 11, 4), TotalAmount = 14200.00m, IsShipped = true },
            new() { OrderId = 105, CustomerName = "Horizon Energy", OrderDate = new DateTime(2026, 11, 5), TotalAmount = 6750.25m, IsShipped = false }
        };

        var reportOptions = new PdfExportOptions
        {
            Title = "Executive Revenue & Shipping Ledger",
            Subtitle = "Exported from live BlazorTwUI DataGrid",
            OrganizationName = "Enterprise Global Group",
            Tagline = "BlazorTwUI High-Performance Design System",
            Accent = AccentTheme.Emerald,
            AutoCalculateSummaries = true,
            ShowAttestationSeal = true,
            Kpis = new List<PdfReportKpi>
            {
                new() { Label = "Total Orders", Value = "$35,001.50", ChangeText = "+18.2%", IsPositive = true },
                new() { Label = "Active Accounts", Value = "5", ChangeText = "+2", IsPositive = true },
                new() { Label = "Fulfillment Rate", Value = "60.0%", ChangeText = "+5.0%", IsPositive = true }
            }
        };

        File.WriteAllBytes(Path.Combine(outDir, "05_BrandedDataReport.pdf"), DataToPdfExporter.ExportFromCollection(orders, reportOptions));
    }

    [Fact]
    public void PdfEngine_FieldCardsAndAuditStamp_NeverOverlapWhenFlattened()
    {
        var msa = PdfEngineService.CreateMasterServicesAgreement();
        PdfEngineService.SealDocument(msa, "127.0.0.1 (Offline Local Engine)");
        PdfEngineService.FlattenDocument(msa);

        var pdfBytes = PdfEngineService.GeneratePdfBytes(msa, isFlattened: true);
        var pdfText = System.Text.Encoding.UTF8.GetString(pdfBytes);

        Assert.Contains("OFFICIALLY FLATTENED & CERTIFIED", pdfText);

        var rectMatches = System.Text.RegularExpressions.Regex.Matches(
            pdfText,
            @"(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+re");

        Assert.NotEmpty(rectMatches);

        var footerMatch = rectMatches.FirstOrDefault(m => m.Groups[4].Value == "44" && m.Groups[3].Value == "512");
        Assert.NotNull(footerMatch);

        double footerY = double.Parse(footerMatch.Groups[2].Value);
        double footerH = double.Parse(footerMatch.Groups[4].Value);
        double footerTop = footerY + footerH;

        var cardMatches = rectMatches.Where(m => m.Groups[4].Value == "44" && m.Groups[3].Value != "512").ToList();
        Assert.NotEmpty(cardMatches);

        foreach (var card in cardMatches)
        {
            double cardY = double.Parse(card.Groups[2].Value);
            Assert.True(cardY >= footerTop + 15, $"Field card bottom at Y={cardY} collides with or is too close to footer top at Y={footerTop}");
        }
    }
}

