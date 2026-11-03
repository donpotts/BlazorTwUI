# BlazorTwUI — Help, Quick Reference & Troubleshooting Guide

This guide is designed for developers, architects, and teams building and maintaining modern Blazor WebAssembly applications with **BlazorTwUI**.

---

## 💡 Why It's So Easy to Understand & Use

BlazorTwUI was built from day one with a simple philosophy: **Zero Friction, Pure C#, and Standard Web Standards**. You don't need to learn a custom DSL, decipher minified wrapper code, or troubleshoot external CDN outages.

### 1. The 60-Second Onboarding Flow
Scaffold, build, and run in 3 quick commands:
```bash
# 1. Install the official .NET template once
dotnet new install https://github.com/donpotts/BlazorTwUI.git

# 2. Create your project
dotnet new blazortw -n MyDashboard

# 3. Launch the app
cd MyDashboard && dotnet run
```

### 2. 1-Line Dependency Injection
In your `Program.cs`, a single line wires up everything:
```csharp
builder.Services.AddBlazorTwUI();
```
This registers:
- `TwSoundService`: Procedural sound synthesizer (0 KB audio downloads, 100% royalty-free).
- `TwPwaService`: PWA installability, update notifications, and offline status.
- `AgenticToolRegistry`: Dual-plane agentic tool execution engine for AI workflows.
- `FormCatalogService`: 100 enterprise form schemas across 10 functional domains.

### 3. Predictable Component Design Patterns
Every component follows the same familiar C# conventions:
- **Two-way Data Binding**: Use standard `@bind-Value` on inputs, sliders, toggles, and dropdowns.
- **Consistent Sizing & Variants**: Components use standard `Size` (`Sm`, `Md`, `Lg`) and `Variant` (`Primary`, `Secondary`, `Outline`, `Danger`, `Ghost`) enums.
- **Direct Tailwind Customization**: Pass any utility class through `Class="my-custom-class"` without fighting component-level `!important` CSS rules.
- **Self-Contained Icons & Emojis**: Render crisp vector icons with `<TwIcon Name="TwIconType.Bolt" Size="20" />` without font files or SVG sprite sheets.

### 4. Interactive Learning Built In
- Press <kbd>Ctrl+K</kbd> anywhere in the application to instantly find any component or action.
- Press <kbd>Shift+T</kbd> to launch the step-by-step interactive onboarding tour.
- Press <kbd>Shift+V</kbd> to watch the offline 1080p video masterclass.

---

## 1. Quick Reference & Cheatsheet

### Global Keyboard Shortcuts
| Shortcut | Action | Description |
|---|---|---|
| <kbd>⌘K</kbd> / <kbd>Ctrl+K</kbd> | Open Command Palette | Global quick navigation, action execution, and component search. |
| <kbd>Shift+V</kbd> | Watch 1080p Video Tour | Opens full HD video masterclass modal with chapter scrubber. |
| <kbd>Shift+T</kbd> | Launch Coaching Tour | Starts 8-step guided interactive walkthrough. |
| <kbd>Shift+C</kbd> | Fire Celebration Confetti | Triggers celebratory canvas particle burst. |
| <kbd>Esc</kbd> | Close Modal / Drawer | Cleanly dismisses any active overlay dialog or drawer. |

---

## 2. Common Recipes

### Recipe A: Adding a Draggable Before/After Image Comparison Slider
```razor
@using BlazorTwUI.Components.UI

<TwImageCompare BeforeSrc="images/compare-before.jpg"
                AfterSrc="images/compare-after.jpg"
                BeforeLabel="Original"
                AfterLabel="Enhanced"
                @bind-Position="_sliderPos"
                Class="aspect-video max-h-[450px]" />

<div class="mt-2 text-xs text-slate-500">
    Position: @_sliderPos.ToString("F0")%
</div>

@code {
    private double _sliderPos = 50.0;
}
```

