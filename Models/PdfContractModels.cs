using System;
using System.Collections.Generic;

namespace BlazorTwUI.Models;

public enum PdfFieldType
{
    SignHere,
    InitialHere,
    DateTimeStamp,
    Text,
    Checkbox,
    Dropdown
}

public enum PdfPageSize
{
    Letter,   // 612 x 792 pt (8.5 x 11 in)
    A4,       // 595.28 x 841.89 pt (210 x 297 mm)
    Legal,    // 612 x 1008 pt (8.5 x 14 in)
    Ledger,   // 792 x 1224 pt (11 x 17 in)
    Custom
}

public enum BatesPosition
{
    BottomRight,
    BottomCenter,
    BottomLeft,
    TopRight,
    TopCenter,
    TopLeft
}

public sealed class PdfPageDimensions
{
    public double Width { get; set; } = 612.0;
    public double Height { get; set; } = 792.0;

    public static PdfPageDimensions FromPageSize(PdfPageSize size, double customWidth = 612.0, double customHeight = 792.0)
    {
        return size switch
        {
            PdfPageSize.A4 => new PdfPageDimensions { Width = 595.28, Height = 841.89 },
            PdfPageSize.Legal => new PdfPageDimensions { Width = 612.0, Height = 1008.0 },
            PdfPageSize.Ledger => new PdfPageDimensions { Width = 792.0, Height = 1224.0 },
            PdfPageSize.Custom => new PdfPageDimensions { Width = customWidth, Height = customHeight },
            _ => new PdfPageDimensions { Width = 612.0, Height = 792.0 } // Letter
        };
    }
}

public sealed class PdfWatermarkOptions
{
    public bool Enabled { get; set; } = false;
    public string Text { get; set; } = "CONFIDENTIAL";
    public double Opacity { get; set; } = 0.12;
    public double RotationDegrees { get; set; } = 45.0;
    public double FontSize { get; set; } = 46.0;
    public double[] ColorRgb { get; set; } = new double[] { 0.75, 0.2, 0.2 }; // Reddish tint
    public bool AsOverlay { get; set; } = true; // Seamless blend overlay across document and field boxes
    public string BlendMode { get; set; } = "Multiply"; // Multiply blend mode mimics authentic physical watermarking
}

public sealed class PdfBatesOptions
{
    public bool Enabled { get; set; } = false;
    public string Prefix { get; set; } = "DOC-";
    public int StartNumber { get; set; } = 1;
    public int Digits { get; set; } = 6;
    public string Suffix { get; set; } = string.Empty;
    public BatesPosition Position { get; set; } = BatesPosition.BottomRight;

    public string FormatNumber(int index)
    {
        int num = StartNumber + index;
        return $"{Prefix}{num.ToString().PadLeft(Digits, '0')}{Suffix}";
    }
}

public sealed class PdfContractField
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public PdfFieldType Type { get; set; } = PdfFieldType.Text;
    public string Label { get; set; } = "Field";
    public string SignerRole { get; set; } = "Primary Signer";
    public int Page { get; set; } = 1;
    public double X { get; set; } = 10.0;
    public double Y { get; set; } = 10.0;
    public double Width { get; set; } = 25.0;
    public double Height { get; set; } = 6.0;
    public string Value { get; set; } = string.Empty;
    public bool IsRequired { get; set; } = true;
    public bool IsReadOnly { get; set; } = false;
    public DateTime? SignedAt { get; set; }
    public string SignerName { get; set; } = string.Empty;
    public string SignatureDataUrl { get; set; } = string.Empty;
    public string SignatureStyle { get; set; } = "cursive";
    public string Placeholder { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();

    public bool IsFilled => Type switch
    {
        PdfFieldType.SignHere => !string.IsNullOrWhiteSpace(SignatureDataUrl) || !string.IsNullOrWhiteSpace(Value),
        PdfFieldType.InitialHere => !string.IsNullOrWhiteSpace(SignatureDataUrl) || !string.IsNullOrWhiteSpace(Value),
        PdfFieldType.DateTimeStamp => !string.IsNullOrWhiteSpace(Value) || SignedAt.HasValue,
        PdfFieldType.Checkbox => Value == "true",
        _ => !string.IsNullOrWhiteSpace(Value)
    };
}

