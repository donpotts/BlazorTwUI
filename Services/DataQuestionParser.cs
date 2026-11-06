using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using BlazorTwUI.Models;

namespace BlazorTwUI.Services;

/// <summary>
/// Maps a typed question to ONE plan from a closed catalogue - count, total / average / lowest / highest of a number, top N by a
/// number, a breakdown by a related record or a yes/no field, or a search - over the app's own schema. There is no model: words
/// are matched against the labels people read (singular and plural, any case, a few synonyms such as cost for a price). A
/// question it cannot map is <see cref="AnswerIntent.Unknown"/> with questions built from THIS schema; it never guesses a
/// field that is not there.
/// </summary>
/// <remarks>Pure: no I/O. The names of related records (brands, tags...) can be passed in so "Northpeak products" can become
/// a filter; without them those words are simply not understood.</remarks>
public sealed partial class DataQuestionParser
{
    private readonly List<(string Key, EntitySchema Schema)> _entities;
    private readonly IReadOnlyDictionary<string, IReadOnlyList<(string Id, string Name)>> _names;

    /// <param name="entities">Every entity, keyed the way relations name their target (<see cref="EntityRelation.TargetEntity"/>), first = the main one.</param>
    /// <param name="names">Optional: the records of each related entity (id, display name), keyed the same way.</param>
    public DataQuestionParser(IEnumerable<KeyValuePair<string, EntitySchema>> entities,
        IReadOnlyDictionary<string, IReadOnlyList<(string Id, string Name)>>? names = null)
    {
        _entities = entities.Select(kv => (kv.Key, kv.Value)).ToList();
        _names = names ?? new Dictionary<string, IReadOnlyList<(string Id, string Name)>>();
    }

    private static readonly Dictionary<string, string> Synonyms = new(StringComparer.Ordinal)
    {
        ["cost"] = "price", ["costs"] = "price", ["priced"] = "price", ["expensive"] = "price", ["cheap"] = "price",
        ["cheapest"] = "price", ["cheaper"] = "price", ["pricey"] = "price",
        ["rated"] = "rating", ["stars"] = "rating", ["star"] = "rating", ["ratings"] = "rating",
    };

    private static readonly string[] DescWords = ["highest", "most", "largest", "biggest", "best", "greatest", "top", "priciest", "expensive"];
    private static readonly string[] AscWords = ["lowest", "least", "smallest", "cheapest", "fewest", "worst", "bottom", "cheap", "cheaper"];

    /// <summary>Words the parser understands on their own (the question's grammar), beyond entity, field and record names.</summary>
    private static readonly HashSet<string> Grammar =
    [
        "how", "many", "much", "count", "number", "average", "avg", "mean", "total", "sum", "minimum", "min", "maximum", "max",
        "top", "first", "vs", "versus", "compared", "or", "not", "out", "by", "per", "each", "grouped", "broken", "down", "split",
        "there", "do", "does", "we", "i", "you", "it", "its", "records", "rows", "value", "values", "be", "and", "was", "were", "they",
        "most", "least",
    ];

    /// <summary>Ordinary words never read as part of a record's name, even when one contains them ("Better than expected").</summary>
    private static readonly HashSet<string> CommonWords =
    [
        "than", "more", "less", "over", "under", "above", "below", "between", "after", "before", "since", "during", "last", "next",
        "this", "these", "those", "from", "into", "only", "just", "very", "every", "each", "other", "same", "new", "old", "good", "bad",
        "better", "worse", "big", "small", "high", "low", "worth", "sold", "made", "used", "like", "than", "then", "when", "who", "why",
        "week", "month", "year", "day", "today", "yesterday", "per", "percent",
    ];

