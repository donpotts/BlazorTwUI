using System.Text.Json.Nodes;
using BlazorTwUI.Components.UI;
using BlazorTwUI.Models;
using BlazorTwUI.Services;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BlazorTwUI.Tests;

/// <summary>The Summit Outfitters sample (template 138 with sample 67), as the generated client describes it.</summary>
internal static class Summit
{
    public static readonly EntitySchema Product = new("Product", "Products",
        [
            new("name", "Name", FieldKind.Text, false),
            new("sku", "Sku", FieldKind.Text, false),
            new("price", "Price", FieldKind.Decimal, false),
            new("inStock", "In stock", FieldKind.Boolean, false),
            new("rating", "Rating", FieldKind.Decimal, false),
            new("description", "Description", FieldKind.LongText, false),
        ],
        [
            new("category", "Category", RelationKind.One, "category") { IdField = "categoryId" },
            new("brand", "Brand", RelationKind.One, "brand") { IdField = "brandId" },
            new("review", "Reviews", RelationKind.Many, "review") { IdField = "reviewIds" },
            new("tag", "Tags", RelationKind.Many, "tag") { IdField = "tagIds" },
        ]);

    public static readonly EntitySchema Category = new("Category", "Categories",
        [new("name", "Name", FieldKind.Text, false), new("description", "Description", FieldKind.LongText, false)],
        [new("product", "Products", RelationKind.Many, "product") { IdField = "productIds" }]);

    public static readonly EntitySchema Brand = new("Brand", "Brands",
        [new("name", "Name", FieldKind.Text, false), new("country", "Country", FieldKind.Text, false), new("founded", "Founded", FieldKind.Number, false)],
        [new("product", "Products", RelationKind.Many, "product") { IdField = "productIds" }]);

    public static readonly EntitySchema Review = new("Review", "Reviews",
        [new("title", "Title", FieldKind.Text, false), new("reviewer", "Reviewer", FieldKind.Text, false), new("rating", "Rating", FieldKind.Number, false)],
        [new("product", "Product", RelationKind.One, "product") { IdField = "productId" }]);

    public static readonly EntitySchema Tag = new("Tag", "Tags",
        [new("name", "Name", FieldKind.Text, false)],
        [new("product", "Products", RelationKind.Many, "product") { IdField = "productIds" }]);

    public static readonly IReadOnlyList<KeyValuePair<string, EntitySchema>> All =
    [
        new("product", Product), new("category", Category), new("brand", Brand), new("review", Review), new("tag", Tag),
    ];

    public static readonly Dictionary<string, IReadOnlyList<(string Id, string Name)>> Names = new()
    {
        ["category"] = [("1", "Tents"), ("2", "Packs")],
        ["brand"] = [("1", "Northpeak"), ("2", "Ridgeline Gear")],
        ["tag"] = [("1", "Ultralight"), ("2", "Waterproof")],
        ["review"] = [("1", "Solid shelter"), ("2", "Too heavy")],
    };

    public static DataQuestionParser Parser(bool names = true) => new(All, names ? Names : null);

    public static ODataEntityMap ProductMap = new("product", "Id", "Category,Brand,Review,Tag",
        "contains(tolower(Name),'{q}') or Tag/any(x: contains(tolower(x/Name),'{q}'))",
        new Dictionary<string, string> { ["name"] = "Name", ["sku"] = "Sku", ["price"] = "Price", ["inStock"] = "InStock", ["rating"] = "Rating", ["description"] = "Description" },
        new Dictionary<string, ODataRelationMap>
        {
            ["category"] = new("Category", "Id"), ["brand"] = new("Brand", "Id"), ["review"] = new("Review", "Id"), ["tag"] = new("Tag", "Id"),
        });

    public static JsonObject Row(int id, string name, decimal? price, bool inStock, string category, string brand, params string[] tags) => new()
    {
        ["id"] = id, ["name"] = name, ["price"] = price is null ? null : JsonValue.Create(price.Value), ["inStock"] = inStock,
        ["category"] = new JsonObject { ["id"] = category.Length, ["name"] = category },
        ["brand"] = new JsonObject { ["id"] = brand.Length, ["name"] = brand },
        ["tag"] = new JsonArray(tags.Select(t => (JsonNode)new JsonObject { ["id"] = t.Length, ["name"] = t }).ToArray()),
    };
}

