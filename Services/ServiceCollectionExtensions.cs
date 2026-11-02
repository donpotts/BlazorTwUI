namespace Microsoft.Extensions.DependencyInjection;

using BlazorTwUI.Models;
using BlazorTwUI.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class BlazorTwUIServiceExtensions
{
    /// <summary>
    /// Registers all BlazorTwUI core services including Theme, Audio FX, PWA, Form Engine, and Tool Registry.
    /// </summary>
    public static IServiceCollection AddBlazorTwUI(this IServiceCollection services)
    {
        services.AddScoped<ThemeService>();
        services.AddScoped<ToastService>();
        services.AddScoped<TwSoundService>();
        services.AddScoped<TwPwaService>();
        services.AddSingleton<ComponentCatalogService>();
        services.AddSingleton<FormCatalogService>();
        services.AddSingleton<AgenticToolRegistry>();
        services.AddScoped<OcrEngineService>();
        services.AddSingleton<TwHydrationService>();
        services.AddSingleton<FullstackAgenticBridge>();
        services.AddSingleton<TwDonutCacheService>();
        services.AddSingleton<TwZstandardService>();
        services.AddSingleton<TwPostQuantumService>();
        services.AddSingleton<TwOpenTelemetryService>();
        services.AddSingleton<TwSimdVectorService>();
        services.AddScoped<TwAspireService>();
        services.AddScoped<ServerPortBridgeService>();
        // Default offline agent; register your own IAgentTransport before this call to drive a live agent.
        services.TryAddScoped<IAgentTransport, SimulatedAgentTransport>();
        // Conversation threads, mirrored to localStorage.
        services.TryAddScoped<AgentThreadStore>();
        services.TryAddScoped<IAgentThreadStore>(sp => sp.GetRequiredService<AgentThreadStore>());

        return services;
    }
}
