using System.Text.Json.Nodes;
using BlazorTwUI.Models;
using BlazorTwUI.Services;
using Xunit;

namespace BlazorTwUI.Tests;

/// <summary>Fix round 1: wrong answers found by tracing the parser by hand. Each test was seen failing before its fix.</summary>
public class AskDataFixRound1Tests
{
    private static readonly Dictionary<string, IReadOnlyList<(string Id, string Name)>> RealNames = new()
    {
        ["category"] = [("1", "Tents & Shelter"), ("2", "Packs & Bags"), ("3", "Day Packs")],
        ["brand"] = [("1", "Northpeak"), ("2", "Ridgeline Gear")],
        ["tag"] = [("1", "Ultralight"), ("2", "Waterproof")],
        // Review titles are sentences, and two share a title: found by the browser run, where "than" in
        // "cost more than 100" was read as part of the review "Better than expected".
        ["review"] = [("1", "Better than expected"), ("2", "Better than expected"), ("3", "Worth every penny")],
    };

    [Theory]
    [InlineData("how many products cost more than 100", "more than 100")]
    [InlineData("how many products worth more than 100", "worth more than 100")]
    public void CommonWords_AreNeverReadAsPartOfARecordName(string q, string words)
    {
        var p = Real().Parse(q);
        Assert.Equal(AnswerIntent.Unknown, p.Intent);
        Assert.Contains($"I could not use “{words}”", p.Message);
    }

    [Fact]
    public void SeveralRecordsWithTheSameName_AreSaidPlainly()
    {
        var names = new Dictionary<string, IReadOnlyList<(string Id, string Name)>> { ["category"] = [("1", "Tents & Shelter"), ("2", "Tents & Shelter")] };
        var p = new DataQuestionParser(Summit.All, names).Parse("average price of tents");
        Assert.Equal(AnswerIntent.Unknown, p.Intent);
        Assert.Contains("2 records named Tents & Shelter", p.Message);
        Assert.DoesNotContain("could mean  or", p.Message);
    }

    private static DataQuestionParser Real() => new(Summit.All, RealNames);

    // ---- 1. ranking direction: an ascending word wins over top/most/best; 5. a stated number is used
    [Theory]
    [InlineData("top 5 cheapest products", false, 5, "price")]
    [InlineData("least expensive products", false, 5, "price")]
    [InlineData("top 3 lowest rated products", false, 3, "rating")]
    [InlineData("most expensive product", true, 1, "price")]
    [InlineData("cheapest product", false, 1, "price")]
    [InlineData("3 cheapest products", false, 3, "price")]
    [InlineData("top 5 products by price", true, 5, "price")]
    [InlineData("highest rated products", true, 5, "rating")]
    public void RankingDirection_AndCount(string q, bool descending, int take, string field)
    {
        var p = Summit.Parser().Parse(q);
        Assert.Equal(AnswerIntent.TopN, p.Intent);
        Assert.Equal(descending, p.Descending);
        Assert.Equal(take, p.Take);
        Assert.Equal(field, p.Field!.Name);
    }

    [Theory]
    [InlineData("what is the lowest price", AggregateKind.Min)]
    [InlineData("what is the least expensive price", AggregateKind.Min)]
    [InlineData("what is the highest price", AggregateKind.Max)]
    public void SingleFigure_UsesTheSameDirection(string q, AggregateKind kind)
    {
        var p = Summit.Parser().Parse(q);
        Assert.Equal(AnswerIntent.Aggregate, p.Intent);
        Assert.Equal(kind, p.Aggregate);
    }

    // ---- 2. words it cannot use are never dropped silently
    [Theory]
    [InlineData("how many products cost more than 100", "more than 100")]
    [InlineData("how many reviews have 5 stars", "5 stars")]
    [InlineData("average price of products sold last week", "sold last week")]
    [InlineData("top 5 products by price in europe", "europe")]
    public void LeftoverWords_AreUnknown_NamingWhatWasNotUnderstood(string q, string words)
    {
        var p = Real().Parse(q);
        Assert.Equal(AnswerIntent.Unknown, p.Intent);
        Assert.Contains($"I could not use “{words}”", p.Message);
        Assert.NotEmpty(p.Suggestions);
    }

