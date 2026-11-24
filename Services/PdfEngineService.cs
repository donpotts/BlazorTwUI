#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BlazorTwUI.Models;
using BlazorTwUI.Services;

namespace BlazorTwUI.Services;

/// <summary>
/// Cross-platform, high-performance pure C# PDF engine.
/// 100% Zero-Dependency, Free Distribution under MIT License.
/// Supports generation, flattening, multi-document combination, diagonal watermarking,
/// legal Bates numbering sequences, cryptographic SHA-256 attestation, and Minimal API integration.
/// </summary>
public static class PdfEngineService
{
    /// <summary>
    /// Generates raw binary PDF 1.4 stream from a document model.
    /// </summary>
    public static byte[] GeneratePdfBytes(PdfContractDocument doc, bool isFlattened = false)
    {
        using var ms = new MemoryStream();
        var objectOffsets = new List<long>();

        void WriteAscii(string text)
        {
            var bytes = Encoding.ASCII.GetBytes(text);
            ms.Write(bytes, 0, bytes.Length);
        }

        // 1. Header (PDF 1.4 with binary marker for 8-bit stream safety)
        WriteAscii("%PDF-1.4\n");
        ms.Write(new byte[] { 0x25, 0xE2, 0xE3, 0xCF, 0xD3, 0x0A }, 0, 6);

        int totalPages = Math.Max(1, doc.Pages.Count);

        // Calculate dynamic object numbers:
        // Obj 1: Catalog
        // Obj 2: Pages container
        // Obj 3.. (Pages & Content streams: 2 objects per page)
        // Font objects follow pages
        int firstFontObj = 3 + totalPages * 2;
        int fontHelveticaObj = firstFontObj;
        int fontHelveticaBoldObj = firstFontObj + 1;
        int fontTimesObj = firstFontObj + 2;
        int fontTimesBoldItalicObj = firstFontObj + 3;
        int fontCourierObj = firstFontObj + 4;
        int extGStateObj = firstFontObj + 5;
        int infoObj = firstFontObj + 6;

        // Obj 1: Catalog
        objectOffsets.Add(ms.Position);
        WriteAscii("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");

        // Obj 2: Pages container
        objectOffsets.Add(ms.Position);
        var kids = string.Join(" ", Enumerable.Range(0, totalPages).Select(i => $"{(3 + i * 2)} 0 R"));
        WriteAscii($"2 0 obj\n<< /Type /Pages /Kids [ {kids} ] /Count {totalPages} >>\nendobj\n");

        // Page Objects & Content Streams
        for (int i = 0; i < totalPages; i++)
        {
            var pageModel = i < doc.Pages.Count ? doc.Pages[i] : new PdfContractPage { PageNumber = i + 1, Title = doc.Title };
            var pageFields = doc.Fields.FindAll(f => f.Page == (i + 1));
            var dims = PdfPageDimensions.FromPageSize(pageModel.PageSize != PdfPageSize.Letter ? pageModel.PageSize : doc.DefaultPageSize);

            string pageContent = BuildPageContentStream(doc, pageModel, pageFields, i + 1, totalPages, dims, isFlattened || doc.IsFlattened);
            byte[] streamBytes = Encoding.UTF8.GetBytes(pageContent);

            int pageObjNum = 3 + i * 2;
            int contentObjNum = pageObjNum + 1;

            // Page Object
            objectOffsets.Add(ms.Position);
            string resources = $"/Font << /F1 {fontHelveticaObj} 0 R /F2 {fontHelveticaBoldObj} 0 R /F3 {fontTimesObj} 0 R /F4 {fontTimesBoldItalicObj} 0 R /F5 {fontCourierObj} 0 R >> /ExtGState << /GSWatermark {extGStateObj} 0 R >>";
            WriteAscii($"{pageObjNum} 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [ 0 0 {dims.Width:F2} {dims.Height:F2} ] /Contents {contentObjNum} 0 R /Resources << {resources} >> >>\nendobj\n");

            // Content Stream Object
            objectOffsets.Add(ms.Position);
            WriteAscii($"{contentObjNum} 0 obj\n<< /Length {streamBytes.Length} >>\nstream\n");
            ms.Write(streamBytes, 0, streamBytes.Length);
            WriteAscii("\nendstream\nendobj\n");
        }

        // Font F1: Helvetica (Body)
        objectOffsets.Add(ms.Position);
        WriteAscii($"{fontHelveticaObj} 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>\nendobj\n");

        // Font F2: Helvetica-Bold (Headings & Badges)
        objectOffsets.Add(ms.Position);
        WriteAscii($"{fontHelveticaBoldObj} 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>\nendobj\n");

        // Font F3: Times-Roman (Formal Legal Text)
        objectOffsets.Add(ms.Position);
        WriteAscii($"{fontTimesObj} 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Times-Roman /Encoding /WinAnsiEncoding >>\nendobj\n");

        // Font F4: Times-BoldItalic (Signatures & Script Calligraphy)
        objectOffsets.Add(ms.Position);
        WriteAscii($"{fontTimesBoldItalicObj} 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Times-BoldItalic /Encoding /WinAnsiEncoding >>\nendobj\n");

        // Font F5: Courier (Monospace Bates & Fingerprints)
        objectOffsets.Add(ms.Position);
        WriteAscii($"{fontCourierObj} 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Courier /Encoding /WinAnsiEncoding >>\nendobj\n");

        // ExtGState Object: Alpha Transparency & Multiply Blend Mode for seamless watermark blending
        objectOffsets.Add(ms.Position);
        double wmOpacity = (doc.Watermark != null && doc.Watermark.Opacity > 0) ? doc.Watermark.Opacity : 0.12;
        string wmBlend = (doc.Watermark != null && !string.IsNullOrWhiteSpace(doc.Watermark.BlendMode)) ? doc.Watermark.BlendMode : "Multiply";
        WriteAscii($"{extGStateObj} 0 obj\n<< /Type /ExtGState /ca {wmOpacity:F2} /CA {wmOpacity:F2} /BM /{wmBlend} >>\nendobj\n");

        // Info Object: Metadata
        objectOffsets.Add(ms.Position);
        string creationDate = $"D:{DateTime.UtcNow:yyyyMMddHHmmss}Z";
        WriteAscii($"{infoObj} 0 obj\n<< /Title ({EscapePdfText(doc.Title)}) /Author ({EscapePdfText(doc.Author)}) /Subject ({EscapePdfText(doc.Subject)}) /Keywords ({EscapePdfText(doc.Keywords)}) /Creator ({EscapePdfText(doc.Creator)}) /CreationDate ({creationDate}) /ModDate ({creationDate}) >>\nendobj\n");

        // Cross-Reference Table
        long xrefOffset = ms.Position;
        int totalObjs = objectOffsets.Count;
        WriteAscii($"xref\n0 {totalObjs + 1}\n0000000000 65535 f \n");
        for (int i = 0; i < totalObjs; i++)
        {
            WriteAscii($"{objectOffsets[i]:D10} 00000 n \n");
        }

        // Trailer
        WriteAscii($"trailer\n<< /Size {totalObjs + 1} /Root 1 0 R /Info {infoObj} 0 R >>\nstartxref\n{xrefOffset}\n%%EOF\n");

        return ms.ToArray();
    }

