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

public class DataTransferWizardTests : TestContext
{
    public class MockProduct
    {
        public string Sku { get; set; } = "SKU-001";
        public string Title { get; set; } = "Enterprise Server";
        public decimal Price { get; set; } = 4500m;
        public int Stock { get; set; } = 15;
    }

    private List<MockProduct> GetSampleProducts() => new()
    {
        new() { Sku = "SKU-100", Title = "Cloud Edge Gateway", Price = 2400m, Stock = 8 },
        new() { Sku = "SKU-200", Title = "Storage Node Array", Price = 5100m, Stock = 12 },
        new() { Sku = "SKU-300", Title = "AI Inference Accelerator", Price = 9800m, Stock = 4 }
    };

    [Fact]
    public void ImportExportWizard_RendersInitialImportState()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var cut = RenderComponent<TwImportExportWizard<MockProduct>>(parameters => parameters
            .Add(p => p.Title, "Product Data Wizard")
        );

        Assert.NotNull(cut.Find("[data-testid=data-transfer-wizard]"));
        Assert.NotNull(cut.Find("[data-testid=import-step-1]"));
        Assert.Contains("Product Data Wizard", cut.Markup);
    }

    [Fact]
    public void ImportExportWizard_StepsThroughFullImportWorkflow()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        DataImportResult<MockProduct>? importedResult = null;

        var cut = RenderComponent<TwImportExportWizard<MockProduct>>(parameters => parameters
            .Add(p => p.OnImportCompleted, res => importedResult = res)
        );

        // Step 1: Click next (auto-loads sample data). Each step's button has its own test id (bac7cda).
        cut.Find("[data-testid=wizard-next-step-1]").Click();
        Assert.NotNull(cut.Find("[data-testid=import-step-2]"));

        // Step 2: Column mapping -> click next
        cut.Find("[data-testid=wizard-next-step-2]").Click();
        Assert.NotNull(cut.Find("[data-testid=import-step-3]"));

        // Step 3: Review staged rows -> click execute
        cut.Find("[data-testid=wizard-execute-import]").Click();
        Assert.NotNull(cut.Find("[data-testid=import-step-4]"));
        Assert.NotNull(importedResult);
        Assert.True(importedResult.ImportedCount > 0);
    }

    [Fact]
    public void ImportExportWizard_ExportWorkflow_GeneratesPayload()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var products = GetSampleProducts();

        var cut = RenderComponent<TwImportExportWizard<MockProduct>>(parameters => parameters
            .Add(p => p.Items, products)
        );

        // Switch to export mode
        cut.Find("[data-testid=wizard-mode-export]").Click();
        Assert.NotNull(cut.Find("[data-testid=export-step-1]"));

        // Step 1 -> Step 2
        cut.Find("[data-testid=wizard-next-export]").Click();
        Assert.NotNull(cut.Find("[data-testid=export-step-2]"));

        // Step 2 -> Step 3
        cut.Find("[data-testid=wizard-next-export]").Click();
        Assert.NotNull(cut.Find("[data-testid=export-step-3]"));
        Assert.Contains("Cloud Edge Gateway", cut.Markup);
        Assert.Contains("AI Inference Accelerator", cut.Markup);
    }

    [Fact]
    public void PivotChart_RendersGroupedSvgBars()
    {
        var products = GetSampleProducts();

        var cut = RenderComponent<TwPivotChart<MockProduct>>(parameters => parameters
            .Add(p => p.Items, products)
            .Add(p => p.RowDimension, "Title")
            .Add(p => p.ColumnDimension, "Sku")
            .Add(p => p.MeasureField, "Price")
            .Add(p => p.Title, "Product Pricing Chart")
        );

        Assert.NotNull(cut.Find("[data-testid=pivot-chart]"));
        var bars = cut.FindAll("[data-testid=pivot-chart-bar]");
        Assert.NotEmpty(bars);
        Assert.Contains("Product Pricing Chart", cut.Markup);
    }
}