public class DataQuestionParserTests
{
    [Fact]
    public void HowManyProducts_IsACount()
    {
        var p = Summit.Parser().Parse("how many products");
        Assert.Equal(AnswerIntent.Count, p.Intent);
        Assert.Equal("product", p.EntityKey);
        Assert.Empty(p.Filters);
    }

    [Fact]
    public void AverageOfANamedBrand_FiltersByThatBrand()
    {
        var p = Summit.Parser().Parse("average price of Northpeak products");
        Assert.Equal(AnswerIntent.Aggregate, p.Intent);
        Assert.Equal(AggregateKind.Average, p.Aggregate);
        Assert.Equal("price", p.Field!.Name);
        var f = Assert.Single(p.Filters);
        Assert.Equal("brand", f.Relation!.Name);
        Assert.Equal("1", f.RecordId);
    }

    [Fact]
    public void Top5ByPrice_IsARankingHighestFirst()
    {
        var p = Summit.Parser().Parse("top 5 products by price");
        Assert.Equal(AnswerIntent.TopN, p.Intent);
        Assert.Equal(5, p.Take);
        Assert.True(p.Descending);
        Assert.Equal("price", p.Field!.Name);
    }

    [Fact]
    public void ProductsByCategory_IsABreakdownByTheRelation()
    {
        var p = Summit.Parser().Parse("products by category");
        Assert.Equal(AnswerIntent.Breakdown, p.Intent);
        Assert.Equal("product", p.EntityKey);
        Assert.Equal("category", p.GroupByRelation!.Name);
        Assert.Equal(AggregateKind.Count, p.Aggregate);
    }

    [Fact]
    public void InStockVsOutOfStock_IsABreakdownByTheYesNoField()
    {
        var p = Summit.Parser().Parse("in stock vs out of stock");
        Assert.Equal(AnswerIntent.Breakdown, p.Intent);
        Assert.Equal("inStock", p.GroupByField!.Name);
        Assert.Empty(p.Filters);
    }

    [Fact]
    public void ShowProductsTaggedUltralight_IsATagFilter()
    {
        // Round 2: a real tag named after "tagged" is a filter (all of them required), never a text search.
        var p = Summit.Parser().Parse("show products tagged ultralight");
        Assert.Equal(AnswerIntent.Count, p.Intent);
        Assert.Null(p.Search);
        Assert.Equal("Ultralight", Assert.Single(p.Filters).RecordName);
        Assert.Equal("product", p.EntityKey);
    }

    [Fact]
    public void FindTent_SearchesTheMainEntity()
    {
        var p = Summit.Parser().Parse("find tent");
        Assert.Equal(AnswerIntent.Find, p.Intent);
        Assert.Equal("tent", p.Search);
    }

    [Fact]
    public void Gibberish_IsUnknown_WithSuggestionsBuiltFromThisSchema()
    {
        var parser = Summit.Parser();
        var p = parser.Parse("zxcv qwerty blorp");
        Assert.Equal(AnswerIntent.Unknown, p.Intent);
        Assert.Contains("I can answer questions like", p.Message);
        Assert.InRange(p.Suggestions.Count, 4, 6);
        Assert.Contains("How many products?", p.Suggestions);
        Assert.Contains("Average price of products", p.Suggestions);
        Assert.Contains("Top 5 products by price", p.Suggestions);
        Assert.Contains("Products by category", p.Suggestions);
        Assert.Contains(p.Suggestions, s => s.Contains("Northpeak"));
        // Every suggestion is a question this parser answers.
        foreach (var s in p.Suggestions) Assert.NotEqual(AnswerIntent.Unknown, parser.Parse(s).Intent);
    }

    [Fact]
    public void Suggestions_ForAnotherSchema_UseItsOwnLabels()
    {
        var book = new EntitySchema("Book", "Books", [new("title", "Title", FieldKind.Text, true), new("pages", "Page count", FieldKind.Number, false)],
            [new("author", "Author", RelationKind.One, "author")]);
        var author = new EntitySchema("Author", "Authors", [new("name", "Name", FieldKind.Text, true)], []);
        var parser = new DataQuestionParser([new("book", book), new("author", author)]);
        var s = parser.Suggestions();
        Assert.Contains("How many books?", s);
        Assert.Contains("Average page count of books", s);
        Assert.Contains("Books by author", s);
        Assert.DoesNotContain(s, x => x.Contains("product", StringComparison.OrdinalIgnoreCase));
        foreach (var q in s) Assert.NotEqual(AnswerIntent.Unknown, parser.Parse(q).Intent);
    }