public sealed class PdfContractSection
{
    public string Heading { get; set; } = string.Empty;
    public List<string> Paragraphs { get; set; } = new();
}

public sealed class PdfContractPage
{
    public int PageNumber { get; set; } = 1;
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public PdfPageSize PageSize { get; set; } = PdfPageSize.Letter;
    public List<PdfContractSection> Sections { get; set; } = new();
}

public sealed class PdfContractDocument
{
    public string DocumentId { get; set; } = $"DOC-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
    public string Title { get; set; } = "Document";
    public string ContractType { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; } = DateTime.UtcNow.Date;
    public string Status { get; set; } = "Draft";
    public string Author { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public string Creator { get; set; } = string.Empty;
    public string? HeaderBannerText { get; set; }
    public string? FieldSectionTitle { get; set; }
    public string? AuditStampText { get; set; }
    public bool IsReadOnly { get; set; } = false;
    public bool IsSealed { get; set; } = false;
    public bool IsFlattened { get; set; } = false;
    public DateTime? FlattenedAtUtc { get; set; }
    public string FlattenedHash { get; set; } = string.Empty;
    public string FlattenedContentStreamId { get; set; } = string.Empty;
    public string Sha256Fingerprint { get; set; } = string.Empty;
    public DateTime? SealedAtUtc { get; set; }
    public string SignerIp { get; set; } = string.Empty;
    public string CertificateId { get; set; } = string.Empty;
    public PdfPageSize DefaultPageSize { get; set; } = PdfPageSize.Letter;
    public PdfWatermarkOptions Watermark { get; set; } = new();
    public PdfBatesOptions BatesNumbering { get; set; } = new();
    public List<PdfContractPage> Pages { get; set; } = new();
    public List<PdfContractField> Fields { get; set; } = new();
}

// Request & Response DTOs for .NET Minimal APIs & Cross-Platform REST Clients
public sealed class PdfGenerateRequest
{
    public PdfContractDocument Document { get; set; } = new();
    public bool Flatten { get; set; } = false;
    public bool ApplyWatermark { get; set; } = false;
    public string? WatermarkText { get; set; }
    public bool ApplyBates { get; set; } = false;
    public string? BatesPrefix { get; set; }
    public bool ReturnBase64 { get; set; } = false;
}

public sealed class PdfGenerateResponse
{
    public string DocumentId { get; set; } = string.Empty;
    public string Filename { get; set; } = string.Empty;
    public string MimeType { get; set; } = "application/pdf";
    public long FileSizeBytes { get; set; }
    public int PageCount { get; set; }
    public string? Base64Data { get; set; }
    public string? Sha256Fingerprint { get; set; }
    public string? DownloadUrl { get; set; }
    public bool IsFlattened { get; set; }
    public bool IsSealed { get; set; }
}

public sealed class PdfFlattenRequest
{
    public PdfContractDocument Document { get; set; } = new();
    public bool ReturnBase64 { get; set; } = true;
}

public sealed class PdfCombineRequest
{
    public string Title { get; set; } = "Merged Legal Portfolio";
    public List<PdfContractDocument> Documents { get; set; } = new();
    public bool FlattenAll { get; set; } = true;
    public bool ApplyContinuousBates { get; set; } = true;
    public string BatesPrefix { get; set; } = "PORTFOLIO-";
    public bool ReturnBase64 { get; set; } = false;
}

public sealed class PdfDocumentInfoResponse
{
    public string DocumentId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int PageCount { get; set; }
    public int FieldCount { get; set; }
    public int RequiredFieldsRemaining { get; set; }
    public bool IsFlattened { get; set; }
    public bool IsSealed { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Sha256Fingerprint { get; set; }
    public string? CertificateId { get; set; }
}

