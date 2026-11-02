using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazorTwUI.Models;

/// <summary>
/// High-performance Tailwind CSS utility conflict resolver and fluent class builder
/// for BlazorTwUI components.
/// </summary>
public static class TwMerge
{
    private static readonly HashSet<string> DisplayTokens = new(StringComparer.Ordinal)
    {
        "block", "inline-block", "inline", "flex", "inline-flex", "grid", "inline-grid", "contents", "table", "hidden"
    };

    private static readonly HashSet<string> PositionTokens = new(StringComparer.Ordinal)
    {
        "static", "fixed", "absolute", "relative", "sticky"
    };

    private static readonly HashSet<string> FontWeightTokens = new(StringComparer.Ordinal)
    {
        "font-thin", "font-extralight", "font-light", "font-normal", "font-medium",
        "font-semibold", "font-bold", "font-extrabold", "font-black"
    };

    private static readonly HashSet<string> TextSizeTokens = new(StringComparer.Ordinal)
    {
        "text-2xs", "text-xs", "text-sm", "text-base", "text-lg", "text-xl",
        "text-2xl", "text-3xl", "text-4xl", "text-5xl", "text-6xl", "text-7xl", "text-8xl", "text-9xl"
    };

    private static readonly HashSet<string> TextAlignTokens = new(StringComparer.Ordinal)
    {
        "text-left", "text-center", "text-right", "text-justify", "text-start", "text-end"
    };

    private static readonly HashSet<string> BgNonColorPrefixes = new(StringComparer.Ordinal)
    {
        "bg-gradient-", "bg-linear-", "bg-radial-", "bg-clip-", "bg-origin-",
        "bg-bottom", "bg-center", "bg-left", "bg-right", "bg-top",
        "bg-repeat", "bg-no-repeat", "bg-cover", "bg-contain", "bg-fixed", "bg-local", "bg-scroll", "bg-none"
    };

    private static readonly HashSet<string> BorderStyleTokens = new(StringComparer.Ordinal)
    {
        "border-solid", "border-dashed", "border-dotted", "border-double", "border-hidden", "border-none"
    };

    /// <summary>
    /// Merges multiple Tailwind CSS class strings, resolving utility conflicts so later classes
    /// deterministically override earlier conflicting utilities within the same variant prefix scope.
    /// </summary>
    public static string Merge(params string?[] classLists)
    {
        if (classLists == null || classLists.Length == 0)
            return string.Empty;

        var tokens = new List<string>(32);
        foreach (var list in classLists)
        {
            if (string.IsNullOrWhiteSpace(list))
                continue;

            var parts = list.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var part in parts)
            {
                tokens.Add(part);
            }
        }

        if (tokens.Count <= 1)
            return tokens.Count == 1 ? tokens[0] : string.Empty;

        // Track active token indices by (variantPrefix, groupKey)
        var groupIndex = new Dictionary<string, List<int>>(StringComparer.Ordinal);
        var removed = new bool[tokens.Count];

        void RemoveGroup(string key)
        {
            if (groupIndex.TryGetValue(key, out var indices))
            {
                foreach (var idx in indices)
                    removed[idx] = true;
                indices.Clear();
            }
        }

        void RegisterGroup(string key, int index)
        {
            if (!groupIndex.TryGetValue(key, out var list))
            {
                list = new List<int>(2);
                groupIndex[key] = list;
            }
            list.Add(index);
        }

