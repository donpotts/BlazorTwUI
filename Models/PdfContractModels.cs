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
    public List<PdfContractSection> Sections { get; set; } = new();
}

public sealed class PdfContractDocument
{
    public string DocumentId { get; set; } = $"DOC-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
    public string Title { get; set; } = "Master Services Agreement & Assignment";
    public string ContractType { get; set; } = "Enterprise Service Agreement";
    public DateTime EffectiveDate { get; set; } = DateTime.UtcNow.Date;
    public string Status { get; set; } = "Pending Signatures";
    public bool IsReadOnly { get; set; } = false;
    public bool IsSealed { get; set; } = false;
    public bool IsFlattened { get; set; } = false;
    public DateTime? FlattenedAtUtc { get; set; }
    public string FlattenedHash { get; set; } = string.Empty;
    public string FlattenedContentStreamId { get; set; } = string.Empty;
    public string Sha256Fingerprint { get; set; } = string.Empty;
    public DateTime? SealedAtUtc { get; set; }
    public string SignerIp { get; set; } = "192.168.1.104 (Authenticated Enclave)";
    public string CertificateId { get; set; } = string.Empty;
    public List<PdfContractPage> Pages { get; set; } = new();
    public List<PdfContractField> Fields { get; set; } = new();
}
