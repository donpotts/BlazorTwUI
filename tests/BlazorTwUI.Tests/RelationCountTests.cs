using System.Text.Json.Nodes;
using BlazorTwUI.Components.UI;
using BlazorTwUI.Models;
using Xunit;

namespace BlazorTwUI.Tests;

/// <summary>A parent row carries its first children plus their total; the cell says how many are not shown.</summary>
public class RelationCountTests
{
    private static readonly EntityRelation Products = new("products", "Products", RelationKind.Many, "/api/app/product");

    [Fact]
    public void Cell_ends_with_plus_n_more_when_the_total_is_larger()
    {
        var row = JsonNode.Parse("""{"products":[{"id":1,"name":"Anvil"},{"id":2,"name":"Bucket"}],"productsCount":39}""")!.AsObject();
        Assert.Equal("Anvil, Bucket +37 more", TwEntityGrid.RelationText(row, Products));
    }

    [Fact]
    public void Cell_is_just_the_names_when_all_are_shown_or_no_total_is_sent()
    {
        var all = JsonNode.Parse("""{"products":[{"id":1,"name":"Anvil"}],"productsCount":1}""")!.AsObject();
        var none = JsonNode.Parse("""{"products":[{"id":1,"name":"Anvil"}]}""")!.AsObject();
        Assert.Equal("Anvil", TwEntityGrid.RelationText(all, Products));
        Assert.Equal("Anvil", TwEntityGrid.RelationText(none, Products));
    }
}
