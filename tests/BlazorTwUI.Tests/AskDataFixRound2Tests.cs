using System.Text.Json.Nodes;
using BlazorTwUI.Models;
using BlazorTwUI.Services;
using Xunit;

namespace BlazorTwUI.Tests;

/// <summary>
/// Fix round 2: the second reviewer's 76-question run, kept as a permanent table (wrong answers AND correct ones), plus the
/// composer and query pieces the fixes touch. The names are the reviewer's (a tag "O'Brien" included).
/// </summary>
public class AskDataFixRound2Tests
{
    private static readonly Dictionary<string, IReadOnlyList<(string Id, string Name)>> Names = new()
    {
        ["category"] = [("1", "Tents"), ("2", "Packs")],
        ["brand"] = [("1", "Northpeak"), ("2", "Ridgeline Gear")],
        ["tag"] = [("1", "Ultralight"), ("2", "Waterproof"), ("3", "O'Brien")],
        ["review"] = [("1", "Solid shelter"), ("2", "Too heavy")],
    };

    private static DataQuestionParser Parser() => new(Summit.All, Names);

    private const string ReadOnly = "I only read your data - I cannot change it.";

    /// <summary>
    /// Expected outcome, compactly: <c>Intent|f=rel:Name,flag:true|s=search|asc|desc|take=N|field=x|grp=flag|grprel=rel</c>,
    /// or <c>Unknown|fragment of the message</c>.
    /// </summary>
    [Theory]
    // ---- the reviewer's wrong answers, now right
    [InlineData("products under 100", "Unknown|“under 100”")]
    [InlineData("products between 50 and 100", "Unknown|“between 50 and 100”")]
    [InlineData("products costing more than 200", "Unknown|“costing more than 200”")]
    [InlineData("products named tent under 100", "Unknown|“under 100”")]
    [InlineData("how many products with rating over 4", "Unknown|“over 4”")]
    [InlineData("how much do tents cost", "Unknown|average, total, lowest or highest price")]
    [InlineData("price of tents", "Unknown|average, total, lowest or highest price")]
    [InlineData("out of stock products", "Count|f=inStock:false")]
    [InlineData("show Northpeak products tagged waterproof", "Count|f=brand:Northpeak,tag:Waterproof")]
    [InlineData("find Northpeak tents", "Count|f=brand:Northpeak,category:Tents")]
    [InlineData("find Northpeak tent", "Find|f=brand:Northpeak|s=tent")]
    [InlineData("show products in stock tagged waterproof", "Count|f=tag:Waterproof,inStock:true")]
    [InlineData("show products tagged waterproof and ultralight", "Count|f=tag:Waterproof,tag:Ultralight")]
    [InlineData("show products tagged waterproof and fluffy", "Unknown|No tag matches “fluffy”")]
    [InlineData("show products tagged o'brien", "Count|f=tag:O'Brien")]
    [InlineData("show products tagged O'BRIEN", "Count|f=tag:O'Brien")]
    [InlineData("show me reviews with 5 stars", "Unknown|“5 stars”")]
    [InlineData("$filter=price gt 1", "Unknown|I could not use")]
    [InlineData("find t-shirt", "Find|s=t-shirt")]
    [InlineData("find o'brien jacket", "Find|s=o'brien jacket")]
    [InlineData("average price of products not in stock or tagged waterproof", "Unknown|combine conditions with “or”")]
    [InlineData("how many in stock and not in stock", "Breakdown|grp=inStock")]
    [InlineData("highest rated cheapest products", "Unknown|highest and lowest in one question")]
    [InlineData("which brand has the most products", "Unknown|“Products by brand”")]
    [InlineData("cheapest 2 brands", "Unknown|Brands have no price")]
    [InlineData("sum of price and rating", "Unknown|one number at a time")]
    [InlineData("average price of tents and packs", "Unknown|cannot be both")]
    [InlineData("delete all products", "Unknown|" + ReadOnly)]
    [InlineData("truncate products", "Unknown|" + ReadOnly)]
    [InlineData("remove the cheapest product", "Unknown|" + ReadOnly)]
    [InlineData("please delete the tents", "Unknown|" + ReadOnly)]
    [InlineData("can you update the price of tents", "Unknown|" + ReadOnly)]
    [InlineData("Add a product called Tent", "Unknown|" + ReadOnly)]
    // ---- answered correctly already: kept so they cannot regress
    [InlineData("how many products", "Count|f=")]
    [InlineData("show products", "Count|f=")]
    [InlineData("how many products are in stock", "Count|f=inStock:true")]
    [InlineData("how many products are out of stock", "Count|f=inStock:false")]
    [InlineData("how many waterproof products", "Count|f=tag:Waterproof")]
    [InlineData("average price of Northpeak products", "Aggregate|f=brand:Northpeak|field=price")]
    [InlineData("what is the cheapest tent", "Aggregate|f=category:Tents|field=price")]
    [InlineData("total rating of reviews", "Aggregate|field=rating")]
    [InlineData("top 5 products by price", "TopN|desc|take=5|field=price")]
    [InlineData("top 5 cheapest products", "TopN|asc|take=5|field=price")]
    [InlineData("least expensive product", "TopN|asc|take=1|field=price")]
    [InlineData("3 cheapest products", "TopN|asc|take=3|field=price")]
    [InlineData("most expensive product", "TopN|desc|take=1|field=price")]
    [InlineData("highest rated products", "TopN|desc|take=5|field=rating")]
    [InlineData("products by category", "Breakdown|grprel=category")]
    [InlineData("average price per brand", "Breakdown|grprel=brand|field=price")]
    [InlineData("in stock vs out of stock", "Breakdown|grp=inStock")]
    [InlineData("in stock or not", "Breakdown|grp=inStock")]
    [InlineData("find tent", "Find|s=tent")]
    [InlineData("show products named alpine down parka", "Find|s=alpine down parka")]
    [InlineData("how many products cost more than 100", "Unknown|“more than 100”")]
    [InlineData("top 5 products by brand", "Unknown|I could not use")]
    [InlineData("products by brand and category", "Unknown|“category”")]
    [InlineData("average rating", "Unknown|which records")]
    [InlineData("top 5 categories by price", "Unknown|price")]
    [InlineData("", "Unknown|Type a question")]
    [InlineData("\U0001F3D5️\U0001F525\U0001F44D", "Unknown|")]
    public void ReviewerTable(string question, string expected)
    {
        var p = Parser().Parse(question);
        var parts = expected.Split('|');
        Assert.Equal(parts[0], p.Intent.ToString());
        if (p.IsUnknown)
        {
            Assert.Contains(parts[1], p.Message);
            Assert.Null(p.Search);
            return;
        }

        foreach (var part in parts.Skip(1))
        {
            if (part.StartsWith("f=", StringComparison.Ordinal))
            {
                var want = part[2..].Split(',', StringSplitOptions.RemoveEmptyEntries).OrderBy(x => x, StringComparer.Ordinal).ToList();
                var got = p.Filters.Select(f => f.Relation is not null ? $"{f.Relation.Name}:{f.RecordName}" : $"{f.Field!.Name}:{(f.Flag ? "true" : "false")}")
                    .OrderBy(x => x, StringComparer.Ordinal).ToList();
                Assert.Equal(want, got);
            }
            else if (part.StartsWith("s=", StringComparison.Ordinal)) Assert.Equal(part[2..], p.Search);
            else if (part == "asc") Assert.False(p.Descending);
            else if (part == "desc") Assert.True(p.Descending);
            else if (part.StartsWith("take=", StringComparison.Ordinal)) Assert.Equal(int.Parse(part[5..]), p.Take);
            else if (part.StartsWith("field=", StringComparison.Ordinal)) Assert.Equal(part[6..], p.Field?.Name);
            else if (part.StartsWith("grp=", StringComparison.Ordinal)) Assert.Equal(part[4..], p.GroupByField?.Name);
            else if (part.StartsWith("grprel=", StringComparison.Ordinal)) Assert.Equal(part[7..], p.GroupByRelation?.Name);
            else throw new InvalidOperationException("Unknown expectation " + part);
        }

        if (p.Intent != AnswerIntent.Find) Assert.Null(p.Search);
    }