    /// <summary>ONE answer to "which way": an ascending word (cheapest, least, lowest...) wins over top / most / best.</summary>
    private static bool Ascending(string q) => AscWords.Any(w => Has(q, w));
    private static readonly string[] GroupWords = [" broken down by ", " grouped by ", " split by ", " for each ", " by ", " per "];
    private static readonly HashSet<string> FindWords = ["show", "find", "search", "list", "which", "display", "look", "get", "give"];
    private static readonly HashSet<string> Filler =
    [
        "show", "find", "search", "list", "which", "display", "look", "get", "give", "me", "for", "all", "the", "a", "an", "any", "some",
        "tagged", "with", "named", "called", "containing", "matching", "match", "about", "that", "are", "is", "have", "has", "of",
        "in", "on", "please", "what", "whose", "where", "to", "up", "my", "our",
    ];

    // ------------------------------------------------------------------ public

    public AnswerPlan Parse(string? question)
    {
        var raw = (question ?? "").Trim();
        var plan = ParseCore(raw);
        if (plan.IsUnknown || plan.Intent == AnswerIntent.Find) return plan;
        return Leftovers(plan, " " + Norm(raw) + " ");
    }

    private AnswerPlan ParseCore(string raw)
    {
        var q = " " + Norm(raw) + " ";
        if (q.Trim().Length == 0) return Unknown(raw, "Type a question about your records.");
        if (_entities.Count == 0) return Unknown(raw, "There is nothing to ask about yet.");

        // ---- the "by ..." / "per ..." clause, and where it starts (entity names after it are the grouping, not the subject)
        var (groupAt, groupWordLen) = FindGroupClause(q);
        var head = groupAt >= 0 ? q[..groupAt] + " " : q;
        var tail = groupAt >= 0 ? " " + q[(groupAt + groupWordLen)..].Trim() + " " : "";

        // ---- the entity asked about
        var (entityKey, schema, singularMention) = MainEntity(head, q);
        var counting = Has(q, "how many") || Has(q, "count") || Has(q, "number of");
        var findish = FindWords.Contains(q.Trim().Split(' ')[0]);
        if (schema is null)
        {
            if (findish && SearchTerm(q, null).Length > 0)
            {
                (entityKey, schema) = _entities[0];
            }
            else
            {
                return Unknown(raw, "I could not tell which records you mean.");
            }
        }

        var numeric = schema.Fields.Where(IsNumeric).ToList();
        var fieldsMentioned = FieldMentions(head, schema);
        var numericMentioned = fieldsMentioned.Where(IsNumeric).ToList();
        var filters = Filters(head, schema);

        var plural = schema.Plural.ToLowerInvariant();
        var basePlan = new AnswerPlan { Question = raw, EntityKey = entityKey, Schema = schema, Filters = filters };

        var avg = HasAny(q, "average", "avg", "mean");
        var sum = HasAny(q, "total", "sum");
        var min = HasAny(q, "minimum", "min");
        var max = HasAny(q, "maximum", "max");
        var topMatch = TopN().Match(q);
        var statedN = StatedNumber(q);
        var desc = DescWords.Any(w => Has(q, w)) && !Has(q, "most recent");
        var asc = Ascending(q);
        var ambiguity = AmbiguousNumbers(head, schema);

        // ---- yes/no breakdown written as "in stock vs out of stock"
        if (groupAt < 0 && HasAny(q, "vs", "versus", "or not", "compared"))
        {
            var flag = schema.Fields.FirstOrDefault(f => f.Kind == FieldKind.Boolean && Forms(f.Label).Any(form => Has(q, form)));
            if (flag is not null)
                return basePlan with { Intent = AnswerIntent.Breakdown, GroupByField = flag, Filters = filters.Where(x => x.Field != flag).ToList() };
        }

        // ---- breakdown, or "top 5 by price"
        if (groupAt >= 0)
        {
            var target = tail.Trim();
            var relation = schema.Relations
                .Select(r => (r, len: RelationForms(r).Where(form => (" " + target + " ").StartsWith(" " + form + " ", StringComparison.Ordinal)).Select(f => f.Length).DefaultIfEmpty(0).Max()))
                .Where(x => x.len > 0).OrderByDescending(x => x.len).Select(x => x.r).FirstOrDefault();
            var field = schema.Fields
                .Select(f => (f, len: FieldForms(f).Where(form => (" " + target + " ").StartsWith(" " + form + " ", StringComparison.Ordinal)).Select(x => x.Length).DefaultIfEmpty(0).Max()))
                .Where(x => x.len > 0).OrderByDescending(x => x.len).Select(x => x.f).FirstOrDefault();

            if (field is not null && IsNumeric(field) && relation is null)
            {
                // "top 5 products by price" - a ranking, not a grouping
                return Rank(basePlan, field, topMatch, statedN, !asc, singularMention);
            }

            if (relation is null && field is null)
            {
                var asked = FirstWords(target, 3);
                var ranking = topMatch.Success || desc || asc;
                return Unknown(raw, $"{schema.Plural} have nothing called “{asked}” to {(ranking ? "rank" : "group")} by.", entityKey);
            }

            var measure = AggregateKind.Count;
            if ((sum || avg) && ambiguity is not null) return Unknown(raw, ambiguity, entityKey);
            var measureField = numericMentioned.FirstOrDefault();
            if (sum && measureField is not null) measure = AggregateKind.Sum;
            else if (avg && measureField is not null) measure = AggregateKind.Average;
            else if ((sum || avg) && measureField is null && !counting)
                return Unknown(raw, $"I could not find a number to {(avg ? "average" : "total")} for {plural}. {NumbersSentence(schema)}", entityKey);

            return basePlan with
            {
                Intent = AnswerIntent.Breakdown,
                GroupByRelation = relation,
                GroupByField = relation is null ? field : null,
                Aggregate = measure,
                Field = measure == AggregateKind.Count ? null : measureField,
            };
        }

        // ---- top N / cheapest / highest rated (a ranking of records)
        if (topMatch.Success || statedN is not null || ((desc || asc) && MentionsEntity(head, schema)))
        {
            if (ambiguity is not null) return Unknown(raw, ambiguity, entityKey);
            var field = numericMentioned.FirstOrDefault();
            if (field is null)
            {
                return Unknown(raw, numeric.Count == 0
                    ? $"{schema.Plural} have no numbers to rank by."
                    : $"I could not tell what to rank {plural} by. {NumbersSentence(schema, "rank them by")}", entityKey);
            }

            return Rank(basePlan, field, topMatch, statedN, !asc, singularMention);
        }

        // ---- a single figure over a number
        if (avg || min || max || (sum && numericMentioned.Count > 0) || ((desc || asc) && numericMentioned.Count > 0))
        {
            if (ambiguity is not null) return Unknown(raw, ambiguity, entityKey);
            var field = numericMentioned.FirstOrDefault();
            if (field is null)
            {
                var other = fieldsMentioned.FirstOrDefault();
                return Unknown(raw, other is not null
                    ? $"{other.Label} is not a number, so it cannot be {(avg ? "averaged" : "added up")}. {NumbersSentence(schema)}"
                    : $"I could not find that number for {plural}. {NumbersSentence(schema)}", entityKey);
            }

            var kind = avg ? AggregateKind.Average : min ? AggregateKind.Min : max ? AggregateKind.Max
                : (desc || asc) && !sum ? (asc ? AggregateKind.Min : AggregateKind.Max) : AggregateKind.Sum;
            return basePlan with
            {
                Intent = AnswerIntent.Aggregate, Aggregate = kind, Field = field,
                Descending = kind != AggregateKind.Min, Take = kind is AggregateKind.Min or AggregateKind.Max ? 1 : 10,
            };
        }

        // ---- a count ("how many", "total products"); "total colour of products" names a number that is not there
        if (sum && !counting && WordAfter(q, "total", "sum") is { } totalOf && !IsCountWord(totalOf, schema))
        {
            return Unknown(raw, $"{schema.Plural} have no number called “{totalOf}”. {NumbersSentence(schema)}", entityKey);
        }

        if (counting || sum)
        {
            return basePlan with { Intent = AnswerIntent.Count };
        }

        // ---- a search ("show products tagged ultralight", "find tent")
        var term = SearchTerm(q, schema);
        if (term.Length > 0)
        {
            if (!findish && filters.Count == 0 && fieldsMentioned.Count == 0 && !MentionsEntity(head, schema))
                return Unknown(raw, "I could not tell what you want to know.", entityKey);
            return basePlan with { Intent = AnswerIntent.Find, Search = term, Filters = Array.Empty<PlanFilter>() };
        }

        // "show products", "Northpeak products": list them, with the count
        if (findish || MentionsEntity(head, schema)) return basePlan with { Intent = AnswerIntent.Count };
        return Unknown(raw, "I could not tell what you want to know.", entityKey);
    }

