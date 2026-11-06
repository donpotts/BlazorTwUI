using BlazorTwUI.Models;
using BlazorTwUI.Services;
using Xunit;

namespace BlazorTwUI.Tests;

/// <summary>
/// The round-2 question list, every one kept as a permanent regression (the ones answered correctly too). Summit Outfitters
/// schema, with a tag named O'Brien. Expected outcome, compactly:
/// <c>Intent|ent=key|f=rel:Name,flag:true|s=search|asc|desc|take=N|field=x|grp=flag|grprel=rel</c>, or <c>Unknown|fragment</c>.
/// An Unknown never carries a search, so nothing is fetched for it.
/// </summary>
public class AskDataQuestionListTests
{
    private static readonly Dictionary<string, IReadOnlyList<(string Id, string Name)>> Names = new()
    {
        ["category"] = [("1", "Tents"), ("2", "Packs")],
        ["brand"] = [("1", "Northpeak"), ("2", "Ridgeline Gear")],
        ["tag"] = [("1", "Ultralight"), ("2", "Waterproof"), ("3", "O'Brien")],
        ["review"] = [("1", "Solid shelter"), ("2", "Too heavy")],
    };

    private const string ReadOnly = "I only read your data - I cannot change it.";

    [Theory]
    [InlineData("how many products", "Count|ent=product|f=")]
    [InlineData("how many product", "Count|ent=product|f=")]
    [InlineData("HOW MANY PRODUCTS?", "Count|ent=product|f=")]
    [InlineData("count of products", "Count|ent=product|f=")]
    [InlineData("number of tags", "Count|ent=tag|f=")]
    [InlineData("how many categories", "Count|ent=category|f=")]
    [InlineData("total price of products", "Aggregate|ent=product|field=price")]
    [InlineData("sum of price", "Aggregate|ent=product|field=price")]
    [InlineData("average rating of products", "Aggregate|ent=product|field=rating")]
    [InlineData("average price of Northpeak products", "Aggregate|f=brand:Northpeak|field=price")]
    [InlineData("average price of brands", "Unknown|Brands have no price")]
    [InlineData("highest priced product", "TopN|desc|take=1|field=price")]
    [InlineData("lowest rated products", "TopN|asc|field=rating")]
    [InlineData("top 3 products by rating", "TopN|desc|take=3|field=rating")]
    [InlineData("top 5 cheapest products", "TopN|asc|take=5|field=price")]
    [InlineData("bottom 3 by price", "TopN|asc|take=3|field=price")]
    [InlineData("3 most expensive", "TopN|desc|take=3|field=price")]
    [InlineData("cheapest 2 brands", "Unknown|Brands have no price")]
    [InlineData("top 3 categories", "Unknown|Categories have no numbers to rank. I can rank Products by price or rating")]
    [InlineData("products by brand", "Breakdown|grprel=brand")]
    [InlineData("products per category", "Breakdown|grprel=category")]
    [InlineData("count products by tag", "Breakdown|grprel=tag")]
    [InlineData("in stock vs out of stock", "Breakdown|grp=inStock")]
    [InlineData("how many products are in stock", "Count|f=inStock:true")]
    [InlineData("how many are out of stock", "Count|f=inStock:false")]
    [InlineData("how many products are not in stock", "Count|f=inStock:false")]
    [InlineData("show products tagged waterproof", "Count|f=tag:Waterproof")]
    [InlineData("show products tagged waterproof and ultralight", "Count|f=tag:Ultralight,tag:Waterproof")]
    [InlineData("find tent", "Find|ent=product|s=tent|f=")]
    [InlineData("show me reviews with 5 stars", "Unknown|“5 stars”")]
    [InlineData("reviews by product", "Breakdown|ent=review|grprel=product")]
    [InlineData("which brand has the most products", "Unknown|“Products by brand”")]
    [InlineData("what is the most popular tag", "Unknown|")]
    [InlineData("products under 100", "Unknown|I could not use “under 100”. I can count, total, average, rank and group")]
    [InlineData("products between 50 and 100", "Unknown|“between 50 and 100”")]
    [InlineData("products costing more than 200", "Unknown|“costing more than 200”")]
    [InlineData("average price in tents", "Aggregate|f=category:Tents|field=price")]
    [InlineData("total price by category", "Breakdown|grprel=category|field=price")]
    [InlineData("how much do tents cost", "Unknown|average, total, lowest or highest price")]
    [InlineData("list all categories", "Count|ent=category|f=")]
    [InlineData("delete all products", "Unknown|" + ReadOnly)]
    [InlineData("make everything free", "Unknown|" + ReadOnly)]
    [InlineData("'; DROP TABLE x;--", "Unknown|" + ReadOnly)]
    [InlineData("$filter=price gt 1", "Unknown|I could not use")]
    [InlineData("", "Unknown|Type a question")]
    [InlineData("products products products", "Count|ent=product|f=")]
    [InlineData("how many products and how many tags", "Unknown|one question at a time")]
    [InlineData("how many products and tags in stock", "Unknown|one question at a time")]
    [InlineData("update the price of tents to 5", "Unknown|" + ReadOnly)]
    [InlineData("out of stock products", "Count|f=inStock:false")]
    [InlineData("highest rated cheapest products", "Unknown|I cannot rank highest and lowest in one question")]
    [InlineData("show Northpeak products tagged waterproof", "Count|f=brand:Northpeak,tag:Waterproof")]
    [InlineData("find Northpeak tents", "Count|f=brand:Northpeak,category:Tents")]
    [InlineData("show products in stock tagged waterproof", "Count|f=inStock:true,tag:Waterproof")]
    [InlineData("price of tents", "Unknown|average, total, lowest or highest price")]
    [InlineData("find t-shirt", "Find|s=t-shirt|f=")]
    [InlineData("show products tagged o'brien", "Count|f=tag:O'Brien")]
    // the rules behind them, said once more in other words
    [InlineData("show products tagged waterproof and fluffy", "Unknown|No tag matches “fluffy”")]
    [InlineData("products rated at least 4", "Unknown|“at least 4”")]
    [InlineData("tents cheaper than 300", "Unknown|“cheaper than 300”")]
    [InlineData("find Northpeak tent", "Find|f=brand:Northpeak|s=tent")]
    [InlineData("find tents that are not in stock", "Count|f=category:Tents,inStock:false")]
    [InlineData("tents in stock or tagged waterproof", "Unknown|I cannot combine conditions with “or” yet")]
    [InlineData("how many in stock and out of stock", "Breakdown|grp=inStock")]
    [InlineData("best cheapest products", "Unknown|I cannot rank highest and lowest in one question")]
    [InlineData("least expensive product", "TopN|asc|take=1|field=price")]
    [InlineData("remove the cheapest product", "Unknown|" + ReadOnly)]
    [InlineData("set every price to 0", "Unknown|" + ReadOnly)]
    [InlineData("rename the tents category", "Unknown|" + ReadOnly)]
    [InlineData("find O'Brien-Smith jacket", "Find|s=o'brien-smith jacket|f=")]
    [InlineData("find jacket tagged waterproof", "Find|s=jacket|f=tag:Waterproof")]
    [InlineData("Northpeak gizmo products", "Unknown|I could not use “gizmo”")]
    public void QuestionList(string question, string expected)
    {
        var p = new DataQuestionParser(Summit.All, Names).Parse(question);
        var parts = expected.Split('|');
        Assert.True(parts[0] == p.Intent.ToString(), $"{question}: expected {parts[0]}, got {p.Intent} ({p.Message})");
        if (p.IsUnknown)
        {
            Assert.Contains(parts[1], p.Message);
            Assert.Null(p.Search);
            Assert.NotEmpty(p.Suggestions);
            return;
        }

        foreach (var part in parts.Skip(1))
        {
            if (part.StartsWith("ent=", StringComparison.Ordinal)) Assert.Equal(part[4..], p.EntityKey);
            else if (part.StartsWith("f=", StringComparison.Ordinal))
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
    public void ASearchWithNoMatches_StillSaysItsFilters()
    {
        var p = new DataQuestionParser(Summit.All, Names).Parse("find Northpeak tent");
        var a = DataAnswerComposer.Compose(p, new AnswerResult([], 0));
        Assert.Contains("No products whose brand is Northpeak match", a.Sentence);
    }

    [Fact]
    public void AComparison_IsRefused_WithSchemaExamples_AndNoRequest()
    {
        var p = new DataQuestionParser(Summit.All, Names).Parse("products under 100");
        Assert.StartsWith("I could not use “under 100”. I can count, total, average, rank and group", p.Message);
        Assert.Contains(p.Suggestions, s => s.Contains("price", StringComparison.OrdinalIgnoreCase));
        Assert.Equal("", ODataAnswerQuery.Filter(p, Summit.ProductMap));
    }
}