### Recipe B: Continuous Infinite Logo / Tech Marquee
```razor
@using BlazorTwUI.Components.UI

<TwMarquee DurationSeconds="20" PauseOnHover="true" FadeEdges="true">
    <div class="flex items-center gap-2 px-4 py-2 rounded-xl bg-slate-100 dark:bg-slate-800">
        <span>⚡</span>
        <span class="font-bold text-sm">Blazor WASM .NET 10+</span>
    </div>
    <div class="flex items-center gap-2 px-4 py-2 rounded-xl bg-slate-100 dark:bg-slate-800">
        <span>🎨</span>
        <span class="font-bold text-sm">Tailwind CSS v3.4+</span>
    </div>
</TwMarquee>
```

### Recipe C: Digital Canvas Signature Pad with PNG Export
```razor
@using BlazorTwUI.Components.UI

<TwSignaturePad Title="Customer Verification Signature"
                Height="220"
                ShowGuide="true"
                OnSave="HandleSignatureSaved" />

@code {
    private string? _savedSignaturePng;

    private void HandleSignatureSaved(string dataUrl)
    {
        _savedSignaturePng = dataUrl;
        // Submit dataUrl or save to backend API
    }
}
```

### Recipe D: Vector QR Code & Code 128 Barcode Generation
```razor
@using BlazorTwUI.Components.UI

<!-- 2D QR Code with Custom Center Icon & Rounded Dots -->
<TwQrCode Value="https://example.com/checkout"
          Title="Secure Payment QR"
          Size="200"
          DotShape="TwQrCode.QrDotShape.Rounded"
          CenterIcon="TwQrCode.QrCenterIcon.Bolt" />

<!-- 1D Logistics Barcode -->
<TwBarcode Value="PKG-982410-USA"
           Title="Tracking Code 128"
           BarHeight="64"
           ShowText="true" />
```

### Recipe E: Celebrating Milestones with Canvas Confetti
```razor
@using BlazorTwUI.Components.UI

<TwButton Variant="ButtonVariant.Primary" OnClick="Celebrate">
    Submit Order 🎉
</TwButton>

<TwConfetti @ref="_confetti" FullScreen="true" />

@code {
    private TwConfetti? _confetti;

    private async Task Celebrate()
    {
        // Process logic...
        if (_confetti != null)
        {
            await _confetti.FireAsync(0.5, 0.4, 120);
        }
    }
}
```

---


### Recipe F: Volumetric 3D Spatial Cards with Gyroscopic Parallax
```razor
@using BlazorTwUI.Components.UI

<TwSpatialCard Title="Quantum Enclave"
               Subtitle="Hardware Security Module • Tier 0 Root"
               BadgeText="Volumetric 3D"
               MaxTiltDeg="14"
               GlowGradient="from-brand-500 via-indigo-600 to-purple-600">
    <p class="text-xs text-slate-600 dark:text-slate-300">
        Move your cursor across this card to experience gyroscopic 3D tilt, specular refraction, and multi-layer Z-depth elevation.
    </p>
</TwSpatialCard>
```

### Recipe G: Anticipatory Intent & Biometric State Engine
```razor
@using BlazorTwUI.Components.UI

<!-- 2027 Anticipatory State Machine ("States over Pages") -->
<TwPredictiveIntent />
```

### Recipe H: Organic Fluid Emotional Mesh with Psychological State Tuning
```razor
@using BlazorTwUI.Components.UI

<TwFluidMesh @bind-Mood="_operatorMood" />

@code {
    private TwFluidMesh.EmotionalState _operatorMood = TwFluidMesh.EmotionalState.DeepFocus;
}
```

### Recipe I: Enterprise Dual-List Transfers with TwPickList
```razor
@using BlazorTwUI.Components.UI

<TwPickList TItem="string"
            Source="_availableRoles"
            @bind-Target="_assignedRoles"
            SourceTitle="Available Roles"
            TargetTitle="Granted Roles"
            AllowFiltering="true"
            ShowMoveAll="true"
            ShowReorder="true"
            Height="h-60"
            OnTransfer="HandleRolesChanged" />

@code {
    private List<string> _availableRoles = new() { "SecOps Lead", "Kubernetes Admin", "Auditor" };
    private IList<string> _assignedRoles = new List<string> { "DevOps Engineer" };

    private void HandleRolesChanged()
    {
        // Reactively regenerate IAM manifests or sync permissions
    }
}
```