    /// <summary>
    /// Four to six questions this assistant CAN answer, made from the real entity and field labels (and, when known, a real
    /// related record's name). Every one of them parses to a plan.
    /// </summary>
    public IReadOnlyList<string> Suggestions()
    {
        var list = new List<string>();
        if (_entities.Count == 0) return list;

        var (_, main) = _entities.FirstOrDefault(e => e.Schema.Fields.Any(IsNumeric) || e.Schema.Relations.Count > 0);
        main ??= _entities[0].Schema;
        var plural = main.Plural.ToLowerInvariant();
        var number = main.Fields.FirstOrDefault(IsNumeric);
        var one = main.Relations.FirstOrDefault(r => r.Kind == RelationKind.One) ?? main.Relations.FirstOrDefault();
        var flag = main.Fields.FirstOrDefault(f => f.Kind == FieldKind.Boolean);

        list.Add($"How many {plural}?");
        if (number is not null) list.Add($"Average {number.Label.ToLowerInvariant()} of {plural}");
        if (number is not null) list.Add($"Top 5 {plural} by {number.Label.ToLowerInvariant()}");
        if (one is not null) list.Add($"{main.Plural} by {Singular(one.Label.ToLowerInvariant())}");
        if (flag is not null) list.Add($"How many {plural} are {flag.Label.ToLowerInvariant()}?");

        // A real related name: "Average price of Northpeak products"
        // Prefer a to-one relation other than the one the breakdown suggestion already shows (a brand rather than a category).
        var named = main.Relations
            .Where(r => r.Kind == RelationKind.One)
            .OrderBy(r => r == one ? 1 : 0)
            .Select(r => (r, names: _names.TryGetValue(r.TargetEntity, out var n) ? n : null))
            .FirstOrDefault(x => x.names is { Count: > 0 });
        var example = named.names?.Select(n => n.Name).FirstOrDefault(n => Norm(n).Length >= 3);
        if (example is not null)
            list.Add(number is not null ? $"Average {number.Label.ToLowerInvariant()} of {example} {plural}" : $"How many {example} {plural}?");

        foreach (var (_, other) in _entities.Where(e => e.Schema != main))
        {
            if (list.Count >= 6) break;
            list.Add($"How many {other.Plural.ToLowerInvariant()}?");
        }

        return list.Take(6).ToList();
    }