    [Fact]
    public void APartialRelatedName_ThatMatchesExactlyOneRecord_IsUsed_AndEchoed()
    {
        var p = Real().Parse("average price of tents");
        Assert.Equal(AnswerIntent.Aggregate, p.Intent);
        var f = Assert.Single(p.Filters);
        Assert.Equal("category", f.Relation!.Name);
        Assert.Equal("Tents & Shelter", f.RecordName);
        var rows = new List<JsonObject> { Summit.Row(1, "Ridge Tent", 400m, true, "Tents & Shelter", "Northpeak") };
        Assert.Contains("Tents & Shelter", DataAnswerComposer.Compose(p, new AnswerResult(rows, 1)).Sentence);
    }

    [Fact]
    public void APartialRelatedName_ThatMatchesSeveral_IsUnknown_ListingThem()
    {
        var p = Real().Parse("average price of packs");
        Assert.Equal(AnswerIntent.Unknown, p.Intent);
        Assert.Contains("Packs & Bags", p.Message);
        Assert.Contains("Day Packs", p.Message);
    }

    [Theory]
    [InlineData("how many products")]
    [InlineData("How many products are in stock?")]
    [InlineData("how many products are out of stock")]
    [InlineData("average price of Northpeak products")]
    [InlineData("Average price of products")]
    [InlineData("what is the highest price")]
    [InlineData("total products")]
    [InlineData("total number of products")]
    [InlineData("products by category")]
    [InlineData("Products by brand")]
    [InlineData("in stock vs out of stock")]
    [InlineData("average price by brand")]
    [InlineData("How many Northpeak products?")]
    [InlineData("How many Ridgeline Gear products?")]
    [InlineData("show products tagged ultralight")]
    [InlineData("find tent")]
    [InlineData("How many categories?")]
    [InlineData("Average price of Northpeak products")]
    public void EverythingThatWorkedBefore_StillWorks(string q) => Assert.NotEqual(AnswerIntent.Unknown, Real().Parse(q).Intent);

    // ---- 3. an exact field name outranks a synonym; a tie asks
    private static EntitySchema Items(params EntityField[] numbers) =>
        new("Item", "Items", [new("name", "Name", FieldKind.Text, true), .. numbers], []);

    [Fact]
    public void AnExactFieldName_OutranksASynonym()
    {
        var a = new DataQuestionParser([new("item", Items(new("price", "Price", FieldKind.Decimal, false), new("cost", "Cost", FieldKind.Decimal, false)))]).Parse("average cost of items");
        Assert.Equal("cost", a.Field!.Name);
        var b = new DataQuestionParser([new("item", Items(new("cost", "Cost", FieldKind.Decimal, false), new("price", "Price", FieldKind.Decimal, false)))]).Parse("average cost of items");
        Assert.Equal("cost", b.Field!.Name);
    }

    [Fact]
    public void TwoFieldsMatchingEquallyWell_AskWhichOneIsMeant()
    {
        var schema = Items(new("price", "Price", FieldKind.Decimal, false), new("unitPrice", "Unit price", FieldKind.Decimal, false));
        var p = new DataQuestionParser([new("item", schema)]).Parse("average cost of items");
        Assert.Equal(AnswerIntent.Unknown, p.Intent);
        Assert.Contains("Did you mean Price or Unit price?", p.Message);
    }

    [Fact]
    public void ASynonym_WithOnlyOneCandidate_IsUsed_AndTheAnswerNamesTheRealField()
    {
        var p = new DataQuestionParser([new("item", Items(new EntityField("price", "Price", FieldKind.Decimal, false)))]).Parse("average cost of items");
        Assert.Equal("price", p.Field!.Name);
        var rows = new List<JsonObject> { new() { ["name"] = "A", ["price"] = 10m } };
        Assert.Contains("average price", DataAnswerComposer.Compose(p, new AnswerResult(rows, 1)).Sentence);
    }

    // ---- 4. a group with no values has no average (not 0)
    [Fact]
    public void BreakdownAverage_AGroupWithNoValues_SaysNoValue()
    {
        var rows = new List<JsonObject>
        {
            Summit.Row(1, "Ridge Tent", 400m, true, "Tents", "Northpeak"),
            Summit.Row(2, "Day Pack", null, true, "Packs", "Northpeak"),
        };
        var p = Summit.Parser().Parse("average price by category");
        Assert.Equal(AnswerIntent.Breakdown, p.Intent);
        var a = DataAnswerComposer.Compose(p, new AnswerResult(rows, 2));
        Assert.Contains(a.Table, r => r[0] == "Packs" && r[1] == "no value");
        Assert.DoesNotContain(a.Table, r => r[1] == "0" || r[1] == "0.00");
        Assert.Contains("Tents with 400.00", a.Sentence);
    }
}
