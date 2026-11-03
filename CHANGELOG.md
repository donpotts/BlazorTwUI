# Changelog

All notable changes to BlazorTwUI are documented here. Version 1.0 has not been released: earlier internal labels of the form 1.x were renumbered to 0.x (for example 1.5.0 is now 0.5.0), so the library is in its 0.n.n line. The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and the project uses [Semantic Versioning](https://semver.org/).

## [0.8.2] - 2026-11-03

### Added
- **Full-Featured PivotTable & Master-Detail Suite (`255` → `259 Components`)**:
  - `TwPivotTable<TItem>`: Generic 4-Axis OLAP cross-tabulation pivot matrix with multi-dimensional Row hierarchy (expand/collapse tree buttons), Column headers, Data measures (Sum, Average, Count, DistinctCount, Min, Max, Product), Page Filters, interactive Field List configurator panel, dynamic heatmap color scaling, cell drill-down raw records inspector dialog, and direct CSV export.
  - `TwMasterDetailGrid<TMaster, TChild>`: Hierarchical data table featuring expandable master rows with nested child grids, chevron toggles, multi-row or auto-collapse expansion modes, independent child column sorting, and add/delete child line item callbacks.
  - `TwMasterDetail<TMaster, TChild>`: Synchronized Master/Detail split-pane workspace with Column (horizontal) and Row (vertical) layout modes, live search, status chips, multi-tab detail inspector (Master Form, Child Records Grid, Audit Timeline), and two-way item selection binding.
  - `TwMasterDetailForm`: Turnkey transactional Master/Detail form designed for Invoices, Purchase Orders, and Service Statements with top metadata fields, dynamic line item table, '+ Add Item', real-time subtotal, tax %, discount, and grand total recalculation.
- **Test Suite Expansion**:
  - Added `PivotAndMasterDetailTests.cs` covering matrix aggregation, heatmap styling, field list toggling, cell drilldown, row expansion, nested child grid rendering, workspace tab switching, and dynamic line item calculation (182 total passing tests).

## [0.8.1] - 2026-11-03

### Added
- **Turnkey Page Templates & UI Building Blocks Suite (`246` → `255 Components`)**:
  - `TwCompactStatCard`: Minimalist enterprise KPI metric card with delta badges and pure mathematical SVG sparklines.
  - `TwLandingHero`: Kinetic split marketing hero with headline slot, announcement pill, action buttons, social proof avatar stack, and elevated preview card.
  - `TwPricingSection`: 3-tier interactive pricing matrix with monthly/annual billing switcher (20% discount calculation), equalized 632.5px card heights, zero layout shift, and 44px uniform action buttons.
  - `TwPromptComposer`: Auto-expanding AI prompt box with suggestion chips, file attachment trigger, voice dictation toggle, and dual-plane execution hooks.
  - `TwActivityTimeline`: Vertical audit and event stream with status dots (`Success`, `Warning`, `Danger`), timestamps, and metadata tags.
  - `FintechAnalyticsDashboard`: Turnkey Fintech & Payments Treasury Cockpit with 4 KPI cards ($14.82M gross volume, $9.45M settled), dual-series SVG line chart, liquidity allocation donut, and live clearance ledger table.
  - `AiOperationsTokenMonitor`: Turnkey AI Operations & Control Plane with 248k/s token throughput, 48ms p95 latency, human-in-the-loop approval gates, prompt composer, and live execution trail.
  - `SaaSProductLanding`: Turnkey SaaS marketing landing page assembling kinetic hero, bento feature grid, pricing matrix, and customer testimonials.
  - `TemplatesGallery`: Interactive preview gallery with live viewport frames (Desktop, Tablet, Mobile) for browsing and testing Page Templates and UI Building Blocks.
- **Showcase & Video Tour Updates**:
  - Added 4K Video Showcase tour and poster assets (`wwwroot/videos/hyperframes_showoff_tour.html`, `showoff-demo.mp4`, `showoff-poster.png`).
  - Added dedicated top-nav, rail, and jump bar navigation entry points for UI Templates & Blocks.
  - Added Playwright automated E2E test suite (`tests/e2e_screenshots.py`) validating all 11 screens at 2x Retina resolution.

### Fixed
- **SaaS Pricing Panels Formatting & Alignment (`TwPricingSection.razor` & `TwPricingTable.razor`)**:
  - Equalized card heights across all three tiers to exactly 632.5px by balancing feature count (6 features each).
  - Standardized action buttons to 44px min touch target (`h-11`) with aligned baselines.
  - Encapsulated billing discount notice into fixed-height containers (`h-6 mt-1.5`) to eliminate vertical layout shift when toggling between Annual and Monthly billing.
  - Elevated popular tier with `md:-translate-y-2`, distinct top floating badge (`★ Most Popular`), and removed redundant badges.
  - Added configurable `Class` parameter to prevent double-padding when embedded in layouts.

## [0.8.0]

### Added
- **Styling Engine & Multi-Slot Architecture (`Models/TwStyleEngine.cs`)**:
  - `TwMerge`: Deterministic Tailwind CSS utility conflict resolver in C# (resolves padding/margin axis hierarchies `p-*` vs `px-*`/`py-*`, border-radius, background colors, typography sizes/weights/colors, shadows, sizing, overflow, display, and variant-prefixed tokens).
  - `TwClassBuilder`: Fluent CSS class builder (`Create`, `Add`, `AddIf`, `Build`) backed by `TwMerge.Merge`.
  - Multi-Part Component `TwSlots` (`TwCardSlots`, `TwButtonSlots`, `TwUserSlots`, `TwHoverCardSlots`, `TwFloatingInputSlots`) for per-slot styling overrides.
  - Expanded `ButtonVariant` with `Soft`, `Shadow`, `Flat`, and `Link` variants and upgraded `TwButton` and `TwCard` to use `TwMerge` + `Classes` slots.
- **10 New UI Components (`236` → `246 Components`)**:
  - `TwUser`: Multi-slot user profile identity component with avatar/initials, status indicator (`online`, `busy`, `away`, `offline`), and `TwUserSlots`.
  - `TwFloatingLabelInput`: Animated peer-focused floating label input (`Outlined`, `Filled`, `Standard`) with inline validation status icons and feedback.
  - `TwValidatorGroup`: Decoupled cascading validation container tracking `ValidCount`, `InvalidCount`, `IsAllValid`, `ValidateAll()`, and `ResetAll()`.
  - `TwHoverCard`: Rich hover/focus preview card with open/close debounce delays, continuous hover bridge, directional arrow, and `TwHoverCardSlots`.
  - `TwSonner`: 3D stacked toast notification deck (`scale`/`translateY` depth stack) that fans out on hover and supports async `PushPromiseAsync` loading-to-success transitions.
  - `TwMenubar`: Desktop application menu bar with keyboard shortcut hints, checkable items, and separators.
  - `TwToggleGroup`: Single and multi-select toolbar toggle group.
  - `TwMockup`: 4-in-1 device & terminal frame mockup (`Browser`, `Phone` with Dynamic Island, `Window`, and `Code` console).
  - `TwSwap`: Animated two-state toggle swapper (`Rotate`, `Flip`, `Fade`).
  - `TwChatBubble`: Conversational chat bubble with start/end alignment, avatar, sender, timestamp, and delivery footer.
  - `TwFrameworkBridgeStudio`: Interactive styling, slots, validation, and primitives studio integrated into the Flagship Showcase tab.

## [0.7.2]

### Added
- **72-Second High-Energy POP Marketing Demo Reel & HyperFrames Engine (`wwwroot/videos/hyperframes_masterclass.html` & `wwwroot/videos/demo-video.mp4`)**:
  - Upgraded the 1080p interactive presentation engine and video tour into a punchy, fast-paced **72-second (1m 12s) Marketing Sizzle Reel** structured into **6 chapters (12s each)** and **18 live-UI cuts (4s each)**.
  - Replaced synthetic slides with **18 authentic, real-time screenshots captured directly from the live running `BlazorTwUI` application** (Bento SaaS Showcase, Word Balloon & Comic Banner Creator Studio, Adaptive GenUI & Smart Chat, PDF Form Maker & Sauvola OCR, Check 21 & NACHA Reverse Check Aggregates, 12-Col Draggable CRM Studio, Enterprise DataGrid v0.7, 100 Enterprise Forms, and ScrollCraft + Prize Wheel).
  - Framed every live UI screen inside a split **ScrollCraft Pinned Visual Stage** (left 8 columns) with comic halftone dot backdrops and floating **Word Balloon POP** callout badges, paired with a **3-Step ScrollCraft Kinetic Scrubber Track** (right 4 columns) where Steps 01 → 02 → 03 dynamically progress and trigger POP callout transitions every 4 seconds.
  - Added built-in interactive `PLAY LIVE / PAUSE LIVE` controls, 60 FPS `requestAnimationFrame` timeline scrubbing, and automatic responsive viewport scaling inside embedded iframes.
  - Synthesized all 6 high-energy marketing chapters using the warm, expressive `en-US-AvaMultilingualNeural` voice (`rate="+6%"`, `44.1 kHz` stereo AAC `192k`) and encoded 18 multi-step 1080p H.264 segments with `-movflags +faststart`.
- **Enhanced `TwScrollCraft` Component (`Components/UI/TwScrollCraft.razor`)**:
  - Added floating **Word Balloon POP** callout badges (`💬 POP: ON/OFF`), an interactive `▶ Auto-Scrub` timer mode, a `0%–100%` timeline scrub slider, and step progress pills on the pinned stage.

### Fixed
- **PWA Service Worker Video Caching Bug (`wwwroot/service-worker.js`)**:
  - Bumped cache version to `btui-pwa-v0.7.3`, bypassed service worker interception for HTTP `Range` requests and `/videos/` media streams, and switched static assets from Cache-First to Network-First with cache fallback so updated demo videos are never shadowed by stale cache entries.
  - Added `?v=0.7.3` cache-busting query strings to `demo-video.mp4`, `poster.jpg`, and `hyperframes_masterclass.html` references.
- **Media Player Scrubber & Seeking Fixes (`Components/UI/TwMediaPlayer.razor` & `wwwroot/js/theme.js`)**:
  - Fixed missing `@ref="_scrubberRef"` on the Audio Mode progress bar in `TwMediaPlayer.razor` so clicking the scrubber in Audio Mode seeks accurately.
  - Added `seeked`, `seeking`, and `loadeddata` event listeners in `window.blazorMediaPlayer` and immediate state synchronization in `setCurrentTime` and `seekFromEvent` so seeking while paused updates the UI timecode and chapter pill immediately.
  - Refined the `TwMediaPlayer` top header bar into compact floating glass pills so video headlines and POP callouts are never obscured.

## [0.7.1]

### Added
- **Word Balloon & Visual Studio Suite**, 2 new components (234 to **236 components**):
  - `TwWordBalloonCreator`: Vector Studio canvas for generating stylized Word Balloons, comic speech bubbles, and pop-art action badges with real-time reactive controls.
    - Word count slider (0 to 25 words) with dynamic Archimedean spiral positioning and responsive sizing.
    - 1-Click Clean Normal Balloon Mode (`💬 Clean Normal Balloon`): instantly resets surrounding word count to 0 and shuts off all overlays to render an uncluttered classic speech bubble with the center word strictly on top.
    - Fine-grain sizing sliders for center word scale, surrounding word scale, and spiral word tilt angle.
    - 11 backdrop balloon shapes: `SpeechBubble`, `ThoughtBubble`, `ActionBlast`, `ExplosionStarburst`, `PillCapsule`, `ComicCloud`, `BadgeShield`, `BannerRibbon`, `RetroArcade`, `SmoothCallout`, `HexagonTech`.
    - Directional pointer tails: `BottomLeft`, `BottomCenter`, `BottomRight`, `TopLeft`, `TopRight`, `None` with styles: `Wedge`, `Curved`, `ThoughtDots`, `Lightning`.
    - 9 typography effects: `Bold`, `Outlined`, `NeonGlow`, `ComicHalftone`, `GlitchEffect`, `FireFlame`, `Retro3D`, `Embossed`, `Underlined`.
    - 9 color palettes & gradients: `ComicYellow`, `ActionRed`, `CyberCyan`, `ElectricViolet`, `SunsetBlaze`, `NeonMatrix`, `MonochromeNoir`, `GoldenHero`, `IndigoViolet`.
    - Overlays engine: Master toggle + individual toggles for Speed sunburst rays, vintage Ben-Day pop-art halftone dots, action sparks & stars, specular gloss sheen, cyber scanlines HUD, and kicker sub-caption ribbons.
    - Dual mode studio switcher: Vector Studio Canvas vs Interactive UI Callout Overlays showcase.
    - Direct client-side vector SVG download and high-resolution 2x retina PNG export via HTML5 canvas.
  - `TwWordBalloonOverlay`: Standalone anchored UI callout overlay component. Wraps any target element (avatar, metric card, button) with floating speech balloon callouts supporting hover, click, or persistent visibility modes, with `AutoResizeBalloon`, `BalloonScale`, and custom width/height controls.
    - Fixed balloon disappearance on mouse hover: added interactive pointer event support, an invisible continuous hover bridge between anchor and balloon, a configurable `CloseDelayMs` debounce grace period (250ms), and `Persistent="true"` mode to prevent anchored callouts from flickering or closing prematurely when hovered.
- **Word Balloon Resizing & Auto-Fit Engine**:
  - Enlarged balloon background shapes with generous horizontal and vertical safety padding to ensure text never clips or crowds shape boundaries.
  - Added Auto-Resize On/Off toggle (`AutoResizeBalloon`) allowing automatic dynamic text fitting or fixed manual sizing.
  - Interactive Shape Scale slider (0.8x to 2.5x) and padding sliders (X: 15–100px, Y: 8–60px).
  - Quick-size preset pills (`1.0x Compact`, `1.35x Standard`, `1.75x Jumbo`, `2.2x Mega`) and step resize buttons (`➖ / ➕`).
  - Manual dimension overrides (`CustomWidth` and `CustomHeight`) when Auto-Resize is toggled off.
- **Modern Media Player Masterclass Demo Video Upgrade**:
  - Re-rendered full 300.0s (5m 00s) 1080p demo video (`demo-video.mp4`) covering all 18 synchronized chapters without cutting off at 172s.
  - Added full neural voice narration for all 18 chapters and faststart `moov` atom placement for instantaneous web streaming playback in `TwMediaPlayer`.
- **Enhanced `TwWordCloud`**:
  - `SurroundingWordCount` (0–25), `SurroundingWordScale`, `CenterWordScale`, `TextRotationAngle`.
  - Configurable pointer tail positions and styles.
  - Support for standalone clean balloon mode when `SurroundingWordCount == 0` or `ShowSpiralWords == false`.
  - Guaranteed top-level z-indexing for the center word.

## [0.7.0]

### Added
- **Data grid** (`TwDataGrid`, `TwDataGridColumn`) rebuilt on the same parameters, with new opt-in features:
  - sortable headers are real buttons with `aria-sort`; Shift adds a second sort with priority numbers
  - grouping with counts and per-group summaries; footer totals (Count, Sum, Average, Min, Max) over all filtered rows
  - master-detail rows, inline row editing with validation (Enter saves, Escape cancels, errors announced), column chooser
  - CSV export of every filtered row and the visible columns, with spreadsheet-formula neutralisation
  - saved layout (`StateKey`: sort, search, filters, grouping, page size, visible columns), frozen column, sticky header with `MaxHeight`, conditional row and cell formatting
  - accessible by default: table caption, labelled checkboxes, `aria-current` paging, no clickable-only headers
- `GridAggregate` and `GridCsv` helpers; demo tab *Data Grid*; browser suite `tests/e2e/grid_e2e.py` (keyboard sorting, real CSV download, reload persistence, sticky column, mobile cards, axe-core).
- **Planning & Enterprise suite**, 15 new components (219 to 234): `TwScheduler`, `TwGantt`, `TwTreeList`, `TwToolbar`, `TwWindow`, `TwLoadingPanel`, `TwStackLayout`, `TwFormLayout`, `TwFormGroup`, `TwFormItem`, `TwListBox`, `TwDropDownBox`, `TwHtmlEditor`, `TwRangeSelector`, `TwBarGauge`. All are our own Tailwind implementations.
- `TwHtmlEditor` sanitizes in the browser with an allowlist (no scripts, handlers, styles, images or frames; only http, https and mailto links; pasted content becomes plain text).
- Demo tab *Planning & Enterprise* and the browser suite `tests/e2e/enterprise_e2e.py` (drag-and-drop, real mouse window drag, hostile-input sanitizer checks, axe-core).

## [0.6.0]

Agent-native release: human-in-the-loop agent components, a live AG-UI transport, and an accessibility layer. 193 to **219 components**. Targets .NET 10+ (`net10.0`).

### Added

**Agent Ops (human in the loop)**
- `AgentRunSession` and the `IAgentTransport` abstraction (an `IAsyncEnumerable<AgUiEvent>` modelled on the AG-UI event stream).
- Approval gates that genuinely block high-risk tool calls, with edited and validated JSON arguments, or rejection with a reason that reroutes the agent.
- Takeover and hand-back: the agent pauses before its next step and the human's edits to shared state change what it does next.
- Exportable JSON audit trail.
- Components: `TwAgentConsole`, `TwApprovalGate`, `TwToolCallCard`, `TwTakeoverBar`, `TwAgentStateInspector`, `TwAuditTrail`, `TwAgentRoster`.
- `SimulatedAgentTransport`: an offline refund-desk agent, so the demo needs no backend.

**Live AG-UI over server-sent events**
- `SseAgentTransport`, `SseParser` and `AgUiWireMapper`: POST the run, parse `text/event-stream`, map AG-UI wire events, and report approval decisions (with edited arguments) back so the server holds the run at the gate.
- `tests/e2e/agui_server.py`: a small reference agent.
- A *Live AG-UI endpoint* switch on the Agent Ops tab (`AgentOpsWorkspace`).

**Threads**
- `AgentThreadStore` / `IAgentThreadStore` (persisted to `localStorage`) and `TwThreadList` (search, new, rename, pin, archive, delete). `TwAgentConsole` gains `ShowThreads`, `Transport` and `TransportLabel`.

**Generative UI, voice and chat pieces**
- `TwA2uiSurface` with `A2uiSurface`: agent-described UI (A2UI v0.9) with data binding and button actions.
- `TwMcpAppFrame`: sandboxed MCP app host (opaque origin, locked-down CSP, `postMessage` bridge).
- `TwVoiceInput`, `TwLiveWaveform`, `TwChatSuggestions`, `TwFeedbackButtons`, `TwAttachmentQueue`, `TwMessageScroller`.

**Agent control plane**
- `TwHandoffPanel`, `TwLiveScreen`, `TwDryRunChanges`, `TwActivityLog`, `TwCommandOutput`, `TwRedactedValue` (with `RedactionService`), `TwGrantList`, `TwMemoryList`, `TwRoutinesList`, `TwQuestionnaire`, `TwAgentOrb`.

**Inputs and controls**
- `TwCalendar` (ARIA date-grid keyboard model), `TwTimePicker`, `TwRadioGroup<TValue>`, `TwTextArea`, `TwButtonGroup`, `TwSpinner`.

**Accessibility**
- High-contrast mode (theme-menu toggle or OS `prefers-contrast`, applied before first paint), `forced-colors` support, global reduced-motion handling, RTL-safe logical properties in the new components.

**Quality**
- bUnit/xUnit suite grown from 0 to 92 tests, including a catalog integrity test.
- Playwright browser suites (`tests/e2e/agent_ops_e2e.py`, `tests/e2e/agent_kit_e2e.py`) with axe-core WCAG 2.x A/AA audits and screenshots.

### Release artifacts
- `BlazorTwUI-0.6.0-x64-Setup.exe` (NSIS), `BlazorTwUI-0.6.0-x64-en-US.msi` (WiX) and `BlazorTwUI-WebDemo-v0.6.0.zip`, with `SHA256SUMS.txt` (SHA-256 checksums).

### Fixed
- The CRM dashboard leaked a `window` resize listener and threw once it was torn down.
- GridStack was referenced by `index.html` but never committed because `.gitignore` excludes `dist/`; version 12.3.3 is now vendored.
- Audit export mixed key casing; it is now camelCase throughout.
- Edited approval arguments carried over into the next run.

### Changed
- `docs/COMPONENT_CATALOG.md` is now generated from `ComponentCatalogService` (source links and parameter tables for all components); a test fails when it is out of date.
- The README runtime badge now reads .NET 10+ to match the project target.
- Roadmap items renumbered so they are unique.

[0.8.0]: https://github.com/donpotts/BlazorTwUI/releases/tag/v0.8.0
[0.7.0]: https://github.com/donpotts/BlazorTwUI/releases/tag/v0.7.0
[0.6.0]: https://github.com/donpotts/BlazorTwUI/releases/tag/v0.6.0