    // ------------------------------------------------------------------ pieces

    private AnswerPlan Rank(AnswerPlan basePlan, EntityField field, Match topMatch, int? statedN, bool descending, bool singular)
    {
        var take = topMatch.Success ? Math.Clamp(int.Parse(topMatch.Groups["n"].Value, CultureInfo.InvariantCulture), 1, 50)
            : statedN is { } n ? Math.Clamp(n, 1, 50)
            : singular ? 1 : 5;
        return basePlan with { Intent = AnswerIntent.TopN, Field = field, Descending = descending, Take = take };
    }

    private AnswerPlan Unknown(string question, string why, string? entityKey = null) => new()
    {
        Question = question,
        Intent = AnswerIntent.Unknown,
        EntityKey = entityKey ?? "",
        Schema = entityKey is null ? null : _entities.FirstOrDefault(e => e.Key == entityKey).Schema,
        Message = why + " I can answer questions like these:",
        Suggestions = Suggestions(),
    };

    private (string Key, EntitySchema? Schema, bool Singular) MainEntity(string head, string whole)
    {
        foreach (var text in new[] { head, null, whole })
        {
            if (text is null)
            {
                // No entity named before "by": a field only one entity has ("average price by brand") decides it.
                var inHead = FieldOwner(head);
                if (inHead.Schema is not null) return (inHead.Key, inHead.Schema, false);
                continue;
            }

            var best = _entities
                .SelectMany(e => new[] { (e, form: Norm(e.Schema.Plural), singular: false), (e, form: Norm(e.Schema.Entity), singular: true) }
                    .Concat(Forms(e.Schema.Entity).Select(f => (e, form: f, singular: f == Norm(e.Schema.Entity)))))
                .Select(x => (x.e, x.form, x.singular, at: text.IndexOf(" " + x.form + " ", StringComparison.Ordinal)))
                .Where(x => x.at >= 0 && x.form.Length > 0)
                .OrderBy(x => x.at).ThenByDescending(x => x.form.Length)
                .FirstOrDefault();
            if (best.form is not null) return (best.e.Key, best.e.Schema, best.singular && best.form != Norm(best.e.Schema.Plural));
        }

        // No entity named: a field only one entity has ("average price") decides it.
        var owner = FieldOwner(whole);
        return owner.Schema is not null ? (owner.Key, owner.Schema, false) : ("", null, false);
    }