    [Theory]
    [InlineData("average weight of products")]
    [InlineData("top 5 products by weight")]
    [InlineData("total colour of products")]
    [InlineData("products by warehouse")]
    public void AFieldThatDoesNotExist_IsUnknown_NeverAGuess(string q)
    {
        var p = Summit.Parser().Parse(q);
        Assert.Equal(AnswerIntent.Unknown, p.Intent);
        Assert.Null(p.Field);
        Assert.NotEmpty(p.Suggestions);
    }

    [Theory]
    [InlineData("How many PRODUCT?")]
    [InlineData("HOW MANY Products")]
    [InlineData("how many products?!")]
    public void Case_AndSingularOrPlural_DoNotMatter(string q)
    {
        var p = Summit.Parser().Parse(q);
        Assert.Equal(AnswerIntent.Count, p.Intent);
        Assert.Equal("product", p.EntityKey);
    }

    [Fact]
    public void HowManyCategories_PicksTheCategoryEntity()
    {
        var p = Summit.Parser().Parse("How many Categories?");
        Assert.Equal(AnswerIntent.Count, p.Intent);
        Assert.Equal("category", p.EntityKey);
    }

    [Fact]
    public void CheapestProduct_IsTheLowestOne()
    {
        var p = Summit.Parser().Parse("cheapest product");
        Assert.Equal(AnswerIntent.TopN, p.Intent);
        Assert.False(p.Descending);
        Assert.Equal(1, p.Take);
        Assert.Equal("price", p.Field!.Name);
    }

    [Fact]
    public void HighestRatedProducts_RankByRating()
    {
        var p = Summit.Parser().Parse("highest rated products");
        Assert.Equal(AnswerIntent.TopN, p.Intent);
        Assert.True(p.Descending);
        Assert.Equal("rating", p.Field!.Name);
        Assert.Equal("product", p.EntityKey);
    }

    [Fact]
    public void OutOfStock_IsAFalseFilter_InStock_ATrueOne()
    {
        var a = Summit.Parser().Parse("how many products are out of stock");
        Assert.Equal(AnswerIntent.Count, a.Intent);
        Assert.False(Assert.Single(a.Filters).Flag);
        var b = Summit.Parser().Parse("how many products are in stock");
        Assert.True(Assert.Single(b.Filters).Flag);
    }

    [Fact]
    public void CostMeansPrice_OnlyWhenPriceIsANumber()
    {
        Assert.Equal("price", Summit.Parser().Parse("average cost of products").Field!.Name);

        var textPrice = new EntitySchema("Item", "Items", [new("name", "Name", FieldKind.Text, true), new("price", "Price", FieldKind.Text, false)], []);
        var p = new DataQuestionParser([new("item", textPrice)]).Parse("average cost of items");
        Assert.Equal(AnswerIntent.Unknown, p.Intent);
    }

    [Fact]
    public void WithoutNames_ABrandIsNotGuessed()
    {
        var p = Summit.Parser(names: false).Parse("average price of Northpeak products");
        // Without the brand names the word cannot be used - and it is never dropped to answer the overall average.
        Assert.Equal(AnswerIntent.Unknown, p.Intent);
        Assert.Contains("northpeak", p.Message);
    }
}

public class DataAnswerComposerTests
{
    private static readonly List<JsonObject> Five =
    [
        Summit.Row(1, "Ridge Tent", 400m, true, "Tents", "Northpeak", "Ultralight"),
        Summit.Row(2, "Day Pack", 80m, true, "Packs", "Northpeak"),
        Summit.Row(3, "Big Pack", 160m, false, "Packs", "Ridgeline Gear", "Waterproof", "Ultralight"),
        Summit.Row(4, "Hut Tent", 300m, true, "Tents", "Ridgeline Gear"),
        Summit.Row(5, "Bivy", null, false, "Tents", "Northpeak"),
    ];

    [Fact]
    public void Average_IsComputedFromTheRows_AndSaysWhatWasLeftOut()
    {
        var plan = Summit.Parser().Parse("average price of products");
        var a = DataAnswerComposer.Compose(plan, new AnswerResult(Five, 5));
        Assert.Equal("235.00", a.Figure);                  // (400+80+160+300)/4
        Assert.Contains("235.00", a.Sentence);
        Assert.Contains("Averaged the price of 4 products", a.How);
        Assert.Contains("1 had no price", a.How);
        Assert.Equal(new[] { "Name", "Price" }, a.Columns);
        Assert.Equal(5, a.Table.Count);
    }