        for (int i = 0; i < tokens.Count; i++)
        {
            var raw = tokens[i];
            SplitVariantAndCore(raw, out var variantPrefix, out var core);

            var group = ClassifyCoreUtility(core, out var invalidatedSubGroups);
            if (group == null)
            {
                // Exact duplicate check
                var exactKey = $"{variantPrefix}::exact::{core}";
                RemoveGroup(exactKey);
                RegisterGroup(exactKey, i);
                continue;
            }

            var scopedGroup = $"{variantPrefix}::{group}";
            RemoveGroup(scopedGroup);

            // If an unprefixed color override (bg-color, text-color, border-color) is supplied later
            // (e.g. from user Class or Slot override), also clear earlier default dark: color of the same group
            // so a single class like "text-amber-300" or "bg-emerald-600" works in both light and dark modes.
            if (string.IsNullOrEmpty(variantPrefix) &&
                (group == "bg-color" || group == "text-color" || group == "border-color"))
            {
                RemoveGroup($"dark::{group}");
            }

            if (invalidatedSubGroups != null)
            {
                foreach (var sub in invalidatedSubGroups)
                {
                    RemoveGroup($"{variantPrefix}::{sub}");
                }
            }

            RegisterGroup(scopedGroup, i);
        }

        var sb = new StringBuilder(tokens.Count * 10);
        for (int i = 0; i < tokens.Count; i++)
        {
            if (removed[i])
                continue;
            if (sb.Length > 0)
                sb.Append(' ');
            sb.Append(tokens[i]);
        }