### Recipe J: Enterprise Cloud File Manager with Live File Inspector
```razor
@using BlazorTwUI.Components.UI

<TwFileManager Items="_files"
               @bind-CurrentPath="_activePath"
               OnFileSelected="HandleFileSelected"
               OnUpload="HandleFileUpload" />

@code {
    private string _activePath = "/";
    private List<TwFileManager.FileItem> _files = new()
    {
        new(Guid.NewGuid().ToString("N"), "Kubernetes-Config.json", "/", 15200, DateTime.Now, false, "application/json", ".json")
    };

    private void HandleFileSelected(TwFileManager.FileItem file)
    {
        // Populate live inspector with SHA-256 and content preview
    }

    private void HandleFileUpload()
    {
        // Ingest new file into active directory
    }
}
```

### Recipe K: Throughput Sankey Flow Diagram with Traffic Surge
```razor
@using BlazorTwUI.Components.UI

<TwSankeyChart Nodes="_nodes"
               Links="_links"
               Title="Workload Telemetry Flow"
               OnNodeClick="HandleNodeClick" />

@code {
    private List<TwSankeyChart.SankeyNode> _nodes = new()
    {
        new("edge", "Edge Ingress", "#6366f1"),
        new("cache", "Memory Cache", "#10b981")
    };
    private List<TwSankeyChart.SankeyLink> _links = new()
    {
        new("edge", "cache", 50000)
    };

    private void HandleNodeClick(TwSankeyChart.SankeyNode node)
    {
        // Inspect node telemetry metrics
    }
}
```

---

### Recipe L: PDF Form Filler, Contract Signer & Stream Flattening
```razor
@using BlazorTwUI.Components.UI

<TwPdfFormFiller />
```
*Capabilities*: Spatial "Sign Here" / "Initial Here" callout flags, 1-click UTC/Local timestamp stamping, cryptographic SHA-256 seal, AcroForm stream flattening into immutable vector typography, zero-dependency PDF 1.4 generation, and native `window.print()` support.

### Recipe M: Mobile-First Draggable & Resizable CRM Dashboard
```razor
@using BlazorTwUI.Components.UI

<TwDashboard StorageKey="my-enterprise-dashboard"
             Title="Executive CRM Dashboard"
             Subtitle="Draggable, resizable, persistent, customizable cards"
             IsLocked="false" />
```
*Capabilities*: 12-column responsive GridStack layout, persistent `localStorage` serialization, dedicated `⠿ .move-handle` touch drag handles with `touch-action: none` and `cursor: grab`, 1-tap mobile positioning buttons, in-place card property customizer, 1-click compact, JSON export/import modal, and built-in pure SVG charts (Sales Line, Lead Doughnut, Funnel Bar, Opportunity Deals table, Activity Feed, Scratchpad).

---

### Recipe N: C# 15 Discriminated Union Boundary & Closed State Machines
```razor
@using BlazorTwUI.Components.UI
@using BlazorTwUI.Models

<TwUnionBoundary TItem="List<Invoice>" State="@invoiceState" OnRetry="FetchInvoicesAsync">
    <Loading>
        <div class="p-6 rounded-2xl bg-slate-100 dark:bg-slate-800 animate-pulse">Loading ledger...</div>
    </Loading>
    <Success Context="invoices">
        <InvoicesTable Items="@invoices" />
    </Success>
    <Empty>
        <div class="p-8 text-center text-slate-500">No invoices found.</div>
    </Empty>
    <Failure Context="err">
        <div class="p-4 rounded-xl bg-rose-50 text-rose-800">Error: @err.Message</div>
    </Failure>
</TwUnionBoundary>
```
*Capabilities*: Type-safe algebraic UI state lifecycle modeling (`UiState<T>`: `Loading`, `Success`, `Empty`, `Failure`), compiler exhaustiveness enforcement, animated pulse skeleton shimmers, failure diagnostic inspection, and automated 1-click retry dispatch.