    private (string Key, EntitySchema? Schema) FieldOwner(string text)
    {
        var byField = _entities.Where(e => FieldMentions(text, e.Schema).Count > 0).ToList();
        if (byField.Count > 1) byField = byField.Where(e => FieldMentions(text, e.Schema).Any(IsNumeric)).ToList();
        return byField.Count == 1 ? (byField[0].Key, byField[0].Schema) : ("", null);
    }

    private static bool MentionsEntity(string text, EntitySchema schema) =>
        Forms(schema.Entity).Concat(Forms(schema.Plural)).Any(f => Has(text, f));

    /// <summary>Fields of <paramref name="schema"/> named in <paramref name="text"/> (by label, singular/plural, or a synonym for a number).</summary>
    private static List<EntityField> FieldMentions(string text, EntitySchema schema) =>
        FieldHits(text, schema).Select(h => h.Field).ToList();

    /// <summary>Every field named in the text, strongest first: an exact label (or its singular/plural) beats a synonym, then the longer match.</summary>
    private static List<(EntityField Field, int Strength, int Length)> FieldHits(string text, EntitySchema schema)
    {
        var hits = new List<(EntityField Field, int Strength, int Length)>();
        foreach (var f in schema.Fields)
        {
            var exact = Forms(f.Label).Where(form => Has(text, form)).Select(form => form.Length).DefaultIfEmpty(0).Max();
            var synonym = FieldForms(f).Except(Forms(f.Label)).Where(form => Has(text, form)).Select(form => form.Length).DefaultIfEmpty(0).Max();
            if (exact > 0) hits.Add((f, 2, exact));
            else if (synonym > 0) hits.Add((f, 1, synonym));
        }

        return hits.OrderByDescending(h => h.Strength).ThenByDescending(h => h.Length).ToList();
    }

    /// <summary>"Did you mean Price or Unit price?" when the two strongest numbers named are named equally strongly.</summary>
    private static string? AmbiguousNumbers(string text, EntitySchema schema)
    {
        var top = FieldHits(text, schema).Where(h => IsNumeric(h.Field)).Take(2).ToList();
        return top.Count == 2 && top[0].Strength == top[1].Strength && top[0].Length == top[1].Length
            ? $"Did you mean {top[0].Field.Label} or {top[1].Field.Label}?"
            : null;
    }

