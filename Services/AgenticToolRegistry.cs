namespace BlazorTwUI.Services;

using BlazorTwUI.Models;

public class AgenticToolRegistry
{
    private readonly List<AgentToolDefinition> _tools = new();

    public AgenticToolRegistry()
    {
        // 1. Frontend WebAssembly Tools
        _tools.Add(new AgentToolDefinition
        {
            Name = "tw_render_widget",
            DisplayName = "Render AGUI Dynamic Widget",
            Source = ToolSource.FrontendWasm,
            Description = "Synthesizes and mounts an adaptive reactive widget onto the client-side DOM tree.",
            Icon = "🎨",
            ParameterSchemas = new()
            {
                ["widgetType"] = "string (card | metric | alert | progress | table)",
                ["title"] = "string",
                ["accentColor"] = "string (blue | emerald | amber | purple | rose | cyan)"
            }
        });

        _tools.Add(new AgentToolDefinition
        {
            Name = "tw_theme_switch",
            DisplayName = "Client Theme & Accent Switcher",
            Source = ToolSource.FrontendWasm,
            Description = "Applies Tailwind CSS dark/light theme and color palette tokens directly in the browser.",
            Icon = "🌓",
            ParameterSchemas = new()
            {
                ["mode"] = "string (dark | light)",
                ["accent"] = "string (indigo | violet | cyan | emerald | amber | rose)"
            }
        });

        _tools.Add(new AgentToolDefinition
        {
            Name = "tw_toast_notify",
            DisplayName = "Dispatch UI Toast Notification",
            Source = ToolSource.FrontendWasm,
            Description = "Fires non-blocking accessible toast notifications in client WASM context.",
            Icon = "🔔",
            ParameterSchemas = new()
            {
                ["title"] = "string",
                ["message"] = "string",
                ["variant"] = "string (Primary | Success | Warning | Danger)"
            }
        });

        _tools.Add(new AgentToolDefinition
        {
            Name = "tw_confetti_burst",
            DisplayName = "Canvas Confetti Particle Explosion",
            Source = ToolSource.FrontendWasm,
            Description = "Executes client-side 60fps HTML5 canvas physics celebration particles.",
            Icon = "🎉",
            ParameterSchemas = new()
            {
                ["count"] = "number (particles)",
                ["spread"] = "number (degrees)"
            }
        });

        _tools.Add(new AgentToolDefinition
        {
            Name = "tw_form_populate",
            DisplayName = "Smart Form Field Hydration",
            Source = ToolSource.FrontendWasm,
            Description = "Injects extracted entities into target active form state with two-way binding.",
            Icon = "📝",
            ParameterSchemas = new()
            {
                ["formId"] = "string",
                ["fields"] = "object (key-value mapping)"
            }
        });

        // 2. Backend Simulated Tools
        _tools.Add(new AgentToolDefinition
        {
            Name = "sys_cloud_quota_check",
            DisplayName = "Cloud Infrastructure Quota Query",
            Source = ToolSource.BackendApi,
            Description = "Queries backend cloud cluster for active vCPU, RAM, container and ingress allocations.",
            Icon = "☁️",
            ParameterSchemas = new()
            {
                ["region"] = "string (us-east, eu-central, ap-northeast)",
                ["clusterId"] = "string"
            }
        });

        _tools.Add(new AgentToolDefinition
        {
            Name = "sys_financial_ticker",
            DisplayName = "High-Frequency Market Data Feed",
            Source = ToolSource.BackendApi,
            Description = "Retrieves live order-book depth, OHLC candlestick aggregates, and 24h volume.",
            Icon = "📈",
            ParameterSchemas = new()
            {
                ["ticker"] = "string (e.g. BTC-USD, NVDA, ETH)",
                ["timeframe"] = "string (1m, 5m, 1h, 1d)"
            }
        });

        _tools.Add(new AgentToolDefinition
        {
            Name = "sys_security_audit",
            DisplayName = "Zero-Trust Security & DSR Audit",
            Source = ToolSource.BackendApi,
            Description = "Scans tenant audit logs for anomalous egress, revoked OAuth tokens, and compliance postures.",
            Icon = "🛡️",
            ParameterSchemas = new()
            {
                ["tenantId"] = "string",
                ["severityThreshold"] = "string (low | medium | high | critical)"
            }
        });

        _tools.Add(new AgentToolDefinition
        {
            Name = "sys_db_query",
            DisplayName = "Distributed Database Read Query",
            Source = ToolSource.BackendApi,
            Description = "Executes parameterized read-only analytics query against distributed data lake.",
            Icon = "🗄️",
            ParameterSchemas = new()
            {
                ["collection"] = "string",
                ["limit"] = "number",
                ["sortBy"] = "string"
            }
        });

        _tools.Add(new AgentToolDefinition
        {
            Name = "sys_webhook_dispatch",
            DisplayName = "Secure Webhook Event Dispatcher",
            Source = ToolSource.BackendApi,
            Description = "Cryptographically signs and delivers HMAC-SHA256 event payloads to external endpoints.",
            Icon = "🚀",
            ParameterSchemas = new()
            {
                ["endpointUrl"] = "string",
                ["eventType"] = "string"
            }
        });
    }

