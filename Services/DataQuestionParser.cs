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
        var raw = new string((question ?? "").Where(c => !char.IsControl(c)).ToArray()).Trim();
        var q = " " + Norm(raw) + " ";
        var words = q.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var (guessKey, guess, _) = MainEntity(q, q);
        var guessPlural = guess?.Plural.ToLowerInvariant() ?? "records";

        // It only reads. A question that asks it to change something gets a plain refusal and nothing is fetched.
        if (WriteVerb(words) is not null)
        {
            return Unknown(raw, "I only read your data - I cannot change it. I can count, total, average, rank, group and find - for example:",
                guess is null ? null : guessKey, appendExamples: false);
        }

        // "under 100", "between 50 and 100", "more than 200": there are no number conditions yet - never a text search of them.
        if (ComparisonPhrase(words) is { } comparison) return CouldNotUse(raw, comparison, guess is null ? null : guessKey, guessPlural);

        // "not in stock or tagged waterproof": conditions are only ever combined with AND, so "or" is refused, not ANDed.
        var or = Array.IndexOf(words, "or");
        if (or >= 0 && (or + 1 >= words.Length || words[or + 1] != "not"))
            return Unknown(raw, "I cannot combine conditions with \u201cor\u201d yet. Ask about each one separately.", guess is null ? null : guessKey);

        // "how many products and how many tags": two subjects joined by "and" are two questions; answering one would drop the other.
        if (TwoSubjects(q) is { } pair)
            return Unknown(raw, $"I answer one question at a time. Ask about {pair.First} first, then {pair.Second}.", guess is null ? null : guessKey);

        // "highest rated cheapest": two directions in one question.
        if (ExplicitDescending(words) && Ascending(q))
            return Unknown(raw, "I cannot rank highest and lowest in one question. Ask for one of them.", guess is null ? null : guessKey);

        var plan = ParseCore(raw);
        if (plan.IsUnknown) return plan;
        // A search is checked too: every word must be the search text, a filter, or understood - nothing is dropped silently.
        return Leftovers(plan, q);
    }

    /// <summary>
    /// Two different entities named as subjects before any "by" clause with "and" between them ("products and tags").
    /// A name right after "tagged" / "with" / "named" is a filter or text, not a subject.
    /// </summary>
    private (string First, string Second)? TwoSubjects(string q)
    {
        var (groupAt, _) = FindGroupClause(q);
        var head = groupAt >= 0 ? q[..groupAt] + " " : q;
        var words = head.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var hits = new List<(int At, EntitySchema Schema)>();
        for (var i = 0; i < words.Length; i++)
        {
            if (i > 0 && words[i - 1] is "tagged" or "with" or "named" or "called" or "containing" or "matching") continue;
            var e = _entities.FirstOrDefault(x => Forms(x.Schema.Entity).Concat(Forms(x.Schema.Plural)).Contains(words[i]));
            if (e.Schema is not null && !hits.Any(h => h.Schema == e.Schema)) hits.Add((i, e.Schema));
        }

        if (hits.Count < 2) return null;
        var between = words[(hits[0].At + 1)..hits[1].At];
        if (!between.Contains("and")) return null;
        return (hits[0].Schema.Plural, hits[1].Schema.Plural);
    }

    private static readonly HashSet<string> WriteVerbs =
        ["delete", "remove", "update", "add", "set", "make", "drop", "truncate", "change", "create", "insert", "edit", "rename", "erase", "clear", "modify", "replace"];

    private static readonly HashSet<string> Politeness = ["please", "can", "could", "would", "will", "you", "i", "want", "to", "lets", "let", "us", "go", "ahead", "and", "now", "kindly"];

    /// <summary>The write verb a question opens with ("delete all products", "please remove the cheapest product"), if any.</summary>
    private static string? WriteVerb(string[] words)
    {
        foreach (var w in words)
        {
            if (Politeness.Contains(w)) continue;
            return WriteVerbs.Contains(w) ? w : null;
        }

        return null;
    }

    private static readonly string[] ComparisonStarts = ["under", "over", "between", "above", "below", "costing", "less", "more", "fewer", "greater", "at", "within", "exceeding", "cheaper", "pricier"];

    /// <summary>"under 100", "between 50 and 100", "more than 200", "at least 4": a comparison word followed by a number.</summary>
    private static string? ComparisonPhrase(string[] words)
    {
        for (var i = 0; i < words.Length; i++)
        {
            if (!ComparisonStarts.Contains(words[i])) continue;
            if (words[i] == "at" && (i + 1 >= words.Length || words[i + 1] is not ("least" or "most"))) continue;
            if (words[i] is "less" or "more" or "fewer" or "greater" or "cheaper" or "pricier" && (i + 1 >= words.Length || words[i + 1] != "than")) continue;
            // the number must follow within a few words ("between 50 and 100", "more than 200")
            var last = -1;
            for (var j = i + 1; j < Math.Min(words.Length, i + 5); j++)
                if (words[j].Any(char.IsDigit)) last = j;
            if (last < 0) continue;
            if (words[i] == "between" && last + 2 < words.Length && words[last + 1] == "and" && words[last + 2].Any(char.IsDigit)) last += 2;
            var from = i > 0 && words[i - 1] is "worth" or "valued" ? i - 1 : i;
            return string.Join(' ', words[from..(last + 1)]);
        }

        return null;
    }

    private static readonly HashSet<string> ExplicitDescWords = ["highest", "most", "best", "largest", "biggest", "greatest", "priciest"];

    /// <summary>An explicitly descending word. "top" is only a count marker, and "expensive" alone is a field word ("least expensive").</summary>
    private static bool ExplicitDescending(string[] words) => words.Any(ExplicitDescWords.Contains);

    private AnswerPlan CouldNotUse(string question, string phrase, string? entityKey, string plural) =>
        Unknown(question, $"I could not use \u201c{phrase}\u201d. I can count, total, average, rank and group {plural}, filtered by a related name or a yes/no field.", entityKey);

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
        var (filters, oneConflict, bothFlag) = Filters(head, schema);
        if (oneConflict is not null) return Unknown(raw, oneConflict, entityKey);

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

        // ---- "how many in stock and not in stock": both sides of a yes/no field are a breakdown on it, never one half
        if (groupAt < 0 && bothFlag is not null)
            return basePlan with { Intent = AnswerIntent.Breakdown, GroupByField = bothFlag, Filters = filters.Where(x => x.Field != bothFlag).ToList() };

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
            if (field is null) return Unknown(raw, CannotRank(head, q, schema), entityKey);

            return Rank(basePlan, field, topMatch, statedN, !asc, singularMention);
        }

        // ---- a single figure over a number
        if (avg || min || max || (sum && numericMentioned.Count > 0) || ((desc || asc) && numericMentioned.Count > 0))
        {
            if (ambiguity is not null) return Unknown(raw, ambiguity, entityKey);
            var numberHits = FieldHits(head, schema).Where(h => IsNumeric(h.Field)).ToList();
            if (numberHits.Count > 1 && numberHits[1].Strength == numberHits[0].Strength)
                return Unknown(raw, $"I work with one number at a time: {numberHits[0].Field.Label} or {numberHits[1].Field.Label}?", entityKey);
            var field = numericMentioned.FirstOrDefault();
            if (field is null)
            {
                if (NumberElsewhere(head, schema) is { } missing) return Unknown(raw, missing, entityKey);
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

        // ---- a listing or a search. Filters already matched are KEPT ("show Northpeak products tagged waterproof");
        // "tagged A and B" needs every name to be a real tag; what is left must be a plain name or text, or it is not searched.
        var (tagged, taggedError) = TaggedClause(raw, schema);
        if (taggedError is not null) return Unknown(raw, taggedError, entityKey);
        foreach (var t in tagged)
            if (!filters.Any(f => f.Relation == t.Relation && f.RecordId == t.RecordId)) filters.Add(t);
        basePlan = basePlan with { Filters = filters };

        var residual = Residual(raw, schema, filters);
        if (residual.Count > 0 && filters.Any(f => f.Relation?.Kind == RelationKind.Many && !tagged.Contains(f)))
        {
            // "find o'brien jacket": a tag name next to other text is part of the text, not a tag ("tagged o'brien" is the tag)
            filters = filters.Where(f => f.Relation?.Kind != RelationKind.Many || tagged.Contains(f)).ToList();
            basePlan = basePlan with { Filters = filters };
            residual = Residual(raw, schema, filters);
        }

        if (residual.Count == 0)
        {
            // "show products", "Northpeak products", "out of stock products": list them, with the count
            if (findish || MentionsEntity(head, schema) || filters.Count > 0) return basePlan with { Intent = AnswerIntent.Count };
            return Unknown(raw, "I could not tell what you want to know.", entityKey);
        }

        var normResidual = residual.SelectMany(r => Norm(r).Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToList();
        if (normResidual.Any(w => w.Any(char.IsDigit)))
            return CouldNotUse(raw, string.Join(' ', residual), entityKey, plural);   // "with 5 stars": no number conditions yet

        var askedField = schema.Fields.Where(IsNumeric).FirstOrDefault(f => FieldForms(f).Any(form => normResidual.Contains(form)));
        if (askedField is not null)
        {
            // "price of tents", "how much do tents cost": which figure is wanted is not said - ask, never search the words
            var label = askedField.Label.ToLowerInvariant();
            return Unknown(raw, $"Do you want the average, total, lowest or highest {label}? For example \u201caverage {label} of {plural}\u201d.", entityKey);
        }

        if (!findish && !Markers.Any(m => Has(q, m)))
            return CouldNotUse(raw, string.Join(' ', residual), entityKey, plural);

        return basePlan with { Intent = AnswerIntent.Find, Search = string.Join(' ', residual) };
    }

    /// <summary>Words that introduce a name or text to look for.</summary>
    private static readonly string[] Markers = ["named", "called", "containing", "matching", "tagged", "with", "like", "about"];

    /// <summary>
    /// "tagged A and B": each name must be a real record of the entity's tag-like link (compared on its real label, any case,
    /// punctuation kept: "o'brien" finds "O'Brien"). A name that is not there is said plainly, never searched as text.
    /// </summary>
    private (List<PlanFilter> Filters, string? Error) TaggedClause(string raw, EntitySchema schema)
    {
        var result = new List<PlanFilter>();
        var lower = raw.ToLowerInvariant();
        var at = System.Text.RegularExpressions.Regex.Match(lower, @"\btagged\s+(?<rest>.+)$");
        var tagRel = schema.Relations.FirstOrDefault(r => Forms(r.Label).Concat(Forms(r.Name)).Any(f => f is "tag" or "tags"));
        if (!at.Success || tagRel is null) return (result, null);
        var tags = _names.TryGetValue(tagRel.TargetEntity, out var n) ? n : Array.Empty<(string Id, string Name)>();
        foreach (var part0 in System.Text.RegularExpressions.Regex.Split(at.Groups["rest"].Value, @"\s*(?:,|\band\b)\s*"))
        {
            var part = part0.Trim().Trim('?', '!', '.', '"', '\u201c', '\u201d', ' ');
            if (part.Length == 0) continue;
            var match = tags.Where(t => part.StartsWith(t.Name.ToLowerInvariant(), StringComparison.Ordinal)
                                        && (part.Length == t.Name.Length || !char.IsLetterOrDigit(part[t.Name.Length])))
                            .OrderByDescending(t => t.Name.Length).FirstOrDefault();
            if (match.Name is null)
                return (result, $"No {Singular(tagRel.Label.ToLowerInvariant())} matches \u201c{part}\u201d.");
            result.Add(new PlanFilter { Relation = tagRel, RecordId = match.Id, RecordName = match.Name });
        }

        return (result, null);
    }

    /// <summary>
    /// The words of the question nobody has claimed: not grammar or filler, not a marker, not the entity, not a matched name,
    /// not a yes/no field, not a field. Taken from the RAW text so "t-shirt" and "o'brien" are searched as typed.
    /// </summary>
    private static List<string> Residual(string raw, EntitySchema schema, List<PlanFilter> filters)
    {
        var known = new HashSet<string>(Filler);
        known.UnionWith(Grammar);
        known.UnionWith(Markers);
        known.UnionWith(FindWords);
        void Add(IEnumerable<string> forms) { foreach (var f in forms) known.UnionWith(f.Split(' ', StringSplitOptions.RemoveEmptyEntries)); }
        Add(Forms(schema.Entity).Concat(Forms(schema.Plural)));
        foreach (var f in filters)
        {
            if (f.Relation is { } r) Add([Norm(f.RecordName)]);
            if (f.Field is { } b) { Add(Forms(b.Label)); known.UnionWith(["not", "out", "of", "non"]); }
        }

        var result = new List<string>();
        var operand = false;   // after "named" / "called" / "containing" / "matching" the rest is the text, as typed
        foreach (var token in raw.ToLowerInvariant().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
        {
            var t = token.Trim('?', '!', '.', ',', ';', ':', '"', '(', ')', '[', ']', '\u201c', '\u201d');
            var parts = Norm(t).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) continue;
            if (operand) { result.Add(t); continue; }
            if (parts is ["named" or "called" or "containing" or "matching"]) { operand = true; continue; }
            if (parts.All(known.Contains)) continue;
            result.Add(t);
        }

        return result;
    }

    /// <summary>"Brands have no price. I can rank Products by price or rating." when the number named belongs to another entity.</summary>
    private string? NumberElsewhere(string head, EntitySchema schema)
    {
        if (FieldHits(head, schema).Any(h => IsNumeric(h.Field))) return null;
        var wanted = _entities.Where(e => e.Schema != schema).SelectMany(e => FieldHits(head, e.Schema)).Where(h => IsNumeric(h.Field)).Select(h => h.Field.Label.ToLowerInvariant()).FirstOrDefault();
        return wanted is null ? null : $"{schema.Plural} have no {wanted}.{RankableElsewhere(schema)}";
    }

    private string RankableElsewhere(EntitySchema schema)
    {
        var others = _entities.Where(e => e.Schema != schema && e.Schema.Fields.Any(IsNumeric))
            .Select(e => $"{e.Schema.Plural} by {string.Join(" or ", e.Schema.Fields.Where(IsNumeric).Select(f => f.Label.ToLowerInvariant()))}").ToList();
        return others.Count == 0 ? "" : " I can rank " + string.Join("; ", others) + ".";
    }

    /// <summary>Why a ranking has no number, and what CAN be ranked.</summary>
    private string CannotRank(string head, string q, EntitySchema schema)
    {
        // "which brand has the most products": that is a count per brand, not a number a brand has.
        var counted = schema.Relations.FirstOrDefault(r => r.Kind == RelationKind.Many && RelationForms(r).Any(f => Has(q, f)));
        if (counted is not null)
            return $"I cannot rank {schema.Plural.ToLowerInvariant()} by how many {counted.Label.ToLowerInvariant()} they have yet. Try \u201c{counted.Label} by {schema.Entity.ToLowerInvariant()}\u201d.";

        // "cheapest 2 brands": cheap means a price, and brands have none
        if (NumberElsewhere(head, schema) is { } missing) return missing;
        if (!schema.Fields.Any(IsNumeric)) return $"{schema.Plural} have no numbers to rank.{RankableElsewhere(schema)}";
        return $"I could not tell what to rank {schema.Plural.ToLowerInvariant()} by. {NumbersSentence(schema, "rank them by")}";
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

    private AnswerPlan Unknown(string question, string why, string? entityKey = null, bool appendExamples = true) => new()
    {
        Question = question,
        Intent = AnswerIntent.Unknown,
        EntityKey = entityKey ?? "",
        Schema = entityKey is null ? null : _entities.FirstOrDefault(e => e.Key == entityKey).Schema,
        Message = appendExamples ? why + " I can answer questions like these:" : why,
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
        var byField = _entities.Where(e => FieldMentions(text, e.Schema).Count > 0 || NegatedFlag(text, e.Schema)).ToList();
        if (byField.Count > 1) byField = byField.Where(e => FieldMentions(text, e.Schema).Any(IsNumeric)).ToList();
        if (byField.Count > 1)
        {
            // "sum of price and rating": only products have both
            var most = byField.Max(e => FieldMentions(text, e.Schema).Count);
            byField = byField.Where(e => FieldMentions(text, e.Schema).Count == most).ToList();
        }

        return byField.Count == 1 ? (byField[0].Key, byField[0].Schema) : ("", null);
    }

    /// <summary>"out of stock", "not in stock": a yes/no field said the negative way still names the entity that has it.</summary>
    private static bool NegatedFlag(string text, EntitySchema schema) =>
        schema.Fields.Where(f => f.Kind == FieldKind.Boolean).Select(f => Norm(f.Label))
            .Any(l => Has(text, "not " + l) || (l.StartsWith("in ", StringComparison.Ordinal) && Has(text, "out of " + l[3..])));

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
        if (plan.Search is { Length: > 0 } search) { Add([Norm(search)]); known.UnionWith(Markers); known.UnionWith(FindWords); }
        foreach (var f in plan.Filters.Where(f => f.Field is not null)) { Add(Forms(f.Field!.Label)); known.UnionWith(["not", "out", "of", "non"]); }

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

    private (List<PlanFilter> Filters, string? OneConflict, EntityField? BothFlag) Filters(string text, EntitySchema schema)
    {
        var filters = new List<PlanFilter>();
        var taken = new List<string>();
        string? conflict = null;
        EntityField? both = null;

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
            if (c.r.Kind == RelationKind.One && filters.FirstOrDefault(f => f.Relation == c.r) is { } other)
            {
                // A product has ONE category: "tents and packs" can never both hold, and picking one would be a guess.
                conflict ??= $"A {schema.Entity.ToLowerInvariant()} has one {c.r.Label.ToLowerInvariant()}, so it cannot be both {other.RecordName} and {c.Name}. Ask about one at a time.";
                continue;
            }

            if (filters.Any(f => f.Relation == c.r && f.RecordId == c.Id)) continue;
            filters.Add(new PlanFilter { Relation = c.r, RecordId = c.Id, RecordName = c.Name });   // several tags: ALL of them
            taken.Add(c.form);
        }

        // Yes/no fields: "in stock" -> true; "not in stock", "out of stock" -> false.
        foreach (var f in schema.Fields.Where(f => f.Kind == FieldKind.Boolean))
        {
            var label = Norm(f.Label);
            var negative = new List<string> { "not " + label, "non " + label };
            if (label.StartsWith("in ", StringComparison.Ordinal)) negative.Add("out of " + label[3..]);
            if (label.StartsWith("is ", StringComparison.Ordinal)) negative.Add("is not " + label[3..]);
            var negatives = negative.Sum(n => Occurrences(text, n));
            var positives = Forms(f.Label).Max(form => Occurrences(text, form)) - negative.Where(n => n.EndsWith(label, StringComparison.Ordinal)).Sum(n => Occurrences(text, n));
            if (negatives > 0 && positives > 0) both ??= f;
            else if (negatives > 0) filters.Add(new PlanFilter { Field = f, Flag = false });
            else if (positives > 0) filters.Add(new PlanFilter { Field = f, Flag = true });
        }

        filters = filters.OrderBy(f => f.Relation is null ? int.MaxValue : (" " + text.Trim() + " ").IndexOf(" " + Norm(f.RecordName) + " ", StringComparison.Ordinal)).ToList();
        return (filters, conflict, both);
    }

    private static int Occurrences(string paddedText, string phrase)
    {
        var text = " " + paddedText.Trim() + " ";
        var needle = " " + phrase + " ";
        var n = 0;
        for (var at = text.IndexOf(needle, StringComparison.Ordinal); at >= 0; at = text.IndexOf(needle, at + 1, StringComparison.Ordinal)) n++;
        return n;
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