    /// <summary>"3 cheapest products": a number right before a ranking word (or before the records named).</summary>
    private int? StatedNumber(string q)
    {
        var words = q.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var rankWords = DescWords.Concat(AscWords).ToHashSet();
        for (var i = 0; i + 1 < words.Length; i++)
        {
            if (!int.TryParse(words[i], NumberStyles.None, CultureInfo.InvariantCulture, out var n) || n < 1) continue;
            if (rankWords.Contains(words[i + 1])) return n;
        }

        return null;
    }

    // ------------------------------------------------------------------ nothing is dropped silently

    /// <summary>
    /// Every word of the question must have been used: the grammar, the entity, the fields, the related names and the number
    /// in a "top 5". A word left over is either a related record named in part ("tents" for the category "Tents &amp; Shelter",
    /// used only when exactly one record matches) or the question is Unknown and says which words it could not use.
    /// </summary>
    private AnswerPlan Leftovers(AnswerPlan plan, string q)
    {
        var schema = plan.Schema!;
        var known = new HashSet<string>(Filler);
        known.UnionWith(Grammar);
        known.UnionWith(DescWords);
        known.UnionWith(AscWords);
        void Add(IEnumerable<string> forms) { foreach (var f in forms) known.UnionWith(f.Split(' ', StringSplitOptions.RemoveEmptyEntries)); }
        Add(Forms(schema.Entity).Concat(Forms(schema.Plural)));
        foreach (var f in schema.Fields) Add(FieldForms(f));
        if (plan.GroupByRelation is { } g) Add(RelationForms(g));
        foreach (var f in plan.Filters)
        {
            if (f.Relation is { } r) { Add(RelationForms(r)); Add([Norm(f.RecordName)]); }
        }

        if (plan.Intent == AnswerIntent.TopN) known.Add(plan.Take.ToString(CultureInfo.InvariantCulture));

        var words = q.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var unknown = Enumerable.Range(0, words.Length).Where(i => !known.Contains(words[i])).ToList();
        if (unknown.Count == 0) return plan;

        // A related record named in part: only when exactly ONE record of the entity's links has that word.
        var filters = plan.Filters.ToList();
        var unresolved = new List<int>();
        foreach (var i in unknown)
        {
            var w = words[i];
            // Only to-one links (a category, a brand: short names), and never an ordinary English word: review titles are
            // sentences, and "than" in "more than 100" once matched the review "Better than expected".
            var matches = w.Length < 3 || CommonWords.Contains(w) ? new List<(EntityRelation r, string Id, string Name)>() : schema.Relations
                .Where(r => r != plan.GroupByRelation && r.Kind == RelationKind.One)
                .SelectMany(r => (_names.TryGetValue(r.TargetEntity, out var n) ? n : Array.Empty<(string Id, string Name)>()).Select(x => (r, x.Id, x.Name)))
                .Where(x => Norm(x.Name).Split(' ').Any(part => part == w || Singular(part) == Singular(w)))
                .ToList();
            if (matches.Count == 1 && !filters.Any(f => f.Relation == matches[0].r))
            {
                filters.Add(new PlanFilter { Relation = matches[0].r, RecordId = matches[0].Id, RecordName = matches[0].Name });
            }
            else if (matches.Count > 1)
            {
                var names = matches.Select(m => m.Name).Distinct().ToList();
                return Unknown(plan.Question, names.Count == 1
                    ? $"“{w}” matches {matches.Count} records named {names[0]}, so I cannot tell which one."
                    : $"“{w}” could mean {string.Join(", ", names.Take(names.Count - 1))} or {names[^1]}. Use the full name.", plan.EntityKey);
            }
            else
            {
                unresolved.Add(i);
            }
        }

        if (unresolved.Count == 0) return plan with { Filters = filters };

        var first = unresolved.Min();
        var last = unresolved.Max();
        // "5 stars": a number is read with the word after it.
        if (int.TryParse(words[last], out _) && last + 1 < words.Length) last++;
        var phrase = string.Join(' ', words[first..(last + 1)]);
        return Unknown(plan.Question,
            $"I could not use “{phrase}”. I can count, total, average, rank and group {schema.Plural.ToLowerInvariant()}, filtered by a related name or a yes/no field.",
            plan.EntityKey);
    }