    public IReadOnlyList<AgentToolDefinition> GetAllTools() => _tools;

    public IReadOnlyList<AgentToolDefinition> GetFrontendTools() =>
        _tools.Where(t => t.Source == ToolSource.FrontendWasm).ToList();

    public IReadOnlyList<AgentToolDefinition> GetBackendTools() =>
        _tools.Where(t => t.Source == ToolSource.BackendApi).ToList();

    public AgentToolDefinition? GetToolByName(string name) =>
        _tools.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public async Task<ToolCallExecution> ExecuteToolAsync(string toolName, Dictionary<string, object?> arguments)
    {
        var tool = GetToolByName(toolName);
        var exec = new ToolCallExecution
        {
            ToolName = toolName,
            DisplayName = tool?.DisplayName ?? toolName,
            Source = tool?.Source ?? ToolSource.FrontendWasm,
            Arguments = new(arguments),
            Status = ToolExecutionStatus.Executing
        };

        var sw = System.Diagnostics.Stopwatch.StartNew();
        // Simulate real latency
        var simulatedDelay = exec.Source == ToolSource.FrontendWasm ? Random.Shared.Next(8, 25) : Random.Shared.Next(65, 140);
        await Task.Delay(simulatedDelay);
        sw.Stop();

        exec.LatencyMs = Math.Round(sw.Elapsed.TotalMilliseconds, 1);
        exec.Status = ToolExecutionStatus.Completed;

        switch (toolName.ToLowerInvariant())
        {
            case "tw_render_widget":
                exec.Result = new { status = "mounted", domNodeId = "agui_root_" + Guid.NewGuid().ToString("N")[..6] };
                break;
            case "tw_theme_switch":
                exec.Result = new { mode = arguments.GetValueOrDefault("mode", "dark"), accent = arguments.GetValueOrDefault("accent", "indigo"), applied = true };
                break;
            case "tw_toast_notify":
                exec.Result = new { dispatched = true, queueLength = 1 };
                break;
            case "tw_confetti_burst":
                exec.Result = new { particleCount = arguments.GetValueOrDefault("count", 100), physicsEngine = "HTML5 Canvas" };
                break;
            case "tw_form_populate":
                exec.Result = new { fieldsPopulated = 5, validationPassed = true };
                break;
            case "sys_cloud_quota_check":
                exec.Result = new { vCpuAllocated = "38/48 (79.1%)", ramGb = "142/192 GB", activePods = 84, healthyNodes = 6 };
                break;
            case "sys_financial_ticker":
                exec.Result = new { ticker = arguments.GetValueOrDefault("ticker", "NVDA"), price = 142.85, change24h = "+4.12%", volume = "48.2M" };
                break;
            case "sys_security_audit":
                exec.Result = new { score = "98/100", compliance = "SOC-2 Type II + GDPR", activeAnomalies = 0 };
                break;
            case "sys_db_query":
                exec.Result = new { rowsReturned = 42, scanTimeMs = 4.2, cacheHit = true };
                break;
            case "sys_webhook_dispatch":
                exec.Result = new { httpStatus = 200, responseSignature = "hmac-sha256-verified", delivered = true };
                break;
            default:
                exec.Result = new { executed = true };
                break;
        }

        return exec;
    }
}
