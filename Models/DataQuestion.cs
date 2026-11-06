using System.Text.Json.Nodes;

namespace BlazorTwUI.Models;

// The closed catalogue behind TwAskData ("Ask your data"): a typed question becomes ONE of these plans, the plan is run
// against whatever API the app has (an executor the app passes in), and the answer is composed from the rows that came
// back. No model and no key: what cannot be mapped is answered honestly as Unknown with questions that CAN be answered.

/// <summary>The kinds of question the assistant answers. Anything else is <see cref="Unknown"/>.</summary>
public enum AnswerIntent { Unknown, Count, Aggregate, TopN, Breakdown, Find }

/// <summary>What is computed over a numeric field (or, for a breakdown, over each group).</summary>
public enum AggregateKind { Count, Sum, Average, Min, Max }

/// <summary>
/// A condition on the rows. <see cref="Relation"/> + <see cref="RecordId"/>: linked to that related record (a brand, a tag);
/// <see cref="Field"/> + <see cref="Flag"/>: a yes/no field is true or false. <see cref="Describe"/> is the plain-words form.
/// </summary>
public sealed record PlanFilter
{
    public EntityRelation? Relation { get; init; }
    public string? RecordId { get; init; }
    public string? RecordName { get; init; }
    public EntityField? Field { get; init; }
    public bool Flag { get; init; }

    /// <summary>"whose brand is Northpeak", "that are in stock", "that are not in stock".</summary>
    public string Describe() => Relation is not null
        ? Relation.Kind == RelationKind.One
            ? $"whose {Relation.Label.ToLowerInvariant()} is {RecordName}"
            : $"with {RecordName} among their {Relation.Label.ToLowerInvariant()}"
        : Field is not null
            ? $"that are {(Flag ? "" : "not ")}{Field.Label.ToLowerInvariant()}"
            : "";
}

/// <summary>
/// What to fetch and compute for one question. Built by <see cref="Services.DataQuestionParser"/> (no I/O); run by the app's
/// executor; turned into words by <see cref="Services.DataAnswerComposer"/>.
/// </summary>
public sealed record AnswerPlan
{
    public required string Question { get; init; }
    public AnswerIntent Intent { get; init; }
    /// <summary>The entity key (the same key relations use as <see cref="EntityRelation.TargetEntity"/>).</summary>
    public string EntityKey { get; init; } = "";
    public EntitySchema? Schema { get; init; }
    public AggregateKind Aggregate { get; init; } = AggregateKind.Count;
    /// <summary>The numeric field aggregated, ordered by, or summed per group.</summary>
    public EntityField? Field { get; init; }
    public IReadOnlyList<PlanFilter> Filters { get; init; } = Array.Empty<PlanFilter>();
    /// <summary>For <see cref="AnswerIntent.Find"/>: the text searched for (the grid's own server search).</summary>
    public string? Search { get; init; }
    /// <summary>Breakdown by a related entity (one group per related record).</summary>
    public EntityRelation? GroupByRelation { get; init; }
    /// <summary>Breakdown by a yes/no or text field.</summary>
    public EntityField? GroupByField { get; init; }
    public bool Descending { get; init; } = true;
    /// <summary>How many rows to show (top N, or the sample under a count or a search).</summary>
    public int Take { get; init; } = 10;
    /// <summary>For <see cref="AnswerIntent.Unknown"/>: why, in plain words.</summary>
    public string? Message { get; init; }
    /// <summary>Questions that can be answered, built from this schema.</summary>
    public IReadOnlyList<string> Suggestions { get; init; } = Array.Empty<string>();

    public bool IsUnknown => Intent == AnswerIntent.Unknown;
}

/// <summary>
/// What the executor brought back. <see cref="Rows"/> are the records read (at most the cap); <see cref="TotalMatched"/> is how
/// many records matched on the server. Nothing else is trusted: every figure shown is computed from these two.
/// </summary>
public sealed record AnswerResult(IReadOnlyList<JsonObject> Rows, int TotalMatched)
{
    /// <summary>The URLs or descriptions of the requests made (shown to nobody; kept for tests and diagnostics).</summary>
    public IReadOnlyList<string> Requests { get; init; } = Array.Empty<string>();
}

/// <summary>An answer ready to show: one sentence, the rows behind it, and how it was worked out.</summary>
public sealed record DataAnswer
{
    public required string Sentence { get; init; }
    /// <summary>The figure in the sentence, as shown ("40", "212.50"), or null when there is none.</summary>
    public string? Figure { get; init; }
    public IReadOnlyList<string> Columns { get; init; } = Array.Empty<string>();
    public IReadOnlyList<IReadOnlyList<string>> Table { get; init; } = Array.Empty<IReadOnlyList<string>>();
    /// <summary>"Counted 40 products; sorted by price, highest first; showing the top 5".</summary>
    public string How { get; init; } = "";
    public bool IsUnknown { get; init; }
    public IReadOnlyList<string> Suggestions { get; init; } = Array.Empty<string>();
}