    [Fact]
    public void Capped_SaysHowManyRowsItWasBasedOn()
    {
        var plan = Summit.Parser().Parse("average price of products");
        var a = DataAnswerComposer.Compose(plan, new AnswerResult(Five, 4380));
        Assert.Contains("based on the first 5 of 4,380 rows", a.How);
    }

    [Fact]
    public void Top_HowLineStatesTheExactQuery()
    {
        var plan = Summit.Parser().Parse("top 2 products by price");
        var sorted = Five.Where(r => r["price"] is not null).OrderByDescending(r => (decimal)r["price"]!).ToList();
        var a = DataAnswerComposer.Compose(plan, new AnswerResult(sorted, 4));
        Assert.Equal("Counted 4 products that have a price; sorted by price, highest first; showing the top 2", a.How);
        Assert.Equal(2, a.Table.Count);
        Assert.Equal(new[] { "1", "Ridge Tent", "400.00" }, a.Table[0]);
        Assert.Equal("400.00", a.Figure);
    }

    [Fact]
    public void Breakdown_CountsSumToTheRowsRead()
    {
        var plan = Summit.Parser().Parse("products by category");
        var a = DataAnswerComposer.Compose(plan, new AnswerResult(Five, 5));
        Assert.Equal(new[] { "Category", "Products" }, a.Columns);
        Assert.Equal(5, a.Table.Sum(r => int.Parse(r[1])));
        Assert.Equal(new[] { "Tents", "3" }, a.Table[0]);
        Assert.Contains("Grouped 5 products by category", a.How);
    }

    [Fact]
    public void YesNoBreakdown_SaysBothSides()
    {
        var plan = Summit.Parser().Parse("in stock vs out of stock");
        var a = DataAnswerComposer.Compose(plan, new AnswerResult(Five, 5));
        Assert.Equal("Of 5 products, 3 are in stock and 2 are not.", a.Sentence);
    }

    [Fact]
    public void Count_UsesTheServerCount_AndShowsTheRowsBehindIt()
    {
        var plan = Summit.Parser().Parse("how many products");
        var a = DataAnswerComposer.Compose(plan, new AnswerResult(Five, 40));
        Assert.Equal("40", a.Figure);
        Assert.Equal("There are 40 products.", a.Sentence);
        Assert.Equal(5, a.Table.Count);
        Assert.Contains("Category", a.Columns);
        Assert.Contains("the table shows the first 5", a.How);
    }

    [Fact]
    public void Find_ShowsTheLinkTheTextWasFoundIn()
    {
        // Without the tag names the word is a text search, and the answer shows the to-many link it was found in.
        var plan = new DataQuestionParser(Summit.All).Parse("find ultralight");
        var hits = Five.Where(r => r["tag"]!.AsArray().Any(t => (string)t!["name"]! == "Ultralight")).ToList();
        var a = DataAnswerComposer.Compose(plan, new AnswerResult(hits, hits.Count));
        Assert.Contains("Tags", a.Columns);
        Assert.Equal("2 products match “ultralight”.", a.Sentence);
    }

    [Fact]
    public void Unknown_IsTheMessageAndTheSuggestions()
    {
        var plan = Summit.Parser().Parse("blorp");
        var a = DataAnswerComposer.Compose(plan, new AnswerResult([], 0));
        Assert.True(a.IsUnknown);
        Assert.Null(a.Figure);
        Assert.NotEmpty(a.Suggestions);
    }
}

public class ODataAnswerQueryTests
{
    private static (IReadOnlyList<JsonObject>, int) Page(int total, int top)
    {
        var rows = Enumerable.Range(0, Math.Min(top, total)).Select(i => Summit.Row(i, "P" + i, 10m, true, "Tents", "Northpeak")).ToList();
        return (rows, total);
    }

    [Fact]
    public async Task Count_IsOneGet_WithTheFilterAndASample()
    {
        var plan = Summit.Parser().Parse("how many Northpeak products");
        var urls = new List<string>();
        var r = await ODataAnswerQuery.RunAsync(plan, Summit.ProductMap, u => { urls.Add(u); return Task.FromResult(Page(5, 10)); });
        var url = Assert.Single(urls);
        Assert.StartsWith("odata/product?$count=true&$skip=0&$top=10", url);
        Assert.Contains(Uri.EscapeDataString("Brand/Id eq 1"), url);
        Assert.Equal(5, r.TotalMatched);
    }

