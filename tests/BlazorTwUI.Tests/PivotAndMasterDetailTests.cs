using System;
using System.Collections.Generic;
using System.Linq;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Xunit;
using BlazorTwUI.Components.UI;
using BlazorTwUI.Models;

namespace BlazorTwUI.Tests;

public class PivotAndMasterDetailTests : TestContext
{
    public record SaleItem(string Region, string Category, string Quarter, decimal Revenue, int Units);

    public record DepartmentOrder(string Id, string Department, string Status, decimal Budget, List<OrderLine> Lines);
    public record OrderLine(string Sku, string ItemName, int Quantity, decimal UnitPrice);

    private List<SaleItem> GetSampleSales() => new()
    {
        new("North America", "Software", "Q1", 120000m, 50),
        new("North America", "Software", "Q2", 150000m, 60),
        new("North America", "Hardware", "Q1", 80000m, 20),
        new("Europe", "Software", "Q1", 95000m, 40),
        new("Europe", "Hardware", "Q2", 110000m, 25),
        new("Asia-Pacific", "Software", "Q1", 130000m, 55),
        new("Asia-Pacific", "Hardware", "Q2", 90000m, 22)
    };

    private List<DepartmentOrder> GetSampleOrders() => new()
    {
        new("ORD-001", "Engineering", "Active", 50000m, new List<OrderLine>
        {
            new("SKU-1", "Cloud GPU Compute", 10, 1500m),
            new("SKU-2", "Developer Workstation", 4, 2500m)
        }),
        new("ORD-002", "Operations", "Pending", 25000m, new List<OrderLine>
        {
            new("SKU-3", "Network Switch 48-port", 2, 3200m)
        })
    };

    [Fact]
    public void PivotTable_RendersMatrix_AndComputesGrandTotals()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var data = GetSampleSales();

        var cut = RenderComponent<TwPivotTable<SaleItem>>(parameters => parameters
            .Add(p => p.Items, data)
            .Add(p => p.Title, "Sales Analytics Pivot")
        );

        Assert.NotNull(cut.Find("[data-testid=pivot-table]"));
        Assert.Contains("Sales Analytics Pivot", cut.Markup);

        // Check that cells are rendered
        var cells = cut.FindAll("[data-testid=pivot-cell]");
        Assert.NotEmpty(cells);