        return sb.ToString();
    }

    private static void SplitVariantAndCore(string token, out string variantPrefix, out string core)
    {
        // Find last ':' that is not inside [...]
        int bracketDepth = 0;
        int lastColon = -1;
        for (int i = 0; i < token.Length; i++)
        {
            char c = token[i];
            if (c == '[') bracketDepth++;
            else if (c == ']' && bracketDepth > 0) bracketDepth--;
            else if (c == ':' && bracketDepth == 0) lastColon = i;
        }

        if (lastColon >= 0)
        {
            variantPrefix = token.Substring(0, lastColon + 1);
            core = token.Substring(lastColon + 1);
        }
        else
        {
            variantPrefix = string.Empty;
            core = token;
        }
    }

    private static string? ClassifyCoreUtility(string core, out string[]? invalidatedSubGroups)
    {
        invalidatedSubGroups = null;

        if (DisplayTokens.Contains(core))
            return "display";

        if (PositionTokens.Contains(core))
            return "position";

        if (FontWeightTokens.Contains(core))
            return "font-weight";

        if (TextAlignTokens.Contains(core))
            return "text-align";

        if (TextSizeTokens.Contains(core))
            return "text-size";

        // Padding hierarchy: p-* overrides px-*, py-*, pt-*, pr-*, pb-*, pl-*
        if (core.StartsWith("p-", StringComparison.Ordinal))
        {
            invalidatedSubGroups = new[] { "px", "py", "pt", "pr", "pb", "pl" };
            return "p";
        }
        if (core.StartsWith("px-", StringComparison.Ordinal))
        {
            invalidatedSubGroups = new[] { "pl", "pr" };
            return "px";
        }
        if (core.StartsWith("py-", StringComparison.Ordinal))
        {
            invalidatedSubGroups = new[] { "pt", "pb" };
            return "py";
        }
        if (core.StartsWith("pt-", StringComparison.Ordinal)) return "pt";
        if (core.StartsWith("pr-", StringComparison.Ordinal)) return "pr";
        if (core.StartsWith("pb-", StringComparison.Ordinal)) return "pb";
        if (core.StartsWith("pl-", StringComparison.Ordinal)) return "pl";

        // Margin hierarchy
        if (core.StartsWith("m-", StringComparison.Ordinal) || core.StartsWith("-m-", StringComparison.Ordinal))
        {
            invalidatedSubGroups = new[] { "mx", "my", "mt", "mr", "mb", "ml" };
            return "m";
        }
        if (core.StartsWith("mx-", StringComparison.Ordinal) || core.StartsWith("-mx-", StringComparison.Ordinal))
        {
            invalidatedSubGroups = new[] { "ml", "mr" };
            return "mx";
        }
        if (core.StartsWith("my-", StringComparison.Ordinal) || core.StartsWith("-my-", StringComparison.Ordinal))
        {
            invalidatedSubGroups = new[] { "mt", "mb" };
            return "my";
        }
        if (core.StartsWith("mt-", StringComparison.Ordinal) || core.StartsWith("-mt-", StringComparison.Ordinal)) return "mt";
        if (core.StartsWith("mr-", StringComparison.Ordinal) || core.StartsWith("-mr-", StringComparison.Ordinal)) return "mr";
        if (core.StartsWith("mb-", StringComparison.Ordinal) || core.StartsWith("-mb-", StringComparison.Ordinal)) return "mb";
        if (core.StartsWith("ml-", StringComparison.Ordinal) || core.StartsWith("-ml-", StringComparison.Ordinal)) return "ml";

        // Border radius
        if (core == "rounded" || core.StartsWith("rounded-", StringComparison.Ordinal))
        {
            if (core.StartsWith("rounded-t-", StringComparison.Ordinal) ||
                core.StartsWith("rounded-b-", StringComparison.Ordinal) ||
                core.StartsWith("rounded-l-", StringComparison.Ordinal) ||
                core.StartsWith("rounded-r-", StringComparison.Ordinal) ||
                core.StartsWith("rounded-tl-", StringComparison.Ordinal) ||
                core.StartsWith("rounded-tr-", StringComparison.Ordinal) ||
                core.StartsWith("rounded-bl-", StringComparison.Ordinal) ||
                core.StartsWith("rounded-br-", StringComparison.Ordinal))
            {
                return null;
            }
            return "rounded";
        }

        // Overflow
        if (core.StartsWith("overflow-x-", StringComparison.Ordinal)) return "overflow-x";
        if (core.StartsWith("overflow-y-", StringComparison.Ordinal)) return "overflow-y";
        if (core.StartsWith("overflow-", StringComparison.Ordinal))
        {
            invalidatedSubGroups = new[] { "overflow-x", "overflow-y" };
            return "overflow";
        }

        // Background color vs other bg utilities
        if (core.StartsWith("bg-", StringComparison.Ordinal))
        {
            foreach (var nonColor in BgNonColorPrefixes)
            {
                if (core.StartsWith(nonColor, StringComparison.Ordinal))
                    return null;
            }
            return "bg-color";
        }

        // Text color vs text size / wrap / overflow
        if (core.StartsWith("text-", StringComparison.Ordinal))
        {
            if (core is "text-wrap" or "text-nowrap" or "text-balance" or "text-pretty" or "text-ellipsis" or "text-clip")
                return "text-wrap";
            if (core.StartsWith("text-[", StringComparison.Ordinal) &&
                (core.EndsWith("px]", StringComparison.Ordinal) || core.EndsWith("rem]", StringComparison.Ordinal) || core.EndsWith("em]", StringComparison.Ordinal)))
            {
                return "text-size";
            }
            return "text-color";
        }

        // Border width vs color
        if (core == "border" || core is "border-0" or "border-2" or "border-4" or "border-8")
            return "border-width";
        if (BorderStyleTokens.Contains(core))
            return "border-style";
        if (core.StartsWith("border-", StringComparison.Ordinal))
        {
            if (core.StartsWith("border-t", StringComparison.Ordinal) ||
                core.StartsWith("border-b", StringComparison.Ordinal) ||
                core.StartsWith("border-l", StringComparison.Ordinal) ||
                core.StartsWith("border-r", StringComparison.Ordinal) ||
                core.StartsWith("border-x", StringComparison.Ordinal) ||
                core.StartsWith("border-y", StringComparison.Ordinal) ||
                core.StartsWith("border-spacing", StringComparison.Ordinal) ||
                core.StartsWith("border-collapse", StringComparison.Ordinal) ||
                core.StartsWith("border-separate", StringComparison.Ordinal))
            {
                return null;
            }
            return "border-color";
        }

        // Shadow
        if (core == "shadow" || core is "shadow-none" or "shadow-2xs" or "shadow-xs" or "shadow-sm" or "shadow-md" or "shadow-lg" or "shadow-xl" or "shadow-2xl" or "shadow-inner")
            return "shadow";

        // Sizing, Gap, Opacity, Z-Index
        if (core.StartsWith("w-", StringComparison.Ordinal)) return "width";
        if (core.StartsWith("h-", StringComparison.Ordinal)) return "height";
        if (core.StartsWith("min-w-", StringComparison.Ordinal)) return "min-width";
        if (core.StartsWith("min-h-", StringComparison.Ordinal)) return "min-height";
        if (core.StartsWith("max-w-", StringComparison.Ordinal)) return "max-width";
        if (core.StartsWith("max-h-", StringComparison.Ordinal)) return "max-height";
        if (core.StartsWith("gap-x-", StringComparison.Ordinal)) return "gap-x";
        if (core.StartsWith("gap-y-", StringComparison.Ordinal)) return "gap-y";
        if (core.StartsWith("gap-", StringComparison.Ordinal))
        {
            invalidatedSubGroups = new[] { "gap-x", "gap-y" };
            return "gap";
        }
        if (core.StartsWith("opacity-", StringComparison.Ordinal)) return "opacity";
        if (core.StartsWith("z-", StringComparison.Ordinal)) return "z-index";

        return null;
    }
}