    [Fact]
    public async Task Average_ReadsPagesOf500_UpTo2000()
    {
        var plan = Summit.Parser().Parse("average price of products");
        var urls = new List<string>();
        var r = await ODataAnswerQuery.RunAsync(plan, Summit.ProductMap, u => { urls.Add(u); return Task.FromResult(Page(4380, 500)); });
        Assert.Equal(4, urls.Count);
        Assert.Contains("$skip=1500&$top=500", urls[3]);
        Assert.Equal(2000, r.Rows.Count);
        Assert.Contains("based on the first 2,000 of 4,380 rows", DataAnswerComposer.Compose(plan, r).How);
    }

    [Fact]
    public async Task Average_StopsWhenEveryRowIsRead()
    {
        var plan = Summit.Parser().Parse("average price of products");
        var urls = new List<string>();
        await ODataAnswerQuery.RunAsync(plan, Summit.ProductMap, u => { urls.Add(u); return Task.FromResult(Page(40, 500)); });
        Assert.Single(urls);
    }

    [Fact]
    public void Top_SortsByTheField_SkipsEmptyValues()
    {
        var plan = Summit.Parser().Parse("top 5 products by price");
        var url = Uri.UnescapeDataString(ODataAnswerQuery.Url(plan, Summit.ProductMap, 0, 5));
        Assert.Contains("$top=5", url);
        Assert.Contains("$orderby=Price desc,Id", url);
        Assert.Contains("$filter=Price ne null", url);
    }

    [Fact]
    public void Filters_ManyRelationsUseAny_FlagsUseTrueFalse()
    {
        var tagged = new AnswerPlan { Question = "x", Intent = AnswerIntent.Count, Schema = Summit.Product, EntityKey = "product",
            Filters = [new PlanFilter { Relation = Summit.Product.Relations[3], RecordId = "1", RecordName = "Ultralight" }, new PlanFilter { Field = Summit.Product.Fields[3], Flag = false }] };
        Assert.Equal("(Tag/any(x: x/Id eq 1)) and (InStock eq false)", ODataAnswerQuery.Filter(tagged, Summit.ProductMap));
    }

    [Fact]
    public void Find_UsesTheListSearch()
    {
        var plan = new DataQuestionParser(Summit.All).Parse("find ultralight");
        Assert.Equal("contains(tolower(Name),'ultralight') or Tag/any(x: contains(tolower(x/Name),'ultralight'))", ODataAnswerQuery.Filter(plan, Summit.ProductMap));
    }
}

public class AskDataComponentTests : TestContext
{
    private readonly List<AnswerPlan> _plans = [];

    public AskDataComponentTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<TwAskData> Render(Func<AnswerPlan, Task<AnswerResult>> executor) =>
        RenderComponent<TwAskData>(p => p
            .Add(c => c.Schema, Summit.All)
            .Add(c => c.Executor, plan => { _plans.Add(plan); return executor(plan); })
            .Add(c => c.NamesLoader, key => Task.FromResult(Summit.Names.TryGetValue(key, out var n) ? n : (IReadOnlyList<(string, string)>)[])));

    [Fact]
    public void Launcher_IsAButton_AndOpensThePanelWithSchemaSuggestions()
    {
        var cut = Render(_ => Task.FromResult(new AnswerResult([], 0)));
        Assert.Empty(cut.FindAll("[data-testid=askdata-panel]"));
        cut.Find("[data-testid=askdata-launcher]").Click();
        var chips = cut.FindAll("[data-testid=chat-suggestion]").Select(b => b.TextContent.Trim()).ToList();
        Assert.Contains("How many products?", chips);
        Assert.Contains("Products by category", chips);
        Assert.Contains("Average price of Northpeak products", chips);
    }

    [Fact]
    public void AnAnswer_ShowsTheSentence_TheRows_AndHowItWasWorkedOut()
    {
        var rows = new List<JsonObject> { Summit.Row(1, "Ridge Tent", 400m, true, "Tents", "Northpeak"), Summit.Row(2, "Day Pack", 80m, true, "Packs", "Northpeak") };
        var cut = Render(_ => Task.FromResult(new AnswerResult(rows, 2)));
        cut.Find("[data-testid=askdata-launcher]").Click();
        cut.Find("[data-testid=askdata-input]").Input("average price of Northpeak products");
        cut.Find("[data-testid=askdata-form]").Submit();

        cut.WaitForAssertion(() => Assert.Contains("240.00", cut.Find("[data-testid=askdata-sentence]").TextContent));
        Assert.Equal(2, cut.FindAll("[data-testid=askdata-row]").Count);
        Assert.Contains("How I got this", cut.Find("[data-testid=askdata-how]").TextContent);
        Assert.Contains("whose brand is Northpeak", cut.Find("[data-testid=askdata-how]").TextContent);
        Assert.Single(_plans);
    }

