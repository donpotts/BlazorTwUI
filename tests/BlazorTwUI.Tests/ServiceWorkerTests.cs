using Xunit;

namespace BlazorTwUI.Tests;

/// <summary>
/// The demo's service worker must never serve a stale Blazor boot: a cached _framework file fails Subresource Integrity
/// after a rebuild ("Failed to find a valid digest") and the app never starts.
/// </summary>
public class ServiceWorkerTests
{
    private static string Worker()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "BlazorTwUI.csproj"))) dir = dir.Parent;
        return File.ReadAllText(Path.Combine(dir!.FullName, "wwwroot", "service-worker.js"));
    }

    [Fact]
    public void Framework_files_are_never_cached_by_the_worker() =>
        Assert.Contains("if (url.pathname.includes('/_framework/')) return;", Worker());

    [Fact]
    public void Localhost_development_bypasses_the_worker_and_clears_old_caches()
    {
        var js = Worker();
        Assert.Contains("const IS_DEV", js);
        Assert.Contains("if (IS_DEV) return; // development", js);
        Assert.Contains("IS_DEV || key !== CACHE_NAME", js);
    }

    [Fact]
    public void A_cache_miss_offline_returns_a_response_not_undefined() =>
        Assert.DoesNotContain(".catch(() => caches.match(event.request))\n", Worker().Replace("\r\n", "\n"));
}