    private static IEnumerable<string> FieldForms(EntityField f)
    {
        var label = Norm(f.Label);
        foreach (var form in Forms(f.Label)) yield return form;
        if (!IsNumeric(f)) yield break;
        foreach (var (word, means) in Synonyms)
            if (label == means || label.EndsWith(" " + means, StringComparison.Ordinal)) yield return word;
    }

    private static IEnumerable<string> RelationForms(EntityRelation r) => Forms(r.Label).Concat(Forms(r.Name)).Concat(Forms(r.TargetEntity)).Distinct();

    private List<PlanFilter> Filters(string text, EntitySchema schema)
    {
        var filters = new List<PlanFilter>();
        var taken = new List<string>();

        // Related records named in the question: "Northpeak products" -> brand is Northpeak (longest names first).
        var candidates = schema.Relations
            .SelectMany(r => (_names.TryGetValue(r.TargetEntity, out var n) ? n : Array.Empty<(string Id, string Name)>())
                .Select(x => (r, x.Id, x.Name, form: Norm(x.Name))))
            .Where(x => x.form.Length >= 3 && x.form.Split(' ').Any(w => !Filler.Contains(w)))
            .Where(x => !Forms(schema.Entity).Concat(Forms(schema.Plural)).Contains(x.form))
            .OrderByDescending(x => x.form.Length);
        foreach (var c in candidates)
        {
            if (!Has(text, c.form) || taken.Any(t => t.Contains(c.form, StringComparison.Ordinal))) continue;
            if (filters.Any(f => f.Relation == c.r)) continue;
            filters.Add(new PlanFilter { Relation = c.r, RecordId = c.Id, RecordName = c.Name });
            taken.Add(c.form);
        }

        // Yes/no fields: "in stock" -> true; "not in stock", "out of stock" -> false.
        foreach (var f in schema.Fields.Where(f => f.Kind == FieldKind.Boolean))
        {
            var label = Norm(f.Label);
            var negative = new List<string> { "not " + label, "non " + label };
            if (label.StartsWith("in ", StringComparison.Ordinal)) negative.Add("out of " + label[3..]);
            if (label.StartsWith("is ", StringComparison.Ordinal)) negative.Add("is not " + label[3..]);
            if (negative.Any(n => Has(text, n))) filters.Add(new PlanFilter { Field = f, Flag = false });
            else if (Forms(f.Label).Any(form => Has(text, form))) filters.Add(new PlanFilter { Field = f, Flag = true });
        }

        return filters;
    }

    private static (int At, int Length) FindGroupClause(string q)
    {
        foreach (var w in GroupWords)
        {
            var at = q.IndexOf(w, StringComparison.Ordinal);
            if (at >= 0) return (at, w.Length);
        }

        return (-1, 0);
    }

    private static string SearchTerm(string q, EntitySchema? schema)
    {
        var skip = new HashSet<string>(Filler);
        if (schema is not null)
        {
            foreach (var form in Forms(schema.Entity).Concat(Forms(schema.Plural)))
                foreach (var w in form.Split(' ')) skip.Add(w);
        }

        var words = q.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Where(w => !skip.Contains(w)).ToList();
        return string.Join(' ', words);
    }