---

### Recipe O: Grouped Category Hubs, Real-Time Navigation Filter & In-Page Jump Anchors
```razor
<!-- In-Sidebar Keystroke Real-Time Filter -->
<div class="px-1 mb-3">
    <div class="relative">
        <input type="text"
               @bind="_navSearchQuery"
               @bind:event="oninput"
               placeholder="Filter navigation..."
               class="w-full pl-8 pr-7 py-1.5 text-xs rounded-xl bg-slate-100 dark:bg-slate-800/80 ..." />
    </div>
</div>

<!-- Sticky 1-Tap Anchor Jump Bar -->
<div data-testid="jump-bar" class="sticky top-16 z-20 py-2.5 px-3 rounded-2xl bg-white/90 ...">
    <button type="button" @onclick='() => SwitchOrScroll("components", "studio-dashboard")'>CRM Dashboard</button>
    <button type="button" @onclick='() => SwitchOrScroll("components", "studio-pdf")'>PDF Form Maker</button>
    <button type="button" @onclick='() => SwitchOrScroll("components", "studio-union")'>C# 15 Union</button>
</div>
```
*Capabilities*: Transforms long flat navigation menus into 5 organized Category Hubs (🚀 Flagship Studios, ⚡ Architecture & AI, 🏢 Enterprise & Business, 🧩 UI Kit Catalog, 🎓 Resources & Docs), real-time query filtering across titles and badges, smooth-scrolling in-page anchors with `scroll-mt-24` offset, and dedicated workspace modes for flagship studios.

---

### Recipe P: Native OpenTelemetry Component Tracing & APM Cockpit
```razor
<!-- Real-Time Header APM Health Indicator -->
<TwActivityMeter OnOpenCockpit="OpenTelemetryCockpit" />

<!-- In-Page APM Telemetry Cockpit -->
<TwTelemetryDashboard />

@code {
    @inject TwOpenTelemetryService OtelService

    private void TraceRenderOperation()
    {
        // Automatic sub-millisecond execution span with memory delta tracking
        using (OtelService.StartSpan("OrderGrid", "ComputeSummary", "Render", new() { ["tenant.id"] = "corp-1" }))
        {
            // Critical rendering or data processing work
        }
    }
}
```
*Capabilities*: Sub-millisecond Blazor component lifecycle tracing via `ActivitySource("BlazorTwUI.Components", "0.7.0")`, pure SVG latency waterfalls with microsecond precision, component performance leaderboard with slow frame warnings (>33ms), real-time WASM heap & GC Generational counters (Gen 0/1/2), and 1-click W3C OTLP JSON export formatted for Jaeger, Grafana Tempo, and Dynatrace.

---

## 3. Desktop Native Installers (Windows MSI & EXE)

BlazorTwUI includes official desktop installers built using Tauri v2, providing native Windows desktop app distribution:

| Installer | Description | Location |
|---|---|---|
| **`BlazorTwUI-0.7.0-x64-Setup.exe`** | NSIS self-extracting installer with desktop shortcuts and silent flags. | `release-packages/BlazorTwUI-0.7.0-x64-Setup.exe` |
| **`BlazorTwUI-0.7.0-x64-en-US.msi`** | WiX toolset enterprise MSI installer supporting Group Policy deployment. | `release-packages/BlazorTwUI-0.7.0-x64-en-US.msi` |
| **`BlazorTwUI-WebDemo-v0.7.0.zip`** | Standalone production WebAssembly bundle for offline/on-premise hosting. | `release-packages/BlazorTwUI-WebDemo-v0.7.0.zip` |

Each release also ships `SHA256SUMS.txt`; verify a download with `sha256sum -c SHA256SUMS.txt` (or `Get-FileHash` on Windows). The v0.7.0 installers were smoke-tested: the desktop app launches, and the WebDemo bundle passes the full browser E2E suite.

