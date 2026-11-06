# BlazorTwUI — Component Catalog & Settings Reference

Comprehensive guide to all **267 components** in **BlazorTwUI** (.NET 10+ WebAssembly, Tailwind CSS v3.4+, TwMerge conflict engine, and multi-part TwSlots).

> This file is generated from `Services/ComponentCatalogService.cs`. Do not edit it by hand; change the catalog and regenerate with `UPDATE_CATALOG_DOC=1 dotnet test --filter CatalogDocTests`. A test fails when this file is out of date.

---

## Table of Contents

- [Advanced Forms & Inputs](#advanced-forms--inputs) (14 components)
- [Agent Ops & Human-in-the-Loop](#agent-ops--human-in-the-loop) (18 components)
- [AI & Agentic Workflows](#ai--agentic-workflows) (1 component)
- [AI Chatbot & Reasoning](#ai-chatbot--reasoning) (3 components)
- [Animation & Micro-Interactions](#animation--micro-interactions) (1 component)
- [Architecture & State](#architecture--state) (2 components)
- [Buttons, Forms & Controls](#buttons-forms--controls) (19 components)
- [Cards & Bento](#cards--bento) (1 component)
- [Cards, Grids & Bento Systems](#cards-grids--bento-systems) (13 components)
- [Chat & Generative UI](#chat--generative-ui) (10 components)
- [Code & Syntax Studio](#code--syntax-studio) (5 components)
- [Dashboards & Layouts](#dashboards--layouts) (1 component)
- [Data Visualization](#data-visualization) (1 component)
- [Developer Studio & Content Creation](#developer-studio--content-creation) (7 components)
- [Documents & Forms](#documents--forms) (1 component)
- [Enterprise & Workplace Operations](#enterprise--workplace-operations) (1 component)
- [Enterprise Data & Tables](#enterprise-data--tables) (13 components)
- [Feedback & Alerts](#feedback--alerts) (1 component)
- [Financial & Analytical Intelligence](#financial--analytical-intelligence) (5 components)
- [Financial & OCR](#financial--ocr) (2 components)
- [Forms & Controls](#forms--controls) (2 components)
- [Forms & Inputs](#forms--inputs) (2 components)
- [Fullstack .NET 10+](#fullstack-net-10) (10 components)
- [Gamification & Celebrations](#gamification--celebrations) (4 components)
- [Hardware & Media Interop](#hardware--media-interop) (1 component)
- [Layout & Application Shell](#layout--application-shell) (12 components)
- [Media, Scanner & Signature](#media-scanner--signature) (6 components)
- [Modals & Overlays](#modals--overlays) (1 component)
- [Navigation & Headers](#navigation--headers) (1 component)
- [Navigation & Shell](#navigation--shell) (2 components)
- [Navigation & Steppers](#navigation--steppers) (11 components)
- [Other Components](#other-components) (18 components)
- [Overlays & Modals](#overlays--modals) (2 components)
- [Overlays & Popovers](#overlays--popovers) (7 components)
- [Page Templates & UI Building Blocks](#page-templates--ui-building-blocks) (9 components)
- [Progress & Steppers](#progress--steppers) (1 component)
- [Scheduling & Planning](#scheduling--planning) (2 components)
- [Security & Compliance](#security--compliance) (1 component)
- [Showcase & Micro-Interactions](#showcase--micro-interactions) (9 components)
- [Smart Components](#smart-components) (1 component)
- [Styling Engine & Multi-Part Primitives](#styling-engine--multi-part-primitives) (10 components)
- [Task-Purpose Enterprise Forms](#task-purpose-enterprise-forms) (8 components)
- [Theme & Palette Customization](#theme--palette-customization) (5 components)
- [Typography & Utilities](#typography--utilities) (1 component)
- [Typography, Badges & Keys](#typography-badges--keys) (9 components)
- [Visualizations & Gauges](#visualizations--gauges) (13 components)

---

## Advanced Forms & Inputs

### `TwCalendar`

**Location:** [`Components/UI/TwCalendar.razor`](../Components/UI/TwCalendar.razor)

Month calendar following the ARIA date-grid pattern: arrow / Home / End / PageUp / PageDown keys, min and max, culture-aware.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `DateOnly?` | `null` | Selected date. |
| `Min` | `DateOnly?` | `null` | Earliest selectable date. |
| `Max` | `DateOnly?` | `null` | Latest selectable date. |
| `FirstDayOfWeek` | `DayOfWeek` | `Sunday` | First column. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwCalendar @bind-Value="_date" Min="_from" Max="_to" />
```

---

### `TwTimePicker`

**Location:** [`Components/UI/TwTimePicker.razor`](../Components/UI/TwTimePicker.razor)

Accessible hour / minute / AM-PM picker built from native selects, 12 or 24 hour, with a minute step.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `TimeOnly?` | `null` | Selected time. |
| `Use24Hour` | `bool` | `false` | 24 hour clock. |
| `MinuteStep` | `int` | `5` | Minute granularity (1-30). |
| `Label` | `string?` | `null` | Group label. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwTimePicker @bind-Value="_time" MinuteStep="15" />
```

---

### `TwListBox`

**Location:** [`Components/UI/TwListBox.razor`](../Components/UI/TwListBox.razor)

Searchable list with checkboxes or radios, Select all (applies to the filtered view) and a live selection count. Over 100 items only the visible rows are drawn.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `IReadOnlyList<TItem>` | `required` | Items to list. |
| `TextSelector` | `Func<TItem,string>?` | `ToString` | Display text. |
| `Selected` | `IReadOnlyList<TItem>` | `[]` | Selected items. |
| `Multiple` | `bool` | `true` | Checkboxes (true) or radios (false). |
| `Label` | `string` | `"Items"` | Region name. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwListBox TItem="string" Items="_names" @bind-Selected="_picked" Label="Teams" />
```

---

### `TwDropDownBox`

**Location:** [`Components/UI/TwDropDownBox.razor`](../Components/UI/TwDropDownBox.razor)

A field that opens a panel hosting any content (a grid, tree or form); closes on Escape, outside click or from the content.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ChildContent` | `RenderFragment<TwDropDownBox>` | `empty` | Panel content; the component is the context so it can call CloseAsync(). |
| `Text` | `string?` | `null` | Shown in the field. |
| `Placeholder` | `string` | `"Select…"` | Shown when Text is empty. |
| `Open` | `bool` | `false` | Panel open. |
| `Disabled` | `bool` | `false` | Disable the field. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwDropDownBox @bind-Open="_open" Text="@_choice">@context ...</TwDropDownBox>
```

---

### `TwHtmlEditor`

**Location:** [`Components/UI/TwHtmlEditor.razor`](../Components/UI/TwHtmlEditor.razor)

Rich-text editor: bold, italic, underline, headings, lists, quotes and links. Output is rebuilt from an allowlist in the browser and pasted text is plain.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `string?` | `null` | Sanitized HTML. |
| `Label` | `string` | `"Rich text"` | Accessible name. |
| `MinHeight` | `string` | `"10rem"` | Editing area height. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwHtmlEditor @bind-Value="_html" Label="Message" />
```

---

### `TwRangeSelector`

**Location:** [`Components/UI/TwRangeSelector.razor`](../Components/UI/TwRangeSelector.razor)

Pick a window over a series with an area preview and two range inputs that cannot cross; reports both indices.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Values` | `IReadOnlyList<double>` | `required` | Series for the preview. |
| `Labels` | `IReadOnlyList<string>?` | `null` | Point labels. |
| `Start` | `int` | `0` | First index. |
| `End` | `int` | `last` | Last index. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwRangeSelector Values="_series" Labels="_months" @bind-Start="_from" @bind-End="_to" />
```

---

### `TwFilterBuilder`

**Location:** [`Components/UI/TwFilterBuilder.razor`](../Components/UI/TwFilterBuilder.razor)

Visual query and filter builder supporting nested boolean AND/OR criteria trees, comparison operators, typed values, and LINQ/SQL predicate generation.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `"Visual Query & Filter Builder"` | Header title or accessible label. |
| `RootGroup` | `FilterGroup` | `new()` | Configures component behavior or styling. |
| `AvailableFields` | `List<FieldDefinition>` | `new()` | Configures component behavior or styling. |
| `ShowPreview` | `bool` | `true` | Configures component behavior or styling. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `OnFilterChanged` | `EventCallback<FilterGroup>` | `null` | Callback invoked when state changes or item is selected. |

#### Razor Usage Example

```razor
<TwFilterBuilder Title="Visual Query & Filter Builder" RootGroup="new()" AvailableFields="new()" />
```

---

### `TwDropzone`

**Location:** [`Components/UI/TwDropzone.razor`](../Components/UI/TwDropzone.razor)

Enterprise multi-file drag-and-drop ingestion zone with animated focus border, file extension badges, individual upload progress bars, and queue management.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `AllowMultiple` | `bool` | `true` | Configures component behavior or styling. |
| `AcceptedExtensions` | `string` | `".png, .jpg, .pdf, .zip, .cs, .json"` | Configures component behavior or styling. |
| `MaxFileSizeMb` | `int` | `25` | Component sizing scale (Xs, Sm, Md, Lg). |
| `Files` | `List<DropzoneFile>` | `new()` | Configures component behavior or styling. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `FilesChanged` | `EventCallback<List<DropzoneFile>>` | `null` | Callback invoked when state changes or item is selected. |

#### Razor Usage Example

```razor
<TwDropzone AllowMultiple="true" AcceptedExtensions=".png, .jpg, .pdf, .zip, .cs, .json" MaxFileSizeMb="25" />
```

---

### `TwOtpInput`

**Location:** [`Components/UI/TwOtpInput.razor`](../Components/UI/TwOtpInput.razor)

Segmented 6-digit one-time password / PIN verification input with auto-advance, backspace navigation, mobile numeric keypad, paste support, and dot masking.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Length` | `int` | `6` | Configures component behavior or styling. |
| `Value` | `string` | `""` | Primary data value or two-way bound model property. |
| `ValueChanged` | `EventCallback<string>` | `null` | Callback invoked when state changes or item is selected. |
| `OnCompleted` | `EventCallback<string>` | `null` | Configures component behavior or styling. |
| `Masked` | `bool` | `null` | Configures component behavior or styling. |
| `Disabled` | `bool` | `null` | Disables user interaction and applies muted opacity styling. |
| `HasError` | `bool` | `null` | Configures component behavior or styling. |
| `ErrorMessage` | `string?` | `null` | Configures component behavior or styling. |
| `Label` | `string?` | `null` | Configures component behavior or styling. |
| `HelperText` | `string?` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwOtpInput Length="6" Value="" />
```

---

### `TwPasswordStrength`

**Location:** [`Components/UI/TwPasswordStrength.razor`](../Components/UI/TwPasswordStrength.razor)

Interactive password input with toggleable eye visibility, 4-stage color-coded entropy bar (Weak, Fair, Good, Strong), and real-time live requirements checklist.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `string` | `""` | Primary data value or two-way bound model property. |
| `ValueChanged` | `EventCallback<string>` | `null` | Callback invoked when state changes or item is selected. |
| `Label` | `string` | `"Password"` | Configures component behavior or styling. |
| `Placeholder` | `string` | `"Enter a secure password..."` | Input placeholder hint. |
| `ShowChecklist` | `bool` | `true` | Configures component behavior or styling. |
| `Disabled` | `bool` | `null` | Disables user interaction and applies muted opacity styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwPasswordStrength Value="" Label="Password" Placeholder="Enter a secure password..." />
```

---

### `TwCombobox`

**Location:** [`Components/UI/TwCombobox.razor`](../Components/UI/TwCombobox.razor)

Searchable autocomplete dropdown with real-time filtering, keyboard navigation (arrows, Page Up/Down, Home/End), enter-to-select, clear button, and empty filter fallback. Over 100 options only the visible ones are drawn; search and the keyboard still reach all of them.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `string` | `""` | Primary data value or two-way bound model property. |
| `ValueChanged` | `EventCallback<string>` | `null` | Callback invoked when state changes or item is selected. |
| `Items` | `IReadOnlyList<ComboboxItem>` | `Array.Empty<ComboboxItem>()` | Configures component behavior or styling. |
| `Label` | `string` | `""` | Configures component behavior or styling. |
| `Placeholder` | `string` | `"Select or search option..."` | Input placeholder hint. |
| `Disabled` | `bool` | `null` | Disables user interaction and applies muted opacity styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwCombobox Value="" Items="Array.Empty<ComboboxItem>()" Label="" />
```

---

### `TwTagsInput`

**Location:** [`Components/UI/TwTagsInput.razor`](../Components/UI/TwTagsInput.razor)

Interactive token tag creator that adds chips on Enter/Comma, supports backspace removal, removable badge pills, and max tags limit.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Tags` | `List<string>` | `new()` | Configures component behavior or styling. |
| `TagsChanged` | `EventCallback<List<string>>` | `null` | Callback invoked when state changes or item is selected. |
| `Label` | `string` | `""` | Configures component behavior or styling. |
| `Placeholder` | `string` | `"Type tag and press Enter..."` | Input placeholder hint. |
| `HelperText` | `string?` | `null` | Configures component behavior or styling. |
| `MaxTags` | `int?` | `null` | Configures component behavior or styling. |
| `AllowDuplicates` | `bool` | `false` | Configures component behavior or styling. |
| `Disabled` | `bool` | `null` | Disables user interaction and applies muted opacity styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwTagsInput Tags="new()" Label="" Placeholder="Type tag and press Enter..." />
```

---

### `TwInputGroup`

**Location:** [`Components/UI/TwInputGroup.razor`](../Components/UI/TwInputGroup.razor)

Composite input field with seamless prepended and appended text, icon slots, and dropdown buttons (currency, protocol, domain).

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `string` | `""` | Primary data value or two-way bound model property. |
| `ValueChanged` | `EventCallback<string>` | `null` | Callback invoked when state changes or item is selected. |
| `Label` | `string` | `""` | Configures component behavior or styling. |
| `Type` | `string` | `"text"` | Configures component behavior or styling. |
| `Placeholder` | `string` | `""` | Input placeholder hint. |
| `PrependText` | `string?` | `null` | Configures component behavior or styling. |
| `AppendText` | `string?` | `null` | Configures component behavior or styling. |
| `PrependContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `AppendContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `HelperText` | `string?` | `null` | Configures component behavior or styling. |
| `ErrorMessage` | `string?` | `null` | Configures component behavior or styling. |
| `HasError` | `bool` | `null` | Configures component behavior or styling. |
| `Disabled` | `bool` | `null` | Disables user interaction and applies muted opacity styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwInputGroup Value="" Label="" Type="text" />
```

---

### `TwTimeSpanPicker`

**Location:** [`Components/UI/TwTimeSpanPicker.razor`](../Components/UI/TwTimeSpanPicker.razor)

Structured duration and timespan picker with Days, Hours, Minutes, Seconds input blocks, step increments, reset trigger, and quick preset duration pills.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `TimeSpan` | `TimeSpan.Zero` | Two-way bindable duration value. |
| `Label` | `string?` | `null` | Optional form label. |
| `AllowDays` | `bool` | `true` | Whether to show the Days stepper unit. |
| `AllowSeconds` | `bool` | `true` | Whether to show the Seconds stepper unit. |
| `ShowPresets` | `bool` | `true` | Show quick duration preset buttons (15m, 1h, 1d, etc.). |
| `ShowReset` | `bool` | `true` | Show zero-reset button. |

#### Razor Usage Example

```razor
<TwTimeSpanPicker Label="SLA Duration" @bind-Value="_sla" ShowPresets="true" />
```

---

## Agent Ops & Human-in-the-Loop

### `TwAgentConsole`

**Location:** [`Components/UI/TwAgentConsole.razor`](../Components/UI/TwAgentConsole.razor)

Complete human-in-the-loop agent workspace: roster, streaming conversation with tool-call cards, an approval gate that genuinely blocks high-risk calls, takeover and hand-back controls, an editable shared-state inspector and an exportable audit trail. Drives any IAgentTransport; ships with an offline simulated refund-desk agent.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |
| `Agents` | `IReadOnlyList<AgentProfile>?` | `one demo agent` | Agents shown in the roster. |
| `Session` | `AgentRunSession` | `n/a (read-only property)` | The live session, for host pages that read status or drive it. |

#### Razor Usage Example

```razor
<TwAgentConsole />
```

---

### `TwApprovalGate`

**Location:** [`Components/UI/TwApprovalGate.razor`](../Components/UI/TwApprovalGate.razor)

Blocking approval card for pending high-risk tool calls. The agent does not execute the call until a human approves, optionally with edited JSON arguments (validated), or rejects with a reason. Announces itself to screen readers via an assertive live region.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Session` | `AgentRunSession` | `required` | Session whose pending approvals are rendered. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwApprovalGate Session="_session" />
```

---

### `TwToolCallCard`

**Location:** [`Components/UI/TwToolCallCard.razor`](../Components/UI/TwToolCallCard.razor)

Collapsible card for one agent tool call showing streamed arguments, result, duration, risk level and a text status (never colour alone). Pending-approval calls are visually emphasised.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Call` | `ToolCallRecord` | `required` | The tool call to render. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwToolCallCard Call="toolCall" />
```

---

### `TwTakeoverBar`

**Location:** [`Components/UI/TwTakeoverBar.razor`](../Components/UI/TwTakeoverBar.razor)

Run status strip with Take over, Hand back and Stop controls. Takeover pauses the agent before its next step so a human can inspect and edit shared state; hand-back resumes it.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Session` | `AgentRunSession` | `required` | Session to display and control. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwTakeoverBar Session="_session" />
```

---

### `TwAgentStateInspector`

**Location:** [`Components/UI/TwAgentStateInspector.razor`](../Components/UI/TwAgentStateInspector.razor)

Live view of the state the agent shares with the UI. Changed keys are highlighted; values become editable while the human is in control, and every edit is audited and visible to the agent's next action.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Session` | `AgentRunSession` | `required` | Session whose shared state is shown. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwAgentStateInspector Session="_session" />
```

---

### `TwAuditTrail`

**Location:** [`Components/UI/TwAuditTrail.razor`](../Components/UI/TwAuditTrail.razor)

Append-only, filterable log of who did what (human, agent, system) with a one-click JSON export for compliance review.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Session` | `AgentRunSession` | `required` | Session whose audit events are shown. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwAuditTrail Session="_session" />
```

---

### `TwAgentRoster`

**Location:** [`Components/UI/TwAgentRoster.razor`](../Components/UI/TwAgentRoster.razor)

Accessible list of available agents (nav with aria-current) showing role, description and the selected agent's run status.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Agents` | `IReadOnlyList<AgentProfile>` | `required` | Agents to list. |
| `SelectedId` | `string?` | `null` | Id of the selected agent. |
| `StatusText` | `string` | `"Idle"` | Run status shown on the selected agent. |
| `OnSelect` | `EventCallback<AgentProfile>` | `null` | Fired when an agent is chosen. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwAgentRoster Agents="_agents" SelectedId="@_selected" OnSelect="a => _selected = a.Id" />
```

---

### `TwThreadList`

**Location:** [`Components/UI/TwThreadList.razor`](../Components/UI/TwThreadList.razor)

Conversation thread sidebar with search, new, rename, pin, archive and delete, backed by IAgentThreadStore (persisted to localStorage).

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Store` | `IAgentThreadStore` | `required` | Thread store to list and edit. |
| `SelectedId` | `string?` | `null` | Id of the open thread. |
| `OnSelect` | `EventCallback<AgentThread>` | `null` | Fired when a thread is opened. |
| `OnCreated` | `EventCallback<AgentThread>` | `null` | Fired after New. |
| `OnDeleted` | `EventCallback<AgentThread>` | `null` | Fired after a thread is deleted. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwThreadList Store="_threads" SelectedId="@_threadId" OnSelect="t => _threadId = t.Id" />
```

---

### `TwDryRunChanges`

**Location:** [`Components/UI/TwDryRunChanges.razor`](../Components/UI/TwDryRunChanges.razor)

Before/after preview of what an action would change, shown before the human approves it.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Changes` | `IReadOnlyList<DryRunChange>` | `required` | Add / Change / Remove rows with Target, Before and After. |
| `Title` | `string` | `"If you approve, this will change:"` | Heading. |
| `ChildContent` | `RenderFragment?` | `null` | Extra content below the list. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwDryRunChanges Changes="_changes" />
```

---

### `TwActivityLog`

**Location:** [`Components/UI/TwActivityLog.razor`](../Components/UI/TwActivityLog.razor)

Timestamped log of what the agent did on its machine (navigate, click, type, command), announced politely to assistive tech.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Entries` | `IReadOnlyList<ActivityEntry>` | `required` | Events: AtUtc, Kind, Text, Detail. |
| `Label` | `string` | `"Agent activity"` | Region heading. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwActivityLog Entries="_entries" />
```

---

### `TwCommandOutput`

**Location:** [`Components/UI/TwCommandOutput.razor`](../Components/UI/TwCommandOutput.razor)

Terminal-style command result with exit code, tail truncation and automatic credential masking.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Command` | `string` | `required` | Command line that was run. |
| `Output` | `string` | `""` | Captured output. |
| `ExitCode` | `int` | `0` | Process exit code. |
| `MaxLines` | `int` | `200` | Keep only the last N lines. |
| `Redact` | `bool` | `true` | Mask passwords, tokens and URL credentials. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwCommandOutput Command="dotnet test" Output="@_out" ExitCode="0" />
```

---

### `TwHandoffPanel`

**Location:** [`Components/UI/TwHandoffPanel.razor`](../Components/UI/TwHandoffPanel.razor)

Alert card where a stuck agent asks a human to take the wheel, then hand the task back.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `AgentName` | `string` | `"The agent"` | Who is asking. |
| `Reason` | `string` | `required` | Why the agent is stuck. |
| `Where` | `string?` | `null` | Page or step it is stuck at. |
| `Resolved` | `bool` | `false` | Show the resolved state. |
| `OnTakeOver` | `EventCallback` | `null` | Human took control. |
| `OnHandBack` | `EventCallback` | `null` | Human returned control. |
| `OnDecline` | `EventCallback` | `null` | Human stopped the task. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwHandoffPanel AgentName="Refund Desk" Reason="A CAPTCHA blocks the portal." OnTakeOver="Take" OnHandBack="GiveBack" />
```

---

### `TwRoutinesList`

**Location:** [`Components/UI/TwRoutinesList.razor`](../Components/UI/TwRoutinesList.razor)

Recurring agent jobs with schedule, next run, run-now and an accessible on/off switch.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `List<Routine>` | `required` | Routines to show. |
| `OnToggled` | `EventCallback<Routine>` | `null` | Enabled state changed. |
| `OnRunNow` | `EventCallback<Routine>` | `null` | Run now pressed. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwRoutinesList Items="_routines" OnToggled="Save" />
```

---

### `TwMemoryList`

**Location:** [`Components/UI/TwMemoryList.razor`](../Components/UI/TwMemoryList.razor)

What the agent remembers, with source and date, searchable, each with a Forget action.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `List<Memory>` | `required` | Remembered facts. |
| `OnForget` | `EventCallback<Memory>` | `null` | A memory was forgotten. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwMemoryList Items="_memories" OnForget="Forget" />
```

---

### `TwGrantList`

**Location:** [`Components/UI/TwGrantList.razor`](../Components/UI/TwGrantList.razor)

Table of tools an agent may use, each with an access level (Ask / Read / Write) and Revoke.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `List<Grant>` | `required` | Granted tools and levels. |
| `OnChanged` | `EventCallback<Grant>` | `null` | Access level changed. |
| `OnRevoked` | `EventCallback<Grant>` | `null` | Grant revoked. |
| `Caption` | `string` | `"Tools the agent may use"` | Table caption for assistive tech. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwGrantList Items="_grants" OnChanged="Save" OnRevoked="Revoke" />
```

---

### `TwLiveScreen`

**Location:** [`Components/UI/TwLiveScreen.razor`](../Components/UI/TwLiveScreen.razor)

The agent's screen with its pointer, a working/in-control badge and Take control; reports normalised click coordinates while you drive.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ImageUrl` | `string?` | `null` | Latest captured frame. |
| `Url` | `string?` | `null` | Page the agent is on. |
| `CursorX` | `double?` | `null` | Agent pointer X, 0..1. |
| `CursorY` | `double?` | `null` | Agent pointer Y, 0..1. |
| `Controlling` | `bool` | `false` | Human is in control. |
| `OnScreenClick` | `EventCallback<(double X,double Y)>` | `null` | Click while controlling, as 0..1 fractions. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwLiveScreen ImageUrl="@_frame" Url="https://portal.test" @bind-Controlling="_driving" OnScreenClick="Click" />
```

---

### `TwAgentOrb`

**Location:** [`Components/UI/TwAgentOrb.razor`](../Components/UI/TwAgentOrb.razor)

Animated agent avatar whose colour, ring and status dot follow the run state, with the state also exposed as text.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Name` | `string` | `"Agent"` | Agent name (initial and accessible name). |
| `Status` | `AgentRunStatus` | `Idle` | Run state. |
| `Size` | `string` | `"md"` | sm, md or lg. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwAgentOrb Name="Refund Desk" Status="@_session.Status" />
```

---

### `TwRedactedValue`

**Location:** [`Components/UI/TwRedactedValue.razor`](../Components/UI/TwRedactedValue.razor)

Shows agent arguments and results with credentials masked; a deliberate Reveal is needed to see the raw value.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `string` | `required` | Raw JSON or text. |
| `Label` | `string` | `"Value"` | Heading. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwRedactedValue Value="@call.ArgsJson" Label="Arguments" />
```

---

## AI & Agentic Workflows

### `TwChatStudio`

**Location:** [`Components/UI/TwChatStudio.razor`](../Components/UI/TwChatStudio.razor)

Full-featured modern conversational AI chat studio with multiline growing composer, staged file attachment preview chips, quick reply suggestion pills, feedback reactions (thumbs up/down), copy to clipboard, and live harmonic typing indicator.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `"AI Enterprise Studio"` | Chat studio title. |
| `ModelBadge` | `string` | `".NET 10+ WASM"` | Model identification badge. |
| `Messages` | `List<ChatMessage>` | `new()` | Chronological chat message stream. |
| `Suggestions` | `List<string>` | `new()` | Quick reply suggestion chips above composer. |
| `IsTyping` | `bool` | `false` | Show animated harmonic typing indicator. |
| `TypingStatusText` | `string` | `"AI is thinking..."` | Typing indicator status message. |
| `OnMessageSent` | `EventCallback<ChatMessage>` | `-` | Invoked when user sends prompt with staged attachments. |

#### Razor Usage Example

```razor
<TwChatStudio Title="AI Enterprise Studio" Messages="_messages" Suggestions="_prompts" OnMessageSent="HandleSend" OnRegenerate="HandleRegen" />
```

---

## AI Chatbot & Reasoning

### `TwChatBot`

**Location:** [`Components/UI/TwChatBot.razor`](../Components/UI/TwChatBot.razor)

Intelligent conversational chatbot interface with streaming message bubbles, thinking progress checklists, and code snippet copying.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `BotName` | `string` | `"NexusAI"` | Configures component behavior or styling. |
| `SelectedModel` | `string` | `"Gemini 2.0 Flash"` | Configures component behavior or styling. |
| `ShowCloseButton` | `bool` | `null` | Configures component behavior or styling. |
| `OnClose` | `EventCallback` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |
| `StarterPrompts` | `IReadOnlyList<StarterPrompt>` | `new List<StarterPrompt>     {         new("Explain Architecture", "Explain how mobile-first Blazor WASM and TailwindCSS work together with zero FOUC."),         new("Generate Code", "Write a reusable C# Blazor component for animated radial progress with Tailwind classes."),         new("Deep Reasoning Demo", "Solve a complex algorithm step-by-step with multi-stage reasoning checklist progress."),         new("Compare Performance", "Compare Blazor WebAssembly AOT compilation vs JIT performance on mobile phones.")     }` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwChatBot BotName="NexusAI" SelectedModel="Gemini 2.0 Flash" StarterPrompts="new List<StarterPrompt>
    {
        new(Explain Architecture, Explain how mobile-first Blazor WASM and TailwindCSS work together with zero FOUC.),
        new(Generate Code, Write a reusable C# Blazor component for animated radial progress with Tailwind classes.),
        new(Deep Reasoning Demo, Solve a complex algorithm step-by-step with multi-stage reasoning checklist progress.),
        new(Compare Performance, Compare Blazor WebAssembly AOT compilation vs JIT performance on mobile phones.)
    }" />
```

---

### `TwChatWidget`

**Location:** [`Components/UI/TwChatWidget.razor`](../Components/UI/TwChatWidget.razor)

Floating launcher button that expands into an interactive AI assistant window (or bottom sheet on mobile).

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `BotName` | `string` | `"NexusAI"` | Configures component behavior or styling. |
| `SelectedModel` | `string` | `"Gemini 2.0 Flash"` | Configures component behavior or styling. |
| `PositionClass` | `string` | `"bottom-20 sm:bottom-6 right-4 sm:right-6"` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwChatWidget BotName="NexusAI" SelectedModel="Gemini 2.0 Flash" PositionClass="bottom-20 sm:bottom-6 right-4 sm:right-6" />
```

---

### `TwAiReasoningProgress`

**Location:** [`Components/UI/TwAiReasoningProgress.razor`](../Components/UI/TwAiReasoningProgress.razor)

Multi-phase thinking checklist component displaying animated progress indicators during AI reasoning tasks.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `IsThinking` | `bool` | `true` | Configures component behavior or styling. |
| `DurationSeconds` | `double` | `2.4` | Configures component behavior or styling. |
| `ThinkingLabel` | `string` | `"Reasoning step-by-step..."` | Configures component behavior or styling. |
| `ReasoningText` | `string?` | `null` | Configures component behavior or styling. |
| `Steps` | `IReadOnlyList<ReasoningStep>?` | `null` | Configures component behavior or styling. |
| `DefaultExpanded` | `bool` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwAiReasoningProgress IsThinking="true" DurationSeconds="2.4" ThinkingLabel="Reasoning step-by-step..." />
```

---

## Animation & Micro-Interactions

### `TwScrollCraft`

**Location:** [`Components/UI/TwScrollCraft.razor`](../Components/UI/TwScrollCraft.razor)

Scroll-driven object animation container with parallax depth layers, focal zone reading fades, and pinned illustration scrubbers linked to viewport progress.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `"Content Scroll Scrubbing"` | Stage title. |
| `Steps` | `List<ScrollStep>` | `Default 4 steps` | List of steps with title, description, badge, and icon. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwScrollCraft Title="Content Scroll Scrubbing" />
```

---

## Architecture & State

### `TwUnionStudio`

**Location:** [`Components/UI/TwUnionStudio.razor`](../Components/UI/TwUnionStudio.razor)

C# 15 Discriminated Union boundary studio with exhaustive pattern matching, algebraic UI states (Idle, Loading, Success, Error), latency simulation, and retry triggers.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `CustomClass` | `string` | `""` | Optional CSS class modifiers. |

#### Razor Usage Example

```razor
<TwUnionStudio />
```

---

### `TwAsyncState`

**Location:** [`Components/UI/TwAsyncState.razor`](../Components/UI/TwAsyncState.razor)

Declarative async boundary container for handling loading skeletons, error states with retry buttons, empty states, and success content.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `State` | `AsyncOperationState` | `-` | The active state enum. |

#### Razor Usage Example

```razor
<TwAsyncState State="@state"><Success>Content</Success></TwAsyncState>
```

---

## Buttons, Forms & Controls

### `TwEntityForm`

**Location:** [`Components/UI/TwEntityForm.razor`](../Components/UI/TwEntityForm.razor)

Schema-driven create/edit form: one input per field (text, number, decimal, date, flag) and a picker per relation, prefilled from the record. Saving sends {relation}Id / {relation}Ids and strips the related objects; an unchanged date keeps its time.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Schema` | `EntitySchema` | `required` | Fields and relations of the entity. |
| `Record` | `JsonObject?` | `null` | Record to edit, or null to create. Never modified; OnSave gets a copy. |
| `LoadOptions` | `Func<string, Task<IReadOnlyList<(string Id, string Name)>>>` | `required` | Every record of a related entity, for the pickers. |
| `OnSave` | `EventCallback<JsonObject>` | `-` | Receives the record to send. |
| `OnCancel` | `EventCallback` | `-` | Cancel pressed. |
| `SaveText` | `string` | `"Save"` | Save button text. |
| `Class` | `string?` | `null` | Extra classes. |

#### Razor Usage Example

```razor
<TwEntityForm Schema="_schema" Record="_editing" LoadOptions="Options.LoadAsync" OnSave="SaveAsync" OnCancel="Close" />
```

---

### `TwRelationPicker`

**Location:** [`Components/UI/TwRelationPicker.razor`](../Components/UI/TwRelationPicker.razor)

Picker for one relation of a record: a single select for a to-one link, a searchable multi-select for a to-many link, prefilled from the record's related objects or ids and loading every option, not a first page.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Relation` | `EntityRelation` | `required` | The relation (target, to-one or to-many). |
| `Record` | `JsonObject?` | `null` | Record being edited; prefills the choice. |
| `LoadOptions` | `Func<string, Task<IReadOnlyList<(string Id, string Name)>>>` | `required` | Every record of the target entity. |
| `SelectedIdsChanged` | `EventCallback<IReadOnlyList<string>>` | `-` | Chosen ids (0 or 1 for a to-one link). |
| `Disabled` | `bool` | `false` | Read-only. |
| `Class` | `string?` | `null` | Extra classes. |

#### Razor Usage Example

```razor
<TwRelationPicker Relation="_schema.Relations[0]" Record="_editing" LoadOptions="Options.LoadAsync" SelectedIdsChanged="ids => _ids = ids" />
```

---

### `TwSpinner`

**Location:** [`Components/UI/TwSpinner.razor`](../Components/UI/TwSpinner.razor)

Busy indicator with an announced label; stops spinning under reduced motion.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Size` | `string` | `"md"` | sm, md or lg. |
| `Label` | `string` | `"Loading"` | Announced label. |
| `ShowLabel` | `bool` | `false` | Show the label visibly. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwSpinner Label="Saving" ShowLabel="true" />
```

---

### `TwButtonGroup`

**Location:** [`Components/UI/TwButtonGroup.razor`](../Components/UI/TwButtonGroup.razor)

Joins buttons into one group with collapsed borders and rounded ends, horizontal or vertical.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ChildContent` | `RenderFragment?` | `null` | Buttons. |
| `Label` | `string?` | `null` | Group name. |
| `Vertical` | `bool` | `false` | Stack vertically. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwButtonGroup Label="View"><button>Day</button><button>Week</button></TwButtonGroup>
```

---

### `TwRadioGroup`

**Location:** [`Components/UI/TwRadioGroup.razor`](../Components/UI/TwRadioGroup.razor)

Generic native radio group with legend, option descriptions, disabled options and error text.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `IReadOnlyList<Option>` | `required` | Value, Label, Description, Disabled. |
| `Value` | `TValue?` | `default` | Selected value. |
| `Legend` | `string?` | `null` | Group label. |
| `Error` | `string?` | `null` | Validation message. |
| `Horizontal` | `bool` | `false` | Lay out in a row. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwRadioGroup TValue="string" Legend="Plan" Items="_plans" @bind-Value="_plan" />
```

---

### `TwTextArea`

**Location:** [`Components/UI/TwTextArea.razor`](../Components/UI/TwTextArea.razor)

Multi-line input with label, hint, error, character counter and auto-grow.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `string?` | `null` | Text. |
| `Label` | `string?` | `null` | Label. |
| `MaxLength` | `int` | `0` | Max characters (0 = none). |
| `AutoGrow` | `bool` | `false` | Grow between MinRows and MaxRows. |
| `Error` | `string?` | `null` | Validation message. |
| `Hint` | `string?` | `null` | Help text. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwTextArea Label="Notes" MaxLength="280" AutoGrow="true" @bind-Value="_notes" />
```

---

### `TwDateRangePicker`

**Location:** [`Components/UI/TwDateRangePicker.razor`](../Components/UI/TwDateRangePicker.razor)

Dual-month calendar popover with quick range presets (Today, 7 Days, 30 Days, This Month, YTD), continuous selection range highlighting, and manual date input.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Label` | `string?` | `"Date Range Selection"` | Configures component behavior or styling. |
| `StartDate` | `DateTime` | `DateTime.Today.AddDays(-14)` | Configures component behavior or styling. |
| `EndDate` | `DateTime` | `DateTime.Today` | Configures component behavior or styling. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwDateRangePicker Label="Date Range Selection" StartDate="DateTime.Today.AddDays(-14)" EndDate="DateTime.Today" />
```

---

### `TwButton`

**Location:** [`Components/UI/TwButton.razor`](../Components/UI/TwButton.razor)

Tactile interactive button featuring 44px touch targets, loading spinner state, semantic variants, and mobile full-width responsive expansion.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `LeadingIcon` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `TrailingIcon` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `Variant` | `ButtonVariant` | `ButtonVariant.Primary` | Semantic visual variant (e.g. Primary, Secondary, Outline). |
| `Size` | `ComponentSize` | `ComponentSize.Md` | Component sizing scale (Xs, Sm, Md, Lg). |
| `Type` | `string` | `"button"` | Configures component behavior or styling. |
| `Disabled` | `bool` | `null` | Disables user interaction and applies muted opacity styling. |
| `Loading` | `bool` | `null` | Displays animated loading spinner indicator. |
| `ResponsiveFullWidth` | `bool` | `null` | Configures component behavior or styling. |
| `FullWidth` | `bool` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |
| `OnClick` | `EventCallback<MouseEventArgs>` | `null` | Callback invoked when state changes or item is selected. |

#### Razor Usage Example

```razor
<TwButton Variant="ButtonVariant.Primary" Size="ComponentSize.Md" Type="button" />
```

---

### `TwInput`

**Location:** [`Components/UI/TwInput.razor`](../Components/UI/TwInput.razor)

Accessible text input with validation error states, helper text slots, leading/trailing icons, and clear button.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Label` | `string?` | `null` | Configures component behavior or styling. |
| `Value` | `string?` | `null` | Primary data value or two-way bound model property. |
| `ValueChanged` | `EventCallback<string?>` | `null` | Callback invoked when state changes or item is selected. |
| `Placeholder` | `string?` | `null` | Input placeholder hint. |
| `Type` | `string` | `"text"` | Configures component behavior or styling. |
| `HelperText` | `string?` | `null` | Configures component behavior or styling. |
| `ErrorText` | `string?` | `null` | Configures component behavior or styling. |
| `Required` | `bool` | `null` | Configures component behavior or styling. |
| `Disabled` | `bool` | `null` | Disables user interaction and applies muted opacity styling. |
| `PrefixContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `SuffixContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwInput Type="text" />
```

---

### `TwSelect`

**Location:** [`Components/UI/TwSelect.razor`](../Components/UI/TwSelect.razor)

Styled select dropdown with floating labels, option groups, and dark mode contrast.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Label` | `string?` | `null` | Configures component behavior or styling. |
| `Value` | `string?` | `null` | Primary data value or two-way bound model property. |
| `ValueChanged` | `EventCallback<string?>` | `null` | Callback invoked when state changes or item is selected. |
| `HelperText` | `string?` | `null` | Configures component behavior or styling. |
| `Disabled` | `bool` | `null` | Disables user interaction and applies muted opacity styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwSelect>
    <span>Content</span>
</TwSelect>
```

---

### `TwSwitch`

**Location:** [`Components/UI/TwSwitch.razor`](../Components/UI/TwSwitch.razor)

Accessible toggle switch with smooth spring transitions and screen-reader label binding.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Checked` | `bool` | `null` | Configures component behavior or styling. |
| `CheckedChanged` | `EventCallback<bool>` | `null` | Callback invoked when state changes or item is selected. |
| `Label` | `string?` | `null` | Configures component behavior or styling. |
| `Description` | `string?` | `null` | Secondary descriptive caption. |
| `Disabled` | `bool` | `null` | Disables user interaction and applies muted opacity styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwSwitch />
```

---

### `TwCheckbox`

**Location:** [`Components/UI/TwCheckbox.razor`](../Components/UI/TwCheckbox.razor)

Touch-friendly checkbox with animated checkmark and focus ring styling.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Checked` | `bool` | `null` | Configures component behavior or styling. |
| `CheckedChanged` | `EventCallback<bool>` | `null` | Callback invoked when state changes or item is selected. |
| `Label` | `string?` | `null` | Configures component behavior or styling. |
| `Description` | `string?` | `null` | Secondary descriptive caption. |
| `Disabled` | `bool` | `null` | Disables user interaction and applies muted opacity styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwCheckbox />
```

---

### `TwSlider`

**Location:** [`Components/UI/TwSlider.razor`](../Components/UI/TwSlider.razor)

Touch-friendly range slider with draggable thumb, track fill, and live value bubble readout.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `double` | `50` | Primary data value or two-way bound model property. |
| `ValueChanged` | `EventCallback<double>` | `null` | Callback invoked when state changes or item is selected. |
| `Min` | `double` | `0` | Configures component behavior or styling. |
| `Max` | `double` | `100` | Configures component behavior or styling. |
| `Step` | `double` | `1` | Configures component behavior or styling. |
| `Label` | `string?` | `null` | Configures component behavior or styling. |
| `ValueUnit` | `string?` | `"%"` | Primary data value or two-way bound model property. |
| `ShowValue` | `bool` | `true` | Primary data value or two-way bound model property. |
| `Disabled` | `bool` | `null` | Disables user interaction and applies muted opacity styling. |
| `HelperText` | `string?` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwSlider Value="50" Min="0" Max="100" />
```

---

### `TwNumeric`

**Location:** [`Components/UI/TwNumeric.razor`](../Components/UI/TwNumeric.razor)

Precise numeric stepper input with increment/decrement buttons, min/max clamping, and step intervals.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `double` | `null` | Primary data value or two-way bound model property. |
| `ValueChanged` | `EventCallback<double>` | `null` | Callback invoked when state changes or item is selected. |
| `Label` | `string?` | `null` | Configures component behavior or styling. |
| `HelperText` | `string?` | `null` | Configures component behavior or styling. |
| `Prefix` | `string?` | `null` | Configures component behavior or styling. |
| `Suffix` | `string?` | `null` | Configures component behavior or styling. |
| `Min` | `double` | `0` | Configures component behavior or styling. |
| `Max` | `double` | `100` | Configures component behavior or styling. |
| `Step` | `double` | `1` | Configures component behavior or styling. |
| `Required` | `bool` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwNumeric Min="0" Max="100" Step="1" />
```

---

### `TwDatePicker`

**Location:** [`Components/UI/TwDatePicker.razor`](../Components/UI/TwDatePicker.razor)

Accessible date picker with calendar popover, quick range presets, and international date formatting.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `DateTime?` | `null` | Primary data value or two-way bound model property. |
| `ValueChanged` | `EventCallback<DateTime?>` | `null` | Callback invoked when state changes or item is selected. |
| `Label` | `string?` | `null` | Configures component behavior or styling. |
| `Placeholder` | `string` | `"Select date..."` | Input placeholder hint. |
| `DateFormat` | `string` | `"MMM dd, yyyy"` | Configures component behavior or styling. |
| `AllowClear` | `bool` | `true` | Configures component behavior or styling. |
| `Required` | `bool` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwDatePicker Placeholder="Select date..." DateFormat="MMM dd, yyyy" AllowClear="true" />
```

---

### `TwFileUpload`

**Location:** [`Components/UI/TwFileUpload.razor`](../Components/UI/TwFileUpload.razor)

Drag-and-drop file upload zone with file size validation, mime type filtering, and upload progress visualization.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Label` | `string?` | `"Upload Documents or Media"` | Configures component behavior or styling. |
| `HelperText` | `string?` | `null` | Configures component behavior or styling. |
| `Required` | `bool` | `null` | Configures component behavior or styling. |
| `Multiple` | `bool` | `true` | Configures component behavior or styling. |
| `Accept` | `string` | `".png,.jpg,.jpeg,.svg,.pdf,.zip"` | Configures component behavior or styling. |
| `MaxFileSize` | `long` | `10 * 1024 * 1024` | Component sizing scale (Xs, Sm, Md, Lg). |
| `FilesChanged` | `EventCallback<IReadOnlyList<IBrowserFile>>` | `null` | Callback invoked when state changes or item is selected. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwFileUpload Label="Upload Documents or Media" Multiple="true" Accept=".png,.jpg,.jpeg,.svg,.pdf,.zip" />
```

---

### `TwSegmented`

**Location:** [`Components/UI/TwSegmented.razor`](../Components/UI/TwSegmented.razor)

iOS-style segmented control pill for switching between mutually exclusive views with sliding background highlight.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `IReadOnlyList<SegmentItem>` | `Array.Empty<SegmentItem>()` | Configures component behavior or styling. |
| `Value` | `string` | `""` | Primary data value or two-way bound model property. |
| `ValueChanged` | `EventCallback<string>` | `null` | Callback invoked when state changes or item is selected. |
| `FullWidth` | `bool` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwSegmented Items="Array.Empty<SegmentItem>()" Value="" />
```

---

### `TwRating`

**Location:** [`Components/UI/TwRating.razor`](../Components/UI/TwRating.razor)

Interactive star rating component supporting full and half-star ratings with hover preview and read-only display mode.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `int` | `4` | Primary data value or two-way bound model property. |
| `ValueChanged` | `EventCallback<int>` | `null` | Callback invoked when state changes or item is selected. |
| `MaxStars` | `int` | `5` | Configures component behavior or styling. |
| `ReadOnly` | `bool` | `null` | Configures component behavior or styling. |
| `ShowValueText` | `bool` | `true` | Primary data value or two-way bound model property. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwRating Value="4" MaxStars="5" ShowValueText="true" />
```

---

### `TwPickList`

**Location:** [`Components/UI/TwPickList.razor`](../Components/UI/TwPickList.razor)

Dual-listbox item transfer picker with instant dual-list search filtering, move selected/all transfer actions, target list reordering, and item badges. Over 100 items a panel draws only the visible rows; Space selects, Enter moves an item across.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Source` | `IEnumerable<TItem>?` | `null` | Source available items collection. |
| `Target` | `IList<TItem>` | `new List<TItem>()` | Selected target items collection. |
| `SourceTitle` | `string` | `"Available Items"` | Left panel header title. |
| `TargetTitle` | `string` | `"Selected Items"` | Right panel header title. |
| `AllowFiltering` | `bool` | `true` | Show real-time filter search input on both lists. |
| `ShowMoveAll` | `bool` | `true` | Show transfer all (>> and <<) buttons. |
| `ShowReorder` | `bool` | `true` | Show up/down reorder buttons in target list. |

#### Razor Usage Example

```razor
<TwPickList TItem="string" Source="_available" @bind-Target="_selected" SourceTitle="Roles" TargetTitle="Assigned" />
```

---

## Cards & Bento

### `TwTile`

**Location:** [`Components/UI/TwTile.razor`](../Components/UI/TwTile.razor)

Compact KPI metric tile with inline trend badge, subtext, and zero-JS sparkline micro-chart.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `""` | Tile title text. |
| `Value` | `string` | `""` | Primary metric value text. |

#### Razor Usage Example

```razor
<TwTile Title="Active Users" Value="4,820" Change="+12%" />
```

---

## Cards, Grids & Bento Systems

### `TwCard`

**Location:** [`Components/UI/TwCard.razor`](../Components/UI/TwCard.razor)

Modular card container with header, body, and footer slots, subtle hover lift, and glassmorphic borders.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string?` | `null` | Header title or accessible label. |
| `Subtitle` | `string?` | `null` | Header title or accessible label. |
| `Hoverable` | `bool` | `null` | Configures component behavior or styling. |
| `NoPadding` | `bool` | `null` | Configures component behavior or styling. |
| `HeaderContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `FooterContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwCard>
    <span>Content</span>
</TwCard>
```

---

### `TwBentoGrid`

**Location:** [`Components/UI/TwBentoGrid.razor`](../Components/UI/TwBentoGrid.razor)

Modern asymmetrical CSS grid for showcasing features with radial mouse-following spotlight gradients.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Cols` | `int` | `3` | Configures component behavior or styling. |
| `GapClass` | `string` | `"gap-4 sm:gap-6"` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |

#### Razor Usage Example

```razor
<TwBentoGrid Cols="3" GapClass="gap-4 sm:gap-6" />
```

---

### `TwBentoCard`

**Location:** [`Components/UI/TwBentoCard.razor`](../Components/UI/TwBentoCard.razor)

Card item for Bento grids with customizable column/row spans, glowing borders, and feature badge slots.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ColSpan` | `int` | `1` | Configures component behavior or styling. |
| `RowSpan` | `int` | `1` | Configures component behavior or styling. |
| `Title` | `string?` | `null` | Header title or accessible label. |
| `Description` | `string?` | `null` | Secondary descriptive caption. |
| `Icon` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `Badge` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `Footer` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `ShowDotPattern` | `bool` | `true` | Configures component behavior or styling. |
| `SpotlightColor` | `string` | `"rgba(99, 102, 241, 0.12)"` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwBentoCard ColSpan="1" RowSpan="1" ShowDotPattern="true" />
```

---

### `TwStatCard`

**Location:** [`Components/UI/TwStatCard.razor`](../Components/UI/TwStatCard.razor)

SaaS metric KPI card with sparklines, percentage change indicators, and target progress bars.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `""` | Header title or accessible label. |
| `Value` | `string` | `""` | Primary data value or two-way bound model property. |
| `Subtitle` | `string?` | `null` | Header title or accessible label. |
| `TrendText` | `string?` | `null` | Configures component behavior or styling. |
| `IsTrendPositive` | `bool` | `true` | Configures component behavior or styling. |
| `ProgressValue` | `int?` | `null` | Primary data value or two-way bound model property. |
| `IconContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `IconBgClass` | `string` | `"bg-brand-50 dark:bg-brand-500/10"` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwStatCard Title="" Value="" IsTrendPositive="true" />
```

---

### `TwKpiMetric`

**Location:** [`Components/UI/TwKpiMetric.razor`](../Components/UI/TwKpiMetric.razor)

Compact micro-metric card with live animated count-up and cubic ease-out transitions.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `string.Empty` | Header title or accessible label. |
| `Value` | `string` | `string.Empty` | Primary data value or two-way bound model property. |
| `Subtitle` | `string?` | `null` | Header title or accessible label. |
| `ChangePercentage` | `double?` | `null` | Configures component behavior or styling. |
| `SparklineData` | `IReadOnlyList<double>?` | `null` | Configures component behavior or styling. |
| `Color` | `string?` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwKpiMetric Title="string.Empty" Value="string.Empty" />
```

---

### `TwMediaCard`

**Location:** [`Components/UI/TwMediaCard.razor`](../Components/UI/TwMediaCard.razor)

Rich media card with 16:9 aspect ratio image header, category badges, author avatar, and reading time.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `""` | Header title or accessible label. |
| `Description` | `string?` | `null` | Secondary descriptive caption. |
| `ImageUrl` | `string?` | `null` | Configures component behavior or styling. |
| `ImageAlt` | `string` | `"Media cover"` | Configures component behavior or styling. |
| `CategoryBadge` | `string?` | `null` | Configures component behavior or styling. |
| `AuthorName` | `string?` | `null` | Configures component behavior or styling. |
| `AuthorInitials` | `string` | `"AM"` | Configures component behavior or styling. |
| `DateText` | `string?` | `null` | Configures component behavior or styling. |
| `Tags` | `IReadOnlyList<string>?` | `null` | Configures component behavior or styling. |
| `ActionContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `OnClick` | `EventCallback<MouseEventArgs>` | `null` | Callback invoked when state changes or item is selected. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwMediaCard Title="" ImageAlt="Media cover" AuthorInitials="AM" />
```

---

### `TwFeatureCard`

**Location:** [`Components/UI/TwFeatureCard.razor`](../Components/UI/TwFeatureCard.razor)

Service feature card with gradient background icon, bold headline, and subtle hover scale.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `""` | Header title or accessible label. |
| `Description` | `string` | `""` | Secondary descriptive caption. |
| `BadgeText` | `string?` | `null` | Configures component behavior or styling. |
| `LinkText` | `string?` | `"Explore feature"` | Configures component behavior or styling. |
| `IconGradientClass` | `string` | `"bg-gradient-to-tr from-brand-600 to-indigo-500"` | Configures component behavior or styling. |
| `IconContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `OnClick` | `EventCallback<MouseEventArgs>` | `null` | Callback invoked when state changes or item is selected. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwFeatureCard Title="" Description="" LinkText="Explore feature" />
```

---

### `TwSpotlightCard`

**Location:** [`Components/UI/TwSpotlightCard.razor`](../Components/UI/TwSpotlightCard.razor)

Interactive card that calculates cursor coordinates to project a dynamic radial spotlight beam across its surface.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `SpotlightColor` | `string` | `"rgba(99, 102, 241, 0.15)"` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwSpotlightCard SpotlightColor="rgba(99, 102, 241, 0.15)" />
```

---

### `TwPricingCard`

**Location:** [`Components/UI/TwPricingCard.razor`](../Components/UI/TwPricingCard.razor)

SaaS subscription pricing tier card with popular badge highlight, price per interval, and feature checklist.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `PlanName` | `string` | `"Pro"` | Configures component behavior or styling. |
| `Description` | `string?` | `null` | Secondary descriptive caption. |
| `Price` | `string` | `"$29"` | Configures component behavior or styling. |
| `Interval` | `string` | `"mo"` | Configures component behavior or styling. |
| `IsPopular` | `bool` | `null` | Configures component behavior or styling. |
| `Features` | `IReadOnlyList<string>?` | `null` | Configures component behavior or styling. |
| `CtaText` | `string` | `"Get Started"` | Configures component behavior or styling. |
| `OnClick` | `EventCallback<MouseEventArgs>` | `null` | Callback invoked when state changes or item is selected. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwPricingCard PlanName="Pro" Price="$29" Interval="mo" />
```

---

### `TwPricingTable`

**Location:** [`Components/UI/TwPricingTable.razor`](../Components/UI/TwPricingTable.razor)

Full billing cycle matrix comparing Free, Pro, and Enterprise tiers with monthly/annual toggle.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Tiers` | `List<PricingTier>` | `new()` | Configures component behavior or styling. |
| `OnSelectTier` | `EventCallback<PricingTier>` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwPricingTable Tiers="new()" />
```

---

### `TwTileManager`

**Location:** [`Components/UI/TwTileManager.razor`](../Components/UI/TwTileManager.razor)

Executive dashboard tile manager with responsive multi-column layouts, 1-click tile focus maximization, minimize collapse, and refresh callbacks.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string?` | `"Executive Operations Command"` | Toolbar title. |
| `ColumnCount` | `int` | `4` | Number of grid columns (Quad, Triple, Dual). |
| `Gap` | `string` | `"gap-4"` | Tailwind gap utility. |
| `ShowToolbar` | `bool` | `true` | Whether to show view switcher toolbar. |

#### Razor Usage Example

```razor
<TwTileManager Title="Executive Command"><TwTile Title="ARR" ColSpan="2">...</TwTile></TwTileManager>
```

---

### `TwFileManager`

**Location:** [`Components/UI/TwFileManager.razor`](../Components/UI/TwFileManager.razor)

Enterprise cloud and asset file manager with folder tree navigation, breadcrumbs, grid/table view modes, search, storage quota meter, and upload/delete actions.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `List<FileItem>` | `new()` | Files and folders collection. |
| `CurrentPath` | `string` | `"/"` | Current active directory path. |
| `OnFileSelected` | `EventCallback<FileItem>` | `-` | Fired when a file or folder is selected. |
| `OnFileOpened` | `EventCallback<FileItem>` | `-` | Fired when a file or folder is double-clicked. |
| `OnUpload` | `EventCallback` | `-` | Fired when user clicks Upload button. |

#### Razor Usage Example

```razor
<TwFileManager Items="_files" CurrentPath="/" OnFileSelected="HandleFileSelected" />
```

---

### `TwSpatialCard`

**Location:** [`Components/UI/TwSpatialCard.razor`](../Components/UI/TwSpatialCard.razor)

2027 volumetric 3D card featuring real-time gyroscopic mouse tilt, dynamic specular refraction glare, multi-layer Z-depth elevation, and tactile organic blur.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string?` | `null` | Card title header. |
| `Subtitle` | `string?` | `null` | Card subtitle text. |
| `BadgeText` | `string?` | `null` | Elevated 3D pill badge text. |
| `MaxTiltDeg` | `double` | `12.0` | Maximum rotation angle degrees on tilt. |
| `GlowGradient` | `string` | `"from-brand-500 to-indigo-600"` | Tailwind gradient classes for dynamic ambient aura. |

#### Razor Usage Example

```razor
<TwSpatialCard Title="Quantum Enclave" MaxTiltDeg="14">...content...</TwSpatialCard>
```

---

## Chat & Generative UI

### `TwChatSuggestions`

**Location:** [`Components/UI/TwChatSuggestions.razor`](../Components/UI/TwChatSuggestions.razor)

Suggested follow-up prompts as buttons, with a loading skeleton.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `IReadOnlyList<string>` | `required` | Suggestion texts. |
| `OnPick` | `EventCallback<string>` | `null` | A suggestion was chosen. |
| `Loading` | `bool` | `false` | Show placeholders. |
| `Disabled` | `bool` | `false` | Disable while a run is active. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwChatSuggestions Items="_ideas" OnPick="s => _prompt = s" />
```

---

### `TwAskData`

**Location:** [`Components/UI/TwAskData.razor`](../Components/UI/TwAskData.razor)

"Ask your data": a floating, read-only panel that answers counts, totals, averages, lowest/highest, top N, breakdowns and searches from your own records with no model and no key. A typed question is mapped to one plan from a closed catalogue (DataQuestionParser); your executor fetches the rows; the answer shows the figure, the rows behind it and how it was worked out. Questions it cannot map get suggestions built from the schema.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Schema` | `IReadOnlyList<KeyValuePair<string, EntitySchema>>` | `required` | Every entity, keyed the way relations name their target; the first is the main one. |
| `Executor` | `Func<AnswerPlan, Task<AnswerResult>>` | `required` | Reads the rows for a plan (ODataAnswerQuery.RunAsync does it for an OData API). Never writes. |
| `NamesLoader` | `Func<string, Task<IReadOnlyList<(string Id, string Name)>>>?` | `null` | Records of a related entity, so a named brand or tag becomes a filter. |
| `Title` | `string` | `"Ask your data"` | Launcher and panel title. |
| `LauncherClass` | `string?` | `null` | Extra classes for the launcher button. |

#### Razor Usage Example

```razor
<TwAskData Schema="_schemas" Executor="plan => Api.RunAsync(plan)" NamesLoader="Options.LoadAsync" />
```

---

### `TwFeedbackButtons`

**Location:** [`Components/UI/TwFeedbackButtons.razor`](../Components/UI/TwFeedbackButtons.razor)

Thumbs up / down on a response, toggling off on a second press, with an optional reason box after thumbs-down.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `FeedbackRating` | `None` | None, Up or Down. |
| `AskReason` | `bool` | `true` | Ask what went wrong after Down. |
| `OnReason` | `EventCallback<string>` | `null` | Reason text committed. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwFeedbackButtons @bind-Value="_rating" OnReason="SaveReason" />
```

---

### `TwAttachmentQueue`

**Location:** [`Components/UI/TwAttachmentQueue.razor`](../Components/UI/TwAttachmentQueue.razor)

Files staged on a message with count, size and duplicate validation and per-file removal.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `List<AttachmentInfo>` | `[]` | Staged files. |
| `MaxFiles` | `int` | `5` | Maximum number of files. |
| `MaxBytes` | `long` | `5 MB` | Maximum size per file. |
| `Accept` | `string?` | `null` | Accepted types. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwAttachmentQueue @bind-Items="_files" MaxFiles="5" />
```

---

### `TwMessageScroller`

**Location:** [`Components/UI/TwMessageScroller.razor`](../Components/UI/TwMessageScroller.razor)

Chat transcript container that follows new content and offers Jump to latest after you scroll up.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ChildContent` | `RenderFragment?` | `null` | Messages. |
| `Label` | `string` | `"Conversation"` | Log region name. |
| `Height` | `string?` | `null` | CSS max-height. |
| `BodyClass` | `string?` | `null` | Classes for the scrolling body. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwMessageScroller Height="24rem">...messages...</TwMessageScroller>
```

---

### `TwVoiceInput`

**Location:** [`Components/UI/TwVoiceInput.razor`](../Components/UI/TwVoiceInput.razor)

Push-to-talk microphone with a live level meter; reports the clip and can transcribe it through your own speech-to-text function.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `OnRecorded` | `EventCallback<VoiceClip>` | `null` | Clip recorded. |
| `Transcribe` | `Func<VoiceClip, Task<string>>?` | `null` | Speech-to-text function. |
| `OnTranscript` | `EventCallback<string>` | `null` | Transcribed text. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwVoiceInput Transcribe="SpeechToText" OnTranscript="t => _prompt = t" />
```

---

### `TwLiveWaveform`

**Location:** [`Components/UI/TwLiveWaveform.razor`](../Components/UI/TwLiveWaveform.razor)

Bar level meter for live audio; maps any number of levels onto any number of bars.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Levels` | `IReadOnlyList<float>` | `[]` | Levels from 0 to 1. |
| `Active` | `bool` | `false` | Live (true) or flat (false). |
| `Bars` | `int` | `24` | Number of bars. |
| `AriaLabel` | `string` | `"Audio level"` | Accessible name. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwLiveWaveform Levels="_levels" Active="_recording" />
```

---

### `TwQuestionnaire`

**Location:** [`Components/UI/TwQuestionnaire.razor`](../Components/UI/TwQuestionnaire.razor)

Structured questions an agent asks, one at a time with progress, single or multiple choice and optional free text.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Questions` | `IReadOnlyList<Question>` | `required` | Questions with options. |
| `OnSubmit` | `EventCallback<IReadOnlyDictionary<string, IReadOnlyList<string>>>` | `null` | Answers by question id. |
| `Title` | `string` | `"Questions from the agent"` | Region name. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwQuestionnaire Questions="_questions" OnSubmit="Answer" />
```

---

### `TwA2uiSurface`

**Location:** [`Components/UI/TwA2uiSurface.razor`](../Components/UI/TwA2uiSurface.razor)

Renders an agent-described interface (A2UI v0.9): streamed components bound to a data model, with button actions returned to the app.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Surface` | `A2uiSurface` | `required` | Model fed with createSurface / updateComponents / updateDataModel messages. |
| `OnAction` | `EventCallback<A2uiUserAction>` | `null` | Button action with resolved context. |
| `Label` | `string` | `"Agent-generated interface"` | Region name. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwA2uiSurface Surface="_surface" OnAction="HandleAction" />
```

---

### `TwMcpAppFrame`

**Location:** [`Components/UI/TwMcpAppFrame.razor`](../Components/UI/TwMcpAppFrame.razor)

Hosts an MCP app in a sandboxed iframe with an opaque origin, a locked-down CSP and a postMessage bridge.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Html` | `string` | `required` | Widget HTML. |
| `Title` | `string` | `"App"` | Frame title. |
| `Height` | `string` | `"16rem"` | Frame height. |
| `AllowedDomains` | `IReadOnlyList<string>?` | `null` | Extra origins permitted by the CSP. |
| `OnMessage` | `EventCallback<string>` | `null` | JSON the app posted to the host. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwMcpAppFrame Html="@_html" AllowedDomains="_cdns" OnMessage="FromApp" />
```

---

## Code & Syntax Studio

### `TwCodeFontModifier`

**Location:** [`Components/UI/TwCodeFontModifier.razor`](../Components/UI/TwCodeFontModifier.razor)

Enterprise code editor toolbar with interactive font sizer (A-/A+), monospace font switcher (JetBrains, Fira Code, Cascadia), ligatures toggle, line height stepper, word wrap, and syntax theme presets.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `"Code Editor & Font Toolbar"` | Header title or accessible label. |
| `FontSize` | `int` | `13` | Component sizing scale (Xs, Sm, Md, Lg). |
| `FontSizeChanged` | `EventCallback<int>` | `null` | Component sizing scale (Xs, Sm, Md, Lg). |
| `FontFamily` | `string` | `"'JetBrains Mono', monospace"` | Configures component behavior or styling. |
| `LineHeight` | `string` | `"leading-normal"` | Configures component behavior or styling. |
| `EnableLigatures` | `bool` | `true` | Configures component behavior or styling. |
| `WordWrap` | `bool` | `false` | Configures component behavior or styling. |
| `ShowLineNumbers` | `bool` | `true` | Configures component behavior or styling. |
| `ThemePreset` | `string` | `"emerald"` | Configures component behavior or styling. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `SampleCode` | `string?` | `@"// High-performance Blazor Tailwind Engine public sealed record ComponentSpec(string Name, string Category) {     public bool HasDarkModeSupport => true` | Configures component behavior or styling. |
| `OnSettingsChanged` | `EventCallback<CodeFontSettings>` | `null` | Callback invoked when state changes or item is selected. |

#### Razor Usage Example

```razor
<TwCodeFontModifier Title="Code Editor & Font Toolbar" FontSize="13" FontFamily="'JetBrains Mono', monospace" />
```

---

### `TwCodeTabs`

**Location:** [`Components/UI/TwCodeTabs.razor`](../Components/UI/TwCodeTabs.razor)

Multi-file tabbed code viewer with window controls, active file tabs, language icons, line numbers, and one-click clipboard copy.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Tabs` | `IReadOnlyList<CodeTab>` | `Array.Empty<CodeTab>()` | Configures component behavior or styling. |
| `ShowLineNumbers` | `bool` | `true` | Configures component behavior or styling. |
| `AllowCopy` | `bool` | `true` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwCodeTabs Tabs="Array.Empty<CodeTab>()" ShowLineNumbers="true" AllowCopy="true" />
```

---

### `TwCodeDiff`

**Location:** [`Components/UI/TwCodeDiff.razor`](../Components/UI/TwCodeDiff.razor)

Git-style visual code diff viewer highlighting additions and deletions with side-by-side line numbering, syntax color coding, and stats.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `FileName` | `string` | `"diff.razor"` | Configures component behavior or styling. |
| `Lines` | `IReadOnlyList<DiffLine>` | `Array.Empty<DiffLine>()` | Configures component behavior or styling. |
| `OldCode` | `string?` | `null` | Configures component behavior or styling. |
| `NewCode` | `string?` | `null` | Configures component behavior or styling. |
| `AllowCopy` | `bool` | `true` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwCodeDiff FileName="diff.razor" Lines="Array.Empty<DiffLine>()" AllowCopy="true" />
```

---

### `TwSnippet`

**Location:** [`Components/UI/TwSnippet.razor`](../Components/UI/TwSnippet.razor)

macOS terminal CLI command snippet container with prompt badges, window controls, and one-click copy transition.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Code` | `string` | `""` | Configures component behavior or styling. |
| `Prefix` | `string` | `"$"` | Configures component behavior or styling. |
| `Title` | `string?` | `null` | Header title or accessible label. |
| `ShowHeader` | `bool` | `true` | Configures component behavior or styling. |
| `ShowWindowControls` | `bool` | `true` | Configures component behavior or styling. |
| `ShowCopyButton` | `bool` | `true` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwSnippet Code="" Prefix="$" ShowHeader="true" />
```

---

### `TwCodeBlock`

**Location:** [`Components/UI/TwCodeBlock.razor`](../Components/UI/TwCodeBlock.razor)

Syntax code viewer with language badges, line numbers, and copy-to-clipboard button.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Code` | `string` | `string.Empty` | Configures component behavior or styling. |
| `Language` | `string?` | `"razor"` | Configures component behavior or styling. |
| `FileName` | `string?` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwCodeBlock Code="string.Empty" Language="razor" />
```

---

## Dashboards & Layouts

### `TwDashboard`

**Location:** [`Components/UI/TwDashboard.razor`](../Components/UI/TwDashboard.razor)

Executive multi-widget draggable dashboard layout with GridStack.js integration and responsive card snapping.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `CustomClass` | `string` | `""` | Optional CSS classes. |

#### Razor Usage Example

```razor
<TwDashboard />
```

---

## Data Visualization

### `TwActivityMeter`

**Location:** [`Components/TwActivityMeter.razor`](../Components/TwActivityMeter.razor)

High-density system activity meter with pulse rate indicators, status badge, and customizable threshold levels.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `double` | `0` | Current numeric activity level. |
| `Max` | `double` | `100` | Maximum activity threshold scale. |

#### Razor Usage Example

```razor
<TwActivityMeter Value="85" Max="100" />
```

---

## Developer Studio & Content Creation

### `TwIconPicker`

**Location:** [`Components/UI/TwIconPicker.razor`](../Components/UI/TwIconPicker.razor)

Interactive search-and-select icon & emoji explorer with live filtering, outline/solid toggles, size previews, and one-click Razor component snippet copying.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `"Open Source Vector Icons & Emoji Studio"` | Header title or accessible label. |
| `Subtitle` | `string` | `"Zero-dependency pure C# WASM vector icons and emojis for enterprise dashboards and forms"` | Header title or accessible label. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwIconPicker Title="Open Source Vector Icons & Emoji Studio" Subtitle="Zero-dependency pure C# WASM vector icons and emojis for enterprise dashboards and forms" CustomClass="" />
```

---

### `TwIcon`

**Location:** [`Components/UI/TwIcon.razor`](../Components/UI/TwIcon.razor)

Zero-dependency pure C# WASM inline vector SVG icon engine supporting 100+ curated icons (Heroicons & Lucide paths), customizable sizes, stroke widths, outline/solid modes, and semantic color inheritance.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Name` | `string` | `"sparkles"` | Configures component behavior or styling. |
| `Size` | `ComponentSize` | `ComponentSize.Md` | Component sizing scale (Xs, Sm, Md, Lg). |
| `IsSolid` | `bool` | `false` | Configures component behavior or styling. |
| `StrokeWidth` | `double` | `1.75` | Configures component behavior or styling. |
| `Class` | `string` | `""` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwIcon Name="sparkles" Size="ComponentSize.Md" StrokeWidth="1.75" />
```

---

### `TwEmoji`

**Location:** [`Components/UI/TwEmoji.razor`](../Components/UI/TwEmoji.razor)

Modern emoji component with shortcode mapping (:rocket:, :fire:, :sparkles:), Unicode glyph parsing, hover scale physics, and click-to-copy clipboard integration.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Shortcode` | `string?` | `null` | Configures component behavior or styling. |
| `Glyph` | `string?` | `null` | Configures component behavior or styling. |
| `Size` | `ComponentSize` | `ComponentSize.Md` | Component sizing scale (Xs, Sm, Md, Lg). |
| `AnimateOnHover` | `bool` | `true` | Configures component behavior or styling. |
| `CopyOnClick` | `bool` | `false` | Callback invoked when state changes or item is selected. |
| `Class` | `string` | `""` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwEmoji Size="ComponentSize.Md" AnimateOnHover="true" Class="" />
```

---

### `TwMarkdownEditor`

**Location:** [`Components/UI/TwMarkdownEditor.razor`](../Components/UI/TwMarkdownEditor.razor)

Enterprise-grade dual Markdown and HTML editor featuring live pure C# WASM typography preview, split/edit/preview modes, toolbar actions (headings, lists, bold, italics, code, tables), fullscreen Zen focus, and media drawer supporting image, video, audio, and link embedding.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string?` | `null` | Header title or accessible label. |
| `Value` | `string` | `""` | Primary data value or two-way bound model property. |
| `ValueChanged` | `EventCallback<string>` | `null` | Callback invoked when state changes or item is selected. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwMarkdownEditor Value="" CustomClass="" />
```

---

### `TwDockManager`

**Location:** [`Components/UI/TwDockManager.razor`](../Components/UI/TwDockManager.razor)

Desktop-grade multi-pane docking studio layout engine featuring Solution Explorer tree, multi-tab central document workspace, bottom diagnostic terminal output, and collapsible properties inspector.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string?` | `null` | Header title or accessible label. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwDockManager CustomClass="" />
```

---

### `TwSpreadsheet`

**Location:** [`Components/UI/TwSpreadsheet.razor`](../Components/UI/TwSpreadsheet.razor)

Spreadsheet calculation engine with alphanumeric grid coordinates (A1..E10), formula bar, formula evaluation (=SUM, =AVG, arithmetic), direct cell inline editing, and formatted total readouts.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `"Enterprise Spreadsheet Matrix"` | Header title or accessible label. |
| `Subtitle` | `string` | `"Excel-compatible cell matrix with live arithmetic, range aggregations, and formula bar"` | Header title or accessible label. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwSpreadsheet Title="Enterprise Spreadsheet Matrix" Subtitle="Excel-compatible cell matrix with live arithmetic, range aggregations, and formula bar" CustomClass="" />
```

---

### `TwOrgChart`

**Location:** [`Components/UI/TwOrgChart.razor`](../Components/UI/TwOrgChart.razor)

Hierarchical organization tree visualizer with pure vector SVG orthogonal connector lines, search highlight filter, recursive branch expand/collapse toggles, and A-/A+ canvas zoom controls.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `"Enterprise Organization Tree"` | Header title or accessible label. |
| `Subtitle` | `string` | `"Executive leadership structure, reporting hierarchies, and departmental clusters"` | Header title or accessible label. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `Root` | `OrgNode?` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwOrgChart Title="Enterprise Organization Tree" Subtitle="Executive leadership structure, reporting hierarchies, and departmental clusters" CustomClass="" />
```

---

## Documents & Forms

### `TwPdfFormFiller`

**Location:** [`Components/UI/TwPdfFormFiller.razor`](../Components/UI/TwPdfFormFiller.razor)

Interactive PDF form filler and attestation engine supporting initial stamping, legal full name signing, UTC timestamps, and corporate authority checkboxes.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `CustomClass` | `string` | `""` | Optional CSS classes. |

#### Razor Usage Example

```razor
<TwPdfFormFiller />
```

---

## Enterprise & Workplace Operations

### `TwRewardsCenter`

**Location:** [`Components/UI/TwRewardsCenter.razor`](../Components/UI/TwRewardsCenter.razor)

Employee perks and recognition rewards catalog with category filtering (Gift Cards, Swag, Workplace Flexibility, Learning, Wellness), points wallet ledger, search bar, and digital voucher redemption modal.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwRewardsCenter />
```

---

## Enterprise Data & Tables

### `TwEntityGrid`

**Location:** [`Components/UI/TwEntityGrid.razor`](../Components/UI/TwEntityGrid.razor)

Schema-driven grid over JSON records: columns come from an EntitySchema, related names sit right after the first two columns, decimals print with two places, and paging, sorting and search go to the server (one request per page, stale answers ignored).

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Schema` | `EntitySchema` | `required` | Fields and relations of the entity. |
| `Load` | `Func<GridDataRequest, Task<GridDataResult<JsonObject>>>` | `required` | Reads one page (skip, take, sort, search) from the server. |
| `PageSize` | `int` | `10` | Rows per page. |
| `ActionsTemplate` | `RenderFragment<JsonObject>?` | `null` | Edit/Delete buttons; shown in a pinned, opaque column. |
| `ActionsTitle` | `string` | `"Actions"` | Header of the actions column. |
| `Class` | `string?` | `null` | Extra classes. |

#### Razor Usage Example

```razor
<TwEntityGrid Schema="_schema" Load="Api.LoadAsync" PageSize="10" />
```

---

### `TwTreeList`

**Location:** [`Components/UI/TwTreeList.razor`](../Components/UI/TwTreeList.razor)

Hierarchical table (treegrid): nests rows by parent key, expand/collapse with Left/Right arrows, per-level sorting, single selection.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `IReadOnlyList<TItem>` | `required` | Flat list with parent keys. |
| `KeySelector` | `Func<TItem,string>` | `required` | Row key. |
| `ParentSelector` | `Func<TItem,string?>` | `required` | Parent key (missing parents become roots). |
| `Columns` | `IReadOnlyList<Column>` | `required` | Header, Value, Sortable. |
| `SelectedKey` | `string?` | `null` | Selected row. |
| `InitiallyExpanded` | `bool` | `false` | Start expanded. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwTreeList TItem="Dept" Items="_depts" KeySelector="d => d.Id" ParentSelector="d => d.ParentId" Columns="_cols" />
```

---

### `TwKanbanBoard`

**Location:** [`Components/UI/TwKanbanBoard.razor`](../Components/UI/TwKanbanBoard.razor)

Interactive workflow Kanban board with customizable columns, WIP limit thresholds, task cards, priority indicators, subtask checklists, and lane transition buttons.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `"Sprint Workflow Kanban"` | Header title or accessible label. |
| `Columns` | `List<KanbanColumn>` | `new()` | Configures component behavior or styling. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `OnCardMoved` | `EventCallback<KanbanCard>` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwKanbanBoard Title="Sprint Workflow Kanban" Columns="new()" CustomClass="" />
```

---

### `TwDataGrid`

**Location:** [`Components/UI/TwDataGrid.razor`](../Components/UI/TwDataGrid.razor)

Enterprise data grid: sortable headers (keyboard accessible, Shift adds a second sort), search and per-column filters, grouping with counts and summaries, footer totals, master-detail rows, inline editing with validation, column chooser, CSV export, saved layout, frozen column, conditional formatting, selection, paging and mobile cards. Over 200 in-memory records it virtualizes (only the rows on screen are drawn, one scrolling body, no pages); search, sort, filters and select-all still cover every record.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `IEnumerable<TItem>` | `empty` | Records to show. |
| `Columns` | `RenderFragment?` | `null` | TwDataGridColumn definitions. |
| `Label` | `string` | `"Data grid"` | Accessible table caption. |
| `Selectable` | `bool` | `false` | Row checkboxes and a select-all for the page. |
| `Virtualize` | `bool?` | `null` | Draw only the visible rows. null = on for more than VirtualizeThreshold in-memory records; always off with ServerData (it pages). |
| `VirtualizeThreshold` | `int` | `200` | Record count above which Virtualize = null switches it on. |
| `RowHeight` | `float?` | `null` | Row height in px when virtualized (72 with an actions column, else 52). Keep rows to one line. |
| `ShowGlobalSearch` | `bool` | `true` | Search box over all columns. |
| `SearchPredicate` | `Func<TItem,string,bool>?` | `null` | Custom search logic. |
| `AllowColumnFiltering` | `bool` | `true` | Per-column filter row. |
| `AllowGrouping` | `bool` | `false` | Group by list; group headers show a count and summaries. |
| `GroupBy` | `string?` | `null` | Title of the column to group by initially. |
| `AllowColumnChooser` | `bool` | `false` | Columns menu to show and hide columns. |
| `AllowExport` | `bool` | `false` | Export CSV of all filtered rows and visible columns (formula-safe). |
| `ExportFileName` | `string` | `"export.csv"` | Download name. |
| `DetailTemplate` | `RenderFragment<TItem>?` | `null` | Expandable detail row under each record. |
| `RowClassSelector` | `Func<TItem,string?>?` | `null` | Conditional formatting for a row. |
| `StickyHeader` | `bool` | `false` | Keep the header in view. |
| `MaxHeight` | `string?` | `null` | CSS max-height; the body scrolls, the header stays. |
| `StateKey` | `string?` | `null` | Remember sort, search, filters, grouping, page size and visible columns in localStorage. |
| `RowSaved` | `EventCallback<TItem>` | `null` | An edited row passed validation and was applied. |
| `ActionsTemplate` | `RenderFragment<TItem>?` | `null` | Per-row action buttons. |
| `ActionsTitle` | `string` | `"Actions"` | Actions column header. |
| `MobileCardTemplate` | `RenderFragment<TItem>?` | `null` | Card layout on phones. |
| `BatchActionsTemplate` | `RenderFragment<IReadOnlyList<TItem>>?` | `null` | Buttons shown while rows are selected. |
| `EmptyText` | `string` | `"No records found matching your criteria."` | Shown when nothing matches. |
| `ShowPagination` | `bool` | `true` | Page controls. |
| `PageSize` | `int` | `5` | Rows per page. |
| `PageSizeOptions` | `int[]` | `5,10,20,50` | Choices in the page-size list. |
| `ShowPageSizeSelector` | `bool` | `true` | Page-size list. |
| `CurrentPage` | `int` | `1` | Current page (two-way with CurrentPageChanged). |
| `CurrentPageChanged` | `EventCallback<int>` | `null` | Page changed. |
| `SelectionChanged` | `EventCallback<IReadOnlyList<TItem>>` | `null` | Selection changed. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwDataGrid TItem="Order" Items="_orders" Label="Orders" Selectable="true" AllowGrouping="true" AllowColumnChooser="true" AllowExport="true" StateKey="orders">
    <Columns>
        <TwDataGridColumn TItem="Order" Title="Customer" Property="o => o.Customer" Editable="true" EditValue="SetCustomer" />
        <TwDataGridColumn TItem="Order" Title="Amount" Property="o => o.Amount" Summary="GridSummary.Sum" SummaryFormat="C0" />
    </Columns>
</TwDataGrid>
```

---

### `TwDataGridColumn`

**Location:** [`Components/UI/TwDataGridColumn.razor`](../Components/UI/TwDataGridColumn.razor)

Column definition for TwDataGrid: binding, sorting, filtering, width, alignment, templates, footer and group summary, grouping and chooser options, frozen column, inline editing with validation, and conditional cell formatting.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `""` | Header text; also the column key for saved state. |
| `Property` | `Func<TItem,object?>?` | `null` | Display value (also used for sort, filter, group, summary, export). |
| `SortBy` | `Func<TItem,object?>?` | `null` | Sort value when it differs from the display value. |
| `Sortable` | `bool` | `true` | Header is a sort button. |
| `Filterable` | `bool` | `true` | Shows a filter box. |
| `Summary` | `GridSummary` | `None` | Count, Sum, Average, Min or Max in the footer and group headers. |
| `SummaryFormat` | `string?` | `null` | .NET numeric format, for example C0 or N1. |
| `Groupable` | `bool` | `true` | Offered in Group by. |
| `Hideable` | `bool` | `true` | Can be hidden in the chooser (the last visible column never can). |
| `Frozen` | `bool` | `false` | Pinned at the start edge while scrolling sideways (use on the first column). |
| `Editable` | `bool` | `false` | Shows an input while its row is edited. |
| `EditValue` | `Action<TItem,string>?` | `null` | Applies the edited text to the item on save. |
| `EditText` | `Func<TItem,string>?` | `display value` | Initial edit text. |
| `Validate` | `Func<string,string?>?` | `null` | Returns an error message to block the save. |
| `InputType` | `string` | `"text"` | text, number, date or email. |
| `CellClassSelector` | `Func<TItem,string?>?` | `null` | Conditional formatting for a cell. |
| `Width` | `string?` | `null` | CSS width. |
| `MinWidth` | `string?` | `null` | CSS min-width. |
| `MaxWidth` | `string?` | `null` | CSS max-width. |
| `Align` | `string` | `"left"` | left, center or right. |
| `CellTemplate` | `RenderFragment<TItem>?` | `null` | Custom cell. |
| `HeaderTemplate` | `RenderFragment?` | `null` | Custom header. |
| `FilterTemplate` | `RenderFragment?` | `null` | Custom filter control. |
| `FilterPlaceholder` | `string?` | `null` | Filter box hint. |
| `HeaderClass` | `string?` | `null` | Extra header classes. |
| `CellClass` | `string?` | `null` | Extra cell classes. |

#### Razor Usage Example

```razor
<TwDataGridColumn TItem="Order" Title="Amount" Property="o => o.Amount" Align="right" Summary="GridSummary.Sum" SummaryFormat="C0" Editable="true" InputType="number" EditValue="SetAmount" Validate="Positive" />
```

---

### `TwMasterDetailGrid`

**Location:** [`Components/UI/TwMasterDetailGrid.razor`](../Components/UI/TwMasterDetailGrid.razor)

Hierarchical data table featuring expandable master rows with nested child grids, chevron toggles, multi-row or auto-collapse expansion modes, independent child column sorting, and add/delete child line item callbacks.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `IEnumerable<TMaster>?` | `null` | Master record dataset. |
| `ChildSelector` | `Func<TMaster, IEnumerable<TChild>>?` | `null` | Delegate resolving nested child records for each master row. |
| `Title` | `string` | `"Enterprise Hierarchical Grid"` | Main table title. |
| `AllowMultipleExpanded` | `bool` | `true` | Permits multiple master rows to be expanded concurrently. |
| `AutoCollapseDetailRow` | `bool` | `false` | Automatically closes other expanded rows when expanding a new master row. |
| `AllowAddChild` | `bool` | `false` | Enables '+ Add Item' action inside the nested detail grid. |
| `AllowDeleteChild` | `bool` | `false` | Enables remove action button on nested child records. |

#### Razor Usage Example

```razor
<TwMasterDetailGrid TMaster="Order" TChild="OrderItem" Items="_orders" ChildSelector="o => o.Items" Title="Orders & Line Items" AllowAddChild="true" />
```

---

### `TwMasterDetail`

**Location:** [`Components/UI/TwMasterDetail.razor`](../Components/UI/TwMasterDetail.razor)

Synchronized Master/Detail split-pane workspace with Column (horizontal) and Row (vertical) layout modes, live search, status chips, multi-tab detail inspector (Master Form, Child Records Grid, Audit Timeline), and two-way item selection binding.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `IEnumerable<TMaster>?` | `null` | Collection of master records for the master pane. |
| `SelectedItem` | `TMaster?` | `null` | Active selected master record rendered in the detail inspector. |
| `ChildSelector` | `Func<TMaster, IEnumerable<TChild>>?` | `null` | Delegate retrieving child items for the selected master record. |
| `AllowCreate` | `bool` | `true` | Enables the 'New Entry' action button in the toolbar. |
| `AllowDelete` | `bool` | `true` | Enables the 'Delete' action in the detail inspector. |
| `Title` | `string` | `"Master-Detail Enterprise Workspace"` | Main workspace header title. |

#### Razor Usage Example

```razor
<TwMasterDetail TMaster="Customer" TChild="Invoice" Items="_customers" ChildSelector="c => c.Invoices" @bind-SelectedItem="_selectedCustomer" />
```

---

### `TwMasterDetailForm`

**Location:** [`Components/UI/TwMasterDetailForm.razor`](../Components/UI/TwMasterDetailForm.razor)

Turnkey transactional Master/Detail form designed for Invoices, Purchase Orders, and Service Statements with top metadata fields, dynamic line item table, '+ Add Item', real-time subtotal, tax %, discount, and grand total recalculation.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `"Invoice & Master Detail Statement"` | Document form title. |
| `DocumentNumber` | `string` | `"INV-2026-8942"` | Unique transaction or invoice identifier. |
| `Status` | `string` | `"Draft"` | Workflow status badge. |
| `ClientName` | `string` | `"Acme Global Financial"` | Recipient or client name. |
| `DocumentDate` | `DateTime` | `DateTime.Today` | Transaction date. |
| `PaymentTerms` | `string` | `"Net 30"` | Contractual payment schedule. |
| `CurrencyCode` | `string` | `"USD"` | Currency code (USD, EUR, GBP, CAD). |
| `TaxRatePercent` | `decimal` | `8.5m` | Tax percentage applied to line item subtotal. |
| `DiscountAmount` | `decimal` | `0m` | Fixed discount subtracted from grand total. |

#### Razor Usage Example

```razor
<TwMasterDetailForm Title="Invoice Statement" DocumentNumber="INV-2026-001" ClientName="Acme Corp" Status="Draft" OnSubmit="HandleSubmit" />
```

---

### `TwImportExportWizard`

**Location:** [`Components/UI/TwImportExportWizard.razor`](../Components/UI/TwImportExportWizard.razor)

Multi-format data interchange wizard supporting CSV, JSON, Excel TSV, ANSI SQL INSERT, and XML serialization with drag-and-drop file ingestion, automatic column mapping, staged data quality review, error filtering, and direct clipboard/file download generation.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `IEnumerable<TItem>?` | `null` | Active entity dataset for export operations. |
| `Title` | `string` | `"Data Import & Export Wizard"` | Main header title. |
| `Subtitle` | `string` | `"Multi-format data interchange tool..."` | Descriptive header subtitle. |
| `Class` | `string` | `""` | Custom Tailwind CSS utility classes. |
| `OnImportCompleted` | `EventCallback<DataImportResult<TItem>>` | `default` | Fires when staged incoming records are confirmed and ingested. |
| `OnExportGenerated` | `EventCallback<DataExportResult>` | `default` | Fires when serialized file payloads are emitted. |

#### Razor Usage Example

```razor
<TwImportExportWizard TItem="Customer" Items="_customers" Title="Data Transfer Hub" OnImportCompleted="HandleImport" OnExportGenerated="HandleExport" />
```

---

### `TwDataTable`

**Location:** [`Components/UI/TwDataTable.razor`](../Components/UI/TwDataTable.razor)

Generic table component that automatically transforms into touch-friendly stacked cards on mobile screens.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `IReadOnlyList<TItem>` | `Array.Empty<TItem>()` | Configures component behavior or styling. |
| `TableHeader` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `RowTemplate` | `RenderFragment<TItem>` | `default!` | Configures component behavior or styling. |
| `MobileTemplate` | `RenderFragment<TItem>?` | `null` | Configures component behavior or styling. |
| `EmptyText` | `string` | `"No records found"` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwDataTable Items="Array.Empty<TItem>()" RowTemplate="default!" EmptyText="No records found" />
```

---

### `TwPagination`

**Location:** [`Components/UI/TwPagination.razor`](../Components/UI/TwPagination.razor)

Responsive pagination control showing Prev/Next on mobile devices and numbered page pills on desktop.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `CurrentPage` | `int` | `1` | Configures component behavior or styling. |
| `TotalPages` | `int` | `1` | Configures component behavior or styling. |
| `CurrentPageChanged` | `EventCallback<int>` | `null` | Callback invoked when state changes or item is selected. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwPagination CurrentPage="1" TotalPages="1" />
```

---

### `TwTree`

**Location:** [`Components/UI/TwTree.razor`](../Components/UI/TwTree.razor)

Hierarchical tree view component with expandable/collapsible nodes and selection tracking.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `SelectedId` | `string?` | `null` | Configures component behavior or styling. |
| `SelectedIdChanged` | `EventCallback<string>` | `null` | Callback invoked when state changes or item is selected. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwTree>
    <span>Content</span>
</TwTree>
```

---

### `TwTreeItem`

**Location:** [`Components/UI/TwTreeItem.razor`](../Components/UI/TwTreeItem.razor)

Individual node item inside TwTree with custom icons and nested child rendering.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Id` | `string` | `Guid.NewGuid().ToString("N")` | Configures component behavior or styling. |
| `Text` | `string` | `""` | Configures component behavior or styling. |
| `Badge` | `string?` | `null` | Configures component behavior or styling. |
| `InitiallyExpanded` | `bool` | `null` | Configures component behavior or styling. |
| `IconContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwTreeItem Id="Guid.NewGuid().ToString(N)" Text="" />
```

---

## Feedback & Alerts

### `TwSnackbar`

**Location:** [`Components/UI/TwSnackbar.razor`](../Components/UI/TwSnackbar.razor)

Actionable bottom floating notification snackbar with action trigger callback, animated progress auto-dismiss countdown bar, pause on hover, and semantic color variants.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Message` | `string` | `""` | Notification message. |
| `ActionText` | `string?` | `null` | Optional action button text (e.g. Undo, Retry). |
| `DurationMs` | `int` | `4000` | Auto-dismiss duration in ms (0 for persistent). |
| `ShowProgress` | `bool` | `true` | Render animated countdown progress bar. |
| `Variant` | `SemanticVariant` | `SemanticVariant.Default` | Color scheme (Default, Info, Success, Warning, Danger). |
| `Position` | `TwSnackbarPosition` | `TwSnackbarPosition.BottomCenter` | Viewport anchoring position. |

#### Razor Usage Example

```razor
<TwSnackbar Message="Invoice archived" ActionText="Undo" Variant="SemanticVariant.Info" @bind-IsOpen="_showSnack" OnAction="HandleUndo" />
```

---

## Financial & Analytical Intelligence

### `TwFinancialChart`

**Location:** [`Components/UI/TwFinancialChart.razor`](../Components/UI/TwFinancialChart.razor)

High-frequency Candlestick and Area financial charting engine with real-time volume histogram, 20-period moving average line, crosshair hover HUD inspector, timeframe range presets (1W, 1M, 3M, YTD, 1Y, ALL), and zero-JS vector SVG rendering.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Ticker` | `string` | `"BLZR"` | Configures component behavior or styling. |
| `CompanyName` | `string` | `"Blazor Technologies Inc."` | Configures component behavior or styling. |
| `Exchange` | `string` | `"NASDAQ"` | Configures component behavior or styling. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `Data` | `List<FinancialCandle>?` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwFinancialChart Ticker="BLZR" CompanyName="Blazor Technologies Inc." Exchange="NASDAQ" />
```

---

### `TwTreemap`

**Location:** [`Components/UI/TwTreemap.razor`](../Components/UI/TwTreemap.razor)

Multi-dimensional area treemap and valuation heatmap visualizing hierarchical assets, portfolio market caps, and performance returns with squarified 2D tiling, dynamic drill-down breadcrumb, and color ramps.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `"Market Valuation Heatmap & Treemap"` | Header title or accessible label. |
| `Subtitle` | `string` | `"Hierarchical squarified area map of enterprise assets and percentage price shifts"` | Header title or accessible label. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `Items` | `List<TreemapItem>?` | `null` | Configures component behavior or styling. |
| `OnItemClick` | `EventCallback<TreemapItem>` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwTreemap Title="Market Valuation Heatmap & Treemap" Subtitle="Hierarchical squarified area map of enterprise assets and percentage price shifts" CustomClass="" />
```

---

### `TwPivotGrid`

**Location:** [`Components/UI/TwPivotGrid.razor`](../Components/UI/TwPivotGrid.razor)

Enterprise OLAP cross-tabulation matrix supporting dynamic row/column field pivoting, value aggregations (Sum, Average, Count), interactive field drop drawer, and hierarchical subtotal and grand total calculations.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `"Enterprise Financial Pivot Grid"` | Header title or accessible label. |
| `Subtitle` | `string` | `"Multi-dimensional cross-tabulation matrix with real-time grouping & aggregation"` | Header title or accessible label. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `Records` | `List<PivotRecord>?` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwPivotGrid Title="Enterprise Financial Pivot Grid" Subtitle="Multi-dimensional cross-tabulation matrix with real-time grouping & aggregation" CustomClass="" />
```

---

### `TwPivotTable`

**Location:** [`Components/UI/TwPivotTable.razor`](../Components/UI/TwPivotTable.razor)

Full-featured generic OLAP 4-Axis pivot matrix (Row Area, Column Area, Data Area, Filter Area) with hierarchical row expansion/collapse, multi-measure aggregators (Sum, Average, Count, DistinctCount, Min, Max, Product), dynamic heatmap color scaling, interactive field list configurator drawer, CSV export, and raw record drill-down cell inspector.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `IEnumerable<TItem>?` | `null` | Raw tabular record collection to pivot. |
| `Fields` | `List<PivotFieldDefinition<TItem>>?` | `null` | Explicit field definitions or auto-discovered public properties. |
| `Config` | `PivotConfig?` | `null` | Configuration of active row/col dimensions, measures, and filter slicers. |
| `Title` | `string` | `"Cross-Tabulation Pivot Table"` | Main title in the header toolbar. |
| `Subtitle` | `string` | `"Multi-dimensional analytical OLAP matrix..."` | Secondary descriptive text. |
| `AllowFieldSelector` | `bool` | `true` | Enables the interactive 4-axis field list configuration panel. |
| `AllowDrillDown` | `bool` | `true` | Clicking an aggregate cell opens the raw record drill-down dialog. |
| `AllowExport` | `bool` | `true` | Enables direct CSV export of the computed cross-tabulation matrix. |
| `Class` | `string` | `""` | Additional Tailwind utility classes applied to the root container. |

#### Razor Usage Example

```razor
<TwPivotTable TItem="SaleRecord" Items="_sales" Title="Global Sales Analysis" AllowFieldSelector="true" AllowDrillDown="true" />
```

---

### `TwPivotChart`

**Location:** [`Components/UI/TwPivotChart.razor`](../Components/UI/TwPivotChart.razor)

Connected OLAP pivot chart visualizer bound directly to dynamic pivot matrix dimensions with grouped SVG mathematical bars, auto-scaled Y-axis gridlines, legend series toggles, and zero external graphing dependencies.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `IEnumerable<TItem>?` | `null` | Dataset to aggregate and visualize. |
| `RowDimension` | `string` | `"Region"` | Category dimension mapped along the X-axis. |
| `ColumnDimension` | `string` | `"Quarter"` | Grouping dimension mapped to color series. |
| `MeasureField` | `string` | `"Revenue"` | Numeric property to compute and aggregate. |
| `Aggregator` | `PivotAggregator` | `PivotAggregator.Sum` | Mathematical operation applied to the measure field. |
| `Title` | `string` | `"Dynamic OLAP Pivot Chart"` | Header title. |
| `Subtitle` | `string` | `"Interactive multi-series visualization..."` | Descriptive subtitle. |

#### Razor Usage Example

```razor
<TwPivotChart TItem="SaleRecord" Items="_sales" RowDimension="Region" ColumnDimension="Quarter" MeasureField="Revenue" Title="Revenue Breakdown" />
```

---

## Financial & OCR

### `TwMicrCheckReader`

**Location:** [`Components/UI/TwMicrCheckReader.razor`](../Components/UI/TwMicrCheckReader.razor)

Check 21 paper check & draft reader powered by 100% local Tesseract WebAssembly, ANSI X9.27 E-13B MICR decoding, 150+ NACHA Bank Routing Directory resolver, Mod-10 Fed routing validation, and NACHA 94-char fixed-width ACH file generation (our own implementation).

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Class` | `string` | `""` | Optional CSS classes. |
| `OnCheckScanned` | `EventCallback<CheckDraftOcrResult>` | `null` | Callback fired when a check or draft is scanned. |
| `OnInsertMarkdown` | `EventCallback<string>` | `null` | Callback fired when Markdown ledger table is exported. |
| `OnDepositApproved` | `EventCallback<CheckDraftOcrResult>` | `null` | Callback fired when remote deposit capture (RDC) is approved. |
| `OnNachaExported` | `EventCallback<string>` | `null` | Callback fired when 94-character fixed-width NACHA ACH batch file is exported. |
| `OnJsonExported` | `EventCallback<string>` | `null` | Callback fired when Check 21 JSON payload is exported. |

#### Razor Usage Example

```razor
<TwMicrCheckReader OnCheckScanned="HandleCheck" OnDepositApproved="HandleApproved" OnNachaExported="HandleNacha" OnJsonExported="HandleJson" />
```

---

### `TwDocumentOcrStudio`

**Location:** [`Components/UI/TwDocumentOcrStudio.razor`](../Components/UI/TwDocumentOcrStudio.razor)

Full Form & PDF OCR Studio supporting paper checks, official bank drafts, commercial invoices, and IRS Form W-9 with spatial bounding boxes and Markdown table generation.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Class` | `string` | `""` | Optional CSS classes. |
| `OnMarkdownExtracted` | `EventCallback<string>` | `null` | Fired when extracted markdown report is copied or exported. |

#### Razor Usage Example

```razor
<TwDocumentOcrStudio OnMarkdownExtracted="HandleMarkdown" />
```

---

## Forms & Controls

### `TwMaskInput`

**Location:** [`Components/UI/TwMaskInput.razor`](../Components/UI/TwMaskInput.razor)

High-performance masked text input with slot validation, prompt guides, and Raw vs WithFormatting modes for Phone, SSN, Credit Card, and custom masks.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `string` | `""` | Two-way bound input value. |
| `Mask` | `string` | `"(###) ###-####"` | Mask pattern (# digit, A letter, * alphanumeric). |
| `Prompt` | `char` | `'_'` | Prompt symbol for unfilled slots. |
| `ValueMode` | `MaskValueMode` | `Raw` | Raw unformatted string vs formatted string with mask literals. |
| `Preset` | `MaskPreset` | `Custom` | Preconfigured mask presets (Phone, Ssn, CreditCard, Date, ZipCode). |
| `Label` | `string?` | `null` | Input label. |
| `PrefixIcon` | `string?` | `null` | Prefix icon symbol. |

#### Razor Usage Example

```razor
<TwMaskInput Label="Phone" Mask="(###) ###-####" @bind-Value="@phone" />
```

---

### `TwRangeSlider`

**Location:** [`Components/UI/TwRangeSlider.razor`](../Components/UI/TwRangeSlider.razor)

Dual-thumb interactive range slider with Lower/Upper thumb bounds, colored track highlights, ticks, and touch-friendly tap targets.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Lower` | `double` | `20` | Lower boundary thumb value (two-way bound). |
| `Upper` | `double` | `80` | Upper boundary thumb value (two-way bound). |
| `Min` | `double` | `0` | Minimum range limit. |
| `Max` | `double` | `100` | Maximum range limit. |
| `Step` | `double` | `1` | Granular increment step. |
| `Prefix` | `string?` | `null` | Prefix symbol for values ($). |
| `Suffix` | `string?` | `null` | Suffix symbol for values (%). |

#### Razor Usage Example

```razor
<TwRangeSlider Label="Budget" Min="0" Max="1000" @bind-Lower="@minVal" @bind-Upper="@maxVal" Prefix="$" />
```

---

## Forms & Inputs

### `TwPredictiveIntent`

**Location:** [`Components/UI/TwPredictiveIntent.razor`](../Components/UI/TwPredictiveIntent.razor)

2027 anticipatory state machine demonstrating zero-click UX: gaze focal tracking, tactile biometric pulse sensor, and automated intent action synthesis.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Class` | `string` | `""` | Optional CSS classes for outer container. |

#### Razor Usage Example

```razor
<TwPredictiveIntent />
```

---

### `TwAguiFormCanvas`

**Location:** [`Components/UI/TwAguiFormCanvas.razor`](../Components/UI/TwAguiFormCanvas.razor)

AG-UI Adaptive Generative UI form engine with AI confidence percentages, 2-way diff binding, dynamic field schema synthesis, and human-in-the-loop gating.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Schema` | `AguiFormSchema` | `null` | The synthesized adaptive form schema. |
| `OnSubmit` | `EventCallback<Dictionary<string, object>>` | `null` | Callback on approved form submission. |

#### Razor Usage Example

```razor
<TwAguiFormCanvas Schema="@schema" OnSubmit="HandleSubmit" />
```

---

## Fullstack .NET 10+

### `TwStreamingGrid`

**Location:** [`Components/TwStreamingGrid.razor`](../Components/TwStreamingGrid.razor)

High-throughput streaming DataGrid consuming IAsyncEnumerable<TItem> server streams with real-time row pulse animations and 1M row virtualization.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `"High-Throughput Streaming Engine (IAsyncEnumerable)"` | Grid title in header toolbar. |
| `Subtitle` | `string` | `"Real-time telemetry pipeline consuming asynchronous server streams..."` | Toolbar subtitle. |
| `MaxBufferCapacity` | `int` | `5000` | Maximum items kept in memory before circular eviction. |
| `Class` | `string` | `""` | Optional CSS classes. |

#### Razor Usage Example

```razor
<TwStreamingGrid Title="Real-time Telemetry" MaxBufferCapacity="5000" />
```

---

### `TwHydratedView`

**Location:** [`Components/TwHydratedView.razor`](../Components/TwHydratedView.razor)

Universal zero-flicker SSR hydration container eliminating duplicate database queries and visual layout shift between prerender and client hydration.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ChildContent` | `RenderFragment` | `null` | The hydrated content to render. |
| `SkeletonTemplate` | `RenderFragment` | `null` | Custom skeleton placeholder to show during initial async load. |
| `IsLoading` | `bool` | `false` | Whether the component is actively fetching initial state. |
| `IsRestored` | `bool` | `false` | Whether the component state was restored from SSR prerender cache. |
| `ShowHydrationBadge` | `bool` | `true` | Displays subtle SSR/Hydrated badge indicator in top right. |
| `Class` | `string` | `""` | Optional CSS classes. |

#### Razor Usage Example

```razor
<TwHydratedView IsRestored="@_isRestored" IsLoading="@_loading"><p>Hydrated Content</p></TwHydratedView>
```

---

### `TwFullstackForm`

**Location:** [`Components/TwFullstackForm.razor`](../Components/TwFullstackForm.razor)

Dual-mode enterprise form engine supporting Static SSR Enhanced Form POST (zero JS) and Interactive Auto Client RPC seamlessly.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `"Universal Fullstack Form Engine"` | Form card title. |
| `Subtitle` | `string` | `"Dual-mode form handling supporting Static SSR and Interactive Auto..."` | Form card subtitle. |
| `Class` | `string` | `""` | Optional CSS classes. |

#### Razor Usage Example

```razor
<TwFullstackForm Title="Universal Fullstack Form" />
```

---

### `TwFullstackHub`

**Location:** [`Components/TwFullstackHub.razor`](../Components/TwFullstackHub.razor)

Full-stack developer architecture explorer, zero-flicker hydration workbench, and streaming playground for .NET 10+ Web Apps.

#### Parameters & Settings

This component has no parameters.

#### Razor Usage Example

```razor
<TwFullstackHub />
```

---

### `TwUnionBoundary`

**Location:** [`Components/UI/TwUnionBoundary.razor`](../Components/UI/TwUnionBoundary.razor)

C# 15 Discriminated Union boundary rendering exhaustive states (Loading, Success, Empty, Failure) with compile-time branch safety.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `State` | `UiState<TItem>` | `required` | Active algebraic union UI state. |
| `Loading` | `RenderFragment` | `null` | Template rendered during loading state. |
| `Success` | `RenderFragment<TItem>` | `null` | Template rendered on successful state with typed item context. |
| `Empty` | `RenderFragment` | `null` | Template rendered on empty collection or record set. |
| `Failure` | `RenderFragment<UiState<TItem>.Failure>` | `null` | Template rendered on error with failure details context. |

#### Razor Usage Example

```razor
<TwUnionBoundary TItem="string" State="@state"><Success Context="d">@d</Success></TwUnionBoundary>
```

---

### `TwDonutCache`

**Location:** [`Components/TwDonutCache.razor`](../Components/TwDonutCache.razor)

Blazor Donut Caching container that edge-caches 95%+ of static SSR page markup while dynamically perforating personalized donut holes.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `CacheKey` | `string` | `"default_donut"` | Unique cache key for the static shell segment. |
| `Duration` | `TimeSpan` | `5 minutes` | Cache expiration time-to-live. |
| `Tags` | `List<string>` | `["catalog"]` | Cache invalidation tags for programmatic cache busting. |
| `StaticContent` | `RenderFragment` | `null` | Static markup cached at the edge across users. |
| `DynamicHole` | `RenderFragment` | `null` | Dynamic personalized slot evaluated per-request. |

#### Razor Usage Example

```razor
<TwDonutCache CacheKey="catalog" Tags="@(["catalog"])"><DynamicHole>...</DynamicHole></TwDonutCache>
```

---

### `TwPostQuantumVault`

**Location:** [`Components/TwPostQuantumVault.razor`](../Components/TwPostQuantumVault.razor)

Post-Quantum Cryptography secure enclave implementing NIST FIPS 203 ML-KEM-768 key encapsulation and zero-trust authenticated encryption.

#### Parameters & Settings

This component has no parameters.

#### Razor Usage Example

```razor
<TwPostQuantumVault />
```

---

### `TwTelemetryDashboard`

**Location:** [`Components/TwTelemetryDashboard.razor`](../Components/TwTelemetryDashboard.razor)

Native OpenTelemetry (OTel) developer cockpit rendering real-time ActivitySource lifecycle spans, render latency waterfalls, and heap telemetry.

#### Parameters & Settings

This component has no parameters.

#### Razor Usage Example

```razor
<TwTelemetryDashboard />
```

---

### `TwVectorMath`

**Location:** [`Components/UI/TwVectorMath.razor`](../Components/UI/TwVectorMath.razor)

WebAssembly SIMD hardware acceleration and vector math engine evaluating high-throughput financial dot products, vector sums, and euclidean distance norms with comparative scalar telemetry.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Class` | `string?` | `null` | Optional CSS utility classes. |

#### Razor Usage Example

```razor
<TwVectorMath />
```

---

### `TwAspireHub`

**Location:** [`Components/UI/TwAspireHub.razor`](../Components/UI/TwAspireHub.razor)

.NET Aspire 11 cloud-native orchestration cockpit with live microservice topology inspector, real-time port health probing, environment variable explorer, structured logs stream, and instant 1-click downloads for executable Program.cs AppHost and docker-compose.yml specifications.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Class` | `string?` | `null` | Optional CSS utility classes. |

#### Razor Usage Example

```razor
<TwAspireHub />
```

---

## Gamification & Celebrations

### `TwPrizeWheel`

**Location:** [`Components/UI/TwPrizeWheel.razor`](../Components/UI/TwPrizeWheel.razor)

Interactive SVG spinning fortune prize wheel with realistic deceleration physics, top pointer tick indicator, customizable prize wedges, celebratory confetti burst, and winner announcement banner for team morale boosts and party giveaways.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |
| `Title` | `string` | `"Spin to Win!"` | Component header title. |
| `Prizes` | `List<PrizeItem>` | `Default 8 wedges` | List of prize wedges with titles, emojis, colors, and descriptions (inspired by MyWheel). |
| `OnPrizeWon` | `EventCallback<PrizeItem>` | `null` | Fired upon completion of deceleration on the winning wedge. |

#### Razor Usage Example

```razor
<TwPrizeWheel OnPrizeWon="(prize) => { /* Handle prize */ }" />
```

---

### `TwCelebrationEditor`

**Location:** [`Components/UI/TwCelebrationEditor.razor`](../Components/UI/TwCelebrationEditor.razor)

Visual workbench for configuring canvas particle celebrations and confetti bursts. Sliders for particle count, velocity, spread angle, gravity, and origin height with instant live testing and copyable C#/Razor markup.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwCelebrationEditor />
```

---

### `TwBirthdayBanner`

**Location:** [`Components/UI/TwBirthdayBanner.razor`](../Components/UI/TwBirthdayBanner.razor)

Company-wide birthday alert banner with quick emoji reaction buttons and celebration confetti trigger.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `EmployeeName` | `string` | `""` | Recipient employee name. |
| `Department` | `string` | `""` | Employee department. |

#### Razor Usage Example

```razor
<TwBirthdayBanner EmployeeName="Jane Doe" Department="Engineering" />
```

---

### `TwBirthdayCard`

**Location:** [`Components/UI/TwBirthdayCard.razor`](../Components/UI/TwBirthdayCard.razor)

Role-tailored interactive 3D card with inside jokes, particle reveals, and peer signing wall.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `RecipientName` | `string` | `""` | Card recipient name. |

#### Razor Usage Example

```razor
<TwBirthdayCard RecipientName="Jane Doe" Role="Architect" />
```

---

## Hardware & Media Interop

### `TwMediaRecorder`

**Location:** [`Components/UI/TwMediaRecorder.razor`](../Components/UI/TwMediaRecorder.razor)

In-browser microphone and HD webcam media recorder with live SVG audio frequency visualizer, recording status pulse, VU meters, playback preview via TwMediaPlayer, and local file download.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |
| `OnRecordingCompleted` | `EventCallback<int>` | `null` | Fired when user completes a recording with the elapsed seconds. |

#### Razor Usage Example

```razor
<TwMediaRecorder OnRecordingCompleted="(duration) => { /* Handle duration */ }" />
```

---

## Layout & Application Shell

### `TwToolbar`

**Location:** [`Components/UI/TwToolbar.razor`](../Components/UI/TwToolbar.razor)

Command toolbar with a roving tab stop, toggle buttons, separators and an overflow menu for items past MaxVisible.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `IReadOnlyList<ToolbarItem>` | `required` | Button, Toggle or Separator items. |
| `MaxVisible` | `int` | `0` | Items beyond this go into the More menu (0 = all). |
| `OnItemClick` | `EventCallback<ToolbarItem>` | `null` | Item clicked. |
| `Label` | `string` | `"Toolbar"` | Accessible name. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwToolbar Items="_items" MaxVisible="5" OnItemClick="Run" />
```

---

### `TwWindow`

**Location:** [`Components/UI/TwWindow.razor`](../Components/UI/TwWindow.razor)

Floating non-modal window: drag by the title bar or use arrow keys (Shift farther, Ctrl resizes); minimize, maximize, close; last touched stays on top.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `required` | Title and accessible name. |
| `Visible` | `bool` | `true` | Shown or closed. |
| `X` | `int` | `80` | Left in px. |
| `Y` | `int` | `80` | Top in px. |
| `Width` | `int` | `420` | Width in px. |
| `Height` | `int` | `300` | Height in px. |
| `OnMoved` | `EventCallback<(int X,int Y)>` | `null` | Position after a move. |
| `CloseOnEscape` | `bool` | `true` | Escape closes. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwWindow Title="Inspector" @bind-Visible="_show" X="120" Y="90">...</TwWindow>
```

---

### `TwLoadingPanel`

**Location:** [`Components/UI/TwLoadingPanel.razor`](../Components/UI/TwLoadingPanel.razor)

Wraps a region and covers it with a busy overlay; the region is aria-busy and its content is inert while loading.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Visible` | `bool` | `false` | Show the overlay. |
| `Message` | `string` | `"Loading…"` | Announced text. |
| `Blur` | `bool` | `true` | Soften the covered content. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwLoadingPanel Visible="_busy" Message="Saving…">...</TwLoadingPanel>
```

---

### `TwStackLayout`

**Location:** [`Components/UI/TwStackLayout.razor`](../Components/UI/TwStackLayout.razor)

One-dimensional flex layout with named gap, alignment, justification, wrapping and reverse order.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Horizontal` | `bool` | `false` | Row instead of column. |
| `Gap` | `int` | `3` | Step 0-6. |
| `Align` | `string` | `"stretch"` | start, center, end, stretch, baseline. |
| `Justify` | `string` | `"start"` | start, center, end, between, around, evenly. |
| `Wrap` | `bool` | `false` | Wrap items. |
| `Reverse` | `bool` | `false` | Reverse order. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwStackLayout Horizontal="true" Gap="4" Justify="between">...</TwStackLayout>
```

---

### `TwFormLayout`

**Location:** [`Components/UI/TwFormLayout.razor`](../Components/UI/TwFormLayout.razor)

Responsive form grid: one column on phones, up to four from the lg breakpoint. Holds TwFormGroup and TwFormItem.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Columns` | `int` | `2` | 1 to 4. |
| `ChildContent` | `RenderFragment?` | `null` | Groups and items. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwFormLayout Columns="2">...</TwFormLayout>
```

---

### `TwFormGroup`

**Location:** [`Components/UI/TwFormGroup.razor`](../Components/UI/TwFormGroup.razor)

Titled, optionally collapsible section of a form that spans the full layout width.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `required` | Group title. |
| `Collapsible` | `bool` | `false` | Show a toggle. |
| `InitiallyCollapsed` | `bool` | `false` | Start collapsed. |
| `Columns` | `int` | `2` | Inner columns. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwFormGroup Title="Billing" Collapsible="true">...</TwFormGroup>
```

---

### `TwFormItem`

**Location:** [`Components/UI/TwFormItem.razor`](../Components/UI/TwFormItem.razor)

A label, control, hint and error wired together: label points at the field, required is announced, errors use role=alert.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Label` | `string` | `required` | Label text. |
| `FieldId` | `string` | `generated` | Id of the control inside. |
| `Required` | `bool` | `false` | Mark required. |
| `Hint` | `string?` | `null` | Help text. |
| `Error` | `string?` | `null` | Validation message (replaces the hint). |
| `LabelBeside` | `bool` | `false` | Label beside the field on wide screens. |
| `ColSpan` | `int` | `1` | Columns spanned, 1-4. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwFormItem Label="Email" FieldId="email" Required="true" Hint="We never share it"><input id="email" /></TwFormItem>
```

---

### `TwAppShell`

**Location:** [`Components/Layout/TwAppShell.razor`](../Components/Layout/TwAppShell.razor)

The root application shell supporting four dynamic layout variants: Sidebar (mobile drawer + desktop sidebar), TopNav (horizontal bar with mobile sheet), CompactRail (icon rail), and Stacked (single column).

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Layout` | `LayoutVariant` | `LayoutVariant.Sidebar` | Configures component behavior or styling. |
| `BrandContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `HeaderActions` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `SidebarContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `SidebarFooter` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `TopNavContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `RailContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `BottomNavContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |

#### Razor Usage Example

```razor
<TwAppShell Layout="LayoutVariant.Sidebar" />
```

---

### `TwContainer`

**Location:** [`Components/Layout/TwContainer.razor`](../Components/Layout/TwContainer.razor)

Responsive max-width container wrapper with automatic padding and breakpoint clamping (sm, md, lg, xl, 2xl, full).

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `MaxWidth` | `string` | `"7xl"` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwContainer MaxWidth="7xl" />
```

---

### `TwGrid`

**Location:** [`Components/Layout/TwGrid.razor`](../Components/Layout/TwGrid.razor)

Mobile-first responsive grid system with configurable column distributions (1-2-3, 1-2-4, 2-3-4, etc.) and gap scales.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `Preset` | `string` | `"1-2-3"` | Configures component behavior or styling. |
| `Gap` | `string` | `"md"` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwGrid Preset="1-2-3" Gap="md" />
```

---

### `TwSplitter`

**Location:** [`Components/UI/TwSplitter.razor`](../Components/UI/TwSplitter.razor)

Enterprise resizable split layout container supporting horizontal and vertical orientations, draggable divider handle, and one-click collapse/reset buttons.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Orientation` | `SplitOrientation` | `SplitOrientation.Horizontal` | Configures component behavior or styling. |
| `SplitPercentage` | `double` | `40` | Configures component behavior or styling. |
| `MinPercentage` | `double` | `15` | Configures component behavior or styling. |
| `MaxPercentage` | `double` | `85` | Configures component behavior or styling. |
| `MinHeight` | `int` | `280` | Configures component behavior or styling. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `PaneOne` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `PaneTwo` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `SplitPercentageChanged` | `EventCallback<double>` | `null` | Callback invoked when state changes or item is selected. |

#### Razor Usage Example

```razor
<TwSplitter Orientation="SplitOrientation.Horizontal" SplitPercentage="40" MinPercentage="15" />
```

---

### `TwBottomNav`

**Location:** [`Components/Layout/TwBottomNav.razor`](../Components/Layout/TwBottomNav.razor)

Fixed application bottom navigation bar for touch devices with active state indicators, notification badges, and haptic tap styling.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `IReadOnlyList<NavItem>` | `Array.Empty<NavItem>()` | Configures component behavior or styling. |
| `ActiveId` | `string?` | `null` | Configures component behavior or styling. |
| `OnSelect` | `EventCallback<string>` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwBottomNav Items="Array.Empty<NavItem>()" />
```

---

## Media, Scanner & Signature

### `TwMediaPlayer`

**Location:** [`Components/UI/TwMediaPlayer.razor`](../Components/UI/TwMediaPlayer.razor)

Custom video and audio player with chapter markers, scrubber seek, playback speed selector (0.5x - 2x), Picture-in-Picture, volume control, and audio visualizer.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Src` | `string` | `string.Empty` | Configures component behavior or styling. |
| `Poster` | `string?` | `null` | Configures component behavior or styling. |
| `Title` | `string?` | `null` | Header title or accessible label. |
| `Subtitle` | `string?` | `null` | Header title or accessible label. |
| `Type` | `MediaKind` | `MediaKind.Video` | Configures component behavior or styling. |
| `Loop` | `bool` | `null` | Configures component behavior or styling. |
| `Chapters` | `List<MediaChapter>?` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwMediaPlayer Src="string.Empty" Type="MediaKind.Video" />
```

---

### `TwQrCode`

**Location:** [`Components/UI/TwQrCode.razor`](../Components/UI/TwQrCode.razor)

Vector SVG and PNG QR code generator supporting custom dot shapes (Square, Rounded, Circle), error correction levels, and center icon branding.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `string` | `"https://tailwindcss.com"` | Primary data value or two-way bound model property. |
| `Title` | `string?` | `null` | Header title or accessible label. |
| `Subtitle` | `string?` | `null` | Header title or accessible label. |
| `Size` | `int` | `200` | Component sizing scale (Xs, Sm, Md, Lg). |
| `ForegroundColor` | `string` | `"#0f172a"` | Configures component behavior or styling. |
| `BackgroundColor` | `string` | `"#ffffff"` | Configures component behavior or styling. |
| `DotShape` | `QrDotShape` | `QrDotShape.Rounded` | Configures component behavior or styling. |
| `CenterIcon` | `QrCenterIcon` | `QrCenterIcon.None` | Configures component behavior or styling. |
| `EccLevel` | `QrEccLevel` | `QrEccLevel.Q` | Configures component behavior or styling. |
| `ShowActions` | `bool` | `true` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwQrCode Value="https://tailwindcss.com" Size="200" ForegroundColor="#0f172a" />
```

---

### `TwBarcode`

**Location:** [`Components/UI/TwBarcode.razor`](../Components/UI/TwBarcode.razor)

High-density Code 128 1D vector barcode generator rendering sharp scalable SVG bars with human-readable text.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `string` | `"SKU-984210"` | Primary data value or two-way bound model property. |
| `Title` | `string?` | `null` | Header title or accessible label. |
| `Subtitle` | `string?` | `null` | Header title or accessible label. |
| `BarHeight` | `int` | `64` | Configures component behavior or styling. |
| `BarWidthMultiplier` | `int` | `2` | Configures component behavior or styling. |
| `ShowText` | `bool` | `true` | Configures component behavior or styling. |
| `QuietZone` | `bool` | `true` | Configures component behavior or styling. |
| `ForegroundColor` | `string` | `"#0f172a"` | Configures component behavior or styling. |
| `BackgroundColor` | `string` | `"#ffffff"` | Configures component behavior or styling. |
| `ShowActions` | `bool` | `true` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwBarcode Value="SKU-984210" BarHeight="64" BarWidthMultiplier="2" />
```

---

### `TwBarcodeScanner`

**Location:** [`Components/UI/TwBarcodeScanner.razor`](../Components/UI/TwBarcodeScanner.razor)

Live camera video and file barcode scanner supporting QR codes, Code 128, EAN, UPC, and Data Matrix with audio beep feedback.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `OnScan` | `EventCallback<BarcodeScanResult>` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwBarcodeScanner />
```

---

### `TwSignaturePad`

**Location:** [`Components/UI/TwSignaturePad.razor`](../Components/UI/TwSignaturePad.razor)

Digital canvas signature pad with smooth Bezier curve drawing, undo/redo history, background transparency, and export to PNG and SVG.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string?` | `null` | Header title or accessible label. |
| `Subtitle` | `string?` | `null` | Header title or accessible label. |
| `Height` | `int` | `220` | Configures component behavior or styling. |
| `ShowGuide` | `bool` | `true` | Configures component behavior or styling. |
| `PenColor` | `string` | `"#0f172a"` | Configures component behavior or styling. |
| `StrokeWidth` | `double` | `2.5` | Configures component behavior or styling. |
| `OnSave` | `EventCallback<string>` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwSignaturePad Height="220" ShowGuide="true" PenColor="#0f172a" />
```

---

### `TwImageCompare`

**Location:** [`Components/UI/TwImageCompare.razor`](../Components/UI/TwImageCompare.razor)

Interactive Before/After visual comparison slider with touch/pointer dragging, glassmorphic center handle, keyboard navigation, and dual badges.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `BeforeSrc` | `string` | `""` | Configures component behavior or styling. |
| `AfterSrc` | `string` | `""` | Configures component behavior or styling. |
| `BeforeAlt` | `string` | `"Before image"` | Configures component behavior or styling. |
| `AfterAlt` | `string` | `"After image"` | Configures component behavior or styling. |
| `BeforeLabel` | `string` | `"Before"` | Configures component behavior or styling. |
| `AfterLabel` | `string` | `"After"` | Configures component behavior or styling. |
| `Position` | `double` | `50.0` | Configures component behavior or styling. |
| `PositionChanged` | `EventCallback<double>` | `null` | Callback invoked when state changes or item is selected. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwImageCompare BeforeSrc="" AfterSrc="" BeforeAlt="Before image" />
```

---

## Modals & Overlays

### `TwCelebrationModal`

**Location:** [`Components/UI/TwCelebrationModal.razor`](../Components/UI/TwCelebrationModal.razor)

Modal dialog with celebration confetti burst animation, congratulatory header, and reward claim buttons.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `IsOpen` | `bool` | `false` | Whether the modal is visible. |

#### Razor Usage Example

```razor
<TwCelebrationModal @bind-IsOpen="isOpen" Title="Congratulations!" />
```

---

## Navigation & Headers

### `TwNavDrawer`

**Location:** [`Components/UI/TwNavDrawer.razor`](../Components/UI/TwNavDrawer.razor)

Enterprise side navigation drawer featuring Mini icon rail (64px) vs Expanded full sidebar (260px) transitions, item badges, and overlay mode.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `BrandTitle` | `string` | `"Enterprise Hub"` | Drawer header brand text. |
| `IsMini` | `bool` | `false` | Mini collapsed icon-only rail mode. |
| `IsOpen` | `bool` | `true` | Drawer open state. |
| `IsOverlay` | `bool` | `false` | Modal overlay mode with backdrop. |
| `AllowToggleMini` | `bool` | `true` | Show collapse/expand toggle button. |

#### Razor Usage Example

```razor
<TwNavDrawer BrandTitle="Enterprise"><TwNavDrawerItem Text="Dashboard" Icon="📊" /></TwNavDrawer>
```

---

## Navigation & Shell

### `TwPwaInstallButton`

**Location:** [`Components/UI/TwPwaInstallButton.razor`](../Components/UI/TwPwaInstallButton.razor)

1-Click Progressive Web App (PWA) installation button with native beforeinstallprompt detection, standalone status badges, and cross-platform installation modal.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Class` | `string?` | `null` | Optional CSS classes. |

#### Razor Usage Example

```razor
<TwPwaInstallButton />
```

---

### `TwNavDrawerItem`

**Location:** [`Components/UI/TwNavDrawerItem.razor`](../Components/UI/TwNavDrawerItem.razor)

Navigation drawer menu item with icon, label, active state indicator, and optional counter badge.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Label` | `string` | `""` | Menu item label text. |
| `Icon` | `string` | `""` | Menu item icon. |
| `IsActive` | `bool` | `false` | Whether the item is currently active. |

#### Razor Usage Example

```razor
<TwNavDrawerItem Icon="📊" Label="Analytics" IsActive="true" />
```

---

## Navigation & Steppers

### `TwRibbon`

**Location:** [`Components/UI/TwRibbon.razor`](../Components/UI/TwRibbon.razor)

Award-winning enterprise Ribbon navigation bar featuring tabbed strips, Quick Access Toolbar (QAT), grouped action clusters, large/small buttons, and collapsible drawer.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Tabs` | `List<RibbonTab>` | `new()` | Configures component behavior or styling. |
| `ActiveTabIndex` | `int` | `0` | Configures component behavior or styling. |
| `IsCollapsed` | `bool` | `false` | Configures component behavior or styling. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `OnActionClicked` | `EventCallback<string>` | `null` | Callback invoked when state changes or item is selected. |
| `ActiveTabIndexChanged` | `EventCallback<int>` | `null` | Callback invoked when state changes or item is selected. |

#### Razor Usage Example

```razor
<TwRibbon Tabs="new()" ActiveTabIndex="0" CustomClass="" />
```

---

### `TwFloatingDock`

**Location:** [`Components/UI/TwFloatingDock.razor`](../Components/UI/TwFloatingDock.razor)

Modern floating glassmorphism dock navigation bar with hover magnification scale, active pill indicators, notification badges, and smooth tooltips.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `List<DockItem>` | `new()` | Configures component behavior or styling. |
| `ActiveId` | `string` | `"home"` | Configures component behavior or styling. |
| `Position` | `string` | `"bottom"` | Configures component behavior or styling. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `OnItemSelected` | `EventCallback<string>` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwFloatingDock Items="new()" ActiveId="home" Position="bottom" />
```

---

### `TwTabs`

**Location:** [`Components/UI/TwTabs.razor`](../Components/UI/TwTabs.razor)

Horizontal scrollable tabs with pill or underline variants and badge counters.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Tabs` | `IReadOnlyList<TabDefinition>` | `Array.Empty<TabDefinition>()` | Configures component behavior or styling. |
| `ActiveTabKey` | `string?` | `null` | Configures component behavior or styling. |
| `ActiveTabKeyChanged` | `EventCallback<string>` | `null` | Callback invoked when state changes or item is selected. |
| `Variant` | `string` | `"underline"` | Semantic visual variant (e.g. Primary, Secondary, Outline). |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwTabs Tabs="Array.Empty<TabDefinition>()" Variant="underline" />
```

---

### `TwSlidingTabs`

**Location:** [`Components/UI/TwSlidingTabs.razor`](../Components/UI/TwSlidingTabs.razor)

Fluid tab strip with an animated gliding background pill that follows the active selection.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Tabs` | `List<TabItem>` | `new()` | Configures component behavior or styling. |
| `ActiveTabId` | `string` | `string.Empty` | Configures component behavior or styling. |
| `ActiveTabIdChanged` | `EventCallback<string>` | `null` | Callback invoked when state changes or item is selected. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwSlidingTabs Tabs="new()" ActiveTabId="string.Empty" />
```

---

### `TwBreadcrumb`

**Location:** [`Components/UI/TwBreadcrumb.razor`](../Components/UI/TwBreadcrumb.razor)

Accessible breadcrumb trail with custom separators and truncated intermediate paths.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `IReadOnlyList<BreadcrumbItem>` | `Array.Empty<BreadcrumbItem>()` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwBreadcrumb Items="Array.Empty<BreadcrumbItem>()" />
```

---

### `TwStepper`

**Location:** [`Components/UI/TwStepper.razor`](../Components/UI/TwStepper.razor)

Multi-step onboarding wizard with completed step indicators and compact mobile layout.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Steps` | `IReadOnlyList<StepItem>` | `Array.Empty<StepItem>()` | Configures component behavior or styling. |
| `CurrentStep` | `int` | `1` | Configures component behavior or styling. |
| `CurrentStepChanged` | `EventCallback<int>` | `null` | Callback invoked when state changes or item is selected. |
| `Clickable` | `bool` | `true` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwStepper Steps="Array.Empty<StepItem>()" CurrentStep="1" Clickable="true" />
```

---

### `TwTimeline`

**Location:** [`Components/UI/TwTimeline.razor`](../Components/UI/TwTimeline.razor)

Vertical activity timeline container for audit logs, delivery tracking, and project milestones.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Position` | `TimelinePosition` | `TimelinePosition.Start` | Configures component behavior or styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwTimeline Position="TimelinePosition.Start" />
```

---

### `TwTimelineItem`

**Location:** [`Components/UI/TwTimelineItem.razor`](../Components/UI/TwTimelineItem.razor)

Individual timeline entry with status dot, icon, timestamp, and card description.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string?` | `null` | Header title or accessible label. |
| `Subtitle` | `string?` | `null` | Header title or accessible label. |
| `Timestamp` | `string?` | `null` | Configures component behavior or styling. |
| `Variant` | `SemanticVariant` | `SemanticVariant.Primary` | Semantic visual variant (e.g. Primary, Secondary, Outline). |
| `Pulse` | `bool` | `null` | Configures component behavior or styling. |
| `IsLast` | `bool` | `null` | Configures component behavior or styling. |
| `DotContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwTimelineItem Variant="SemanticVariant.Primary" />
```

---

### `TwScrollToTop`

**Location:** [`Components/UI/TwScrollToTop.razor`](../Components/UI/TwScrollToTop.razor)

Floating smooth scroll-to-top button with scroll position threshold and mobile safe-area clearance.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Variant` | `ScrollToTopVariant` | `ScrollToTopVariant.Vertical` | Semantic visual variant (e.g. Primary, Secondary, Outline). |
| `Label` | `string` | `"TOP"` | Configures component behavior or styling. |
| `ShowProgress` | `bool` | `true` | Configures component behavior or styling. |
| `Threshold` | `int` | `150` | Configures component behavior or styling. |
| `AlwaysVisible` | `bool` | `null` | Configures component behavior or styling. |
| `PositionClass` | `string` | `"bottom-36 sm:bottom-24 right-4 sm:right-6"` | Configures component behavior or styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwScrollToTop Variant="ScrollToTopVariant.Vertical" Label="TOP" ShowProgress="true" />
```

---

### `TwSpeedDial`

**Location:** [`Components/UI/TwSpeedDial.razor`](../Components/UI/TwSpeedDial.razor)

Floating action button that expands into a radial or vertical speed-dial menu of quick actions.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Actions` | `IReadOnlyList<SpeedDialAction>` | `Array.Empty<SpeedDialAction>()` | Configures component behavior or styling. |
| `OnActionSelected` | `EventCallback<string>` | `null` | Configures component behavior or styling. |
| `PositionClass` | `string` | `"bottom-20 sm:bottom-8 left-4 sm:left-8"` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwSpeedDial Actions="Array.Empty<SpeedDialAction>()" PositionClass="bottom-20 sm:bottom-8 left-4 sm:left-8" />
```

---

### `TwTour`

**Location:** [`Components/UI/TwTour.razor`](../Components/UI/TwTour.razor)

Interactive guided walkthrough coaching tour with spotlight backdrop, step popovers, and automatic restart lifecycle.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `IsOpen` | `bool` | `null` | Two-way bindable visibility flag. |
| `IsOpenChanged` | `EventCallback<bool>` | `null` | Two-way bindable visibility flag. |
| `Steps` | `IReadOnlyList<TourStep>` | `Array.Empty<TourStep>()` | Configures component behavior or styling. |
| `OnCompleted` | `EventCallback` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwTour Steps="Array.Empty<TourStep>()" />
```

---

## Other Components

### `TwAguiCanvas`

**Location:** [`Components/UI/TwAguiCanvas.razor`](../Components/UI/TwAguiCanvas.razor)

Reusable Blazor WebAssembly component styled with Tailwind CSS.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Schema` | `AguiWidgetSchema?` | `null` | Configures component behavior or styling. |
| `OnActionTriggered` | `EventCallback<AguiActionItem>` | `null` | Configures component behavior or styling. |
| `Class` | `string` | `""` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwAguiCanvas Class="" />
```

---

### `TwAguiStudio`

**Location:** [`Components/UI/TwAguiStudio.razor`](../Components/UI/TwAguiStudio.razor)

Reusable Blazor WebAssembly component styled with Tailwind CSS.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Class` | `string` | `""` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwAguiStudio Class="" />
```

---

### `TwAvatar`

**Location:** [`Components/UI/TwAvatar.razor`](../Components/UI/TwAvatar.razor)

Reusable Blazor WebAssembly component styled with Tailwind CSS.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ImageUrl` | `string?` | `null` | Configures component behavior or styling. |
| `Initials` | `string` | `"UI"` | Configures component behavior or styling. |
| `Alt` | `string` | `"Avatar"` | Configures component behavior or styling. |
| `Size` | `ComponentSize` | `ComponentSize.Md` | Configures component behavior or styling. |
| `StatusColor` | `string?` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwAvatar Initials="UI" Alt="Avatar" Size="ComponentSize.Md" />
```

---

### `TwAvatarGroup`

**Location:** [`Components/UI/TwAvatarGroup.razor`](../Components/UI/TwAvatarGroup.razor)

Reusable Blazor WebAssembly component styled with Tailwind CSS.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Avatars` | `List<AvatarItem>` | `new()` | Configures component behavior or styling. |
| `Max` | `int` | `4` | Configures component behavior or styling. |
| `Size` | `ComponentSize` | `ComponentSize.Md` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwAvatarGroup Avatars="new()" Max="4" Size="ComponentSize.Md" />
```

---

### `TwCarousel`

**Location:** [`Components/UI/TwCarousel.razor`](../Components/UI/TwCarousel.razor)

Reusable Blazor WebAssembly component styled with Tailwind CSS.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ChildContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `ActiveIndex` | `int` | `null` | Configures component behavior or styling. |
| `ActiveIndexChanged` | `EventCallback<int>` | `null` | Configures component behavior or styling. |
| `AutoCycle` | `bool` | `true` | Configures component behavior or styling. |
| `IntervalSeconds` | `int` | `4` | Configures component behavior or styling. |
| `ShowArrows` | `bool` | `true` | Configures component behavior or styling. |
| `ShowIndicators` | `bool` | `true` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwCarousel AutoCycle="true" IntervalSeconds="4" ShowArrows="true" />
```

---

### `TwCarouselSlide`

**Location:** [`Components/UI/TwCarouselSlide.razor`](../Components/UI/TwCarouselSlide.razor)

Reusable Blazor WebAssembly component styled with Tailwind CSS.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ChildContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwCarouselSlide>
    <span>Content</span>
</TwCarouselSlide>
```

---

### `TwCommandPalette`

**Location:** [`Components/UI/TwCommandPalette.razor`](../Components/UI/TwCommandPalette.razor)

Reusable Blazor WebAssembly component styled with Tailwind CSS.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `IsOpen` | `bool` | `null` | Configures component behavior or styling. |
| `IsOpenChanged` | `EventCallback<bool>` | `null` | Configures component behavior or styling. |
| `Items` | `List<CommandPaletteItem>` | `new()` | Configures component behavior or styling. |
| `OnItemSelected` | `EventCallback<CommandPaletteItem>` | `null` | Configures component behavior or styling. |
| `Placeholder` | `string` | `"Type a command, component name, or search..."` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwCommandPalette Items="new()" Placeholder="Type a command, component name, or search..." />
```

---

### `TwCompanyPartyForm`

**Location:** [`Components/UI/TwCompanyPartyForm.razor`](../Components/UI/TwCompanyPartyForm.razor)

Reusable Blazor WebAssembly component styled with Tailwind CSS.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Model` | `PartyModel` | `new()` | Configures component behavior or styling. |
| `Rsvp` | `GuestRsvpModel` | `new()` | Configures component behavior or styling. |
| `OnPartySaved` | `EventCallback<PartyModel>` | `null` | Configures component behavior or styling. |
| `OnRsvpSubmitted` | `EventCallback<GuestRsvpModel>` | `null` | Configures component behavior or styling. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwCompanyPartyForm Model="new()" Rsvp="new()" CustomClass="" />
```

---

### `TwCompanyPlanForm`

**Location:** [`Components/UI/TwCompanyPlanForm.razor`](../Components/UI/TwCompanyPlanForm.razor)

Reusable Blazor WebAssembly component styled with Tailwind CSS.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Model` | `CompanyPlanModel` | `new()` | Configures component behavior or styling. |
| `OnPlanSubmitted` | `EventCallback<CompanyPlanModel>` | `null` | Configures component behavior or styling. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwCompanyPlanForm Model="new()" CustomClass="" />
```

---

### `TwDocsExplorer`

**Location:** [`Components/UI/TwDocsExplorer.razor`](../Components/UI/TwDocsExplorer.razor)

Reusable Blazor WebAssembly component styled with Tailwind CSS.

#### Parameters & Settings

This component has no parameters.

#### Razor Usage Example

```razor
<TwDocsExplorer />
```

---

### `TwEmployeeAwardForm`

**Location:** [`Components/UI/TwEmployeeAwardForm.razor`](../Components/UI/TwEmployeeAwardForm.razor)

Reusable Blazor WebAssembly component styled with Tailwind CSS.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Model` | `EmployeeAwardModel` | `new()` | Configures component behavior or styling. |
| `OnAwardNominated` | `EventCallback<EmployeeAwardModel>` | `null` | Configures component behavior or styling. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwEmployeeAwardForm Model="new()" CustomClass="" />
```

---

### `TwFormEngine`

**Location:** [`Components/UI/TwFormEngine.razor`](../Components/UI/TwFormEngine.razor)

Reusable Blazor WebAssembly component styled with Tailwind CSS.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Schema` | `FormSchema?` | `null` | Configures component behavior or styling. |
| `OnSubmit` | `EventCallback<FormSubmissionResult>` | `null` | Configures component behavior or styling. |
| `OnChange` | `EventCallback<Dictionary<string, object?>>` | `null` | Configures component behavior or styling. |
| `ShowHeader` | `bool` | `true` | Configures component behavior or styling. |
| `ShowCardWrapper` | `bool` | `true` | Configures component behavior or styling. |
| `ReadOnly` | `bool` | `false` | Configures component behavior or styling. |
| `Class` | `string` | `""` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwFormEngine ShowHeader="true" ShowCardWrapper="true" Class="" />
```

---

### `TwFormStudio`

**Location:** [`Components/UI/TwFormStudio.razor`](../Components/UI/TwFormStudio.razor)

Reusable Blazor WebAssembly component styled with Tailwind CSS.

#### Parameters & Settings

This component has no parameters.

#### Razor Usage Example

```razor
<TwFormStudio />
```

---

### `TwProfitLossForm`

**Location:** [`Components/UI/TwProfitLossForm.razor`](../Components/UI/TwProfitLossForm.razor)

Reusable Blazor WebAssembly component styled with Tailwind CSS.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Model` | `ProfitLossModel` | `new()` | Configures component behavior or styling. |
| `OnStatementSaved` | `EventCallback<ProfitLossModel>` | `null` | Configures component behavior or styling. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwProfitLossForm Model="new()" CustomClass="" />
```

---

### `TwSmartFilter`

**Location:** [`Components/UI/TwSmartFilter.razor`](../Components/UI/TwSmartFilter.razor)

Reusable Blazor WebAssembly component styled with Tailwind CSS.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `OnFilterChanged` | `EventCallback<Dictionary<string, string>>` | `null` | Configures component behavior or styling. |
| `Class` | `string` | `""` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwSmartFilter Class="" />
```

---

### `TwSmartPasteButton`

**Location:** [`Components/UI/TwSmartPasteButton.razor`](../Components/UI/TwSmartPasteButton.razor)

Reusable Blazor WebAssembly component styled with Tailwind CSS.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `OnApplied` | `EventCallback<Dictionary<string, string>>` | `null` | Configures component behavior or styling. |
| `Class` | `string` | `""` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwSmartPasteButton Class="" />
```

---

### `TwSmartSearch`

**Location:** [`Components/UI/TwSmartSearch.razor`](../Components/UI/TwSmartSearch.razor)

Reusable Blazor WebAssembly component styled with Tailwind CSS.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `SearchQuery` | `string` | `""` | Configures component behavior or styling. |
| `SearchQueryChanged` | `EventCallback<string>` | `null` | Configures component behavior or styling. |
| `OnSearchSubmitted` | `EventCallback<string>` | `null` | Configures component behavior or styling. |
| `Placeholder` | `string` | `"Semantic natural language search (e.g. 'high salary engineers in cloud')..."` | Configures component behavior or styling. |
| `Class` | `string` | `""` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwSmartSearch SearchQuery="" Placeholder="Semantic natural language search (e.g. 'high salary engineers in cloud')..." Class="" />
```

---

### `TwSmartTextArea`

**Location:** [`Components/UI/TwSmartTextArea.razor`](../Components/UI/TwSmartTextArea.razor)

Reusable Blazor WebAssembly component styled with Tailwind CSS.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Label` | `string?` | `null` | Configures component behavior or styling. |
| `Value` | `string` | `"We are thrilled to announce our next-generation WebAssembly architecture which reduces client latency by over 80 percent across all devices."` | Configures component behavior or styling. |
| `ValueChanged` | `EventCallback<string>` | `null` | Configures component behavior or styling. |
| `Placeholder` | `string` | `"Start typing and watch smart completions appear..."` | Configures component behavior or styling. |
| `Rows` | `int` | `4` | Configures component behavior or styling. |
| `Class` | `string` | `""` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwSmartTextArea Value="We are thrilled to announce our next-generation WebAssembly architecture which reduces client latency by over 80 percent across all devices." Placeholder="Start typing and watch smart completions appear..." Rows="4" />
```

---

## Overlays & Modals

### `TwPopover`

**Location:** [`Components/UI/TwPopover.razor`](../Components/UI/TwPopover.razor)

Anchored contextual floating popover with arrow pointer, 8-way placement, click or hover triggers, keyboard Escape dismiss, and optional header/footer slots.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string?` | `null` | Optional header title text. |
| `Placement` | `TwPopoverPlacement` | `TwPopoverPlacement.Bottom` | Placement relative to anchor (Top, Bottom, Left, Right, etc.). |
| `Trigger` | `TwPopoverTrigger` | `TwPopoverTrigger.Click` | Activation trigger mode (Click, Hover, Manual). |
| `ShowArrow` | `bool` | `true` | Render pointer arrow pointing to anchor. |
| `Width` | `string` | `"w-72"` | Flyout container width utility. |
| `IsOpen` | `bool` | `false` | Two-way bindable open state. |

#### Razor Usage Example

```razor
<TwPopover Title="Filter Settings" Placement="TwPopoverPlacement.Bottom"><AnchorContent><button class="btn">Filter</button></AnchorContent><ChildContent><p>Options...</p></ChildContent></TwPopover>
```

---

### `TwWordBalloonOverlay`

**Location:** [`Components/UI/TwWordBalloonOverlay.razor`](../Components/UI/TwWordBalloonOverlay.razor)

Anchored contextual floating Word Balloon and speech bubble overlay with directional pointer tail, 11 shapes, 9 themes, 9 effects, and hover/click/always-visible trigger modes for avatars, metric cards, and action buttons.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ChildContent` | `RenderFragment?` | `null` | Anchor target content. |
| `Text` | `string` | `"KABOOM!"` | Primary center word or phrase. |
| `Kicker` | `string?` | `null` | Optional top kicker badge or sub-caption. |
| `Shape` | `WordBalloonShape` | `WordBalloonShape.SpeechBubble` | Backdrop balloon shape geometry. |
| `Effect` | `WordBalloonEffect` | `WordBalloonEffect.Outlined` | Typography text effect. |
| `Theme` | `WordBalloonTheme` | `WordBalloonTheme.ComicYellow` | Color palette gradient theme. |
| `Placement` | `TwPopoverPlacement` | `TwPopoverPlacement.Top` | Placement relative to anchor element. |
| `Trigger` | `TwPopoverTrigger` | `TwPopoverTrigger.Hover` | Trigger activation mode (Hover, Click, Manual). |
| `IsOpen` | `bool` | `true` | Two-way bindable visibility state. |
| `Persistent` | `bool` | `false` | Keep balloon permanently visible and prevent mouseleave dismissal. |
| `Interactive` | `bool` | `true` | Enable pointer interaction and hover bridge over balloon. |
| `CloseDelayMs` | `int` | `250` | Hover grace delay in milliseconds before closing. |
| `AutoResizeBalloon` | `bool` | `true` | Auto-resize backdrop shape to fit text with safety margins. |
| `BalloonScale` | `double` | `1.30` | Backdrop balloon scale multiplier (0.7x to 2.8x). |
| `PaddingX` | `double` | `36.0` | Horizontal internal padding. |
| `PaddingY` | `double` | `18.0` | Vertical internal padding. |
| `CustomWidth` | `double` | `0.0` | Manual fixed width override when AutoResizeBalloon is false. |
| `CustomHeight` | `double` | `0.0` | Manual fixed height override when AutoResizeBalloon is false. |

#### Razor Usage Example

```razor
<TwWordBalloonOverlay Text="KABOOM!" Shape="WordBalloonShape.SpeechBubble" Theme="WordBalloonTheme.ComicYellow" Placement="TwPopoverPlacement.Top"><button class="btn">Target</button></TwWordBalloonOverlay>
```

---

## Overlays & Popovers

### `TwContextMenu`

**Location:** [`Components/UI/TwContextMenu.razor`](../Components/UI/TwContextMenu.razor)

Desktop-grade right-click context menu and anchored flyout callout featuring nested submenus, keyboard shortcut badges, icons, and click-outside dismissal.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `List<ContextMenuItem>` | `new()` | Configures component behavior or styling. |
| `PreventDefaultContextMenu` | `bool` | `true` | Configures component behavior or styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `OnItemSelected` | `EventCallback<string>` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwContextMenu Items="new()" PreventDefaultContextMenu="true" CustomClass="" />
```

---

### `TwModal`

**Location:** [`Components/UI/TwModal.razor`](../Components/UI/TwModal.razor)

Accessible dialog that presents as a bottom sheet on mobile screens and a centered modal dialog on desktop.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `IsOpen` | `bool` | `null` | Two-way bindable visibility flag. |
| `IsOpenChanged` | `EventCallback<bool>` | `null` | Two-way bindable visibility flag. |
| `Title` | `string?` | `null` | Header title or accessible label. |
| `Description` | `string?` | `null` | Secondary descriptive caption. |
| `MaxWidth` | `string` | `"md"` | Configures component behavior or styling. |
| `CloseOnBackdropClick` | `bool` | `true` | Configures component behavior or styling. |
| `HeaderContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `FooterContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwModal MaxWidth="md" CloseOnBackdropClick="true" />
```

---

### `TwDrawer`

**Location:** [`Components/UI/TwDrawer.razor`](../Components/UI/TwDrawer.razor)

Off-canvas sliding drawer supporting Left, Right, and Bottom slide orientations with backdrop blur.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `IsOpen` | `bool` | `null` | Two-way bindable visibility flag. |
| `IsOpenChanged` | `EventCallback<bool>` | `null` | Two-way bindable visibility flag. |
| `Title` | `string?` | `null` | Header title or accessible label. |
| `Subtitle` | `string?` | `null` | Header title or accessible label. |
| `Placement` | `DrawerPlacement` | `DrawerPlacement.Right` | Configures component behavior or styling. |
| `Width` | `string` | `"max-w-md"` | Configures component behavior or styling. |
| `CloseOnBackdropClick` | `bool` | `true` | Configures component behavior or styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `FooterContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwDrawer Placement="DrawerPlacement.Right" Width="max-w-md" CloseOnBackdropClick="true" />
```

---

### `TwDropdown`

**Location:** [`Components/UI/TwDropdown.razor`](../Components/UI/TwDropdown.razor)

Click-outside popover menu with keyboard navigation and position alignment (Left, Right, Center).

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `TriggerContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `Alignment` | `string` | `"right"` | Configures component behavior or styling. |
| `CloseOnSelect` | `bool` | `true` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwDropdown Alignment="right" CloseOnSelect="true" />
```

---

### `TwTooltip`

**Location:** [`Components/UI/TwTooltip.razor`](../Components/UI/TwTooltip.razor)

Hover and tap tooltip popover with auto-flipping placement and arrow indicator.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Text` | `string` | `""` | Configures component behavior or styling. |
| `Placement` | `string` | `"top"` | Configures component behavior or styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwTooltip Text="" Placement="top" />
```

---

### `TwAccordion`

**Location:** [`Components/UI/TwAccordion.razor`](../Components/UI/TwAccordion.razor)

Collapsible accordion container with single-expand or multi-expand modes.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `AllowMultiple` | `bool` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwAccordion>
    <span>Content</span>
</TwAccordion>
```

---

### `TwAccordionItem`

**Location:** [`Components/UI/TwAccordionItem.razor`](../Components/UI/TwAccordionItem.razor)

Individual accordion panel with smooth height transition and animated chevron.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `""` | Header title or accessible label. |
| `InitiallyOpen` | `bool` | `null` | Configures component behavior or styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwAccordionItem Title="" />
```

---

## Page Templates & UI Building Blocks

### `TwCompactStatCard`

**Location:** [`Components/Templates/Blocks/TwCompactStatCard.razor`](../Components/Templates/Blocks/TwCompactStatCard.razor)

Minimalist enterprise KPI metric card with uppercase title, large numeric value, percentage delta badge (+18.4%), and pure native mathematical SVG sparklines.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `string.Empty` | Stat card headline label. |
| `Value` | `string` | `string.Empty` | Stat value formatted for display. |
| `Caption` | `string?` | `null` | Optional descriptive subtitle. |
| `Delta` | `string?` | `null` | Percentage or numerical delta text. |
| `IsPositive` | `bool` | `true` | Whether the delta is positive (green) or negative (red). |
| `SparklineData` | `List<double>?` | `null` | List of data points to render as an inline SVG sparkline. |
| `SparklineColor` | `string` | `#10b981` | Stroke color for the SVG sparkline polyline. |

#### Razor Usage Example

```razor
<TwCompactStatCard Title="Gross Volume" Value="$14.8M" Delta="+18.4%" SparklineData="@_inflowTrend" />
```

---

### `TwLandingHero`

**Location:** [`Components/Templates/Blocks/TwLandingHero.razor`](../Components/Templates/Blocks/TwLandingHero.razor)

Kinetic split marketing hero with headline, animated announcement pill, action buttons, social proof avatar stack, and floating preview card with realistic shadow.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Headline` | `string` | `string.Empty` | Main hero headline text. |
| `BadgeText` | `string?` | `null` | Pill announcement text at top of hero. |
| `Description` | `string?` | `null` | Hero supporting copy. |
| `ActionSlot` | `RenderFragment?` | `null` | Slot for primary and secondary action buttons. |
| `SocialProofSlot` | `RenderFragment?` | `null` | Slot for avatar ratings or metrics. |
| `PreviewSlot` | `RenderFragment?` | `null` | Elevated preview card or screenshot slot. |

#### Razor Usage Example

```razor
<TwLandingHero Headline="One Platform. Infinite Enterprise Velocity." BadgeText="v1.0 Live">
    <ActionSlot><button class="btn-primary">Get Started</button></ActionSlot>
</TwLandingHero>
```

---

### `TwPricingSection`

**Location:** [`Components/Templates/Blocks/TwPricingSection.razor`](../Components/Templates/Blocks/TwPricingSection.razor)

3-tier interactive pricing matrix with monthly/annual billing switcher (20% discount calculation), highlighted popular plan, and feature checklists.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `Predictable Plans for Every Stage` | Pricing section main heading. |
| `Subtitle` | `string?` | `null` | Supporting description text. |
| `BadgeText` | `string` | `Simple Pricing` | Category eyebrow label. |
| `Class` | `string?` | `null` | Custom CSS classes for outer padding or layout. |
| `Plans` | `List<PricingPlan>` | `Default 3 plans` | Collection of pricing tier plans. |
| `OnSelectPlan` | `EventCallback<PricingPlan>` | `null` | Callback triggered when a user selects a plan. |

#### Razor Usage Example

```razor
<TwPricingSection Title="Simple, Transparent Pricing" OnSelectPlan="HandlePlanSelection" />
```

---

### `TwPromptComposer`

**Location:** [`Components/Templates/Blocks/TwPromptComposer.razor`](../Components/Templates/Blocks/TwPromptComposer.razor)

Auto-expanding AI prompt box with suggestion chips, file attachment trigger, voice dictation toggle, and dual-plane execution hooks.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Placeholder` | `string` | `Ask an enterprise agent...` | Input placeholder text. |
| `Rows` | `int` | `3` | Initial textarea row height. |
| `IsLoading` | `bool` | `false` | Shows spinning loading state when processing. |
| `SuggestedPrompts` | `List<string>?` | `null` | Clickable starter prompt pills. |
| `OnSend` | `EventCallback<string>` | `null` | Event callback invoked when prompt is submitted. |

#### Razor Usage Example

```razor
<TwPromptComposer Placeholder="Ask an enterprise agent..." SuggestedPrompts="@_prompts" OnSend="HandleSend" />
```

---

### `TwActivityTimeline`

**Location:** [`Components/Templates/Blocks/TwActivityTimeline.razor`](../Components/Templates/Blocks/TwActivityTimeline.razor)

Vertical audit activity stream with status dots (Info, Success, Warning, Danger), timestamps, descriptions, and metadata badges.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `Audit & Event Activity` | Timeline card title. |
| `Subtitle` | `string?` | `null` | Supporting subtitle text. |
| `Events` | `List<TimelineEvent>` | `empty` | List of timeline events to display. |

#### Razor Usage Example

```razor
<TwActivityTimeline Title="Live Audit Trail" Events="@_events" />
```

---

### `FintechAnalyticsDashboard`

**Location:** [`Components/Templates/Pages/FintechAnalyticsDashboard.razor`](../Components/Templates/Pages/FintechAnalyticsDashboard.razor)

Turnkey Fintech and Payment Treasury Cockpit featuring real-time settlement KPIs, multi-series cash flow line chart, treasury liquidity donut, and clearance ledger table.

#### Parameters & Settings

This component has no parameters.

#### Razor Usage Example

```razor
<FintechAnalyticsDashboard />
```

---

### `AiOperationsTokenMonitor`

**Location:** [`Components/Templates/Pages/AiOperationsTokenMonitor.razor`](../Components/Templates/Pages/AiOperationsTokenMonitor.razor)

Turnkey AI Operations & Control Plane with real-time token throughput KPIs, latency percentiles, human-in-the-loop approval gates, prompt composer, and live execution trail.

#### Parameters & Settings

This component has no parameters.

#### Razor Usage Example

```razor
<AiOperationsTokenMonitor />
```

---

### `SaaSProductLanding`

**Location:** [`Components/Templates/Pages/SaaSProductLanding.razor`](../Components/Templates/Pages/SaaSProductLanding.razor)

Turnkey SaaS product marketing page assembling kinetic hero, bento feature matrix, 3-tier pricing matrix, and customer testimonials.

#### Parameters & Settings

This component has no parameters.

#### Razor Usage Example

```razor
<SaaSProductLanding />
```

---

### `TemplatesGallery`

**Location:** [`Components/Templates/TemplatesGallery.razor`](../Components/Templates/TemplatesGallery.razor)

Interactive preview gallery with live viewport frames (Desktop, Tablet, Mobile) for browsing and testing Page Templates and UI Building Blocks.

#### Parameters & Settings

This component has no parameters.

#### Razor Usage Example

```razor
<TemplatesGallery />
```

---

## Progress & Steppers

### `TwModernProgress`

**Location:** [`Components/UI/TwModernProgress.razor`](../Components/UI/TwModernProgress.razor)

Cool modern progress indicators: SVG glowing circular rings, dashboard speedometer arcs, multi-segment capsule pill steppers, neon liquid shimmer, and Apple Fitness style triple concentric activity rings.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Type` | `ProgressType` | `ProgressType.RadialRing` | Visual variant: RadialRing, SpeedometerArc, SegmentedCapsule, NeonLiquidGlow, ConcentricMultiRing. |
| `Value` | `int` | `75` | Primary progress percentage (0-100). |
| `SecondaryValue` | `int` | `60` | Secondary ring percentage for concentric mode. |
| `TertiaryValue` | `int` | `92` | Tertiary ring percentage for concentric mode. |
| `Label` | `string?` | `null` | Text description or phase label. |
| `PixelSize` | `int` | `110` | Diameter size in pixels for ring/arc modes. |

#### Razor Usage Example

```razor
<TwModernProgress Type="TwModernProgress.ProgressType.RadialRing" Value="75" Label="Done" />
```

---

## Scheduling & Planning

### `TwScheduler`

**Location:** [`Components/UI/TwScheduler.razor`](../Components/UI/TwScheduler.razor)

Day, Week and Month scheduler with overlap-aware columns, click-to-create slots, drag-and-drop moves and an Alt+arrow keyboard alternative.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Appointments` | `IReadOnlyList<Appointment>` | `[]` | Id, Title, Start, End, Color. |
| `Date` | `DateOnly` | `today` | Focused date. |
| `View` | `SchedulerView` | `Week` | Day, Week or Month. |
| `StartHour` | `int` | `8` | First visible hour. |
| `EndHour` | `int` | `18` | Last visible hour. |
| `SlotMinutes` | `int` | `30` | Slot length. |
| `FirstDayOfWeek` | `DayOfWeek` | `Monday` | First column. |
| `OnSlotClick` | `EventCallback<DateTime>` | `null` | An empty slot was clicked. |
| `OnAppointmentClick` | `EventCallback<Appointment>` | `null` | An appointment was clicked. |
| `OnAppointmentMoved` | `EventCallback<Appointment>` | `null` | After a drag or keyboard move; the appointment carries its new times. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwScheduler Appointments="_appts" @bind-Date="_date" @bind-View="_view" OnSlotClick="Create" OnAppointmentMoved="Save" />
```

---

### `TwGantt`

**Location:** [`Components/UI/TwGantt.razor`](../Components/UI/TwGantt.razor)

Project timeline with a task table, day-scaled bars, progress fill, finish-to-start dependency arrows, a today line and conflict detection.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Tasks` | `IReadOnlyList<GanttTask>` | `required` | Id, Name, Start, End, Progress (0-1), DependsOn. |
| `DayWidth` | `int` | `28` | Pixels per day. |
| `SelectedId` | `string?` | `null` | Selected task. |
| `OnTaskClick` | `EventCallback<GanttTask>` | `null` | A bar was clicked. |
| `Label` | `string` | `"Project timeline"` | Region name. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwGantt Tasks="_tasks" DayWidth="24" OnTaskClick="Select" />
```

---

## Security & Compliance

### `TwMfaAuthSuite`

**Location:** [`Components/UI/TwMfaAuthSuite.razor`](../Components/UI/TwMfaAuthSuite.razor)

Enterprise-grade multi-factor authentication suite supporting TOTP authenticator app QR setup, SMS/Email 6-digit OTP with auto-advancing segmented inputs, FIDO2 / WebAuthn biometric passkeys, and single-use emergency recovery codes.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |
| `OnEnrollmentChanged` | `EventCallback<bool>` | `null` | Callback fired when user enrolls or updates 2FA status. |

#### Razor Usage Example

```razor
<TwMfaAuthSuite OnEnrollmentChanged="(isEnrolled) => { /* Handle state */ }" />
```

---

## Showcase & Micro-Interactions

### `TwMarquee`

**Location:** [`Components/UI/TwMarquee.razor`](../Components/UI/TwMarquee.razor)

Infinite continuous horizontal scrolling ticker with CSS keyframes, pause-on-hover, and edge gradient masks.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `DurationSeconds` | `int` | `25` | Configures component behavior or styling. |
| `Reverse` | `bool` | `false` | Configures component behavior or styling. |
| `PauseOnHover` | `bool` | `true` | Configures component behavior or styling. |
| `FadeEdges` | `bool` | `true` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |
| `TrackClass` | `string?` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwMarquee DurationSeconds="25" PauseOnHover="true" FadeEdges="true" />
```

---

### `TwLottie`

**Location:** [`Components/UI/TwLottie.razor`](../Components/UI/TwLottie.razor)

Plays a Lottie animation (.lottie or .json) with the bundled dotlottie-web player (no CDN; the script loads only on pages that use it). Under prefers-reduced-motion it holds the file's poster frame instead of playing.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Src` | `string` | `""` | URL of a .lottie or Lottie .json file. |
| `Loop` | `bool` | `false` | Repeat the animation. |
| `Autoplay` | `bool` | `true` | Start playing when loaded (never under reduced motion). |
| `Speed` | `double` | `1` | Playback speed. |
| `Width` | `string` | `"120px"` | CSS width; Height defaults to the same. |
| `ReducedMotion` | `LottieReducedMotion` | `Poster` | Poster (hold the poster frame), Hide (show nothing) or Play. |
| `PosterFrame` | `int?` | `null` | Frame to hold under reduced motion; default is the file's "poster" marker, else frame 0. |
| `AriaLabel` | `string?` | `null` | With a label it is role="img"; without one it is hidden from screen readers. |
| `OnComplete` | `EventCallback` | `-` | Raised when a non-looping animation finishes. |

#### Razor Usage Example

```razor
<TwLottie Src="animations/success-check.lottie" Width="96px" AriaLabel="Saved" />
```

---

### `TwCelebration`

**Location:** [`Components/UI/TwCelebration.razor`](../Components/UI/TwCelebration.razor)

One-shot celebration (success, level up, confetti, badge, streak) from the bundled samples, plus a polite live-region message so the moment is announced and readable without the animation. Confetti is skipped under reduced motion.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Kind` | `CelebrationKind` | `Success` | Success, LevelUp, Confetti, Badge or Streak. |
| `Message` | `string?` | `null` | Text announced and shown; defaults to a short phrase for the kind. |
| `ShowMessage` | `bool` | `true` | Show the message under the animation (always announced). |
| `Size` | `string` | `"120px"` | Animation size. |

#### Razor Usage Example

```razor
<TwCelebration Kind="CelebrationKind.LevelUp" Message="Level 3 reached" />
```

---

### `TwConfetti`

**Location:** [`Components/UI/TwConfetti.razor`](../Components/UI/TwConfetti.razor)

Canvas physics celebratory particle explosion system with gravity, air resistance, and 3D rotation.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `FullScreen` | `bool` | `false` | Configures component behavior or styling. |
| `AutoFire` | `bool` | `false` | Configures component behavior or styling. |
| `ParticleCount` | `int` | `85` | Configures component behavior or styling. |
| `RespectReducedMotion` | `bool` | `true` | Under prefers-reduced-motion draw one still frame instead of moving particles. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwConfetti ParticleCount="85" />
```

---

### `TwSkeleton`

**Location:** [`Components/UI/TwSkeleton.razor`](../Components/UI/TwSkeleton.razor)

Animated shimmer placeholder for cards, tables, text, and avatars during asynchronous data loading.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Variant` | `string` | `"text"` | Semantic visual variant (e.g. Primary, Secondary, Outline). |
| `Width` | `string?` | `null` | Configures component behavior or styling. |
| `Height` | `string?` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwSkeleton Variant="text" />
```

---

### `TwEmptyState`

**Location:** [`Components/UI/TwEmptyState.razor`](../Components/UI/TwEmptyState.razor)

Illustrated zero-data state placeholder with title, description, and call-to-action button.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `"No items found"` | Header title or accessible label. |
| `Description` | `string?` | `null` | Secondary descriptive caption. |
| `IconContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `ActionContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwEmptyState Title="No items found" />
```

---

### `TwBanner`

**Location:** [`Components/UI/TwBanner.razor`](../Components/UI/TwBanner.razor)

Full-width announcement banner with dismiss button and action link.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Text` | `string` | `string.Empty` | Configures component behavior or styling. |
| `BadgeText` | `string?` | `null` | Configures component behavior or styling. |
| `ActionText` | `string?` | `null` | Configures component behavior or styling. |
| `OnAction` | `EventCallback` | `null` | Configures component behavior or styling. |
| `Dismissible` | `bool` | `true` | Configures component behavior or styling. |
| `IsSticky` | `bool` | `false` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwBanner Text="string.Empty" Dismissible="true" />
```

---

### `TwAlert`

**Location:** [`Components/UI/TwAlert.razor`](../Components/UI/TwAlert.razor)

Dismissible inline feedback alert with semantic variants (Success, Warning, Danger, Info).

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string?` | `null` | Header title or accessible label. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `Variant` | `SemanticVariant` | `SemanticVariant.Info` | Semantic visual variant (e.g. Primary, Secondary, Outline). |
| `Dismissible` | `bool` | `null` | Configures component behavior or styling. |
| `OnDismiss` | `EventCallback` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwAlert Variant="SemanticVariant.Info" />
```

---

### `TwToastContainer`

**Location:** [`Components/UI/TwToastContainer.razor`](../Components/UI/TwToastContainer.razor)

Fixed container managing reactive floating toast alert notifications.

#### Parameters & Settings

This component has no parameters.

#### Razor Usage Example

```razor
<TwToastContainer />
```

---

## Smart Components

### `TwSmartOcrButton`

**Location:** [`Components/UI/TwSmartOcrButton.razor`](../Components/UI/TwSmartOcrButton.razor)

Drop-in Smart Component button that triggers instant client-side OCR for paper checks, invoices, and PDF forms.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Class` | `string` | `""` | Optional CSS classes. |
| `OnDocumentScanned` | `EventCallback<DocumentOcrResult>` | `null` | Fired when a document is scanned and entities are extracted. |
| `OnCheckScanned` | `EventCallback<CheckDraftOcrResult>` | `null` | Fired when a paper check or bank draft is scanned. |

#### Razor Usage Example

```razor
<TwSmartOcrButton OnDocumentScanned="HandleDoc" OnCheckScanned="HandleCheck" />
```

---

## Styling Engine & Multi-Part Primitives

### `TwUser`

**Location:** [`Components/UI/TwUser.razor`](../Components/UI/TwUser.razor)

Multi-part user profile badge with avatar/initials, status dot indicator, primary name, secondary handle/role description, and per-slot customization via TwUserSlots.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Name` | `string` | `""` | User display name. |
| `Description` | `string?` | `null` | Secondary role or handle description. |
| `AvatarUrl` | `string?` | `null` | Optional avatar image URL. |
| `Initials` | `string?` | `null` | Fallback avatar initials. |
| `Status` | `string?` | `null` | Presence status dot (online, busy, away, offline). |
| `Classes` | `TwUserSlots?` | `null` | Multi-part slot class overrides (Base, Avatar, StatusDot, Wrapper, Name, Description). |
| `Class` | `string?` | `null` | Root container utility overrides resolved via TwMerge. |

#### Razor Usage Example

```razor
<TwUser Name="Elena Rostova" Description="Principal UI Architect" Initials="ER" Status="online" />
```

---

### `TwFloatingLabelInput`

**Location:** [`Components/UI/TwFloatingLabelInput.razor`](../Components/UI/TwFloatingLabelInput.razor)

Animated peer-floating label input supporting Outlined, Filled, and Standard underline variants, multi-part TwFloatingInputSlots, and automatic registration inside a parent TwValidatorGroup.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Label` | `string` | `"Label"` | Floating label text. |
| `Value` | `string?` | `null` | Two-way bound input value. |
| `Variant` | `FloatingLabelVariant` | `FloatingLabelVariant.Outlined` | Visual style (Outlined, Filled, Standard). |
| `Status` | `ValidationStatus` | `ValidationStatus.None` | Explicit validation status when used standalone. |
| `Required` | `bool` | `false` | Marks field as required during validation. |
| `MinLength` | `int?` | `null` | Minimum character length rule. |
| `Pattern` | `string?` | `null` | Regular expression validation pattern. |
| `Validator` | `Func<string?, string?>?` | `null` | Custom validation delegate returning null when valid or an error message. |
| `HelperText` | `string?` | `null` | Neutral helper caption below the input. |
| `ErrorMessage` | `string?` | `null` | Custom error message when validation fails. |
| `SuccessMessage` | `string?` | `null` | Caption displayed when validation succeeds. |
| `Classes` | `TwFloatingInputSlots?` | `null` | Multi-part slot class overrides (Base, Input, Label, HelperText). |

#### Razor Usage Example

```razor
<TwFloatingLabelInput Label="Workspace Slug" Variant="FloatingLabelVariant.Outlined" @bind-Value="_slug" Required="true" MinLength="4" />
```

---

### `TwValidatorGroup`

**Location:** [`Components/UI/TwValidatorGroup.razor`](../Components/UI/TwValidatorGroup.razor)

Decoupled cascading reactive validation coordinator that tracks child TwFloatingLabelInput instances and exposes ValidateAll(), ResetAll(), ValidCount, InvalidCount, and IsAllValid.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ChildContent` | `RenderFragment?` | `null` | Child form inputs subscribing to the cascading validation scope. |
| `OnStatusChanged` | `EventCallback<bool>` | `default` | Fires whenever aggregate validation status changes. |
| `Class` | `string?` | `null` | Container CSS utility classes. |

#### Razor Usage Example

```razor
<TwValidatorGroup @ref="_validatorGroup"><TwFloatingLabelInput Label="Email" @bind-Value="_email" Required="true" /></TwValidatorGroup>
```

---

### `TwHoverCard`

**Location:** [`Components/UI/TwHoverCard.razor`](../Components/UI/TwHoverCard.razor)

Hover-intent preview card with configurable open/close delays, an invisible cursor bridge so users can move into the floating card without it closing, directional arrow, and TwHoverCardSlots.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `TriggerContent` | `RenderFragment?` | `null` | Inline trigger element that opens the card on hover or focus. |
| `ChildContent` | `RenderFragment?` | `null` | Floating preview card body content. |
| `Placement` | `TwPopoverPlacement` | `TwPopoverPlacement.Bottom` | Floating card placement relative to trigger. |
| `OpenDelayMs` | `int` | `120` | Hover intent delay in milliseconds before opening. |
| `CloseDelayMs` | `int` | `220` | Grace delay in milliseconds before closing after mouse leave. |
| `ShowArrow` | `bool` | `true` | Whether to render the directional pointer arrow. |
| `Classes` | `TwHoverCardSlots?` | `null` | Multi-part slot overrides (Base, Trigger, Content, Arrow). |

#### Razor Usage Example

```razor
<TwHoverCard><TriggerContent><span>Hover Profile</span></TriggerContent><ChildContent><p>Rich preview content</p></ChildContent></TwHoverCard>
```

---

### `TwSonner`

**Location:** [`Components/UI/TwSonner.razor`](../Components/UI/TwSonner.razor)

3D stacked toast notification deck with depth-scaled cards, hover/programmatic fan-out expansion, semantic rich colors, and async PushPromiseAsync loading-to-success transitions.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `List<SonnerToastItem>` | `new()` | Active toast notifications in the stack. |
| `Expanded` | `bool` | `false` | Programmatically expands the 3D toast stack into full list view. |
| `MaxVisible` | `int` | `4` | Maximum number of toasts rendered in the visible stack. |
| `RichColors` | `bool` | `true` | Applies semantic color palettes for Success, Warning, Danger, and Info toasts. |
| `OnActionClicked` | `EventCallback<SonnerToastItem>` | `default` | Callback invoked when a toast action button is clicked. |

#### Razor Usage Example

```razor
<TwSonner @ref="_sonner" Items="_toasts" Expanded="false" />
```

---

### `TwMenubar`

**Location:** [`Components/UI/TwMenubar.razor`](../Components/UI/TwMenubar.razor)

Desktop-grade horizontal application menubar with click-to-open menus, automatic hover-switching between sibling menus, keyboard shortcut badges, separators, and checkable items.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Menus` | `List<MenubarMenu>` | `new()` | Top-level menu definitions and their dropdown items. |
| `OnItemSelected` | `EventCallback<MenubarItem>` | `default` | Invoked when a menu item is clicked or toggled. |
| `Class` | `string?` | `null` | Optional root utility class overrides. |

#### Razor Usage Example

```razor
<TwMenubar Menus="@_menus" OnItemSelected="HandleCommand" />
```

---

### `TwToggleGroup`

**Location:** [`Components/UI/TwToggleGroup.razor`](../Components/UI/TwToggleGroup.razor)

Single or multiple selection segmented toggle bar with WAI-ARIA aria-pressed states, icon + label items, and Outline/Soft variants.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Options` | `List<ToggleItem>` | `new()` | Toggle button items (Value, Label, Icon, Tooltip, Disabled). |
| `Multiple` | `bool` | `false` | Allows selecting multiple options simultaneously. |
| `Value` | `string?` | `null` | Selected value in single-select mode. |
| `SelectedValues` | `HashSet<string>` | `new()` | Selected values set in multi-select mode. |
| `Variant` | `ButtonVariant` | `ButtonVariant.Outline` | Visual container variant (Outline or Soft). |
| `Size` | `ComponentSize` | `ComponentSize.Sm` | Button size (Xs, Sm, Md, Lg). |

#### Razor Usage Example

```razor
<TwToggleGroup Options="@_options" Multiple="true" @bind-SelectedValues="_selected" Variant="ButtonVariant.Soft" />
```

---

### `TwMockup`

**Location:** [`Components/UI/TwMockup.razor`](../Components/UI/TwMockup.razor)

Device and application frame container supporting Browser (with traffic lights and URL bar), Phone (with Dynamic Island notch), Window, and Terminal Code (with line prefixes and highlights) modes.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Kind` | `MockupKind` | `MockupKind.Browser` | Mockup frame style (Browser, Phone, Window, Code). |
| `Url` | `string` | `"https://blazortwui.dev/studio"` | URL displayed in Browser mode address bar. |
| `Title` | `string` | `"BlazorTwUI — Studio"` | Window/Terminal title bar caption. |
| `CodeLines` | `List<MockupCodeLine>?` | `null` | Terminal command/output lines when Kind is Code. |
| `ChildContent` | `RenderFragment?` | `null` | Viewport body content. |
| `Class` | `string?` | `null` | Root frame utility class overrides resolved via TwMerge. |

#### Razor Usage Example

```razor
<TwMockup Kind="MockupKind.Browser" Url="https://blazortwui.dev"><p>Preview</p></TwMockup>
```

---

### `TwSwap`

**Location:** [`Components/UI/TwSwap.razor`](../Components/UI/TwSwap.razor)

Animated two-state toggle button that transitions between OnContent and OffContent using CSS Rotate, 3D Flip, or Fade animations with WAI-ARIA aria-pressed compliance.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Active` | `bool` | `false` | Two-way bound active state. |
| `Animation` | `SwapAnimation` | `SwapAnimation.Rotate` | Transition style (Rotate, Flip, Fade). |
| `OnContent` | `RenderFragment?` | `null` | Content rendered when Active is true. |
| `OffContent` | `RenderFragment?` | `null` | Content rendered when Active is false. |
| `Class` | `string?` | `null` | Button utility class overrides. |

#### Razor Usage Example

```razor
<TwSwap @bind-Active="_active" Animation="SwapAnimation.Rotate"><OnContent>☀️</OnContent><OffContent>🌙</OffContent></TwSwap>
```

---

### `TwChatBubble`

**Location:** [`Components/UI/TwChatBubble.razor`](../Components/UI/TwChatBubble.razor)

Semantic start/end aligned conversation bubble with avatar initials, sender name, timestamp header, delivery status footer, and semantic color variants.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Sender` | `string?` | `null` | Sender display name. |
| `Timestamp` | `string?` | `null` | Message timestamp caption. |
| `Initials` | `string?` | `null` | Avatar initials. |
| `Message` | `string?` | `null` | Message text content. |
| `FooterStatus` | `string?` | `null` | Delivery or read-receipt status text. |
| `IsEnd` | `bool` | `false` | Right-aligns the chat bubble for outgoing messages. |
| `Variant` | `SemanticVariant` | `SemanticVariant.Default` | Bubble color variant (Default, Primary, Success, Warning, Danger, Info). |

#### Razor Usage Example

```razor
<TwChatBubble Sender="System" Initials="BT" Timestamp="14:32" Message="Deployment ready!" FooterStatus="Delivered" />
```

---

## Task-Purpose Enterprise Forms

### `TwUserRegistrationForm`

**Location:** [`Components/UI/TwUserRegistrationForm.razor`](../Components/UI/TwUserRegistrationForm.razor)

Turnkey enterprise user registration form featuring full name, username, corporate email, role selector, real-time password strength meter, terms consent, and federated SSO options.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `"Create Your Account"` | Header title or accessible label. |
| `Subtitle` | `string` | `"Deploy enterprise WebAssembly apps with instant Tailwind CSS parity"` | Header title or accessible label. |
| `ShowSocialLogins` | `bool` | `true` | Configures component behavior or styling. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `OnRegistered` | `EventCallback<RegistrationModel>` | `null` | Configures component behavior or styling. |
| `OnSsoRequested` | `EventCallback<string>` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwUserRegistrationForm Title="Create Your Account" Subtitle="Deploy enterprise WebAssembly apps with instant Tailwind CSS parity" ShowSocialLogins="true" />
```

---

### `TwUserEditForm`

**Location:** [`Components/UI/TwUserEditForm.razor`](../Components/UI/TwUserEditForm.razor)

Comprehensive profile and security management form featuring interactive avatar selection, corporate role attributes, 2FA toggle, session management, and audited account deactivation.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Model` | `UserProfileModel?` | `null` | Configures component behavior or styling. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `OnSaved` | `EventCallback<UserProfileModel>` | `null` | Configures component behavior or styling. |
| `OnDeactivate` | `EventCallback` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwUserEditForm CustomClass="" />
```

---

### `TwLoginForm`

**Location:** [`Components/UI/TwLoginForm.razor`](../Components/UI/TwLoginForm.razor)

Split-screen enterprise authentication form featuring curated Unsplash architectural imagery, inspiring technical quotes, federated SSO buttons, password visibility toggle, and remember-me persistence.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `"Sign in to Enterprise Hub"` | Header title or accessible label. |
| `Subtitle` | `string` | `"Access mission-critical dashboards, design tokens, and components"` | Header title or accessible label. |
| `ImageUrl` | `string` | `"https://images.unsplash.com/photo-1517694712202-14dd9538aa97?auto=format&fit=crop&w=1200&q=80"` | Configures component behavior or styling. |
| `QuoteText` | `string` | `"Simplicity is prerequisite for reliability."` | Configures component behavior or styling. |
| `QuoteAuthor` | `string` | `"Edsger W. Dijkstra"` | Configures component behavior or styling. |
| `QuoteCitation` | `string` | `"Turing Award Laureate"` | Configures component behavior or styling. |
| `ShowSocialLogins` | `bool` | `true` | Configures component behavior or styling. |
| `AllowRememberMe` | `bool` | `true` | Configures component behavior or styling. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `OnLogin` | `EventCallback<LoginCredentials>` | `null` | Configures component behavior or styling. |
| `OnSsoLogin` | `EventCallback<string>` | `null` | Configures component behavior or styling. |
| `OnForgotPassword` | `EventCallback` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwLoginForm Title="Sign in to Enterprise Hub" Subtitle="Access mission-critical dashboards, design tokens, and components" ImageUrl="https://images.unsplash.com/photo-1517694712202-14dd9538aa97?auto=format&fit=crop&w=1200&q=80" />
```

---

### `TwTermsOfUseForm`

**Location:** [`Components/UI/TwTermsOfUseForm.razor`](../Components/UI/TwTermsOfUseForm.razor)

Executive legal terms agreement viewer with section jump pills, PDF export, review progress tracking, signatory name/company binding, and tamper-evident SHA-256 hash stamp.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `"Enterprise Terms of Service"` | Header title or accessible label. |
| `Version` | `string` | `"v3.2"` | Configures component behavior or styling. |
| `LastUpdated` | `string` | `"October 2026"` | Configures component behavior or styling. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `OnAccepted` | `EventCallback<TermsAcceptanceModel>` | `null` | Configures component behavior or styling. |
| `OnDeclined` | `EventCallback` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwTermsOfUseForm Title="Enterprise Terms of Service" Version="v3.2" LastUpdated="October 2026" />
```

---

### `TwCookieConsent`

**Location:** [`Components/UI/TwCookieConsent.razor`](../Components/UI/TwCookieConsent.razor)

GDPR and CCPA compliant cookie preference manager featuring strictly necessary lock, customizable functional/telemetry/marketing toggles, and instant preferences persistence.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `"Enterprise Cookie & Privacy Settings"` | Header title or accessible label. |
| `Description` | `string` | `"We use minimal first-party cookies to secure user sessions, maintain UI state, and measure performance latency. We never sell your personal data."` | Secondary descriptive caption. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `OnSaved` | `EventCallback<CookiePreferencesModel>` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwCookieConsent Title="Enterprise Cookie & Privacy Settings" Description="We use minimal first-party cookies to secure user sessions, maintain UI state, and measure performance latency. We never sell your personal data." CustomClass="" />
```

---

### `TwPrivacyPolicyForm`

**Location:** [`Components/UI/TwPrivacyPolicyForm.razor`](../Components/UI/TwPrivacyPolicyForm.razor)

Enterprise privacy charter featuring zero-CDN data guarantees and an automated self-service Data Subject Request (DSR) portal for machine-readable JSON/ZIP exports and audited account erasure.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `"Enterprise Privacy & Data Governance"` | Header title or accessible label. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `OnExportRequested` | `EventCallback<string>` | `null` | Configures component behavior or styling. |
| `OnErasureRequested` | `EventCallback` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwPrivacyPolicyForm Title="Enterprise Privacy & Data Governance" CustomClass="" />
```

---

### `TwAboutMissionForm`

**Location:** [`Components/UI/TwAboutMissionForm.razor`](../Components/UI/TwAboutMissionForm.razor)

Enterprise vision and architectural charter banner with live performance metrics, engineering pillars, leadership team cards, and an interactive executive consultation inquiry form.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `MissionHeading` | `string` | `"Empowering Modern Enterprises with Zero-Latency Blazor UI"` | Configures component behavior or styling. |
| `MissionStatement` | `string` | `"Our mission is to build the world's most performant, secure, and developer-delightful component library for .NET 10+ WebAssembly, bridging modern Tailwind CSS agility with enterprise-grade reliability and zero third-party telemetry."` | Configures component behavior or styling. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `OnInquirySubmitted` | `EventCallback<ContactInquiryModel>` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwAboutMissionForm MissionHeading="Empowering Modern Enterprises with Zero-Latency Blazor UI" MissionStatement="Our mission is to build the world's most performant, secure, and developer-delightful component library for .NET 10+ WebAssembly, bridging modern Tailwind CSS agility with enterprise-grade reliability and zero third-party telemetry." CustomClass="" />
```

---

### `TwPledgeForm`

**Location:** [`Components/UI/TwPledgeForm.razor`](../Components/UI/TwPledgeForm.razor)

Official Code of Conduct, security compliance, and supply chain integrity pledge form with three sworn adherence principles, signatory details, and digital signature capture.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `"Enterprise Engineering & Ethics Pledge"` | Header title or accessible label. |
| `Subtitle` | `string` | `"Legally affirmed compliance with software supply chain integrity and responsible AI"` | Header title or accessible label. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `OnPledgeAffirmed` | `EventCallback<PledgeModel>` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwPledgeForm Title="Enterprise Engineering & Ethics Pledge" Subtitle="Legally affirmed compliance with software supply chain integrity and responsible AI" CustomClass="" />
```

---

## Theme & Palette Customization

### `ThemeProvider`

**Location:** [`Components/Theme/ThemeProvider.razor`](../Components/Theme/ThemeProvider.razor)

Cascading state provider managing light, dark, and system color mode preferences with localStorage synchronization and zero-FOUC initialization.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |

#### Razor Usage Example

```razor
<ThemeProvider>
    <span>Content</span>
</ThemeProvider>
```

---

### `ThemeSwitcher`

**Location:** [`Components/Theme/ThemeSwitcher.razor`](../Components/Theme/ThemeSwitcher.razor)

Segmented mode selector with accent swatches for quick switching between Light, Dark, and OS System appearance.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Compact` | `bool` | `null` | Configures component behavior or styling. |
| `ShowAccentPicker` | `bool` | `true` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<ThemeSwitcher ShowAccentPicker="true" />
```

---

### `TwThemeSelector`

**Location:** [`Components/Theme/TwThemeSelector.razor`](../Components/Theme/TwThemeSelector.razor)

Modern comprehensive color dropdown selector featuring preset palette swatches and 16M+ custom hex color picker integration.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Compact` | `bool` | `null` | Configures component behavior or styling. |
| `Alignment` | `string` | `"right"` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwThemeSelector Alignment="right" />
```

---

### `TwThemeSwitcher`

**Location:** [`Components/Theme/TwThemeSwitcher.razor`](../Components/Theme/TwThemeSwitcher.razor)

Compact pill switcher for toggling between dark, light, and system themes.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Compact` | `bool` | `null` | Configures component behavior or styling. |
| `ShowAccentPicker` | `bool` | `true` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwThemeSwitcher ShowAccentPicker="true" />
```

---

### `TwColorPicker`

**Location:** [`Components/UI/TwColorPicker.razor`](../Components/UI/TwColorPicker.razor)

Interactive 16M+ hex and RGB color picker with preview swatch, hex validation, and opacity support.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `string` | `"#6366f1"` | Primary data value or two-way bound model property. |
| `ValueChanged` | `EventCallback<string>` | `null` | Callback invoked when state changes or item is selected. |
| `Label` | `string?` | `"Pick Accent Color"` | Configures component behavior or styling. |
| `ShowValueBadge` | `bool` | `true` | Primary data value or two-way bound model property. |
| `AllowCustom` | `bool` | `true` | Configures component behavior or styling. |
| `ShowCopy` | `bool` | `true` | Configures component behavior or styling. |
| `Inline` | `bool` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwColorPicker Value="#6366f1" Label="Pick Accent Color" ShowValueBadge="true" />
```

---

## Typography & Utilities

### `TwHighlight`

**Location:** [`Components/UI/TwHighlight.razor`](../Components/UI/TwHighlight.razor)

Fast text search highlighter supporting case-sensitive matching, whole-word boundaries, match count badges, and Next/Prev navigation.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Text` | `string` | `""` | Full body text to search. |
| `SearchText` | `string` | `""` | Query term to highlight. |
| `CaseSensitive` | `bool` | `false` | Whether search matches exact case. |
| `WholeWord` | `bool` | `false` | Match entire words only. |
| `ActiveIndex` | `int` | `0` | Index of currently focused active match. |
| `ShowNavigation` | `bool` | `true` | Displays Next/Prev navigation bar with match count. |

#### Razor Usage Example

```razor
<TwHighlight Text="@body" SearchText="@query" ShowNavigation="true" />
```

---

## Typography, Badges & Keys

### `TwPill`

**Location:** [`Components/UI/TwPill.razor`](../Components/UI/TwPill.razor)

Modern pill badge supporting glowing pulse dots, mini avatars, counter badges, dismissible close buttons, and semantic variants (Soft, Solid, Outline, Glow).

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Text` | `string?` | `null` | Configures component behavior or styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `Variant` | `string` | `"Soft"` | Semantic visual variant (e.g. Primary, Secondary, Outline). |
| `Color` | `string` | `"Brand"` | Configures component behavior or styling. |
| `Size` | `string` | `"md"` | Component sizing scale (Xs, Sm, Md, Lg). |
| `ShowDot` | `bool` | `false` | Configures component behavior or styling. |
| `PulseDot` | `bool` | `false` | Configures component behavior or styling. |
| `AvatarUrl` | `string?` | `null` | Configures component behavior or styling. |
| `Icon` | `string?` | `null` | Configures component behavior or styling. |
| `Count` | `int?` | `null` | Configures component behavior or styling. |
| `Removable` | `bool` | `false` | Configures component behavior or styling. |
| `Clickable` | `bool` | `false` | Configures component behavior or styling. |
| `Selected` | `bool` | `false` | Configures component behavior or styling. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `OnClick` | `EventCallback` | `null` | Callback invoked when state changes or item is selected. |
| `OnDismiss` | `EventCallback` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwPill Variant="Soft" Color="Brand" Size="md" />
```

---

### `TwPillGroup`

**Location:** [`Components/UI/TwPillGroup.razor`](../Components/UI/TwPillGroup.razor)

Segmented pill filter collection for single or multi-select filtering with smooth active transitions.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `List<PillItem>` | `new()` | Configures component behavior or styling. |
| `SelectedValues` | `List<string>` | `new()` | Primary data value or two-way bound model property. |
| `SelectedValuesChanged` | `EventCallback<List<string>>` | `null` | Callback invoked when state changes or item is selected. |
| `MultiSelect` | `bool` | `false` | Configures component behavior or styling. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |

#### Razor Usage Example

```razor
<TwPillGroup Items="new()" SelectedValues="new()" CustomClass="" />
```

---

### `TwBadge`

**Location:** [`Components/UI/TwBadge.razor`](../Components/UI/TwBadge.razor)

Status and counter badge supporting semantic colors (Primary, Success, Warning, Danger, Info), rounded pill styling, and optional pulsing status dots.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `Variant` | `SemanticVariant` | `SemanticVariant.Default` | Semantic visual variant (e.g. Primary, Secondary, Outline). |
| `ShowDot` | `bool` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwBadge Variant="SemanticVariant.Default" />
```

---

### `TwNotificationBadge`

**Location:** [`Components/UI/TwNotificationBadge.razor`](../Components/UI/TwNotificationBadge.razor)

Floating count badge designed to overlay icons or avatars for unread alerts and incoming message counters.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `Visible` | `bool` | `true` | Configures component behavior or styling. |
| `DotOnly` | `bool` | `null` | Configures component behavior or styling. |
| `ShowPing` | `bool` | `null` | Configures component behavior or styling. |
| `Count` | `int` | `null` | Configures component behavior or styling. |
| `MaxCount` | `int` | `99` | Configures component behavior or styling. |
| `Variant` | `SemanticVariant` | `SemanticVariant.Danger` | Semantic visual variant (e.g. Primary, Secondary, Outline). |
| `Placement` | `BadgePlacement` | `BadgePlacement.TopRight` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwNotificationBadge Visible="true" MaxCount="99" Variant="SemanticVariant.Danger" />
```

---

### `TwChip`

**Location:** [`Components/UI/TwChip.razor`](../Components/UI/TwChip.razor)

Interactive tag chip with optional leading icon, selectable toggle state, and dismissible close button.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Text` | `string?` | `null` | Configures component behavior or styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `LeadingIcon` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `AvatarContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `Variant` | `SemanticVariant` | `SemanticVariant.Default` | Semantic visual variant (e.g. Primary, Secondary, Outline). |
| `Size` | `ComponentSize` | `ComponentSize.Sm` | Component sizing scale (Xs, Sm, Md, Lg). |
| `Outlined` | `bool` | `null` | Configures component behavior or styling. |
| `Selectable` | `bool` | `null` | Configures component behavior or styling. |
| `Selected` | `bool` | `null` | Configures component behavior or styling. |
| `SelectedChanged` | `EventCallback<bool>` | `null` | Callback invoked when state changes or item is selected. |
| `Removable` | `bool` | `null` | Configures component behavior or styling. |
| `OnRemove` | `EventCallback` | `null` | Configures component behavior or styling. |
| `OnClick` | `EventCallback` | `null` | Callback invoked when state changes or item is selected. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwChip Variant="SemanticVariant.Default" Size="ComponentSize.Sm" />
```

---

### `TwChipGroup`

**Location:** [`Components/UI/TwChipGroup.razor`](../Components/UI/TwChipGroup.razor)

Container for managing groups of filter chips with single-select or multi-select modes.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwChipGroup>
    <span>Content</span>
</TwChipGroup>
```

---

### `TwKbd`

**Location:** [`Components/UI/TwKbd.razor`](../Components/UI/TwKbd.razor)

3D-embossed tactile keyboard shortcut badge with automatic modifier symbol mapping (⌘, ⌥, ⇧, ⌃, ↵, ⎋, ←, →).

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Key` | `string?` | `null` | Configures component behavior or styling. |
| `Size` | `string` | `"md"` | Component sizing scale (Xs, Sm, Md, Lg). |
| `Variant` | `string` | `"default"` | Semantic visual variant (e.g. Primary, Secondary, Outline). |
| `Tooltip` | `string?` | `null` | Configures component behavior or styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwKbd Size="md" Variant="default" />
```

---

### `TwDivider`

**Location:** [`Components/UI/TwDivider.razor`](../Components/UI/TwDivider.razor)

Subtle horizontal or vertical divider with optional centered text or icon badge.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Text` | `string?` | `null` | Configures component behavior or styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwDivider>
    <span>Content</span>
</TwDivider>
```

---

### `TwVirtualKeyboard`

**Location:** [`Components/UI/TwVirtualKeyboard.razor`](../Components/UI/TwVirtualKeyboard.razor)

On-screen virtual keyboard for kiosks, touch displays, POS terminals, and anti-keylogger secure PIN entry with QWERTY, Numeric, and scrambled PIN modes.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `string` | `""` | Two-way bindable typed text/PIN value. |
| `Mode` | `VirtualKeyboardMode` | `VirtualKeyboardMode.Qwerty` | Key layout mode (Qwerty, Numeric, Pin). |
| `IsPassword` | `bool` | `false` | Mask display output with bullet points. |
| `ShowDisplay` | `bool` | `true` | Show top value display bar with clear button. |
| `AllowModeSwitch` | `bool` | `true` | Show mode switcher tabs. |
| `OnSubmit` | `EventCallback` | `-` | Callback invoked when Enter / Submit key is pressed. |

#### Razor Usage Example

```razor
<TwVirtualKeyboard @bind-Value="_enteredPin" Mode="VirtualKeyboardMode.Pin" IsPassword="true" OnSubmit="HandlePinSubmit" />
```

---

## Visualizations & Gauges

### `TwBarGauge`

**Location:** [`Components/UI/TwBarGauge.razor`](../Components/UI/TwBarGauge.razor)

Concentric 270-degree bars, one per value, with a legend carrying the exact numbers.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `IReadOnlyList<GaugeItem>` | `required` | Label and Value. |
| `Min` | `double` | `0` | Scale minimum. |
| `Max` | `double` | `100` | Scale maximum. |
| `Unit` | `string` | `""` | Suffix shown in the legend. |
| `Class` | `string?` | `null` | Custom Tailwind CSS classes. |

#### Razor Usage Example

```razor
<TwBarGauge Items="_items" Unit="%" />
```

---

### `TwSparkline`

**Location:** [`Components/UI/TwSparkline.razor`](../Components/UI/TwSparkline.razor)

Zero-dependency pure C# inline SVG micro-chart engine supporting Line, Area, Bar, and Win/Loss modes for table cells and executive KPI widgets.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Type` | `SparklineType` | `SparklineType.Line` | Configures component behavior or styling. |
| `Points` | `List<double>` | `new() { 12, 18, 14, 22, 19, 28, 25, 34, 30, 42 }` | Configures component behavior or styling. |
| `Width` | `int` | `110` | Configures component behavior or styling. |
| `Height` | `int` | `28` | Configures component behavior or styling. |
| `StrokeWidth` | `double` | `2.0` | Configures component behavior or styling. |
| `Color` | `string` | `"brand"` | Configures component behavior or styling. |
| `ShowMinMaxDots` | `bool` | `true` | Configures component behavior or styling. |
| `ShowValueBadge` | `bool` | `false` | Primary data value or two-way bound model property. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwSparkline Type="SparklineType.Line" Points="new() { 12, 18, 14, 22, 19, 28, 25, 34, 30, 42 }" Width="110" />
```

---

### `TwProgressAdvanced`

**Location:** [`Components/UI/TwProgressAdvanced.razor`](../Components/UI/TwProgressAdvanced.razor)

Advanced enterprise progress visualization supporting 5 styles: Multi-segment stacked bar with tooltips, SVG circular radial ring, flowing striped animated bar, discrete LED audio-meter bar, and milestone step tracker.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Type` | `ProgressType` | `ProgressType.Stacked` | Configures component behavior or styling. |
| `Title` | `string?` | `null` | Header title or accessible label. |
| `Label` | `string?` | `null` | Configures component behavior or styling. |
| `Value` | `double` | `65` | Primary data value or two-way bound model property. |
| `ShowHeader` | `bool` | `true` | Configures component behavior or styling. |
| `ShowLegend` | `bool` | `true` | Configures component behavior or styling. |
| `Color` | `string` | `"brand"` | Configures component behavior or styling. |
| `CustomClass` | `string` | `""` | Configures component behavior or styling. |
| `Segments` | `List<ProgressSegment>` | `new()     {         new ProgressSegment { Label = "Code & Engine", Percentage = 42, ColorClass = "bg-brand-500" },         new ProgressSegment { Label = "Assets & Icons", Percentage = 28, ColorClass = "bg-emerald-500" },         new ProgressSegment { Label = "Cached Data", Percentage = 18, ColorClass = "bg-amber-500" },         new ProgressSegment { Label = "Free Memory", Percentage = 12, ColorClass = "bg-slate-300 dark:bg-slate-700" }     }` | Configures component behavior or styling. |
| `RadialSize` | `int` | `100` | Component sizing scale (Xs, Sm, Md, Lg). |
| `RadialStrokeWidth` | `int` | `8` | Configures component behavior or styling. |
| `RadialContent` | `RenderFragment?` | `null` | Configures component behavior or styling. |
| `TotalSegments` | `int` | `15` | Configures component behavior or styling. |
| `Steps` | `List<string>` | `new() { "Initialize", "Validate", "Compile", "Deploy" }` | Configures component behavior or styling. |
| `CurrentStep` | `int` | `2` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwProgressAdvanced Type="ProgressType.Stacked" Value="65" ShowHeader="true" />
```

---

### `TwChart`

**Location:** [`Components/UI/TwChart.razor`](../Components/UI/TwChart.razor)

Native C# inline SVG visualization engine for donut charts, sparklines, and bar charts with zero external JS dependencies.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Type` | `ChartType` | `ChartType.Donut` | Configures component behavior or styling. |
| `Title` | `string?` | `null` | Header title or accessible label. |
| `Subtitle` | `string?` | `null` | Header title or accessible label. |
| `CenterValue` | `string` | `"100%"` | Primary data value or two-way bound model property. |
| `CenterLabel` | `string` | `"Total"` | Configures component behavior or styling. |
| `DataItems` | `IReadOnlyList<ChartDataItem>` | `Array.Empty<ChartDataItem>()` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwChart Type="ChartType.Donut" CenterValue="100%" CenterLabel="Total" />
```

---

### `TwGauge`

**Location:** [`Components/UI/TwGauge.razor`](../Components/UI/TwGauge.razor)

Zero-JS radial speedometer gauge rendering pure inline SVG arcs with stroke-dasharray calculations and CSS transitions.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `double` | `72` | Primary data value or two-way bound model property. |
| `Min` | `double` | `0` | Configures component behavior or styling. |
| `Max` | `double` | `100` | Configures component behavior or styling. |
| `Unit` | `string` | `"%"` | Configures component behavior or styling. |
| `Label` | `string?` | `"Efficiency"` | Configures component behavior or styling. |
| `Sublabel` | `string?` | `null` | Configures component behavior or styling. |
| `Color` | `string?` | `null` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwGauge Value="72" Min="0" Max="100" />
```

---

### `TwProgressBar`

**Location:** [`Components/UI/TwProgressBar.razor`](../Components/UI/TwProgressBar.razor)

Semantic progress bar with animated stripes, smooth transitions, and percentage labels.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `int` | `null` | Primary data value or two-way bound model property. |
| `Label` | `string?` | `null` | Configures component behavior or styling. |
| `ShowLabel` | `bool` | `true` | Configures component behavior or styling. |
| `Animated` | `bool` | `null` | Configures component behavior or styling. |
| `Variant` | `SemanticVariant` | `SemanticVariant.Primary` | Semantic visual variant (e.g. Primary, Secondary, Outline). |
| `Size` | `ComponentSize` | `ComponentSize.Md` | Component sizing scale (Xs, Sm, Md, Lg). |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwProgressBar ShowLabel="true" Variant="SemanticVariant.Primary" Size="ComponentSize.Md" />
```

---

### `TwCountUp`

**Location:** [`Components/UI/TwCountUp.razor`](../Components/UI/TwCountUp.razor)

Smooth numeric counter animation with customizable duration, prefix/suffix, and easing.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `StartValue` | `double` | `0` | Primary data value or two-way bound model property. |
| `EndValue` | `double` | `100` | Primary data value or two-way bound model property. |
| `DurationMs` | `int` | `1200` | Configures component behavior or styling. |
| `Decimals` | `int` | `0` | Configures component behavior or styling. |
| `Prefix` | `string?` | `null` | Configures component behavior or styling. |
| `Suffix` | `string?` | `null` | Configures component behavior or styling. |
| `AutoStart` | `bool` | `true` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwCountUp StartValue="0" EndValue="100" DurationMs="1200" />
```

---

### `TwAnimate`

**Location:** [`Components/UI/TwAnimate.razor`](../Components/UI/TwAnimate.razor)

Declarative CSS animation wrapper providing fade, zoom, bounce, and slide keyframes.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Type` | `AnimationType` | `AnimationType.FadeInUp` | Configures component behavior or styling. |
| `DurationMs` | `int` | `400` | Configures component behavior or styling. |
| `DelayMs` | `int` | `0` | Configures component behavior or styling. |
| `Infinite` | `bool` | `null` | Configures component behavior or styling. |
| `OnHover` | `bool` | `null` | Configures component behavior or styling. |
| `Play` | `bool` | `true` | Configures component behavior or styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |

#### Razor Usage Example

```razor
<TwAnimate Type="AnimationType.FadeInUp" DurationMs="400" DelayMs="0" />
```

---

### `TwTransition`

**Location:** [`Components/UI/TwTransition.razor`](../Components/UI/TwTransition.razor)

Smooth enter/leave transition container for mounting and unmounting Blazor components.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Show` | `bool` | `true` | Configures component behavior or styling. |
| `ChildContent` | `RenderFragment?` | `null` | Child content or nested markup rendered inside the component slot. |
| `Enter` | `string` | `"transition duration-300 ease-out"` | Configures component behavior or styling. |
| `EnterFrom` | `string` | `"opacity-0 scale-95 -translate-y-2"` | Configures component behavior or styling. |
| `EnterTo` | `string` | `"opacity-100 scale-100 translate-y-0"` | Configures component behavior or styling. |
| `Leave` | `string` | `"transition duration-200 ease-in"` | Configures component behavior or styling. |
| `LeaveFrom` | `string` | `"opacity-100 scale-100 translate-y-0"` | Configures component behavior or styling. |
| `LeaveTo` | `string` | `"opacity-0 scale-95 -translate-y-2"` | Configures component behavior or styling. |
| `Class` | `string?` | `null` | Additional custom Tailwind CSS utility classes. |
| `Style` | `string?` | `null` | Configures component behavior or styling. |

#### Razor Usage Example

```razor
<TwTransition Show="true" Enter="transition duration-300 ease-out" EnterFrom="opacity-0 scale-95 -translate-y-2" />
```

---

### `TwWordCloud`

**Location:** [`Components/UI/TwWordCloud.razor`](../Components/UI/TwWordCloud.razor)

Pure SVG weighted term and keyword frequency cloud with Archimedean spiral positioning, harmonic color palettes, hover scaling, center word balloon overlay, and click callbacks.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Words` | `IReadOnlyList<WordCloudItem>` | `[]` | Weighted words collection with Text and Weight. |
| `Title` | `string?` | `"Keyword Density & Term Frequency"` | Chart header title. |
| `Width` | `int` | `640` | SVG viewBox width. |
| `Height` | `int` | `320` | SVG viewBox height. |
| `SurroundingWordCount` | `int` | `11` | Count of surrounding spiral words (0 to 25; 0 renders clean center balloon). |
| `SurroundingWordScale` | `double` | `1.0` | Font scale multiplier for surrounding words. |
| `CenterWordScale` | `double` | `1.0` | Font scale multiplier for main center word. |
| `AutoResizeBalloon` | `bool` | `true` | Auto-resize center balloon shape to fit word with generous safety padding. |
| `BalloonScale` | `double` | `1.35` | Center balloon backdrop scale multiplier (0.8x to 2.5x). |
| `BalloonPaddingX` | `double` | `40.0` | Horizontal internal padding. |
| `BalloonPaddingY` | `double` | `20.0` | Vertical internal padding. |
| `BalloonCustomWidth` | `double` | `0.0` | Manual fixed width override when AutoResizeBalloon is false. |
| `BalloonCustomHeight` | `double` | `0.0` | Manual fixed height override when AutoResizeBalloon is false. |
| `BalloonShape` | `WordBalloonShape` | `WordBalloonShape.PillCapsule` | Center balloon backdrop shape geometry. |
| `BalloonEffect` | `WordBalloonEffect` | `WordBalloonEffect.Bold` | Center word typography effect. |
| `BalloonTheme` | `WordBalloonTheme` | `WordBalloonTheme.IndigoViolet` | Color palette gradient theme. |
| `ShowSpeedSunburst` | `bool` | `false` | Render radial action speed rays background overlay. |
| `ShowHalftoneDots` | `bool` | `false` | Render vintage Ben-Day pop-art halftone dots background overlay. |
| `ShowActionSparks` | `bool` | `false` | Render action stars and sparkle glints overlay. |
| `ShowSpecularSheen` | `bool` | `false` | Render glossy specular highlight sheen overlay. |
| `ShowScanlineHud` | `bool` | `false` | Render cyber scanlines and corner HUD brackets overlay. |
| `OnWordClick` | `EventCallback<WordCloudItem>` | `-` | Fired when user clicks any keyword. |

#### Razor Usage Example

```razor
<TwWordCloud Words="_keywords" Title="Sentiment Keywords" SurroundingWordCount="12" BalloonShape="WordBalloonShape.SpeechBubble" OnWordClick="HandleWordClick" />
```

---

### `TwWordBalloonCreator`

**Location:** [`Components/UI/TwWordBalloonCreator.razor`](../Components/UI/TwWordBalloonCreator.razor)

Interactive Vector Word Balloon & Comic Banner Creator studio with 11 backdrop shapes, 9 typography effects, 9 color palettes, surrounding words count/scale sliders, visual graphic overlays (speed sunburst rays, Ben-Day halftone dots, action sparks, specular sheen, cyber scanlines, kicker ribbons), and 1-click clean normal balloon mode.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Class` | `string` | `""` | Additional CSS class names. |

#### Razor Usage Example

```razor
<TwWordBalloonCreator />
```

---

### `TwSankeyChart`

**Location:** [`Components/UI/TwSankeyChart.razor`](../Components/UI/TwSankeyChart.razor)

Pure SVG flow and migration diagram rendering source/target nodes and cubic bezier ribbons with thickness proportional to flow volume.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Nodes` | `IReadOnlyList<SankeyNode>` | `[]` | Nodes collection with Id, Label, and Color. |
| `Links` | `IReadOnlyList<SankeyLink>` | `[]` | Ribbon links collection with SourceId, TargetId, and Value. |
| `Title` | `string?` | `"Enterprise Pipeline & Resource Flow"` | Chart header title. |
| `Width` | `int` | `700` | SVG viewBox width. |
| `Height` | `int` | `360` | SVG viewBox height. |
| `OnNodeClick` | `EventCallback<SankeyNode>` | `-` | Callback invoked when a node is clicked. |

#### Razor Usage Example

```razor
<TwSankeyChart Nodes="_nodes" Links="_links" Title="Pipeline Throughput Flow" />
```

---

### `TwFluidMesh`

**Location:** [`Components/UI/TwFluidMesh.razor`](../Components/UI/TwFluidMesh.razor)

Tactile generative fluid gradient shader with organic motion and real-time psychological emotional state resonance tuning for operator cognitive optimization.

#### Parameters & Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Mood` | `EmotionalState` | `EmotionalState.DeepFocus` | Active emotional and cognitive resonance frequency preset. |
| `MoodChanged` | `EventCallback<EmotionalState>` | `-` | Fired when operator toggles sentiment mood. |

#### Razor Usage Example

```razor
<TwFluidMesh @bind-Mood="_operatorMood" />
```

---
