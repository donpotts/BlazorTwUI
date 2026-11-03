namespace BlazorTwUI.Models;

using System;
using System.Collections.Generic;

/// <summary>
/// Operational mode for the Data Transfer Wizard.
/// </summary>
public enum DataTransferMode
{
    Import,
    Export
}

/// <summary>
/// Supported data interchange serialization formats.
/// </summary>
public enum DataTransferFormat
{
    Csv,
    Json,
    ExcelTsv,
    SqlInsert,
    Xml
}

/// <summary>
/// Ingestion strategy when applying imported records to an existing dataset.
/// </summary>
public enum DataImportStrategy
{
    Append,
    Replace,
    MergeByKey
}

/// <summary>
/// Mapping between incoming file/text column headers and target entity properties.
/// </summary>
public class DataColumnMapping
{
    public string SourceColumn { get; set; } = "";
    public string TargetProperty { get; set; } = "";
    public string DataType { get; set; } = "string"; // "string", "int", "decimal", "datetime", "bool"
    public bool IsMapped { get; set; } = true;
    public bool IsRequired { get; set; } = false;
    public string? DefaultValue { get; set; }
}

/// <summary>
/// Status of a validated row during the import staging preview.
/// </summary>
public enum ImportRowStatus
{
    Valid,
    Warning,
    Error
}

/// <summary>
/// Staged incoming record with validation feedback and field dictionary.
/// </summary>
public class StagedImportRow
{
    public int RowIndex { get; set; }
    public Dictionary<string, string?> RawValues { get; set; } = new();
    public ImportRowStatus Status { get; set; } = ImportRowStatus.Valid;
    public List<string> ValidationMessages { get; set; } = new();
}

/// <summary>
/// Result emitted when an import operation completes.
/// </summary>
public class DataImportResult<TItem>
{
    public bool Success => ErrorCount == 0;
    public int TotalProcessed { get; set; }
    public int ImportedCount { get; set; }
    public int SkippedCount { get; set; }
    public int ErrorCount { get; set; }
    public List<TItem> ImportedItems { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public DataImportStrategy Strategy { get; set; } = DataImportStrategy.Append;
}

/// <summary>
/// Payload emitted when an export file is generated.
/// </summary>
public class DataExportResult
{
    public DataTransferFormat Format { get; set; }
    public string FileName { get; set; } = "export.csv";
    public string ContentType { get; set; } = "text/csv";
    public string ContentText { get; set; } = "";
    public int RowCount { get; set; }
}
