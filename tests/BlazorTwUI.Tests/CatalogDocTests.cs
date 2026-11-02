using System.Text;
using BlazorTwUI.Models;
using BlazorTwUI.Services;
using Xunit;

namespace BlazorTwUI.Tests;

/// <summary>
/// docs/COMPONENT_CATALOG.md is generated from <see cref="ComponentCatalogService"/>, the single source of truth.
/// To regenerate after changing the catalog:
///   UPDATE_CATALOG_DOC=1 dotnet test --filter CatalogDocTests      (PowerShell: $env:UPDATE_CATALOG_DOC=1)
/// </summary>
public class CatalogDocTests
{
    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "BlazorTwUI.csproj"))) dir = dir.Parent;
        return dir?.FullName ?? throw new InvalidOperationException("Repo root not found.");
    }

    private static string DocPath() => Path.Combine(RepoRoot(), "docs", "COMPONENT_CATALOG.md");

    private static string BaseName(string name)
    {
        var i = name.IndexOf('<');
        return i < 0 ? name : name[..i];
    }

    private static string? Locate(string root, string name)
    {
        var baseName = BaseName(name);
        foreach (var pattern in new[] { baseName + ".razor", baseName + ".cs" })
        {
            var hit = Directory.EnumerateFiles(root, pattern, SearchOption.AllDirectories)
                .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")
                         && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                         && !f.Contains($"{Path.DirectorySeparatorChar}node_modules{Path.DirectorySeparatorChar}")
                         && !f.Contains($"{Path.DirectorySeparatorChar}tests{Path.DirectorySeparatorChar}"))
                .OrderBy(f => f.Length)
                .FirstOrDefault();
            if (hit is not null) return Path.GetRelativePath(root, hit).Replace('\\', '/');
        }
        return null;
    }

    // GitHub heading anchors: lowercase, drop punctuation, spaces become hyphens ("&" leaves a double hyphen).
    private static string Anchor(string heading)
    {
        var sb = new StringBuilder();
        foreach (var c in heading.ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(c) || c == '-') sb.Append(c);
            else if (c == ' ') sb.Append('-');
        }
        return sb.ToString();
    }

    private static string Cell(string s) => s.Replace("|", "\\|").Replace("\r", " ").Replace("\n", " ");

    public static string Generate(IReadOnlyList<ComponentDocItem> items, string root)
    {
        var groups = items.GroupBy(i => i.Category).OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase).ToList();
        var sb = new StringBuilder();

        sb.Append("# BlazorTwUI — Component Catalog & Settings Reference\n\n");
        sb.Append($"Comprehensive guide to all **{items.Count} components** in **BlazorTwUI** (.NET 10+ WebAssembly, Tailwind CSS v3.4+, TwMerge conflict engine, and multi-part TwSlots).\n\n");
        sb.Append("> This file is generated from `Services/ComponentCatalogService.cs`. Do not edit it by hand; change the catalog and regenerate with `UPDATE_CATALOG_DOC=1 dotnet test --filter CatalogDocTests`. A test fails when this file is out of date.\n\n");
        sb.Append("---\n\n## Table of Contents\n\n");
        foreach (var g in groups)
            sb.Append($"- [{g.Key}](#{Anchor(g.Key)}) ({g.Count()} component{(g.Count() == 1 ? "" : "s")})\n");
        sb.Append("\n---\n");

        foreach (var g in groups)
        {
            sb.Append($"\n## {g.Key}\n");
            foreach (var item in g)
            {
                sb.Append($"\n### `{item.Name}`\n\n");
                if (Locate(root, item.Name) is { } path)
                    sb.Append($"**Location:** [`{path}`](../{path})\n\n");
                sb.Append(item.Description.Trim()).Append("\n\n");

                sb.Append("#### Parameters & Settings\n\n");
                if (item.Parameters.Count == 0)
                {
                    sb.Append("This component has no parameters.\n\n");
                }
                else
                {
                    sb.Append("| Parameter | Type | Default | Description |\n|---|---|---|---|\n");
                    foreach (var p in item.Parameters)
                        sb.Append($"| `{Cell(p.Name)}` | `{Cell(p.Type)}` | `{Cell(p.DefaultValue)}` | {Cell(p.Description)} |\n");
                    sb.Append('\n');
                }

                sb.Append("#### Razor Usage Example\n\n```razor\n").Append(item.RazorSnippet.Replace("\r\n", "\n")).Append("\n```\n\n---\n");
            }
        }
        return sb.ToString();
    }

    private static string Normalize(string s) => s.Replace("\r\n", "\n").TrimEnd() + "\n";

    [Fact]
    public void ComponentCatalogDoc_IsUpToDate_WithTheCatalog()
    {
        var generated = Generate(new ComponentCatalogService().GetAll(), RepoRoot());

        if (Environment.GetEnvironmentVariable("UPDATE_CATALOG_DOC") == "1")
        {
            File.WriteAllText(DocPath(), generated, new UTF8Encoding(false));
            return;
        }

        var current = Normalize(File.ReadAllText(DocPath()));
        Assert.True(current == Normalize(generated),
            "docs/COMPONENT_CATALOG.md is out of date. Regenerate it: UPDATE_CATALOG_DOC=1 dotnet test --filter CatalogDocTests");
    }

    [Fact]
    public void GeneratedDoc_CoversEveryComponent_WithAccurateCounts()
    {
        var items = new ComponentCatalogService().GetAll();
        var doc = Generate(items, RepoRoot());

        Assert.Contains($"all **{items.Count} components**", doc);
        foreach (var item in items)
            Assert.Contains($"### `{item.Name}`", doc);

        // Every table-of-contents entry points at a real heading.
        foreach (var category in items.Select(i => i.Category).Distinct())
        {
            Assert.Contains($"- [{category}](#{Anchor(category)})", doc);
            Assert.Contains($"\n## {category}\n", doc);
        }
    }
}