### Building Installers from Source:
```bash
# 1. Compile Tailwind CSS and WebAssembly dist bundle
npm run build:dist

# 2. Package Windows MSI and NSIS installers via Tauri CLI
npx tauri build
```
The compiled bundles are automatically placed in `src-tauri/target/release/bundle/msi` and `nsis`.

---

## 4. Troubleshooting & FAQs

### Q: Why do I see an HTTP 416 (Range Not Satisfiable) error on video playback?
**Cause**: When replacing a video or media file on disk while the ASP.NET Core DevServer is running, the server caches the previous file size in memory. When the browser makes a byte-range request for the old file length, a 416 status occurs.  
**Fix**: Restart the development server (`dotnet run`) after changing static media files in `wwwroot/`.

### Q: Why does the Guided Coaching Tour show a blank modal when launched a second time?
**Cause**: When `TwTour` finishes, its internal step index is at the end. Without a lifecycle hook, reopening leaves the step index at the terminal state.  
**Fix**: `TwTour.razor` implements `OnParametersSet()` to automatically reset `CurrentStepIndex = 0` whenever `IsOpen` transitions from `false` to `true`.

### Q: How do I compile new custom Tailwind classes?
Run the standalone Tailwind CLI:
```bash
npm run build:css
```
This scans all `.razor` and `.cs` files and compiles minified CSS to `wwwroot/css/app.min.css`.

---

## 5. Architectural Rules Summary
1. **Zero Prohibited Commercial Brands**: Strictly vendor-neutral.
2. **Strict .NET 10+ WebAssembly**: No server-side rendering dependencies for components.
3. **Pure Mobile-First**: 44px min touch targets, adaptive card fallbacks, and safe-area padding.

---

## 6. Page Templates & UI Building Blocks Guide

BlazorTwUI v0.8.0+ introduces higher-order UX composition through **Page Templates** and **UI Building Blocks**. Instead of building complex screens from scratch, developers can instantiate complete turnkey solutions or combine individual section blocks.

### 1. Using Turnkey Page Templates

Turnkey templates provide full-page assemblies complete with responsive layouts, KPI metrics, and native SVG visualizations:

```razor
@using BlazorTwUI.Components.Templates.Pages

<!-- 1. Fintech & Payments Treasury Cockpit -->
<FintechAnalyticsDashboard />

<!-- 2. AI Operations & Agent Control Plane -->
<AiOperationsTokenMonitor />

<!-- 3. SaaS Product Marketing Landing -->
<SaaSProductLanding />
```

### 2. Composing Sectional Building Blocks

Section blocks can be placed anywhere in your Blazor pages:

#### A. Compact KPI Stat Cards with SVG Sparklines
```razor
@using BlazorTwUI.Components.Templates.Blocks

<div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
    <TwCompactStatCard Title="Gross Volume"
                       Value="$14.8M"
                       Caption="+18.4% vs Q3"
                       Delta="+18.4%"
                       IsPositive="true"
                       SparklineColor="#4f46e5"
                       SparklineData="@([10.2, 11.5, 12.1, 13.4, 13.8, 14.8])" />
</div>
```

#### B. 3-Tier Interactive Pricing Matrix
```razor
<TwPricingSection Title="Simple, Transparent Plans"
                  OnSelectPlan="HandlePlanSelected" />
```

#### C. AI Prompt Box with Dual-Plane Dispatch
```razor
<TwPromptComposer Placeholder="Describe a task for the agent..."
                  SuggestedPrompts="@([\"Re-balance treasury\", \"Check token latency\"])"
                  OnSend="HandlePromptSubmit" />
```

#### D. Vertical Audit & Event Stream
```razor
<TwActivityTimeline Title="Live Audit Trail"
                    Events="@_systemEvents" />
```

### 3. Interactive Templates Gallery

Preview and test all templates across Desktop, Tablet, and Mobile viewport frames by rendering the gallery:

```razor
@using BlazorTwUI.Components.Templates

<TemplatesGallery />
```
Or open the **Templates** tab directly on the home page.