    [Fact]
    public void ReadOnly_Refusal_IsExact_AndOffersExamples()
    {
        var p = Parser().Parse("delete all products");
        Assert.Equal("I only read your data - I cannot change it. I can count, total, average, rank, group and find - for example:", p.Message);
        Assert.NotEmpty(p.Suggestions);
    }

    [Fact]
    public void ATagNamedWithAnApostrophe_IsFound_AndSearchKeepsPunctuation()
    {
        var tagged = Parser().Parse("show products tagged o'brien");
        Assert.Equal("3", Assert.Single(tagged.Filters).RecordId);

        var search = Parser().Parse("find o'brien");
        Assert.Equal(AnswerIntent.Count, search.Intent);   // the tag O'Brien is named exactly: a filter, not a search
        var text = new DataQuestionParser(Summit.All, new Dictionary<string, IReadOnlyList<(string Id, string Name)>>()).Parse("find o'brien");
        Assert.Equal("o'brien", text.Search);
        // the OData single quote is still doubled
        Assert.Contains("'o''brien'", ODataAnswerQuery.Filter(text, Summit.ProductMap));
    }

    [Fact]
    public void SeveralTags_AreAll_Required_AndSaidAsTagged()
    {
        var p = Parser().Parse("show products tagged waterproof and ultralight");
        Assert.Equal("(Tag/any(x: x/Id eq 2)) and (Tag/any(x: x/Id eq 1))", ODataAnswerQuery.Filter(p, Summit.ProductMap));
        var a = DataAnswerComposer.Compose(p, new AnswerResult([], 0));
        Assert.Contains("tagged Waterproof and Ultralight", a.Sentence);
    }

    [Fact]
    public void ASearch_KeepsItsFilters_AndSaysThem()
    {
        var p = Parser().Parse("find Northpeak tent");
        Assert.Contains("Brand/Id eq 1", ODataAnswerQuery.Filter(p, Summit.ProductMap));
        Assert.Contains("contains(tolower(Name),'tent')", ODataAnswerQuery.Filter(p, Summit.ProductMap));
        var rows = new List<JsonObject> { Summit.Row(1, "Ridge Tent", 400m, true, "Tents", "Northpeak") };
        var a = DataAnswerComposer.Compose(p, new AnswerResult(rows, 1));
        Assert.Contains("whose brand is Northpeak", a.Sentence);
        Assert.Contains("“tent”", a.Sentence);
    }
}