/// <summary>
/// Fluent CSS class builder with built-in TwMerge conflict resolution.
/// </summary>
public sealed class TwClassBuilder
{
    private readonly List<string?> _parts = new(8);

    public static TwClassBuilder Create(string? baseClasses = null)
    {
        var builder = new TwClassBuilder();
        if (!string.IsNullOrWhiteSpace(baseClasses))
            builder._parts.Add(baseClasses);
        return this_init(builder);
    }

    private static TwClassBuilder this_init(TwClassBuilder b) => b;

    public TwClassBuilder Add(string? classes)
    {
        if (!string.IsNullOrWhiteSpace(classes))
            _parts.Add(classes);
        return this;
    }

    public TwClassBuilder AddIf(bool condition, string? trueClasses, string? falseClasses = null)
    {
        var chosen = condition ? trueClasses : falseClasses;
        if (!string.IsNullOrWhiteSpace(chosen))
            _parts.Add(chosen);
        return this;
    }

    public string Build() => TwMerge.Merge(_parts.ToArray());

    public override string ToString() => Build();
}

/// <summary>
/// Multi-part Slot classes for TwCard.
/// Allows targeting Base, Header, Title, Subtitle, Body, and Footer independently.
/// </summary>
public sealed class TwCardSlots
{
    public string? Base { get; set; }
    public string? Header { get; set; }
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public string? Body { get; set; }
    public string? Footer { get; set; }
}

/// <summary>
/// Multi-part Slot classes for TwButton.
/// </summary>
public sealed class TwButtonSlots
{
    public string? Base { get; set; }
    public string? LeadingIcon { get; set; }
    public string? TrailingIcon { get; set; }
    public string? Spinner { get; set; }
}

/// <summary>
/// Multi-part Slot classes for TwUser.
/// </summary>
public sealed class TwUserSlots
{
    public string? Base { get; set; }
    public string? AvatarWrapper { get; set; }
    public string? Avatar { get; set; }
    public string? StatusDot { get; set; }
    public string? Wrapper { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Multi-part Slot classes for TwHoverCard.
/// </summary>
public sealed class TwHoverCardSlots
{
    public string? Base { get; set; }
    public string? Trigger { get; set; }
    public string? Content { get; set; }
    public string? Arrow { get; set; }
}

/// <summary>
/// Multi-part Slot classes for TwFloatingLabelInput.
/// </summary>
public sealed class TwFloatingInputSlots
{
    public string? Base { get; set; }
    public string? Input { get; set; }
    public string? Label { get; set; }
    public string? Feedback { get; set; }
}
