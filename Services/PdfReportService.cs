using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using BlazorTwUI.Models;

namespace BlazorTwUI.Services;

/// <summary>
/// Pure C# MIT-licensed engine that renders structured data collections and grids into
/// high-fidelity PDF reports matching BlazorTwUI's theme branding, accent colors,
/// KPI bento cards, and typography.
/// </summary>
public static class PdfReportService
{
    public static byte[] GenerateReportPdf(PdfBrandedReportDocument doc)
    {
        using var ms = new MemoryStream();
        using var writer = new StreamWriter(ms, Encoding.ASCII, leaveOpen: true);

        // Compute SHA-256 integrity seal
        if (doc.ShowAttestationSeal && string.IsNullOrEmpty(doc.Sha256Seal))
        {
            var seed = $"{doc.ReportId}:{doc.Title}:{doc.GeneratedAt:O}:{doc.Rows.Count}:{doc.Columns.Count}";
            using var sha = SHA256.Create();
            doc.Sha256Seal = Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(seed))).ToLowerInvariant();
        }

        // Determine page dimensions
        var dims = PdfPageDimensions.FromPageSize(doc.PageSize);
        double pw = doc.Landscape ? dims.Height : dims.Width;
        double ph = doc.Landscape ? dims.Width : dims.Height;

        // Accent Color
        var accentColor = !string.IsNullOrEmpty(doc.CustomAccentHex)
            ? PdfReportColor.FromHex(doc.CustomAccentHex)
            : PdfReportColor.FromAccent(doc.Accent);

        // Partition rows into pages
        var pages = PartitionIntoPages(doc, ph);
        int totalPages = pages.Count;

        // Write PDF Header
        writer.Write("%PDF-1.4\n%\xE2\xE3\xCF\xD3\n");
        writer.Flush();

        var objectOffsets = new List<long>();

        // Catalog & Page Tree
        objectOffsets.Add(ms.Position);
        writer.Write("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");
        writer.Flush();

        int fontHelveticaObj = 3 + totalPages * 2;
        int fontHelveticaBoldObj = fontHelveticaObj + 1;
        int fontCourierObj = fontHelveticaBoldObj + 1;
        int extGStateObj = fontCourierObj + 1;
        int infoObj = extGStateObj + 1;

        // Page tree object
        objectOffsets.Add(ms.Position);
        var kidsBuilder = new StringBuilder();
        for (int i = 0; i < totalPages; i++)
        {
            kidsBuilder.Append($"{3 + i * 2} 0 R ");
        }
        writer.Write($"2 0 obj\n<< /Type /Pages /Kids [ {kidsBuilder} ] /Count {totalPages} >>\nendobj\n");
        writer.Flush();

        // Pages and Content Streams
        for (int p = 0; p < totalPages; p++)
        {
            var pageData = pages[p];
            string contentStream = BuildReportPageStream(doc, pageData, p + 1, totalPages, pw, ph, accentColor);
            byte[] streamBytes = Encoding.UTF8.GetBytes(contentStream);

            int pageObj = 3 + p * 2;
            int streamObj = pageObj + 1;

            // Page Object
            objectOffsets.Add(ms.Position);
            writer.Write($"{pageObj} 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [ 0 0 {pw:F2} {ph:F2} ] /Contents {streamObj} 0 R /Resources << /Font << /F1 {fontHelveticaObj} 0 R /F2 {fontHelveticaBoldObj} 0 R /F5 {fontCourierObj} 0 R >> /ExtGState << /GSWatermark {extGStateObj} 0 R >> >> >>\nendobj\n");
            writer.Flush();

            // Stream Object
            objectOffsets.Add(ms.Position);
            writer.Write($"{streamObj} 0 obj\n<< /Length {streamBytes.Length} >>\nstream\n");
            writer.Flush();
            ms.Write(streamBytes, 0, streamBytes.Length);
            writer.Write("\nendstream\nendobj\n");
            writer.Flush();
        }

        // Fonts
        objectOffsets.Add(ms.Position);
        writer.Write($"{fontHelveticaObj} 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>\nendobj\n");
        writer.Flush();

        objectOffsets.Add(ms.Position);
        writer.Write($"{fontHelveticaBoldObj} 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>\nendobj\n");
        writer.Flush();

        objectOffsets.Add(ms.Position);
        writer.Write($"{fontCourierObj} 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Courier /Encoding /WinAnsiEncoding >>\nendobj\n");
        writer.Flush();

        // ExtGState Object: Alpha Transparency & Multiply Blend Mode
        objectOffsets.Add(ms.Position);
        writer.Write($"{extGStateObj} 0 obj\n<< /Type /ExtGState /ca 0.12 /CA 0.12 /BM /Multiply >>\nendobj\n");
        writer.Flush();

        // Info Metadata
        objectOffsets.Add(ms.Position);
        string dateStr = $"D:{DateTime.UtcNow:yyyyMMddHHmmss}Z";
        writer.Write($"{infoObj} 0 obj\n<< /Title ({EscapePdf(doc.Title ?? "")}) /Author ({EscapePdf(doc.Author ?? "")}) /Subject ({EscapePdf(doc.Subtitle ?? "")}) /Creator (BlazorTwUI Branded Data Engine) /CreationDate ({dateStr}) >>\nendobj\n");
        writer.Flush();

        // XRef Table
        long xrefPos = ms.Position;
        int totalObjs = objectOffsets.Count;
        writer.Write($"xref\n0 {totalObjs + 1}\n0000000000 65535 f \n");
        for (int i = 0; i < totalObjs; i++)
        {
            writer.Write($"{objectOffsets[i]:D10} 00000 n \n");
        }

        writer.Write($"trailer\n<< /Size {totalObjs + 1} /Root 1 0 R /Info {infoObj} 0 R >>\nstartxref\n{xrefPos}\n%%EOF\n");
        writer.Flush();

        return ms.ToArray();
    }

    private static List<ReportPageSlice> PartitionIntoPages(PdfBrandedReportDocument doc, double pageHeight)
    {
        var slices = new List<ReportPageSlice>();
        double printableBottom = 55.0; // bottom margin for footer

        // Page 1 header + KPI cards consume more vertical room
        double p1Top = pageHeight - 40.0;
        double p1Used = 120.0; // Header band + org + title + meta
        if (doc.Kpis != null && doc.Kpis.Count > 0)
        {
            p1Used += 65.0; // KPI Cards bento row
        }
        p1Used += 30.0; // Table column header height + gap

        double p1TableAvailableHeight = p1Top - p1Used - printableBottom;
        double rowHeight = 20.0;
        int p1MaxRows = Math.Max(1, (int)(p1TableAvailableHeight / rowHeight));

        // Subsequent pages have compact header
        double pSubTop = pageHeight - 40.0;
        double pSubUsed = 50.0 + 30.0; // Compact header + table column header
        double pSubTableAvailableHeight = pSubTop - pSubUsed - printableBottom;
        int pSubMaxRows = Math.Max(1, (int)(pSubTableAvailableHeight / rowHeight));

        int totalRows = doc.Rows.Count;
        int currentIndex = 0;

        // Page 1
        int p1Count = Math.Min(totalRows, p1MaxRows);
        slices.Add(new ReportPageSlice
        {
            StartRowIndex = currentIndex,
            RowCount = p1Count,
            IsFirstPage = true,
            HasSummaryOnThisPage = (p1Count == totalRows) && (doc.SummaryRow != null)
        });
        currentIndex += p1Count;

        // Subsequent pages
        while (currentIndex < totalRows)
        {
            int count = Math.Min(totalRows - currentIndex, pSubMaxRows);
            bool isLast = (currentIndex + count) >= totalRows;
            slices.Add(new ReportPageSlice
            {
                StartRowIndex = currentIndex,
                RowCount = count,
                IsFirstPage = false,
                HasSummaryOnThisPage = isLast && (doc.SummaryRow != null)
            });
            currentIndex += count;
        }

        if (slices.Count == 0)
        {
            slices.Add(new ReportPageSlice { StartRowIndex = 0, RowCount = 0, IsFirstPage = true });
        }

        return slices;
    }

    private static string BuildReportPageStream(
        PdfBrandedReportDocument doc,
        ReportPageSlice slice,
        int pageNumber,
        int totalPages,
        double pw,
        double ph,
        PdfReportColor accent)
    {
        var sb = new StringBuilder();

        double leftMargin = 40.0;
        double rightMargin = pw - 40.0;
        double tableWidth = rightMargin - leftMargin;

        double currentY = ph - 30.0;

        // 2. HEADER
        if (slice.IsFirstPage)
        {
            // Top Accent Brand Stripe
            sb.AppendLine($"{accent.R:F3} {accent.G:F3} {accent.B:F3} rg {leftMargin:F0} {currentY:F0} {tableWidth:F0} 4 re f");
            currentY -= 18.0;

            // Organization & Tagline (dynamic, only rendered if provided)
            if (!string.IsNullOrWhiteSpace(doc.OrganizationName))
            {
                sb.AppendLine("BT");
                sb.AppendLine("/F2 10 Tf");
                sb.AppendLine($"{accent.R:F3} {accent.G:F3} {accent.B:F3} rg");
                sb.AppendLine($"{leftMargin:F0} {currentY:F0} Td ({EscapePdf(doc.OrganizationName)}) Tj");
                sb.AppendLine("ET");
            }

            // Environment Pill on Top Right (dynamic, only rendered if provided)
            if (!string.IsNullOrWhiteSpace(doc.Environment))
            {
                double pillW = Math.Max(50.0, doc.Environment.Length * 5.5 + 16.0);
                double pillH = 14.0;
                double pillX = rightMargin - pillW;
                sb.AppendLine($"0.92 0.95 0.98 rg {pillX:F0} {currentY - 3:F0} {pillW:F0} {pillH:F0} re f");
                sb.AppendLine($"0.7 0.8 0.9 RG 0.5 w {pillX:F0} {currentY - 3:F0} {pillW:F0} {pillH:F0} re S");
                sb.AppendLine($"BT /F2 7 Tf {accent.R:F3} {accent.G:F3} {accent.B:F3} rg {pillX + 8:F0} {currentY:F0} Td ({EscapePdf(doc.Environment)}) Tj ET");
            }

            if (!string.IsNullOrWhiteSpace(doc.OrganizationName) || !string.IsNullOrWhiteSpace(doc.Environment))
            {
                currentY -= 20.0;
            }

            // Report Title
            sb.AppendLine("BT");
            sb.AppendLine("/F2 16 Tf");
            sb.AppendLine("0.08 0.12 0.2 rg");
            sb.AppendLine($"{leftMargin:F0} {currentY:F0} Td ({EscapePdf(doc.Title)}) Tj");
            sb.AppendLine("ET");

            currentY -= 14.0;

            // Subtitle & Meta Line
            sb.AppendLine("BT");
            sb.AppendLine("/F1 8.5 Tf");
            sb.AppendLine("0.4 0.45 0.5 rg");
            string metaLine = string.IsNullOrWhiteSpace(doc.Subtitle)
                ? $"Generated: {doc.GeneratedAt:yyyy-MM-dd HH:mm:ss} UTC   |   Doc ID: {doc.ReportId}"
                : $"{doc.Subtitle}   |   Generated: {doc.GeneratedAt:yyyy-MM-dd HH:mm:ss} UTC   |   Doc ID: {doc.ReportId}";
            sb.AppendLine($"{leftMargin:F0} {currentY:F0} Td ({EscapePdf(metaLine)}) Tj");
            sb.AppendLine("ET");

            currentY -= 20.0;

            // 3. KPI BENTO CARDS (if present)
            if (doc.Kpis != null && doc.Kpis.Count > 0)
            {
                int kpiCount = Math.Min(4, doc.Kpis.Count);
                double gap = 10.0;
                double cardW = (tableWidth - (gap * (kpiCount - 1))) / kpiCount;
                double cardH = 50.0;

                for (int k = 0; k < kpiCount; k++)
                {
                    var kpi = doc.Kpis[k];
                    double cx = leftMargin + k * (cardW + gap);
                    double cy = currentY - cardH;

                    // Card Background & Border (Tailwind bg-white / rounded-xl / border-slate-200)
                    sb.AppendLine($"0.98 0.98 0.99 rg {cx:F0} {cy:F0} {cardW:F0} {cardH:F0} re f");
                    sb.AppendLine($"0.88 0.90 0.93 RG 0.75 w {cx:F0} {cy:F0} {cardW:F0} {cardH:F0} re S");

                    // Label (7pt Slate 500)
                    sb.AppendLine($"BT /F2 6.5 Tf 0.45 0.5 0.55 rg {cx + 8:F0} {cy + 36:F0} Td ({EscapePdf(kpi.Label)}) Tj ET");

                    // Value (15pt Slate 900 Bold)
                    sb.AppendLine($"BT /F2 14 Tf 0.1 0.15 0.2 rg {cx + 8:F0} {cy + 18:F0} Td ({EscapePdf(kpi.Value)}) Tj ET");

                    // Change / Badge Indicator (Green / Amber pill)
                    if (!string.IsNullOrEmpty(kpi.ChangeText))
                    {
                        bool isPos = kpi.IsPositive ?? true;
                        if (isPos)
                            sb.AppendLine($"BT /F2 7 Tf 0.05 0.6 0.35 rg {cx + 8:F0} {cy + 7:F0} Td ({EscapePdf(kpi.ChangeText)}) Tj ET");
                        else
                            sb.AppendLine($"BT /F2 7 Tf 0.85 0.15 0.25 rg {cx + 8:F0} {cy + 7:F0} Td ({EscapePdf(kpi.ChangeText)}) Tj ET");
                    }

                    if (!string.IsNullOrEmpty(kpi.Subtext))
                    {
                        sb.AppendLine($"BT /F1 6.5 Tf 0.5 0.55 0.6 rg {cx + cardW - 65:F0} {cy + 7:F0} Td ({EscapePdf(kpi.Subtext)}) Tj ET");
                    }
                }

                currentY -= (cardH + 18.0);
            }
        }
        else
        {
            // Compact Header for Page 2+
            sb.AppendLine($"{accent.R:F3} {accent.G:F3} {accent.B:F3} rg {leftMargin:F0} {currentY:F0} {tableWidth:F0} 2 re f");
            currentY -= 14.0;
            sb.AppendLine($"BT /F2 9 Tf 0.2 0.25 0.35 rg {leftMargin:F0} {currentY:F0} Td ({EscapePdf(doc.Title)} [Continued - Page {pageNumber} of {totalPages}]) Tj ET");
            currentY -= 16.0;
        }

        // 4. DATA TABLE
        // Compute column widths based on weights
        double totalWeight = 0;
        foreach (var col in doc.Columns) totalWeight += Math.Max(0.1, col.WidthWeight);
        if (totalWeight <= 0) totalWeight = 1.0;

        var colWidths = new List<double>();
        foreach (var col in doc.Columns)
        {
            colWidths.Add(tableWidth * (col.WidthWeight / totalWeight));
        }

        // Table Header Band
        double thHeight = 22.0;
        sb.AppendLine($"{accent.R:F3} {accent.G:F3} {accent.B:F3} rg {leftMargin:F0} {currentY - thHeight:F0} {tableWidth:F0} {thHeight:F0} re f");

        double colCursor = leftMargin;
        for (int c = 0; c < doc.Columns.Count; c++)
        {
            var col = doc.Columns[c];
            double cw = colWidths[c];
            double tx = col.Alignment switch
            {
                PdfReportAlignment.Right => colCursor + cw - 8.0 - (col.Header.Length * 4.5),
                PdfReportAlignment.Center => colCursor + (cw / 2.0) - (col.Header.Length * 2.2),
                _ => colCursor + 8.0
            };

            sb.AppendLine($"BT /F2 8 Tf 1.0 1.0 1.0 rg {tx:F0} {currentY - 14:F0} Td ({EscapePdf(col.Header)}) Tj ET");
            colCursor += cw;
        }

        currentY -= thHeight;

        // Table Rows
        double rowH = 19.0;
        for (int r = 0; r < slice.RowCount; r++)
        {
            int rowIdx = slice.StartRowIndex + r;
            if (rowIdx >= doc.Rows.Count) break;
            var row = doc.Rows[rowIdx];

            // Zebra Striping (Even rows get soft Slate 50 background #f8fafc)
            if (rowIdx % 2 == 1)
            {
                sb.AppendLine($"0.97 0.98 0.99 rg {leftMargin:F0} {currentY - rowH:F0} {tableWidth:F0} {rowH:F0} re f");
            }

            // Bottom Border Line for row
            sb.AppendLine($"0.9 0.92 0.95 RG 0.5 w {leftMargin:F0} {currentY - rowH:F0} m {rightMargin:F0} {currentY - rowH:F0} l S");

            colCursor = leftMargin;
            for (int c = 0; c < doc.Columns.Count; c++)
            {
                var col = doc.Columns[c];
                double cw = colWidths[c];
                string cellVal = c < row.Cells.Count ? row.Cells[c] : string.Empty;

                if (col.IsBadge && !string.IsNullOrWhiteSpace(cellVal))
                {
                    // Draw Tailwind Badge Pill
                    double bw = Math.Min(cw - 12.0, Math.Max(38.0, cellVal.Length * 5.2 + 10.0));
                    double bh = 12.0;
                    double bx = colCursor + (cw - bw) / 2.0;
                    double by = currentY - 15.0;

                    // Tone resolution
                    string tone = col.BadgeTone?.ToLowerInvariant() ?? InferTone(cellVal);
                    var (fillR, fillG, fillB, textR, textG, textB) = GetBadgeColors(tone);

                    sb.AppendLine($"{fillR:F2} {fillG:F2} {fillB:F2} rg {bx:F0} {by:F0} {bw:F0} {bh:F0} re f");
                    sb.AppendLine($"BT /F2 6.5 Tf {textR:F2} {textG:F2} {textB:F2} rg {bx + 4:F0} {by + 3:F0} Td ({EscapePdf(cellVal)}) Tj ET");
                }
                else
                {
                    // Regular Text
                    double textY = currentY - 13.0;
                    double tx = col.Alignment switch
                    {
                        PdfReportAlignment.Right => colCursor + cw - 8.0 - (cellVal.Length * 4.8),
                        PdfReportAlignment.Center => colCursor + (cw / 2.0) - (cellVal.Length * 2.4),
                        _ => colCursor + 8.0
                    };

                    sb.AppendLine($"BT /F1 7.5 Tf 0.15 0.2 0.25 rg {tx:F0} {textY:F0} Td ({EscapePdf(cellVal)}) Tj ET");
                }

                colCursor += cw;
            }

            currentY -= rowH;
        }

        // 5. SUMMARY ROW (if configured and on last page)
        if (slice.HasSummaryOnThisPage && doc.SummaryRow != null)
        {
            double sumH = 22.0;
            sb.AppendLine($"0.94 0.96 0.98 rg {leftMargin:F0} {currentY - sumH:F0} {tableWidth:F0} {sumH:F0} re f");
            sb.AppendLine($"0.6 0.65 0.75 RG 1 w {leftMargin:F0} {currentY:F0} m {rightMargin:F0} {currentY:F0} l S");
            sb.AppendLine($"0.6 0.65 0.75 RG 1.5 w {leftMargin:F0} {currentY - sumH:F0} m {rightMargin:F0} {currentY - sumH:F0} l S");

            colCursor = leftMargin;
            for (int c = 0; c < doc.Columns.Count; c++)
            {
                var col = doc.Columns[c];
                double cw = colWidths[c];
                string cellVal = c < doc.SummaryRow.Count ? doc.SummaryRow[c] : string.Empty;

                if (!string.IsNullOrWhiteSpace(cellVal))
                {
                    double tx = col.Alignment switch
                    {
                        PdfReportAlignment.Right => colCursor + cw - 8.0 - (cellVal.Length * 5.0),
                        PdfReportAlignment.Center => colCursor + (cw / 2.0) - (cellVal.Length * 2.5),
                        _ => colCursor + 8.0
                    };

                    sb.AppendLine($"BT /F2 8 Tf 0.08 0.12 0.2 rg {tx:F0} {currentY - 15:F0} Td ({EscapePdf(cellVal)}) Tj ET");
                }
                colCursor += cw;
            }

            currentY -= sumH;
        }

        // 6. FOOTER & VERIFICATION SEAL
        double footerY = 32.0;
        sb.AppendLine($"0.85 0.88 0.9 RG 0.5 w {leftMargin:F0} {footerY + 12:F0} m {rightMargin:F0} {footerY + 12:F0} l S");

        // Footer Text (dynamic)
        sb.AppendLine("BT");
        sb.AppendLine("/F1 6.5 Tf");
        sb.AppendLine("0.45 0.5 0.55 rg");
        string footerLeft = string.IsNullOrWhiteSpace(doc.Tagline)
            ? $"Report: {doc.ReportId}"
            : $"{doc.Tagline}  |  Report: {doc.ReportId}";
        sb.AppendLine($"{leftMargin:F0} {footerY:F0} Td ({EscapePdf(footerLeft)}) Tj");
        sb.AppendLine("ET");

        // Page Number
        sb.AppendLine("BT");
        sb.AppendLine("/F2 6.5 Tf");
        sb.AppendLine("0.35 0.4 0.45 rg");
        string pageStr = $"Page {pageNumber} of {totalPages}";
        sb.AppendLine($"{rightMargin - 55:F0} {footerY:F0} Td ({EscapePdf(pageStr)}) Tj");
        sb.AppendLine("ET");

        // SHA-256 Tamper-Evident Seal Badge
        if (doc.ShowAttestationSeal && !string.IsNullOrEmpty(doc.Sha256Seal))
        {
            string shortHash = doc.Sha256Seal.Length > 16 ? doc.Sha256Seal[..16] : doc.Sha256Seal;
            sb.AppendLine("BT");
            sb.AppendLine("/F5 6 Tf");
            sb.AppendLine("0.3 0.4 0.5 rg");
            sb.AppendLine($"{leftMargin + 220:F0} {footerY:F0} Td ({EscapePdf($"SHA-256 SEAL: {shortHash}...")}) Tj");
            sb.AppendLine("ET");
        }

        // 7. DIAGONAL WATERMARK OVERLAY (with ISO 32000-1 Multiply blend mode)
        // Seamlessly floats across table rows, borders, and KPI boxes without cutouts
        if (doc.ShowWatermark && !string.IsNullOrWhiteSpace(doc.WatermarkText))
        {
            sb.AppendLine("q");
            sb.AppendLine("/GSWatermark gs");
            sb.AppendLine("0.85 0.25 0.25 rg");
            sb.AppendLine($"0.7071 0.7071 -0.7071 0.7071 {pw / 2:F1} {ph / 2:F1} cm");
            sb.AppendLine("BT");
            sb.AppendLine("/F2 44 Tf");
            double textOffset = -(doc.WatermarkText.Length * 12.0);
            sb.AppendLine($"{textOffset:F1} -10 Td");
            sb.AppendLine($"({EscapePdf(doc.WatermarkText)}) Tj");
            sb.AppendLine("ET");
            sb.AppendLine("Q");
        }

        return sb.ToString();
    }

    private static string InferTone(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "neutral";
        var lower = text.ToLowerInvariant();
        if (lower == "true" || lower == "yes" || lower == "success" || lower == "ok")
            return "success";
        if (lower == "warning" || lower == "warn")
            return "warning";
        if (lower == "false" || lower == "no" || lower == "error" || lower == "danger" || lower == "failed")
            return "danger";
        return "neutral";
    }

    private static (double fillR, double fillG, double fillB, double textR, double textG, double textB) GetBadgeColors(string tone) => tone switch
    {
        "success" => (0.85, 0.96, 0.89, 0.02, 0.48, 0.28), // emerald-100 / emerald-700
        "warning" => (0.99, 0.95, 0.82, 0.71, 0.42, 0.04), // amber-100 / amber-700
        "danger" => (0.99, 0.89, 0.91, 0.78, 0.11, 0.25),  // rose-100 / rose-700
        "info" => (0.88, 0.95, 0.99, 0.01, 0.45, 0.70),    // sky-100 / sky-700
        _ => (0.93, 0.95, 0.97, 0.30, 0.35, 0.45)           // slate-100 / slate-700
    };

    private static string EscapePdf(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        return text.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
    }

    private sealed class ReportPageSlice
    {
        public int StartRowIndex { get; set; }
        public int RowCount { get; set; }
        public bool IsFirstPage { get; set; }
        public bool HasSummaryOnThisPage { get; set; }
    }
}