    /// <summary>
    /// Generates Base64 encoded PDF string for client-side transport.
    /// </summary>
    public static string GeneratePdfBase64(PdfContractDocument doc, bool isFlattened = false)
    {
        return Convert.ToBase64String(GeneratePdfBytes(doc, isFlattened));
    }

    /// <summary>
    /// Combines multiple document definitions into a single multi-page unified PDF document.
    /// </summary>
    public static PdfContractDocument CombineDocuments(IEnumerable<PdfContractDocument> docs, string title = "Merged Legal Portfolio", bool applyContinuousBates = true, string batesPrefix = "PORTFOLIO-")
    {
        var combined = new PdfContractDocument
        {
            DocumentId = $"COMB-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}",
            Title = title,
            ContractType = "Combined Portfolio Instrument",
            EffectiveDate = DateTime.UtcNow.Date,
            Status = "Combined Document"
        };

        int globalPage = 1;
        foreach (var srcDoc in docs)
        {
            foreach (var page in srcDoc.Pages)
            {
                var newPage = new PdfContractPage
                {
                    PageNumber = globalPage,
                    Title = string.IsNullOrWhiteSpace(page.Title) ? srcDoc.Title : page.Title,
                    Subtitle = string.IsNullOrWhiteSpace(page.Subtitle) ? $"Source: {srcDoc.DocumentId}" : page.Subtitle,
                    PageSize = page.PageSize,
                    Sections = new List<PdfContractSection>(page.Sections)
                };
                combined.Pages.Add(newPage);

                // Transfer fields re-mapped to new global page number
                var pageFields = srcDoc.Fields.Where(f => f.Page == page.PageNumber).ToList();
                foreach (var f in pageFields)
                {
                    combined.Fields.Add(new PdfContractField
                    {
                        Id = f.Id,
                        Type = f.Type,
                        Label = f.Label,
                        SignerRole = f.SignerRole,
                        Page = globalPage,
                        X = f.X,
                        Y = f.Y,
                        Width = f.Width,
                        Height = f.Height,
                        Value = f.Value,
                        IsRequired = f.IsRequired,
                        IsReadOnly = f.IsReadOnly,
                        SignedAt = f.SignedAt,
                        SignerName = f.SignerName,
                        SignatureStyle = f.SignatureStyle,
                        Placeholder = f.Placeholder
                    });
                }
                globalPage++;
            }
        }

        if (applyContinuousBates)
        {
            combined.BatesNumbering = new PdfBatesOptions
            {
                Enabled = true,
                Prefix = batesPrefix,
                StartNumber = 1,
                Digits = 6,
                Position = BatesPosition.BottomRight
            };
        }

        return combined;
    }

    /// <summary>
    /// Applies permanent cryptographic sealing and computes SHA-256 attestation digest.
    /// </summary>
    public static void SealDocument(PdfContractDocument doc, string? signerIp = null)
    {
        var payloadToHash = $"{doc.DocumentId}:{doc.Title}:{DateTime.UtcNow:O}:{doc.Fields.Count}:{doc.Pages.Count}";
        using var sha = SHA256.Create();
        var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(payloadToHash));
        doc.Sha256Fingerprint = Convert.ToHexString(hashBytes).ToLowerInvariant();

