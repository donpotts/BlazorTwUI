namespace BlazorTwUI.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorTwUI.Models;

/// <summary>
/// Bridges client-side AG-UI components with server-side AI execution hubs (Semantic Kernel / Ollama / OpenAI)
/// over SignalR or streaming HTTP endpoints in .NET 10+ Full-Stack Blazor apps.
/// </summary>
public class FullstackAgenticBridge
{
    private readonly AgenticToolRegistry _toolRegistry;

    public FullstackAgenticBridge(AgenticToolRegistry toolRegistry)
    {
        _toolRegistry = toolRegistry;
    }

    /// <summary>
    /// Simulates server-side streaming generation of an AG-UI form or widget schema from natural language prompt.
    /// </summary>
    public async IAsyncEnumerable<string> StreamAgentThoughtAndToolExecutionAsync(
        string userPrompt, 
        string targetDomain = "CloudOps")
    {
        yield return $"[Planner] Received prompt: \"{userPrompt}\"";
        await Task.Delay(80);

        yield return $"[SemanticKernel] Analyzing domain constraints for: {targetDomain}";
        await Task.Delay(100);

        yield return $"[ToolDispatch:Server] Executing policy lookup on backend authority...";
        await Task.Delay(120);

        yield return $"[Synthesizer] Compiling reactive AguiFormSchema with validation rules...";
        await Task.Delay(90);

        yield return $"[Completed] Form schema successfully delivered to client-side TwAguiFormCanvas.";
    }

    /// <summary>
    /// Evaluates if an agentic action requires server-side Human-In-The-Loop (HITL) gate before execution.
    /// </summary>
    public bool RequiresHumanApproval(string toolName, IDictionary<string, object> parameters)
    {
        if (toolName.Contains("Delete", StringComparison.OrdinalIgnoreCase) ||
            toolName.Contains("Provision", StringComparison.OrdinalIgnoreCase) ||
            toolName.Contains("Disburse", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (parameters.TryGetValue("Amount", out var amt) && amt is double d && d > 10000.0)
        {
            return true;
        }

        return false;
    }
}
