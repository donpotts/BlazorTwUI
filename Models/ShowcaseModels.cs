namespace BlazorTwUI.Models;

public class CommandPaletteItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string Category { get; set; } = "General"; // Pages, Components, Actions, Documentation
    public string? IconSvg { get; set; }
    public string? ShortcutKey { get; set; }
    public Action? Action { get; set; }
    public string? TargetUrl { get; set; }
}

public class PricingFeature
{
    public string Name { get; set; } = string.Empty;
    public bool Included { get; set; } = true;
    public string? Note { get; set; }
}

public class PricingTier
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal MonthlyPrice { get; set; }
    public decimal AnnualMonthlyPrice { get; set; }
    public bool IsPopular { get; set; }
    public string CtaText { get; set; } = "Get Started";
    public List<PricingFeature> Features { get; set; } = new();
}

public class AvatarItem
{
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Initials { get; set; } = "UI";
    public string Status { get; set; } = "online"; // online, away, busy, offline
    public string? Role { get; set; }
}
