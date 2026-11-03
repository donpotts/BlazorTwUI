# BlazorTwUI

<p align="center">
  <img src="docs/images/agui-studio-cloud-quota.png" alt="BlazorTwUI" width="840" style="border-radius: 12px; box-shadow: 0 10px 30px rgba(0,0,0,0.15);" />
</p>

<p align="center">
  <strong>A modern, mobile-first component library and Adaptive Generative UI (AGUI) engine for Blazor WebAssembly and Tailwind CSS.</strong>
</p>

<p align="center">
  <a href="https://dotnet.microsoft.com/"><img src="https://img.shields.io/badge/.NET-10%2B_WASM-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt=".NET 10+" /></a>
  <a href="https://tailwindcss.com/"><img src="https://img.shields.io/badge/Tailwind_CSS-v3.4+-06B6D4?style=flat-square&logo=tailwindcss&logoColor=white" alt="Tailwind CSS" /></a>
  <a href="CHANGELOG.md"><img src="https://img.shields.io/badge/Release-v0.8.2-10B981?style=flat-square" alt="Release v0.8.2" /></a>
  <a href="docs/COMPONENT_CATALOG.md"><img src="https://img.shields.io/badge/Components-259_Ready-3B82F6?style=flat-square" alt="259 Components" /></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-F59E0B?style=flat-square" alt="MIT License" /></a>
</p>

<p align="center">
  <a href="#-quick-start">Quick Start</a> •
  <a href="#-component-examples">Examples</a> •
  <a href="#-component-catalog-259-components">Component Catalog</a> •
  <a href="docs/COMPONENT_CATALOG.md">Full API Docs</a> •
  <a href="wwwroot/videos/hyperframes_masterclass.html">1080p Video Tour</a> •
  <a href="wwwroot/videos/hyperframes_showoff_tour.html">4K Video Showcase</a>
</p>

---

## ⚡ Features

