using BlazorTwUI.Components.UI;
using BlazorTwUI.Services;
using Xunit;

namespace BlazorTwUI.Tests;

public class CatalogTests
{
    private static IEnumerable<string> Names()
    {
        var catalog = new ComponentCatalogService();
        return catalog.GetAll().Select(c => c.Name);
    }

    [Fact]
    public void Catalog_HasAtLeast200Components_WithUniqueNames()
    {
        var names = Names().ToList();
        Assert.True(names.Count >= 200, $"catalog has {names.Count}");
        Assert.Equal(names.Count, names.Distinct().Count());
    }

    [Fact]
    public void EveryCatalogEntry_IsARealComponent()
    {
        var types = typeof(TwButton).Assembly.GetTypes().Select(t => t.Name.Split('`')[0]).ToHashSet();
        var missing = Names().Where(n => !types.Contains(n)).ToList();
        Assert.True(missing.Count == 0, "No component class for: " + string.Join(", ", missing));
    }
}