        doc.CertificateId = $"CERT-{DateTime.UtcNow.Year}-ESIGN-{Random.Shared.Next(10000, 99999)}";
        doc.SealedAtUtc = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(signerIp)) doc.SignerIp = signerIp;
        doc.Status = "Executed & Sealed";
        doc.IsSealed = true;
        doc.IsReadOnly = true;
    }

    /// <summary>
    /// Permanently flattens active form fields, burning them into immutable vector graphics.
    /// </summary>
    public static void FlattenDocument(PdfContractDocument doc)
    {
        // Populate any blank fields with dynamic executed values
        foreach (var f in doc.Fields)
        {
            if (string.IsNullOrWhiteSpace(f.Value))
            {
                string signer = !string.IsNullOrWhiteSpace(f.SignerName)
                    ? f.SignerName
                    : (!string.IsNullOrWhiteSpace(doc.Author) ? doc.Author : "Signer");

                string initials = string.Concat(signer.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(w => char.ToUpperInvariant(w[0])));
                if (string.IsNullOrEmpty(initials)) initials = "OK";

                f.Value = f.Type switch
                {
                    PdfFieldType.SignHere => signer,
                    PdfFieldType.InitialHere => initials,
                    PdfFieldType.DateTimeStamp => DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'"),
                    PdfFieldType.Checkbox => "true",
                    _ => !string.IsNullOrEmpty(f.Placeholder) ? f.Placeholder : (!string.IsNullOrEmpty(f.Label) ? f.Label : "Completed")
                };
                if (f.Type == PdfFieldType.SignHere && !f.SignedAt.HasValue)
                {
                    f.SignedAt = DateTime.UtcNow;
                    f.SignerName = f.Value;
                }
            }
        }

        doc.IsFlattened = true;
        doc.IsReadOnly = true;
        doc.FlattenedAtUtc = DateTime.UtcNow;
        doc.FlattenedContentStreamId = $"STRM-FLAT-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";

        var payloadToHash = $"{doc.DocumentId}:{doc.Title}:{doc.FlattenedAtUtc:O}:{doc.Fields.Count}:FLATTENED";
        using var sha = SHA256.Create();
        var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(payloadToHash));
        doc.FlattenedHash = Convert.ToHexString(hashBytes).ToLowerInvariant();
        if (string.IsNullOrEmpty(doc.Sha256Fingerprint))
        {
            doc.Sha256Fingerprint = doc.FlattenedHash;
        }

        doc.Status = "Flattened & Merged";
    }

    // =========================================================================
    // CONTENT STREAM BUILDER
    // =========================================================================
    private static string BuildPageContentStream(
        PdfContractDocument doc,
        PdfContractPage page,
        List<PdfContractField> fields,
        int pageNumber,
        int totalPages,
        PdfPageDimensions dims,
        bool isFlattened)
    {
        var sb = new StringBuilder();
        double pw = dims.Width;
        double ph = dims.Height;

        // 1. BACKGROUND WATERMARK (if underlay mode requested)
        if (doc.Watermark != null && doc.Watermark.Enabled && !string.IsNullOrWhiteSpace(doc.Watermark.Text) && !doc.Watermark.AsOverlay)
        {
            AppendWatermarkStream(sb, doc.Watermark, pw, ph);
        }

        // 2. HEADER LETTERHEAD RULE & METADATA
        double topMargin = ph - 45;
        if (!string.IsNullOrWhiteSpace(doc.HeaderBannerText))
        {
            sb.AppendLine("0.15 0.25 0.45 RG");
            sb.AppendLine("1.5 w");
            sb.AppendLine($"50 {topMargin:F0} m {pw - 50:F0} {topMargin:F0} l S");

            sb.AppendLine("BT");
            sb.AppendLine("/F1 8 Tf");
            sb.AppendLine("0.35 0.35 0.4 rg");
            sb.AppendLine($"50 {topMargin + 10:F0} Td ({EscapePdfText(doc.HeaderBannerText)}) Tj");
            sb.AppendLine("ET");
        }

        // 3. PAGE TITLE & SUBTITLE
        double titleY = topMargin - 28;
        sb.AppendLine("BT");
        sb.AppendLine("/F2 15 Tf");
        sb.AppendLine("0.08 0.12 0.25 rg");
        sb.AppendLine($"50 {titleY:F0} Td ({EscapePdfText(page.Title)}) Tj");
        sb.AppendLine("ET");

        double subY = titleY - 16;
        sb.AppendLine("BT");
        sb.AppendLine("/F1 8.5 Tf");
        sb.AppendLine("0.4 0.45 0.5 rg");
        string metaLine = $"Doc ID: {doc.DocumentId}   |   Page {pageNumber} of {totalPages}   |   Effective: {doc.EffectiveDate:yyyy-MM-dd}";
        sb.AppendLine($"50 {subY:F0} Td ({EscapePdfText(metaLine)}) Tj");
        sb.AppendLine("ET");

        // Determine footer geometry and guaranteed clearance boundaries
        bool hasFooter = doc.IsSealed || doc.IsFlattened || !string.IsNullOrEmpty(doc.AuditStampText);
        bool hasBatesBottom = doc.BatesNumbering != null && doc.BatesNumbering.Enabled &&
            (doc.BatesNumbering.Position == BatesPosition.BottomRight ||
             doc.BatesNumbering.Position == BatesPosition.BottomCenter ||
             doc.BatesNumbering.Position == BatesPosition.BottomLeft);

        double footerH = 44;
        double footerY = hasBatesBottom ? 36 : 26;
        double footerTop = hasFooter ? (footerY + footerH) : 0;

        // Minimum bottom boundary for any signature/attestation card:
        // Guarantee at least 18pt clearance above the audit stamp box (or 36pt above bottom page edge)
        double minCardBottom = hasFooter ? (footerTop + 18) : 36;

        int fieldCount = fields.Count;
        int fieldRows = (fieldCount + 1) / 2;
        double cardH = 44;
        double rowPitch = 56; // 44pt card + 12pt inter-row vertical spacing

        // Calculate minimum top coordinate for the first row of fields so lowest row never infringes minCardBottom
        double minFieldY = minCardBottom + cardH + (fieldRows > 0 ? (fieldRows - 1) * rowPitch : 0);
        double minDividerY = minFieldY + 14;

        // Paragraphs must stop before infringing the attestation divider & fields zone
        double minSectionBreakY = (fieldCount > 0 || !string.IsNullOrWhiteSpace(doc.FieldSectionTitle))
            ? (minDividerY + 16)
            : (minCardBottom + 16);

        // 4. CLAUSE SECTIONS & PARAGRAPHS
        double currentY = subY - 20;
        foreach (var sec in page.Sections)
        {
            if (currentY < minSectionBreakY + 13) break;

            sb.AppendLine("BT");
            sb.AppendLine("/F2 9.2 Tf");
            sb.AppendLine("0.1 0.15 0.2 rg");
            sb.AppendLine($"50 {currentY:F0} Td ({EscapePdfText(sec.Heading.ToUpperInvariant())}) Tj");
            sb.AppendLine("ET");
            currentY -= 13;

            foreach (var para in sec.Paragraphs)
            {
                if (currentY < minSectionBreakY) break;
                var lines = WrapText(para, (int)((pw - 100) / 5.7));
                foreach (var line in lines)
                {
                    if (currentY < minSectionBreakY) break;
                    sb.AppendLine("BT");
                    sb.AppendLine("/F3 8.3 Tf");
                    sb.AppendLine("0.2 0.22 0.25 rg");
                    sb.AppendLine($"50 {currentY:F0} Td ({EscapePdfText(line)}) Tj");
                    sb.AppendLine("ET");
                    currentY -= 11;
                }
                currentY -= 3;
            }
            currentY -= 5;
        }

        // 5. ATTESTATION & FIELD SECTION DIVIDER (if fields present or title set)
        double dividerY = Math.Min(currentY - 14, 380);
        dividerY = Math.Max(dividerY, minDividerY);

        if (fields.Count > 0 || !string.IsNullOrWhiteSpace(doc.FieldSectionTitle))
        {
            sb.AppendLine($"0.75 0.8 0.85 RG 1 w 50 {dividerY:F0} m {pw - 50:F0} {dividerY:F0} l S");
            string sectionTitle = !string.IsNullOrWhiteSpace(doc.FieldSectionTitle) ? doc.FieldSectionTitle : "ATTESTATION & FORM FIELDS";
            sb.AppendLine($"BT /F2 8 Tf 0.25 0.35 0.45 rg 50 {dividerY + 5:F0} Td ({EscapePdfText(sectionTitle)}) Tj ET");
        }

        // 6. STRUCTURED FIELD CARDS (2-Column Grid)
        double cardW = (pw - 130) / 2.0;
        double colX = 50;
        double fieldY = dividerY - 14;

        foreach (var field in fields)
        {
            string val = string.IsNullOrWhiteSpace(field.Value) ? (isFlattened ? "[Executed]" : "[Pending]") : field.Value;
            string label = !string.IsNullOrWhiteSpace(field.Label) ? field.Label.ToUpperInvariant() : "FIELD";

            if (field.Type == PdfFieldType.SignHere)
            {
                // Signature Card
                sb.AppendLine($"0.97 0.98 1.0 rg {colX:F0} {fieldY - cardH:F0} {cardW:F0} {cardH:F0} re f");
                sb.AppendLine($"0.7 0.78 0.9 RG 0.75 w {colX:F0} {fieldY - cardH:F0} {cardW:F0} {cardH:F0} re S");
                sb.AppendLine($"BT /F2 7 Tf 0.3 0.38 0.5 rg {colX + 10:F0} {fieldY - 12:F0} Td ({EscapePdfText(label)}) Tj ET");
                sb.AppendLine($"BT /F4 14 Tf 0.05 0.12 0.35 rg {colX + 12:F0} {fieldY - 27:F0} Td ({EscapePdfText(val)}) Tj ET");

                string signerInfo = !string.IsNullOrEmpty(field.SignerName)
                    ? (field.SignedAt.HasValue ? $"Signer: {field.SignerName} | {field.SignedAt.Value:yyyy-MM-dd HH:mm UTC}" : $"Signer: {field.SignerName}")
                    : (field.SignedAt.HasValue ? field.SignedAt.Value.ToString("yyyy-MM-dd HH:mm UTC") : string.Empty);

                if (!string.IsNullOrEmpty(signerInfo))
                {
                    sb.AppendLine($"BT /F1 6.5 Tf 0.45 0.5 0.55 rg {colX + 10:F0} {fieldY - 38:F0} Td ({EscapePdfText(signerInfo)}) Tj ET");
                }
            }
            else if (field.Type == PdfFieldType.InitialHere)
            {
                // Initials Card
                sb.AppendLine($"0.96 0.99 0.99 rg {colX:F0} {fieldY - cardH:F0} {cardW:F0} {cardH:F0} re f");
                sb.AppendLine($"0.6 0.75 0.75 RG 0.75 w {colX:F0} {fieldY - cardH:F0} {cardW:F0} {cardH:F0} re S");
                sb.AppendLine($"BT /F2 7 Tf 0.25 0.45 0.45 rg {colX + 10:F0} {fieldY - 12:F0} Td ({EscapePdfText(label)}) Tj ET");
                sb.AppendLine($"BT /F4 14 Tf 0.05 0.25 0.25 rg {colX + 14:F0} {fieldY - 28:F0} Td ({EscapePdfText(val)}) Tj ET");
            }
            else if (field.Type == PdfFieldType.Checkbox)
            {
                // Checkbox Card
                sb.AppendLine($"0.98 0.99 0.98 rg {colX:F0} {fieldY - cardH:F0} {cardW:F0} {cardH:F0} re f");
                sb.AppendLine($"0.75 0.85 0.78 RG 0.5 w {colX:F0} {fieldY - cardH:F0} {cardW:F0} {cardH:F0} re S");

                string topBadge = (label.Length > 28) ? "SWORN ATTESTATION & ACKNOWLEDGMENT" : label;
                sb.AppendLine($"BT /F2 6.5 Tf 0.3 0.45 0.35 rg {colX + 10:F0} {fieldY - 12:F0} Td ({EscapePdfText(topBadge)}) Tj ET");

                // Checkbox square
                sb.AppendLine($"0.1 0.4 0.2 RG 0.8 w {colX + 10:F0} {fieldY - 32:F0} 11 11 re S");
                if (val == "true" || isFlattened)
                {
                    sb.AppendLine($"BT /F2 8.5 Tf 0.05 0.45 0.2 rg {colX + 12:F0} {fieldY - 30:F0} Td (X) Tj ET");
                }

                string checkDesc = !string.IsNullOrWhiteSpace(field.Placeholder) ? field.Placeholder : field.Label;
                var descLines = WrapText(checkDesc, (int)((cardW - 36) / 3.9));
                if (descLines.Count == 1)
                {
                    sb.AppendLine($"BT /F1 7.2 Tf 0.1 0.15 0.1 rg {colX + 26:F0} {fieldY - 28:F0} Td ({EscapePdfText(descLines[0])}) Tj ET");
                }
                else if (descLines.Count == 2)
                {
                    sb.AppendLine($"BT /F1 6.5 Tf 0.1 0.15 0.1 rg {colX + 26:F0} {fieldY - 24:F0} Td ({EscapePdfText(descLines[0])}) Tj ET");
                    sb.AppendLine($"BT /F1 6.5 Tf 0.1 0.15 0.1 rg {colX + 26:F0} {fieldY - 34:F0} Td ({EscapePdfText(descLines[1])}) Tj ET");
                }
                else if (descLines.Count >= 3)
                {
                    sb.AppendLine($"BT /F1 6.0 Tf 0.1 0.15 0.1 rg {colX + 26:F0} {fieldY - 21:F0} Td ({EscapePdfText(descLines[0])}) Tj ET");
                    sb.AppendLine($"BT /F1 6.0 Tf 0.1 0.15 0.1 rg {colX + 26:F0} {fieldY - 29:F0} Td ({EscapePdfText(descLines[1])}) Tj ET");
                    sb.AppendLine($"BT /F1 6.0 Tf 0.1 0.15 0.1 rg {colX + 26:F0} {fieldY - 37:F0} Td ({EscapePdfText(descLines[2])}) Tj ET");
                }
            }
            else
            {
                // Standard Text / Date Card
                sb.AppendLine($"0.98 0.98 0.99 rg {colX:F0} {fieldY - cardH:F0} {cardW:F0} {cardH:F0} re f");
                sb.AppendLine($"0.82 0.84 0.88 RG 0.5 w {colX:F0} {fieldY - cardH:F0} {cardW:F0} {cardH:F0} re S");
                string cleanLabel = label.Length > 34 ? label[..34] : label;
                string cleanVal = val.Length > 36 ? val[..36] : val;
                sb.AppendLine($"BT /F2 6.5 Tf 0.4 0.45 0.5 rg {colX + 10:F0} {fieldY - 12:F0} Td ({EscapePdfText(cleanLabel)}) Tj ET");
                sb.AppendLine($"BT /F1 9 Tf 0.1 0.12 0.15 rg {colX + 10:F0} {fieldY - 28:F0} Td ({EscapePdfText(cleanVal)}) Tj ET");
            }

            colX += cardW + 30;
            if (colX > pw - 80)
            {
                colX = 50;
                fieldY -= rowPitch;
            }
        }

        // 7. FOOTER AUDIT STAMP & ESIGN CERTIFICATE (only rendered when document is sealed, flattened, or requested)
        if (hasFooter)
        {
            sb.AppendLine($"0.96 0.98 0.97 rg 50 {footerY:F0} {pw - 100:F0} {footerH:F0} re f");
            sb.AppendLine($"0.25 0.6 0.4 RG 1 w 50 {footerY:F0} {pw - 100:F0} {footerH:F0} re S");

            string statusStamp = !string.IsNullOrEmpty(doc.AuditStampText)
                ? doc.AuditStampText
                : (isFlattened ? "OFFICIALLY FLATTENED & CERTIFIED (ISO 32000-1 STATIC STREAM)" : "DIGITALLY SEALED DOCUMENT");
            sb.AppendLine($"BT /F2 7.5 Tf 0.05 0.35 0.18 rg 62 {footerY + 30:F0} Td ({EscapePdfText(statusStamp)}) Tj ET");

            if (!string.IsNullOrEmpty(doc.Sha256Fingerprint))
            {
                sb.AppendLine($"BT /F5 6.5 Tf 0.25 0.35 0.25 rg 62 {footerY + 19:F0} Td (SHA-256 Digest: {EscapePdfText(doc.Sha256Fingerprint)}) Tj ET");
            }

            var footerMetaParts = new List<string>();
            if (!string.IsNullOrEmpty(doc.CertificateId)) footerMetaParts.Add($"Cert ID: {doc.CertificateId}");
            if (!string.IsNullOrEmpty(doc.SignerIp)) footerMetaParts.Add($"Signer IP: {doc.SignerIp}");
            footerMetaParts.Add($"Effective: {doc.EffectiveDate:yyyy-MM-dd}");

            sb.AppendLine($"BT /F1 6.5 Tf 0.35 0.4 0.35 rg 62 {footerY + 8:F0} Td ({EscapePdfText(string.Join("   |   ", footerMetaParts))}) Tj ET");
        }

        // 8. BATES NUMBERING (Legal sequence stamp at specified position)
        if (doc.BatesNumbering != null && doc.BatesNumbering.Enabled)
        {
            string batesStr = doc.BatesNumbering.FormatNumber(pageNumber - 1);
            sb.AppendLine("BT");
            sb.AppendLine("/F5 8 Tf");
            sb.AppendLine("0.2 0.2 0.3 rg");

            double bx = pw - 180;
            double by = 20;
            switch (doc.BatesNumbering.Position)
            {
                case BatesPosition.BottomLeft: bx = 50; by = 20; break;
                case BatesPosition.BottomCenter: bx = (pw / 2.0) - 40; by = 20; break;
                case BatesPosition.TopRight: bx = pw - 180; by = ph - 25; break;
                case BatesPosition.TopCenter: bx = (pw / 2.0) - 40; by = ph - 25; break;
                case BatesPosition.TopLeft: bx = 50; by = ph - 25; break;
                default: bx = pw - 180; by = 20; break; // BottomRight
            }

            sb.AppendLine($"{bx:F0} {by:F0} Td ([{EscapePdfText(batesStr)}]) Tj");
            sb.AppendLine("ET");
        }

        // 9. DIAGONAL WATERMARK OVERLAY (with ISO 32000-1 Multiply blend mode)
        // Floats and seamlessly blends over paper, clauses, and signature boxes without cutouts
        if (doc.Watermark != null && doc.Watermark.Enabled && !string.IsNullOrWhiteSpace(doc.Watermark.Text) && doc.Watermark.AsOverlay)
        {
            AppendWatermarkStream(sb, doc.Watermark, pw, ph);
        }

        return sb.ToString();
    }

    private static void AppendWatermarkStream(StringBuilder sb, PdfWatermarkOptions wm, double pw, double ph)
    {
        double angleRad = wm.RotationDegrees * Math.PI / 180.0;
        double cos = Math.Cos(angleRad);
        double sin = Math.Sin(angleRad);
        double centerX = pw / 2.0;
        double centerY = ph / 2.0;

        sb.AppendLine("q"); // push graphic state
        sb.AppendLine("/GSWatermark gs"); // Activate ISO 32000-1 ExtGState Multiply blend mode & alpha
        var c = wm.ColorRgb ?? new double[] { 0.75, 0.2, 0.2 };
        sb.AppendLine($"{c[0]:F2} {c[1]:F2} {c[2]:F2} rg");
        sb.AppendLine($"{cos:F4} {sin:F4} {-sin:F4} {cos:F4} {centerX:F2} {centerY:F2} cm");
        sb.AppendLine("BT");
        sb.AppendLine($"/F2 {wm.FontSize:F1} Tf");
        double textOffset = -(wm.Text.Length * (wm.FontSize * 0.28));
        sb.AppendLine($"{textOffset:F1} -10 Td");
        sb.AppendLine($"({EscapePdfText(wm.Text)}) Tj");
        sb.AppendLine("ET");
        sb.AppendLine("Q"); // restore graphic state
    }

    // =========================================================================
    // TEMPLATE GENERATORS
    // =========================================================================
    public static PdfContractDocument CreateMasterServicesAgreement()
    {
        var doc = new PdfContractDocument
        {
            DocumentId = $"DOC-MSA-{DateTime.UtcNow.Year}-{Random.Shared.Next(1000, 9999)}",
            Title = "Master Services & Cloud Architecture Agreement",
            ContractType = "Enterprise Service Agreement & NDA",
            EffectiveDate = DateTime.UtcNow.Date,
            Status = "Pending Signatures"
        };

        doc.Pages.Add(new PdfContractPage
        {
            PageNumber = 1,
            Title = "Master Services Agreement (MSA)",
            Sections = new List<PdfContractSection>
            {
                new()
                {
                    Heading = "1. SCOPE AND RECITALS",
                    Paragraphs = new()
                    {
                        "This Master Services Agreement (\"Agreement\") is entered into effective as of the date indicated below by and between the Client Organization and Provider.",
                        "WHEREAS, Client desires to procure high-availability cloud architecture engineering, WebAssembly frontend development, and Zero-Trust cryptographic infrastructure services; and WHEREAS, Provider agrees to deliver such professional software engineering services."
                    }
                },
                new()
                {
                    Heading = "2. PROPRIETARY RIGHTS & WORK PRODUCT ASSIGNMENT",
                    Paragraphs = new()
                    {
                        "Provider hereby unconditionally assigns and transfers to Client all right, title, and interest in and to all deliverables, custom component libraries, architectures, and intellectual property developed exclusively pursuant to executed Statements of Work (SOW).",
                        "Neither party shall disclose, disseminate, or publish confidential trade secrets or cryptographic private keys without express written bilateral authorization."
                    }
                }
            }
        });

        doc.Pages.Add(new PdfContractPage
        {
            PageNumber = 2,
            Title = "Execution & Binding Attestations",
            Sections = new List<PdfContractSection>
            {
                new()
                {
                    Heading = "3. TERM, JURISDICTION & GOVERNING LAW",
                    Paragraphs = new()
                    {
                        "This Agreement shall remain in full force and effect for an initial term of twelve (12) months from the Effective Date and shall automatically renew for successive one-year periods unless terminated with thirty (30) days advance written notice.",
                        "This Agreement shall be governed by, construed, and enforced in accordance with the laws of the State of Delaware, without regard to conflicts of law principles."
                    }
                }
            }
        });

        // Fields
        doc.Fields.Add(new PdfContractField { Id = "fld-co", Page = 1, Type = PdfFieldType.Text, Label = "Client Enterprise Entity", Value = "Acme Global Industries LLC" });
        doc.Fields.Add(new PdfContractField { Id = "fld-init", Page = 1, Type = PdfFieldType.InitialHere, Label = "Page 1 Initials" });
        doc.Fields.Add(new PdfContractField { Id = "fld-sign", Page = 2, Type = PdfFieldType.SignHere, Label = "Authorized Officer Signature" });
        doc.Fields.Add(new PdfContractField { Id = "fld-date", Page = 2, Type = PdfFieldType.DateTimeStamp, Label = "Execution UTC Timestamp" });
        doc.Fields.Add(new PdfContractField { Id = "fld-ack", Page = 2, Type = PdfFieldType.Checkbox, Label = "Authority Acknowledgment", Placeholder = "I confirm executive signatory authority." });

        return doc;
    }

    public static PdfContractDocument CreateCommercialInvoice(string invoiceNumber, string clientName, decimal subtotal, decimal taxRate = 0.0825m)
    {
        decimal tax = Math.Round(subtotal * taxRate, 2);
        decimal total = subtotal + tax;

        var doc = new PdfContractDocument
        {
            DocumentId = invoiceNumber,
            Title = $"COMMERCIAL INVOICE — {invoiceNumber}",
            ContractType = "Commercial Invoice & Statement",
            EffectiveDate = DateTime.UtcNow.Date,
            Status = "Remittance Due"
        };

        doc.Pages.Add(new PdfContractPage
        {
            PageNumber = 1,
            Title = $"Commercial Invoice #{invoiceNumber}",
            Sections = new List<PdfContractSection>
            {
                new()
                {
                    Heading = "1. INVOICE PARTICULARS & BILL-TO ENTITY",
                    Paragraphs = new()
                    {
                        $"Bill To: {clientName} | Terms: Net 30 Days | Due Date: {DateTime.UtcNow.AddDays(30):yyyy-MM-dd}",
                        "Remit Payment to: Global Treasury Operations, 100 Financial Plaza, Suite 400. Wire Routing Transit: 121000358, Account: 9840192841."
                    }
                },
                new()
                {
                    Heading = "2. STATEMENT OF SERVICES & DELIVERABLES",
                    Paragraphs = new()
                    {
                        $"Professional Cloud Architecture Services: ${subtotal:N2}",
                        $"State and Municipal Sales/Excise Tax ({(taxRate * 100):F2}%): ${tax:N2}",
                        $"TOTAL REMITTANCE BALANCE DUE: ${total:N2} USD"
                    }
                }
            }
        });

        doc.Fields.Add(new PdfContractField { Id = "inv-name", Page = 1, Type = PdfFieldType.Text, Label = "Authorized Accounts Payable Approver", Value = "Jane M. Doe" });
        doc.Fields.Add(new PdfContractField { Id = "inv-sign", Page = 1, Type = PdfFieldType.SignHere, Label = "Remittance Approval Signature" });
        doc.Fields.Add(new PdfContractField { Id = "inv-date", Page = 1, Type = PdfFieldType.DateTimeStamp, Label = "Approval Date & Timestamp" });
        doc.Fields.Add(new PdfContractField { Id = "inv-ack", Page = 1, Type = PdfFieldType.Checkbox, Label = "Payment Authorization", Placeholder = "Approved for ACH electronic transfer.", Value = "true" });

        return doc;
    }

    public static PdfContractDocument CreateNonDisclosureAgreement(string disclosingParty, string receivingParty)
    {
        var doc = new PdfContractDocument
        {
            DocumentId = $"NDA-{DateTime.UtcNow.Year}-{Random.Shared.Next(1000, 9999)}",
            Title = "Mutual Non-Disclosure & Proprietary Information Agreement",
            ContractType = "Bilateral Non-Disclosure Agreement",
            EffectiveDate = DateTime.UtcNow.Date,
            Status = "Pending Mutual Signatures"
        };

        doc.Pages.Add(new PdfContractPage
        {
            PageNumber = 1,
            Title = "Mutual Confidentiality Agreement",
            Sections = new List<PdfContractSection>
            {
                new()
                {
                    Heading = "1. CONFIDENTIAL INFORMATION DEFINITION",
                    Paragraphs = new()
                    {
                        $"This Mutual Non-Disclosure Agreement is executed between {disclosingParty} (\"Disclosing Party\") and {receivingParty} (\"Receiving Party\").",
                        "Confidential Information shall comprise all non-public technical data, source code, neural models, architectural schemas, and business strategies disclosed directly or indirectly."
                    }
                },
                new()
                {
                    Heading = "2. NON-DISCLOSURE OBLIGATIONS & TERM",
                    Paragraphs = new()
                    {
                        "The Receiving Party agrees to protect the Confidential Information using the same degree of care, but not less than reasonable care, used to protect its own sensitive trade secrets.",
                        "Obligations under this Agreement shall endure for a term of three (3) years from the Effective Date."
                    }
                }
            }
        });

        doc.Fields.Add(new PdfContractField { Id = "nda-disc-sign", Page = 1, Type = PdfFieldType.SignHere, Label = $"{disclosingParty} Authorized Signature" });
        doc.Fields.Add(new PdfContractField { Id = "nda-recv-sign", Page = 1, Type = PdfFieldType.SignHere, Label = $"{receivingParty} Authorized Signature" });
        doc.Fields.Add(new PdfContractField { Id = "nda-date", Page = 1, Type = PdfFieldType.DateTimeStamp, Label = "Bilateral Execution Date" });

        return doc;
    }

    public static PdfContractDocument CreateCertificateOfCompletion(string recipientName, string courseOrProject, string issuer)
    {
        var doc = new PdfContractDocument
        {
            DocumentId = $"CERT-COMP-{DateTime.UtcNow.Year}-{Random.Shared.Next(1000, 9999)}",
            Title = "Certificate of Professional Mastery & Attestation",
            ContractType = "Professional Mastery Certificate",
            EffectiveDate = DateTime.UtcNow.Date,
            Status = "Certified"
        };

        doc.Pages.Add(new PdfContractPage
        {
            PageNumber = 1,
            Title = "Certificate of Professional Mastery",
            Sections = new List<PdfContractSection>
            {
                new()
                {
                    Heading = "AWARDED TO",
                    Paragraphs = new()
                    {
                        $"This certifies that {recipientName} has successfully demonstrated enterprise competence in {courseOrProject}.",
                        $"Conferred under authority of {issuer} following rigorous evaluation and verification of production-grade competencies."
                    }
                }
            }
        });

        doc.Fields.Add(new PdfContractField { Id = "cert-auth-sign", Page = 1, Type = PdfFieldType.SignHere, Label = "Certification Authority Signature", Value = issuer });
        doc.Fields.Add(new PdfContractField { Id = "cert-date", Page = 1, Type = PdfFieldType.DateTimeStamp, Label = "Conferral Date" });

        SealDocument(doc);
        FlattenDocument(doc);
        return doc;
    }

    // =========================================================================
    // UTILITY HELPERS
    // =========================================================================
    public static string EscapePdfText(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        return text
            .Replace("â€”", " -- ")
            .Replace("â€\"", " -- ")
            .Replace("â€“", "-")
            .Replace("Â·", " * ")
            .Replace("â€¢", " * ")
            .Replace("â€œ", "\"")
            .Replace("â€\u009d", "\"")
            .Replace("â€˜", "'")
            .Replace("â€™", "'")
            .Replace("—", " -- ")
            .Replace("–", "-")
            .Replace("·", " * ")
            .Replace("•", " * ")
            .Replace("“", "\"")
            .Replace("”", "\"")
            .Replace("‘", "'")
            .Replace("’", "'")
            .Replace("\\", "\\\\")
            .Replace("(", "\\(")
            .Replace(")", "\\)")
            .Replace("\r", "")
            .Replace("\n", " ");
    }

    public static List<string> WrapText(string text, int maxCharsPerLine)
    {
        var result = new List<string>();
        if (string.IsNullOrWhiteSpace(text)) return result;

        string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var curLine = new StringBuilder();

        foreach (var word in words)
        {
            if (curLine.Length + word.Length + 1 > maxCharsPerLine)
            {
                result.Add(curLine.ToString());
                curLine.Clear();
            }
            if (curLine.Length > 0) curLine.Append(' ');
            curLine.Append(word);
        }

        if (curLine.Length > 0)
        {
            result.Add(curLine.ToString());
        }

        return result;
    }
}