    [Fact]
    public void AStaleAnswer_IsDiscarded_WhenANewerQuestionArrives()
    {
        var first = new TaskCompletionSource<AnswerResult>();
        var second = new TaskCompletionSource<AnswerResult>();
        var cut = Render(plan => plan.Intent == AnswerIntent.Count ? first.Task : second.Task);
        cut.Find("[data-testid=askdata-launcher]").Click();

        _ = cut.InvokeAsync(() => cut.Instance.AskAsync("how many products"));
        _ = cut.InvokeAsync(() => cut.Instance.AskAsync("average price of products"));
        cut.InvokeAsync(() => second.SetResult(new AnswerResult([Summit.Row(1, "A", 10m, true, "Tents", "Northpeak")], 1)));
        cut.InvokeAsync(() => first.SetResult(new AnswerResult([], 999)));

        cut.WaitForAssertion(() => Assert.Contains("10.00", cut.Find("[data-testid=askdata-sentence]").TextContent));
        Assert.Single(cut.FindAll("[data-testid=askdata-answer]"));
        Assert.Single(cut.FindAll("[data-testid=askdata-superseded]"));
        Assert.DoesNotContain("999", cut.Markup);
    }

    [Fact]
    public void AnUnknownQuestion_NeverCallsTheExecutor_AndOffersQuestions()
    {
        var cut = Render(_ => throw new InvalidOperationException("must not be called"));
        cut.Find("[data-testid=askdata-launcher]").Click();
        cut.InvokeAsync(() => cut.Instance.AskAsync("what is the weather"));
        cut.WaitForAssertion(() => Assert.Equal("true", cut.Find("[data-testid=askdata-answer]").GetAttribute("data-unknown")));
        Assert.Empty(_plans);
        Assert.NotEmpty(cut.Find("[data-testid=askdata-answer]").QuerySelectorAll("[data-testid=chat-suggestion]"));
    }

    [Fact]
    public void NothingButTheExecutorAndTheNamesLoaderIsEverCalled()
    {
        // Any HTTP the component made on its own would go through this client; it must see nothing.
        var handler = new RecordingHandler();
        Services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri("https://localhost/") });
        var cut = Render(_ => Task.FromResult(new AnswerResult([], 0)));
        cut.Find("[data-testid=askdata-launcher]").Click();
        foreach (var q in new[] { "how many products", "products by category", "top 5 products by price" })
            cut.InvokeAsync(() => cut.Instance.AskAsync(q));
        cut.WaitForAssertion(() => Assert.Equal(3, _plans.Count));
        Assert.Empty(handler.Requests);
    }

    [Fact]
    public void AnExecutorFailure_IsShownHonestly()
    {
        var cut = Render(_ => throw new InvalidOperationException("The server answered 500."));
        cut.Find("[data-testid=askdata-launcher]").Click();
        cut.InvokeAsync(() => cut.Instance.AskAsync("how many products"));
        cut.WaitForAssertion(() => Assert.Equal("I could not get an answer just now. Try again.", cut.Find("[data-testid=askdata-error]").TextContent.Trim()));
        Assert.DoesNotContain("The server answered 500.", cut.Markup);   // raw exception text goes to the console only
        Assert.Empty(cut.FindAll("[data-testid=askdata-sentence]"));
    }

    [Fact]
    public void Close_ReturnsFocusToTheLauncher()
    {
        var cut = Render(_ => Task.FromResult(new AnswerResult([], 0)));
        cut.Find("[data-testid=askdata-launcher]").Click();
        var before = JSInterop.Invocations.Count(i => i.Identifier.Contains("focus", StringComparison.OrdinalIgnoreCase));
        cut.Find("[data-testid=askdata-close]").Click();
        cut.Find("[data-testid=askdata-launcher]");
        cut.WaitForAssertion(() => Assert.True(JSInterop.Invocations.Count(i => i.Identifier.Contains("focus", StringComparison.OrdinalIgnoreCase)) > before,
            "Closing the panel did not move focus back to the launcher."));
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public List<HttpRequestMessage> Requests { get; } = [];
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request);
            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
        }
    }
}