- **Turnkey Page Templates & UI Building Blocks** — Modular, slot-composable full-page applications and 50+ section blocks, including the Fintech & Payments Cockpit, AI Agent Telemetry & Control Plane, and SaaS Marketing Suite. 100% native SVG math visualizations with zero heavy graphing dependencies.
- **259 Components & Deterministic Styling Engine** — Built for .NET 10 Blazor WebAssembly and Tailwind CSS v3.4+, featuring `TwMerge` utility conflict resolution, `TwClassBuilder`, multi-part `TwSlots`, full-featured 4-Axis OLAP `TwPivotTable`, and complete Master-Detail nested grids & split-pane studios, with self-hosted local assets for GridStack, Tesseract OCR, and ZXing barcode scanning (zero external CDN dependencies).
- **Mobile-First by Default** — 44px touch targets, bottom sheets, swipe gestures, and responsive DataGrid mobile cards.
- **Adaptive Generative UI (AGUI)** — Runtime prompt-to-UI synthesis with dual-plane tool calling (Frontend WASM + Backend API).
- **Agent Ops (human-in-the-loop)** — A transport-agnostic agent session with approval gates that genuinely block high-risk tool calls, takeover and hand-back, an editable shared-state inspector and an exportable audit trail. See [Agent Ops](#-agent-ops-approvals-takeover--audit).
- **Live AG-UI over SSE + Agent Kit** — `SseAgentTransport` talks to a real AG-UI agent over server-sent events and reports approval decisions back so the server waits at the gate. Includes 26 agent-native components (thread list, A2UI generative UI, sandboxed MCP apps, voice, live screen, handoff, grants, memory, routines) and core inputs (calendar, time picker, radio group, text area). See [Agent Kit](#-agent-kit-generative-ui-voice-threads-and-the-agent-control-plane).
- **Accessible by Default** — High-contrast mode (toggle or OS `prefers-contrast`), `forced-colors` support, reduced motion, RTL-safe layout, and axe-core WCAG 2.x A/AA checks in the browser tests.
- **Zero-FOUC Theme Engine** — Instant dark mode and 12 curated color presets (Indigo, Emerald, Violet, Rose, Cyan, Amber, etc.) with custom hex palette generation.
- **Enterprise Flagships** — CRM Draggable Dashboard (`GridStack`), Flattened PDF Contract Signer with SHA-256 sealing, C# 15 Discriminated Union boundary, 100% offline Tesseract WebAssembly OCR (`wwwroot/lib/tesseract/`), 150+ NACHA Bank Routing resolver, and ChoETL.NACHA 94-character fixed-width ACH exporter.

---

## 🚀 Quick Start

### 1. Register Services
Add BlazorTwUI services in `Program.cs`:

```csharp
builder.Services.AddBlazorTwUI();
```

### 2. Include Assets
Add the stylesheet and scripts to `wwwroot/index.html`:

```html
<link rel="stylesheet" href="css/app.min.css" />
<script src="js/theme.js"></script>
<script src="js/kanban.js"></script>
```

### 3. Import Namespaces
Add to your `_Imports.razor`:

```razor
@using BlazorTwUI.Components.UI
@using BlazorTwUI.Models
```

---

## 💡 Component Examples

### Buttons & Badges
```razor
<TwButton Variant="ButtonVariant.Primary" Size="ComponentSize.Md" OnClick="HandleSubmit">
    Submit Request
</TwButton>

<TwBadge Variant="SemanticVariant.Success" Size="ComponentSize.Sm" ShowDot="true">
    Active
</TwBadge>
```

### Responsive DataGrid
```razor
<TwDataGrid TItem="UserRecord" Items="@Users" Selectable="true" PageSize="10">
    <Columns>
        <TwDataGridColumn TItem="UserRecord" Title="Name" Property="u => u.Name" SortBy="u => u.Name" />
        <TwDataGridColumn TItem="UserRecord" Title="Role" Property="u => u.Role" />
        <TwDataGridColumn TItem="UserRecord" Title="Status">
            <CellTemplate Context="user">
                <TwBadge Variant="@(user.IsActive ? SemanticVariant.Success : SemanticVariant.Default)" ShowDot="true">
                    @(user.IsActive ? "Active" : "Offline")
                </TwBadge>
            </CellTemplate>
        </TwDataGridColumn>
    </Columns>
</TwDataGrid>
```

### Responsive Modal & Bottom Sheet
```razor
<TwButton OnClick="() => _isOpen = true">Open Dialog</TwButton>

<TwModal @bind-IsOpen="_isOpen" Title="Confirm Action" Description="Desktop centered dialog, mobile bottom sheet.">
    <p class="text-sm text-slate-600 dark:text-slate-400">Are you sure you want to proceed?</p>
    <FooterContent>
        <TwButton Variant="ButtonVariant.Ghost" OnClick="() => _isOpen = false">Cancel</TwButton>
        <TwButton Variant="ButtonVariant.Primary" OnClick="Save">Confirm</TwButton>
    </FooterContent>
</TwModal>
```

### Modern Progress & Activity Rings
```razor
<!-- Concentric activity rings -->
<TwModernProgress Type="TwModernProgress.ProgressType.ConcentricMultiRing"
                  Value="85" SecondaryValue="68" TertiaryValue="94" />

<!-- Speedometer dashboard arc gauge -->
<TwModernProgress Type="TwModernProgress.ProgressType.SpeedometerArc"
                  Value="84" Label="Velocity" />
```

### TwMerge Conflict Resolution, Multi-Part Slots & Reactive Validation (v0.8.0)
```razor
<!-- Multi-part TwCardSlots + TwButtonSlots with deterministic TwMerge overrides -->
<TwCard Title="Enterprise Edge Deployment"
        Subtitle="Multi-slot styling without CSS specificity clashes"
        Classes="@(new TwCardSlots {
            Base = "border-2 border-indigo-500/50 shadow-xl",
            Title = "text-amber-400 font-black"
        })">
    <TwValidatorGroup @ref="_validator">
        <TwFloatingLabelInput Label="Workspace Slug"
                              Variant="FloatingLabelVariant.Outlined"
                              @bind-Value="_slug"
                              Required="true"
                              MinLength="4"
                              Pattern="^[a-z0-9-]+$" />
    </TwValidatorGroup>
</TwCard>
```

### Check 21 MICR & Local Tesseract OCR
```razor
<TwMicrCheckReader OnCheckScanned="HandleScanned"
                   OnDepositApproved="HandleApproved"
                   OnNachaExported="HandleNachaExport"
                   OnJsonExported="HandleJsonExport" />
```

---

## 🛡️ Agent Ops: Approvals, Takeover & Audit

Drop in a complete human-in-the-loop agent workspace:

```razor
<TwAgentConsole />
```

<p align="center">
  <img src="docs/images/agent-ops-2-approval-gate.png" alt="Agent blocked on a high-risk refund, waiting for human approval" width="760" />
</p>

What it does (all of it is real behavior, exercised by unit and browser tests):

- **Approval gates block.** A tool call marked as requiring approval is held until a human approves (optionally with edited, validated JSON arguments) or rejects with a reason. A rejection reroutes the agent; it is not a notification.
- **Takeover pauses the agent.** The agent stops before its next step; the human edits shared state, and the edit changes what the agent does next. Hand back resumes it.
- **Everything is audited.** Human, agent and system events are recorded in order and can be exported as JSON.

| Approval gate | Rejected, agent escalated | Takeover |
| :---: | :---: | :---: |
| ![Approval gate](docs/images/agent-ops-2-approval-gate.png) | ![Rejected](docs/images/agent-ops-4-rejected.png) | ![Takeover](docs/images/agent-ops-5-takeover.png) |

### Drive your own agent

The console runs on `AgentRunSession`, which consumes an `IAgentTransport`, an `IAsyncEnumerable<AgUiEvent>` modelled on the [AG-UI](https://github.com/ag-ui-protocol/ag-ui) event stream (text deltas, tool call start/args/end/result, state deltas). The session pulls events lazily, so anything your transport does between two yields is held back while the run is paused or awaiting approval.

```csharp
public sealed class MyAgentTransport : IAgentTransport
{
    public async IAsyncEnumerable<AgUiEvent> RunAsync(string prompt, AgentRunContext context,
        [EnumeratorCancellation] CancellationToken ct)
    {
        yield return new AgUiEvent(AgUiEventType.ToolCallStart, "call_1", "issue_refund",
            Risk: ToolRisk.High, RequiresApproval: true);
        yield return new AgUiEvent(AgUiEventType.ToolCallArgs, "call_1", Text: "{\"amount\":\"249.00\"}");
        yield return new AgUiEvent(AgUiEventType.ToolCallEnd, "call_1");

        // The session has already waited for the human; read the decision.
        if (context.DecisionFor("call_1") is { Approved: true } decision)
            yield return new AgUiEvent(AgUiEventType.ToolCallResult, "call_1", Text: "Refund issued.");
    }
}

builder.Services.AddScoped<IAgentTransport, MyAgentTransport>(); // before AddBlazorTwUI()
builder.Services.AddBlazorTwUI();
```

Compose the pieces yourself with `TwApprovalGate`, `TwToolCallCard`, `TwTakeoverBar`, `TwAgentStateInspector`, `TwAuditTrail` and `TwAgentRoster`, each taking the same `AgentRunSession`. The default transport is an offline simulated refund-desk agent, so the demo runs with no backend.

---

## 🧰 Agent Kit: generative UI, voice, threads and the agent control plane

### Live AG-UI over server-sent events

```csharp
builder.Services.AddScoped<IAgentTransport>(sp => new SseAgentTransport(
    sp.GetRequiredService<HttpClient>(),
    new SseAgentTransportOptions { RunUrl = "https://agent.example.com/agui/run", DecisionUrl = "https://agent.example.com/agui/decision" }));
```

`SseAgentTransport` POSTs the prompt and shared state, parses the `text/event-stream` (`RUN_STARTED`, `TEXT_MESSAGE_CONTENT`, `TOOL_CALL_START/ARGS/END/RESULT`, `STATE_SNAPSHOT`, `STATE_DELTA`, `RUN_FINISHED`, `RUN_ERROR`) and, when a tool call is gated, **posts the human's decision (with any edited arguments) to `DecisionUrl`**, so the server holds the run until you decide. A tool call can declare `"risk"` and `"requiresApproval"` to engage the gate. The Agent Ops tab has a *Live AG-UI endpoint* switch; `tests/e2e/agui_server.py` is a small reference agent you can run against it.

| Threads, suggestions, feedback | Live agent held at the gate |
| :---: | :---: |
| ![Threads](docs/images/agent-kit-1-threads.png) | ![Live gate](docs/images/agent-kit-2-live-gate.png) |

### New components

| Area | Components | What they do |
| --- | --- | --- |
| **Threads** | `TwThreadList` | Conversation threads: search, new, rename, pin, archive, delete; persisted to `localStorage` and wired into `TwAgentConsole` (`ShowThreads`). |
| **Generative UI** | `TwA2uiSurface` | Renders an interface the agent describes as JSON (A2UI v0.9): Text, Column, Row, Card, Button, TextField, CheckBox, ChoicePicker, Slider, Tabs, Image, Icon, Divider; two-way data binding; button actions return their resolved context. |
| | `TwMcpAppFrame` | MCP app host in a sandboxed iframe with an opaque origin (never `allow-same-origin`), a locked-down CSP with validated extra domains, and a `postMessage` bridge. |
| **Voice and chat** | `TwVoiceInput`, `TwLiveWaveform` | Push-to-talk microphone with a live level meter and optional speech-to-text hook. |
| | `TwChatSuggestions`, `TwFeedbackButtons`, `TwAttachmentQueue`, `TwMessageScroller` | Suggested prompts, thumbs up/down with reason, validated file staging, and a transcript that follows new content with Jump to latest. |
| **Agent control plane** | `TwHandoffPanel`, `TwLiveScreen` | A stuck agent asks you to take the wheel; its screen with pointer and click reporting while you drive. |
| | `TwDryRunChanges`, `TwActivityLog`, `TwCommandOutput`, `TwRedactedValue` | What an action would change, what the agent did, terminal output, and credential masking (`RedactionService`). |
| | `TwGrantList`, `TwMemoryList`, `TwRoutinesList`, `TwQuestionnaire`, `TwAgentOrb` | Tool access levels with revoke, searchable memory, scheduled routines, structured agent questions, and a state-aware avatar. |
| **Core inputs** | `TwCalendar`, `TwTimePicker`, `TwRadioGroup<T>`, `TwTextArea`, `TwButtonGroup`, `TwSpinner` | Keyboard-first date grid (ARIA pattern), 12/24-hour picker, generic radios, counter and auto-grow, joined buttons, and a reduced-motion-aware spinner. |

See them working in the **AI** area of the demo: *Agent Ops*, *Agent Kit* and *Inputs &amp; Pickers*.

| A2UI generative form | Control plane | Inputs and pickers |
| :---: | :---: | :---: |
| ![A2UI](docs/images/agent-kit-3-a2ui.png) | ![Control](docs/images/agent-kit-5-control.png) | ![Inputs](docs/images/agent-kit-6-inputs.png) |

---

## 📅 Planning & Enterprise Suite

Fifteen components for scheduling, project planning, hierarchical data and forms, all keyboard accessible, Tailwind-native, and tested in a real browser (axe-core clean).

| Area | Components | What they do |
| --- | --- | --- |
| **Scheduling** | `TwScheduler`, `TwGantt` | Day / Week / Month scheduler with overlap-aware columns, click-to-create and drag-and-drop (or Alt+arrows) moves. Gantt timeline with progress, finish-to-start dependency arrows, a today line and conflict detection. |
| **Data** | `TwTreeList`, `TwRangeSelector`, `TwBarGauge` | Treegrid with per-level sorting and arrow-key expand/collapse; range window over a series; concentric bar gauge whose legend carries the exact numbers. |
| **Shell** | `TwToolbar`, `TwWindow`, `TwLoadingPanel`, `TwStackLayout` | Roving-tab-stop toolbar with an overflow menu; floating window (drag, arrow-key move, resize, minimize, maximize, stacking); busy overlay that makes covered content inert; flex stack layout. |
| **Forms** | `TwFormLayout`, `TwFormGroup`, `TwFormItem`, `TwListBox`, `TwDropDownBox`, `TwHtmlEditor` | Responsive form grid with collapsible groups and labelled items; searchable list box; drop-down hosting any content; rich-text editor whose output is rebuilt from an allowlist (no scripts, handlers or unsafe links; pasted content is plain text). |

See them in the **AI** area of the demo under *Planning & Enterprise*.

| Scheduler | Project timeline |
| :---: | :---: |
| ![Scheduler](docs/images/enterprise-1-scheduler.png) | ![Gantt](docs/images/enterprise-2-gantt.png) |

---

## 🧮 Data Grid

`TwDataGrid` keeps its familiar parameters and adds opt-in enterprise features, all keyboard accessible and tested in a real browser.

```razor
<TwDataGrid TItem="Order" Items="_orders" Label="Orders" Selectable="true"
            AllowGrouping="true" AllowColumnChooser="true" AllowExport="true" StateKey="orders" MaxHeight="30rem">
    <Columns>
        <TwDataGridColumn TItem="Order" Title="Order" Frozen="true" Hideable="false" Property="o => o.Id" />
        <TwDataGridColumn TItem="Order" Title="Customer" Property="o => o.Customer" Editable="true"
                          EditValue="SetCustomer" Validate="RequireText" />
        <TwDataGridColumn TItem="Order" Title="Amount" Align="right" Property="o => o.Amount"
                          Summary="GridSummary.Sum" SummaryFormat="C0" />
    </Columns>
    <DetailTemplate Context="o"><p>@o.Notes</p></DetailTemplate>
</TwDataGrid>
```

| Feature | How it works |
| --- | --- |
| **Sorting** | Headers are buttons with `aria-sort`. Shift-click adds a second sort; priority numbers show. |
| **Grouping and totals** | Group by any column: headers show a count and each column's summary. Footer totals cover every filtered row, not just the page. |
| **Master-detail** | `DetailTemplate` adds an expandable row (`aria-expanded` / `aria-controls`). |
| **Inline editing** | `Editable` columns become inputs; `Validate` blocks the save and announces the error; Enter saves, Escape cancels; one row at a time. |
| **Column chooser, frozen column, sticky header** | Show and hide columns (never the last one), pin the first column, keep the header in view. |
| **CSV export** | All filtered rows and visible columns. Cells that start with `=`, `+`, `-` or `@` are neutralised so a spreadsheet cannot run them. |
| **Saved layout** | `StateKey` restores sort, search, filters, grouping, page size and visible columns after a reload. |
| **Conditional formatting** | `RowClassSelector` and `CellClassSelector`. |

| Default | Grouped | Inline edit |
| :---: | :---: | :---: |
| ![Grid](docs/images/grid-1-default.png) | ![Grouped](docs/images/grid-2-grouped.png) | ![Edit](docs/images/grid-4-edit-errors.png) |

---

## ♿ Accessibility

- **High contrast:** toggle it in the theme menu, or let it follow the OS `prefers-contrast` setting. It sets `data-contrast="high"` on `<html>` before first paint (no flash), turns borders solid, strengthens muted text and focus rings, and drops shadows and blur.
- **Forced colors / Windows High Contrast:** system colors win; focus, pressed and disabled states stay visible.
- **Reduced motion:** `prefers-reduced-motion` disables animations and transitions globally.
- **RTL:** the Agent Ops components use logical properties and mirror correctly under `dir="rtl"`.
- **Verified in a browser:** the E2E suite runs axe-core (WCAG 2.0/2.1/2.2 A and AA) against the agent console.

| High contrast | RTL | Forced colors |
| :---: | :---: | :---: |
| ![High contrast](docs/images/agent-ops-6-high-contrast.png) | ![RTL](docs/images/agent-ops-7-rtl-high-contrast.png) | ![Forced colors](docs/images/agent-ops-8-forced-colors.png) |

---

## 🧪 Testing

```bash
# Unit + component tests (xUnit + bUnit): session logic, gating, takeover, audit, theme contrast, UI behavior
dotnet test tests/BlazorTwUI.Tests

# Browser E2E with screenshots and an axe-core audit
npm install
npm run build:css
dotnet run --no-launch-profile --urls http://127.0.0.1:5199
python tests/e2e/agent_ops_e2e.py http://127.0.0.1:5199
# Threads, live AG-UI SSE (start tests/e2e/agui_server.py first), Agent Kit and Inputs tabs, with axe-core
python tests/e2e/agent_kit_e2e.py
# Planning & Enterprise tab: scheduler, Gantt, tree list, window, rich-text sanitizer, with axe-core
python tests/e2e/enterprise_e2e.py
# Data grid: keyboard sorting, grouping, editing, CSV download, saved state, mobile, axe-core
python tests/e2e/grid_e2e.py
```

The E2E run drives the real UI (approve with edited arguments, reject, takeover and state edit, audit export, contrast, RTL, forced colors), asserts the outcomes, fails on any console error, and writes screenshots to `docs/images/`.

---

## 📦 Component Catalog (259 Components)

All components are documented with parameters and copy-paste snippets in the [Full Component Catalog](docs/COMPONENT_CATALOG.md).

| Category | Components | Description |
| :--- | :--- | :--- |
| **Page Templates & UI Building Blocks (v0.8.1)** | `TwCompactStatCard`, `TwLandingHero`, `TwPricingSection`, `TwPromptComposer`, `TwActivityTimeline`, `FintechAnalyticsDashboard`, `AiOperationsTokenMonitor`, `SaaSProductLanding`, `TemplatesGallery` | Turnkey full-page solutions (Fintech Treasury, AI Control Plane, SaaS Marketing) and composable building blocks with pure native SVG math analytics and zero-shift pricing matrices. |
| **Styling Engine & Multi-Part Primitives (v0.8.0)** | `TwMerge`, `TwClassBuilder`, `TwUser`, `TwFloatingLabelInput`, `TwValidatorGroup`, `TwHoverCard`, `TwSonner`, `TwMenubar`, `TwToggleGroup`, `TwMockup`, `TwSwap`, `TwChatBubble` | Deterministic Tailwind utility conflict resolution, multi-part `Classes` slots, 3D stacked toasts, reactive floating-label validation, hover preview cards, menubars, device mockups, and animated swaps. |
| **Actions & Buttons** | `TwButton`, `TwButtonGroup`, `TwSplitButton`, `TwDropdown`, `TwSegmentedControl` | Touch-friendly triggers with loading spinners, icons, and `Soft`, `Shadow`, `Flat`, and `Link` variants. |
| **Forms & Inputs** | `TwInput`, `TwSelect`, `TwCheckbox`, `TwRadioGroup`, `TwToggle`, `TwTextarea`, `TwDatePicker`, `TwTimeSpanPicker`, `TwColorPicker`, `TwFloatingLabelInput`, `TwValidatorGroup` | Two-way data binding, decoupled reactive validation groups, and Outlined/Filled/Standard floating labels. |
| **Data & Tables** | `TwDataGrid`, `TwDataGridColumn`, `TwKanbanBoard`, `TwVirtualGrid`, `TwPickList`, `TwTreeTable` | Sorting, multi-column search, batch actions, and mobile cards. |
| **Feedback & Badges** | `TwBadge`, `TwAlert`, `TwToast`, `TwSonner`, `TwSkeleton`, `TwProgressBar`, `TwModernProgress` | Status chips, alerts, 3D stacked toast decks, shimmers, radial rings, and concentric gauges. |
| **Modals & Overlays** | `TwModal`, `TwBottomSheet`, `TwDrawer`, `TwPopover`, `TwHoverCard`, `TwTooltip`, `TwContextMenu`, `TwWordBalloonOverlay` | Accessible dialogs, hover-safe preview cards, popovers, and anchored word balloons with directional pointer tails. |
| **Navigation** | `TwNavbar`, `TwMenubar`, `TwSidebar`, `TwTabs`, `TwBottomNav`, `TwBreadcrumb`, `TwPagination`, `TwScrollToTop` | Desktop application menubars, responsive multi-tier navigation bars, and tabs. |
| **Cards & Layout** | `TwCard`, `TwMockup`, `TwGrid`, `TwSplitter`, `TwAccordion`, `TwTimeline`, `TwStatCard` | Multi-slot cards, Browser/Phone/Window/Code mockups, bento grids, collapsible panels, and responsive containers. |
| **Agent Kit** | `TwThreadList`, `TwA2uiSurface`, `TwMcpAppFrame`, `TwVoiceInput`, `TwLiveScreen`, `TwHandoffPanel`, `TwGrantList`, `TwMemoryList`, `TwRoutinesList`, `TwQuestionnaire` and 16 more | Generative UI, sandboxed MCP apps, voice, threads, handoff, and access control. |
| **Inputs & Pickers** | `TwCalendar`, `TwTimePicker`, `TwRadioGroup<T>`, `TwTextArea`, `TwButtonGroup`, `TwToggleGroup`, `TwSwap`, `TwSpinner` | Keyboard-first inputs, single/multi-select toggle groups, and animated two-state swaps with ARIA patterns. |
| **Agent Ops** | `TwAgentConsole`, `TwApprovalGate`, `TwToolCallCard`, `TwTakeoverBar`, `TwAgentStateInspector`, `TwAuditTrail`, `TwAgentRoster` | Blocking approvals, takeover/hand-back, shared state and audit trail for any AG-UI-style agent. |
| **AI & AG-UI** | `TwAguiStudio`, `TwAguiCanvas`, `TwChatbot`, `TwChatBubble`, `TwVoiceRecorder`, `TwCopilot` | Generative UI prompt synthesis, semantic chat bubbles, and dual-plane tool calling. |
| **Enterprise Studios** | `TwDashboard`, `TwWordBalloonCreator`, `TwWordCloud`, `TwPdfFormFiller`, `TwUnionBoundary`, `TwFinancialChart`, `TwTreemap`, `TwPivotTable`, `TwMarkdownEditor`, `TwFrameworkBridgeStudio` | Draggable CRM dashboards, vector Word Balloon Studio with clean mode/sliders/overlays, sealed PDFs, and financial charts. |
| **Media & Scanner** | `TwMediaPlayer`, `TwQrScanner`, `TwBarcodeScanner`, `TwSignaturePad`, `TwMicrCheckReader`, `TwDocumentOcrStudio` | 100% Local Tesseract OCR WASM, Check 21 MICR reading, 150+ NACHA bank directory, and audio/video players. |

---

## 📚 Documentation & Guides

- 📖 **[Component Catalog (259 Components)](docs/COMPONENT_CATALOG.md)** — Complete parameter tables, variants, and API reference.
- 🎓 **[Developer Coaching & Architecture Principles](docs/DEVELOPER_COACHING.md)** — Mobile-first design patterns, zero-FOUC theming, and performance tips.
- 🎬 **[HyperFrames 1080p Video Masterclass](wwwroot/videos/hyperframes_masterclass.html)** — Interactive 18-chapter video walkthrough.
- 🎥 **[HyperFrames 4K Video Showcase](wwwroot/videos/hyperframes_showoff_tour.html)** — Interactive 4K multi-chapter product tour with audio narration.
- 🗺️ **[Release Roadmap & Changelog](docs/ROADMAP.md)** — Feature milestones and version history.

---

## 📄 License

BlazorTwUI is licensed under the [MIT License](LICENSE).
