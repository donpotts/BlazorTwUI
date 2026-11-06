using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace BlazorTwUI.Tests;

/// <summary>
/// Every third-party library that ships inside wwwroot (or is a runtime npm dependency) must have a
/// "## Name" section in THIRD_PARTY_NOTICES.md, so a bundle that redistributes the kit keeps the licenses.
/// </summary>
public class NoticesTests
{
    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "BlazorTwUI.csproj"))) dir = dir.Parent;
        return dir?.FullName ?? throw new InvalidOperationException("Repo root not found.");
    }

    // Vendored single files in wwwroot/js whose name is not the library name.
    private static readonly Dictionary<string, string> VendoredJsFiles = new(StringComparer.OrdinalIgnoreCase)
    {
        ["zxing.min.js"] = "ZXing",
    };

    // Runtime npm dependencies that are test tooling and never ship.
    private static readonly HashSet<string> NotShipped = new(StringComparer.OrdinalIgnoreCase) { "playwright" };

    [Fact]
    public void Every_vendored_library_has_a_notice()
    {
        var root = RepoRoot();
        var notices = File.ReadAllText(Path.Combine(root, "THIRD_PARTY_NOTICES.md"));
        var headings = Regex.Matches(notices, @"^##\s+(.+?)\s*$", RegexOptions.Multiline)
            .Select(m => m.Groups[1].Value).ToList();

        var expected = new List<string>();

        foreach (var dir in Directory.GetDirectories(Path.Combine(root, "wwwroot", "lib")))
            expected.Add(Path.GetFileName(dir));

        foreach (var file in Directory.GetFiles(Path.Combine(root, "wwwroot", "js"), "*.min.js"))
            expected.Add(VendoredJsFiles.TryGetValue(Path.GetFileName(file), out var n) ? n : Path.GetFileName(file));

        using (var pkg = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "package.json"))))
            if (pkg.RootElement.TryGetProperty("dependencies", out var deps))
                foreach (var d in deps.EnumerateObject())
                    if (!NotShipped.Contains(d.Name)) expected.Add(d.Name.StartsWith("@") ? d.Name.TrimStart((char)64).Split((char)47)[0] : d.Name);

        var missing = expected.Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(e => !headings.Any(h => h.Contains(e, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        Assert.True(missing.Count == 0, "THIRD_PARTY_NOTICES.md has no '## ' section for: " + string.Join(", ", missing));
    }
}