        // Check Grand Total row exists
        Assert.Contains("Grand Total", cut.Markup);
    }

    [Fact]
    public void PivotTable_HeatmapToggle_AppliesColorShading()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var data = GetSampleSales();

        var cut = RenderComponent<TwPivotTable<SaleItem>>(parameters => parameters
            .Add(p => p.Items, data)
        );

        // Initially heatmap is off
        var toggle = cut.Find("[data-testid=pivot-heatmap-toggle]");
        toggle.Click();

        // After toggle, heatmap style should be active
        Assert.Contains("Heatmap Active", cut.Markup);
        var cellsWithStyle = cut.FindAll("[data-testid=pivot-cell]").Where(c => c.GetAttribute("style")?.Contains("background-color") == true);
        Assert.NotEmpty(cellsWithStyle);
    }

    [Fact]
    public void PivotTable_FieldListToggle_ShowsFieldSelectorPanel()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var cut = RenderComponent<TwPivotTable<SaleItem>>(parameters => parameters
            .Add(p => p.Items, GetSampleSales())
        );

        Assert.Empty(cut.FindAll("[data-testid=pivot-field-selector-panel]"));

        cut.Find("[data-testid=pivot-field-list-toggle]").Click();
        Assert.NotNull(cut.Find("[data-testid=pivot-field-selector-panel]"));
    }

    [Fact]
    public void PivotTable_CellClick_OpensDrillDownModal()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var cut = RenderComponent<TwPivotTable<SaleItem>>(parameters => parameters
            .Add(p => p.Items, GetSampleSales())
            .Add(p => p.AllowDrillDown, true)
        );

        Assert.Empty(cut.FindAll("[data-testid=pivot-drilldown-modal]"));

        // Click first cell
        var firstCell = cut.Find("[data-testid=pivot-cell]");
        firstCell.Click();

        // Drilldown modal should now appear
        Assert.NotNull(cut.Find("[data-testid=pivot-drilldown-modal]"));
        Assert.Contains("Drill-Down Raw Records Inspector", cut.Markup);
    }

    [Fact]
    public void MasterDetailGrid_ExpandsRow_AndRendersNestedChildGrid()
    {
        var orders = GetSampleOrders();
        var cut = RenderComponent<TwMasterDetailGrid<DepartmentOrder, OrderLine>>(parameters => parameters
            .Add(p => p.Items, orders)
            .Add(p => p.ChildSelector, o => o.Lines)
            .Add(p => p.Title, "Department Orders")
        );

        Assert.NotNull(cut.Find("[data-testid=master-detail-grid]"));
        var masterRows = cut.FindAll("[data-testid=master-row]");
        Assert.Equal(2, masterRows.Count);

        // Nested child rows initially not visible
        Assert.Empty(cut.FindAll("[data-testid=nested-child-row]"));

        // Click expand chevron on first master row
        var expandBtn = cut.FindAll("[data-testid=master-expand-toggle]")[0];
        expandBtn.Click();

        // Nested child row is now displayed
        var nestedRow = cut.Find("[data-testid=nested-child-row]");
        Assert.NotNull(nestedRow);
        Assert.NotNull(cut.Find("[data-testid=nested-child-table]"));
        Assert.Contains("Cloud GPU Compute", nestedRow.TextContent);
        Assert.Contains("Developer Workstation", nestedRow.TextContent);
    }

    [Fact]
    public void MasterDetailGrid_ToggleAll_ExpandsAllRows()
    {
        var orders = GetSampleOrders();
        var cut = RenderComponent<TwMasterDetailGrid<DepartmentOrder, OrderLine>>(parameters => parameters
            .Add(p => p.Items, orders)
            .Add(p => p.ChildSelector, o => o.Lines)
        );

        Assert.Empty(cut.FindAll("[data-testid=nested-child-row]"));

        cut.Find("[data-testid=master-grid-toggle-all]").Click();
        var nestedRows = cut.FindAll("[data-testid=nested-child-row]");
        Assert.Equal(2, nestedRows.Count);

        // Collapse all
        cut.Find("[data-testid=master-grid-toggle-all]").Click();
        Assert.Empty(cut.FindAll("[data-testid=nested-child-row]"));
    }

    [Fact]
    public void MasterDetailWorkspace_SelectsItem_AndSwitchesTabs()
    {
        var orders = GetSampleOrders();
        DepartmentOrder? selected = null;

        var cut = RenderComponent<TwMasterDetail<DepartmentOrder, OrderLine>>(parameters => parameters
            .Add(p => p.Items, orders)
            .Add(p => p.ChildSelector, o => o.Lines)
            .Add(p => p.SelectedItemChanged, o => selected = o)
        );

        Assert.NotNull(cut.Find("[data-testid=master-detail-workspace]"));
        var cards = cut.FindAll("[data-testid=master-item-card]");
        Assert.Equal(2, cards.Count);

        // Click second card
        cards[1].Click();
        Assert.Equal("ORD-002", selected?.Id);

        // Switch to child records tab
        cut.Find("[data-testid=tab-children]").Click();
        Assert.NotNull(cut.Find("[data-testid=child-records-view]"));

        // Switch to activity tab
        cut.Find("[data-testid=tab-activity]").Click();
        Assert.NotNull(cut.Find("[data-testid=activity-timeline-view]"));
    }

    [Fact]
    public void MasterDetailForm_CalculatesSubtotalAndTax_AndAddsLineItem()
    {
        var cut = RenderComponent<TwMasterDetailForm>(parameters => parameters
            .Add(p => p.Title, "Invoice Creator")
            .Add(p => p.DocumentNumber, "INV-2026-TEST")
        );

        Assert.NotNull(cut.Find("[data-testid=master-detail-form]"));
        Assert.Contains("INV-2026-TEST", cut.Markup);

        var initialTotal = cut.Instance.GrandTotal;
        Assert.True(initialTotal > 0);

        // Add line item
        cut.Find("[data-testid=form-add-line-btn]").Click();
        var newTotal = cut.Instance.GrandTotal;
        Assert.True(newTotal > initialTotal);

        // Remove the last line item
        var removeButtons = cut.FindAll("[data-testid=form-remove-line-btn]");
        removeButtons.Last().Click();
        Assert.Equal(initialTotal, cut.Instance.GrandTotal);
    }
}