    private static string NumbersSentence(EntitySchema schema, string verb = "ask about")
    {
        var numbers = schema.Fields.Where(IsNumeric).Select(f => f.Label).ToList();
        return numbers.Count == 0
            ? $"{schema.Plural} have no numbers to work with."
            : $"Numbers I can {verb} for {schema.Plural.ToLowerInvariant()}: {string.Join(", ", numbers)}.";
    }

    /// <summary>The first word after one of <paramref name="keys"/>, skipping "the" and "of".</summary>
    private static string? WordAfter(string q, params string[] keys)
    {
        var words = q.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        for (var i = 0; i < words.Length; i++)
        {
            if (!keys.Contains(words[i])) continue;
            var next = words.Skip(i + 1).FirstOrDefault(w => w is not ("the" or "of" or "all" or "my" or "our"));
            return next;
        }

        return null;
    }

    private static bool IsCountWord(string word, EntitySchema schema) =>
        word is "number" or "count" or "amount" or "records" or "rows"
        || Forms(schema.Entity).Concat(Forms(schema.Plural)).Any(f => f.Split(' ')[0] == word);

    private static string FirstWords(string text, int n) => string.Join(' ', text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(n));

    internal static bool IsNumeric(EntityField f) => f.Kind is FieldKind.Number or FieldKind.Decimal;

    private static bool Has(string paddedText, string phrase) =>
        phrase.Length > 0 && (" " + paddedText.Trim() + " ").Contains(" " + phrase + " ", StringComparison.Ordinal);

    private static bool HasAny(string q, params string[] phrases) => phrases.Any(p => Has(q, p));

    /// <summary>Lower case, letters and digits only, single spaces.</summary>
    internal static string Norm(string? text)
    {
        var sb = new StringBuilder();
        foreach (var c in (text ?? "").ToLowerInvariant().Normalize(NormalizationForm.FormKD))
        {
            if (char.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark) continue;
            sb.Append(char.IsLetterOrDigit(c) ? c : ' ');
        }

        return Spaces().Replace(sb.ToString(), " ").Trim();
    }

    /// <summary>A label and its singular and plural forms (on the last word), normalized.</summary>
    internal static IEnumerable<string> Forms(string label)
    {
        var n = Norm(label);
        if (n.Length == 0) yield break;
        yield return n;
        var s = Singular(n);
        if (s != n) yield return s;
        var p = Plural(n);
        if (p != n) yield return p;
    }

    internal static string Singular(string phrase)
    {
        var i = phrase.LastIndexOf(' ');
        var (head, w) = i < 0 ? ("", phrase) : (phrase[..(i + 1)], phrase[(i + 1)..]);
        string s = w;
        if (w.EndsWith("ies", StringComparison.Ordinal) && w.Length > 3) s = w[..^3] + "y";
        else if (w.EndsWith("sses", StringComparison.Ordinal) || w.EndsWith("xes", StringComparison.Ordinal) || w.EndsWith("ches", StringComparison.Ordinal) || w.EndsWith("shes", StringComparison.Ordinal)) s = w[..^2];
        else if (w.EndsWith('s') && !w.EndsWith("ss", StringComparison.Ordinal) && w.Length > 2) s = w[..^1];
        return head + s;
    }

    internal static string Plural(string phrase)
    {
        var i = phrase.LastIndexOf(' ');
        var (head, w) = i < 0 ? ("", phrase) : (phrase[..(i + 1)], phrase[(i + 1)..]);
        if (w.EndsWith('s')) return phrase;
        string p = w.EndsWith('y') && w.Length > 1 && !"aeiou".Contains(w[^2]) ? w[..^1] + "ies"
            : w.EndsWith("x", StringComparison.Ordinal) || w.EndsWith("ch", StringComparison.Ordinal) || w.EndsWith("sh", StringComparison.Ordinal) ? w + "es"
            : w + "s";
        return head + p;
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex Spaces();

    [GeneratedRegex(@" (?:top|first|bottom) (?<n>\d{1,3}) ")]
    private static partial Regex TopN();
}
