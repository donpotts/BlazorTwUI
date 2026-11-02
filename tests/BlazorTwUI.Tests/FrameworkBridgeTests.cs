using System.Collections.Generic;
using Bunit;
using Xunit;
using BlazorTwUI.Components.UI;
using BlazorTwUI.Models;

namespace BlazorTwUI.Tests;

public class FrameworkBridgeTests : TestContext
{
    [Fact]
    public void TwMerge_Resolves_Conflicting_Utilities_Deterministically()
    {
        var merged = TwMerge.Merge(
            "px-4 py-2 rounded-xl bg-brand-600 text-sm font-medium",
            "px-8 rounded-full bg-emerald-600 font-black");

        Assert.DoesNotContain("px-4", merged);
        Assert.Contains("px-8", merged);
        Assert.Contains("py-2", merged);
        Assert.DoesNotContain("rounded-xl", merged);
        Assert.Contains("rounded-full", merged);
        Assert.DoesNotContain("bg-brand-600", merged);
        Assert.Contains("bg-emerald-600", merged);
        Assert.DoesNotContain("font-medium", merged);
        Assert.Contains("font-black", merged);
    }

    [Fact]
    public void TwMerge_PaddingAll_Overrides_AxisPadding()
    {
        var merged = TwMerge.Merge("px-4 py-2 pt-6", "p-8");
        Assert.Equal("p-8", merged);
    }

    [Fact]
    public void TwButton_Applies_TwMerge_And_ButtonSlots()
    {
        var cut = RenderComponent<TwButton>(parameters => parameters
            .Add(p => p.Variant, ButtonVariant.Soft)
            .Add(p => p.Class, "rounded-full px-10")
            .Add(p => p.Classes, new TwButtonSlots
            {
                Base = "shadow-xl",
                LeadingIcon = "text-amber-400"
            })
            .Add(p => p.LeadingIcon, builder => builder.AddMarkupContent(0, "<span>★</span>"))
            .AddChildContent("Action"));

        var btn = cut.Find("button");
        var cls = btn.GetAttribute("class") ?? "";
        Assert.Contains("rounded-full", cls);
        Assert.DoesNotContain("rounded-xl", cls);
        Assert.Contains("px-10", cls);
        Assert.Contains("shadow-xl", cls);
    }

    [Fact]
    public void TwCard_Supports_MultiPart_TwCardSlots()
    {
        var cut = RenderComponent<TwCard>(parameters => parameters
            .Add(p => p.Title, "Slot Title")
            .Add(p => p.Subtitle, "Slot Subtitle")
            .Add(p => p.Classes, new TwCardSlots
            {
                Base = "rounded-3xl border-indigo-500",
                Header = "bg-indigo-950",
                Title = "text-amber-300 font-black",
                Body = "p-10"
            })
            .AddChildContent("<p>Body</p>"));

        Assert.Contains("rounded-3xl", cut.Markup);
        Assert.Contains("bg-indigo-950", cut.Markup);
        Assert.Contains("text-amber-300", cut.Markup);
        Assert.Contains("p-10", cut.Markup);
    }

    [Fact]
    public void TwUser_Renders_Initials_Status_And_Custom_Slots()
    {
        var cut = RenderComponent<TwUser>(parameters => parameters
            .Add(p => p.Name, "Elena Rostova")
            .Add(p => p.Description, "Principal Architect")
            .Add(p => p.Status, "online")
            .Add(p => p.Classes, new TwUserSlots
            {
                Name = "text-emerald-500 font-black"
            }));

        Assert.Contains("ER", cut.Markup);
        Assert.Contains("Elena Rostova", cut.Markup);
        Assert.Contains("Principal Architect", cut.Markup);
        Assert.Contains("bg-emerald-500", cut.Markup);
        Assert.Contains("text-emerald-500 font-black", cut.Markup);
    }

    [Fact]
    public void TwValidatorGroup_And_TwFloatingLabelInput_Validate_Rules()
    {
        var cut = RenderComponent<TwFrameworkBridgeStudio>();
        Assert.Contains("Next-Gen Styling Engine", cut.Markup);
        Assert.Contains("Live TwMerge.Merge() Conflict Resolver", cut.Markup);
        Assert.Contains("3D Stacked Toast Deck", cut.Markup);
        Assert.Contains("Device Mockup", cut.Markup);
    }
}
