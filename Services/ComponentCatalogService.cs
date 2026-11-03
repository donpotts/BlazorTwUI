// Auto-generated Component Catalog Service for all BlazorTwUI components
using System.Collections.Generic;
using System.Linq;
using BlazorTwUI.Models;

namespace BlazorTwUI.Services;

public class ComponentCatalogService
{
    private static readonly List<ComponentDocItem> _items = new()
    {
        new ComponentDocItem
        {
            Name = "TwAgentConsole",
            Category = "Agent Ops & Human-in-the-Loop",
            Description = "Complete human-in-the-loop agent workspace: roster, streaming conversation with tool-call cards, an approval gate that genuinely blocks high-risk calls, takeover and hand-back controls, an editable shared-state inspector and an exportable audit trail. Drives any IAgentTransport; ships with an offline simulated refund-desk agent.",
            RazorSnippet = "<TwAgentConsole />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes."),
                new ComponentParameterDoc("Agents", "IReadOnlyList<AgentProfile>?", "one demo agent", "Agents shown in the roster."),
                new ComponentParameterDoc("Session", "AgentRunSession", "n/a (read-only property)", "The live session, for host pages that read status or drive it.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwApprovalGate",
            Category = "Agent Ops & Human-in-the-Loop",
            Description = "Blocking approval card for pending high-risk tool calls. The agent does not execute the call until a human approves, optionally with edited JSON arguments (validated), or rejects with a reason. Announces itself to screen readers via an assertive live region.",
            RazorSnippet = "<TwApprovalGate Session=\"_session\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Session", "AgentRunSession", "required", "Session whose pending approvals are rendered."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwToolCallCard",
            Category = "Agent Ops & Human-in-the-Loop",
            Description = "Collapsible card for one agent tool call showing streamed arguments, result, duration, risk level and a text status (never colour alone). Pending-approval calls are visually emphasised.",
            RazorSnippet = "<TwToolCallCard Call=\"toolCall\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Call", "ToolCallRecord", "required", "The tool call to render."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwTakeoverBar",
            Category = "Agent Ops & Human-in-the-Loop",
            Description = "Run status strip with Take over, Hand back and Stop controls. Takeover pauses the agent before its next step so a human can inspect and edit shared state; hand-back resumes it.",
            RazorSnippet = "<TwTakeoverBar Session=\"_session\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Session", "AgentRunSession", "required", "Session to display and control."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwAgentStateInspector",
            Category = "Agent Ops & Human-in-the-Loop",
            Description = "Live view of the state the agent shares with the UI. Changed keys are highlighted; values become editable while the human is in control, and every edit is audited and visible to the agent's next action.",
            RazorSnippet = "<TwAgentStateInspector Session=\"_session\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Session", "AgentRunSession", "required", "Session whose shared state is shown."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwAuditTrail",
            Category = "Agent Ops & Human-in-the-Loop",
            Description = "Append-only, filterable log of who did what (human, agent, system) with a one-click JSON export for compliance review.",
            RazorSnippet = "<TwAuditTrail Session=\"_session\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Session", "AgentRunSession", "required", "Session whose audit events are shown."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwAgentRoster",
            Category = "Agent Ops & Human-in-the-Loop",
            Description = "Accessible list of available agents (nav with aria-current) showing role, description and the selected agent's run status.",
            RazorSnippet = "<TwAgentRoster Agents=\"_agents\" SelectedId=\"@_selected\" OnSelect=\"a => _selected = a.Id\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Agents", "IReadOnlyList<AgentProfile>", "required", "Agents to list."),
                new ComponentParameterDoc("SelectedId", "string?", "null", "Id of the selected agent."),
                new ComponentParameterDoc("StatusText", "string", "\"Idle\"", "Run status shown on the selected agent."),
                new ComponentParameterDoc("OnSelect", "EventCallback<AgentProfile>", "null", "Fired when an agent is chosen."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwThreadList",
            Category = "Agent Ops & Human-in-the-Loop",
            Description = "Conversation thread sidebar with search, new, rename, pin, archive and delete, backed by IAgentThreadStore (persisted to localStorage).",
            RazorSnippet = "<TwThreadList Store=\"_threads\" SelectedId=\"@_threadId\" OnSelect=\"t => _threadId = t.Id\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Store", "IAgentThreadStore", "required", "Thread store to list and edit."),
                new ComponentParameterDoc("SelectedId", "string?", "null", "Id of the open thread."),
                new ComponentParameterDoc("OnSelect", "EventCallback<AgentThread>", "null", "Fired when a thread is opened."),
                new ComponentParameterDoc("OnCreated", "EventCallback<AgentThread>", "null", "Fired after New."),
                new ComponentParameterDoc("OnDeleted", "EventCallback<AgentThread>", "null", "Fired after a thread is deleted."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwDryRunChanges",
            Category = "Agent Ops & Human-in-the-Loop",
            Description = "Before/after preview of what an action would change, shown before the human approves it.",
            RazorSnippet = "<TwDryRunChanges Changes=\"_changes\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Changes", "IReadOnlyList<DryRunChange>", "required", "Add / Change / Remove rows with Target, Before and After."),
                new ComponentParameterDoc("Title", "string", "\"If you approve, this will change:\"", "Heading."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Extra content below the list."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwActivityLog",
            Category = "Agent Ops & Human-in-the-Loop",
            Description = "Timestamped log of what the agent did on its machine (navigate, click, type, command), announced politely to assistive tech.",
            RazorSnippet = "<TwActivityLog Entries=\"_entries\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Entries", "IReadOnlyList<ActivityEntry>", "required", "Events: AtUtc, Kind, Text, Detail."),
                new ComponentParameterDoc("Label", "string", "\"Agent activity\"", "Region heading."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwCommandOutput",
            Category = "Agent Ops & Human-in-the-Loop",
            Description = "Terminal-style command result with exit code, tail truncation and automatic credential masking.",
            RazorSnippet = "<TwCommandOutput Command=\"dotnet test\" Output=\"@_out\" ExitCode=\"0\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Command", "string", "required", "Command line that was run."),
                new ComponentParameterDoc("Output", "string", "\"\"", "Captured output."),
                new ComponentParameterDoc("ExitCode", "int", "0", "Process exit code."),
                new ComponentParameterDoc("MaxLines", "int", "200", "Keep only the last N lines."),
                new ComponentParameterDoc("Redact", "bool", "true", "Mask passwords, tokens and URL credentials."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwHandoffPanel",
            Category = "Agent Ops & Human-in-the-Loop",
            Description = "Alert card where a stuck agent asks a human to take the wheel, then hand the task back.",
            RazorSnippet = "<TwHandoffPanel AgentName=\"Refund Desk\" Reason=\"A CAPTCHA blocks the portal.\" OnTakeOver=\"Take\" OnHandBack=\"GiveBack\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("AgentName", "string", "\"The agent\"", "Who is asking."),
                new ComponentParameterDoc("Reason", "string", "required", "Why the agent is stuck."),
                new ComponentParameterDoc("Where", "string?", "null", "Page or step it is stuck at."),
                new ComponentParameterDoc("Resolved", "bool", "false", "Show the resolved state."),
                new ComponentParameterDoc("OnTakeOver", "EventCallback", "null", "Human took control."),
                new ComponentParameterDoc("OnHandBack", "EventCallback", "null", "Human returned control."),
                new ComponentParameterDoc("OnDecline", "EventCallback", "null", "Human stopped the task."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwRoutinesList",
            Category = "Agent Ops & Human-in-the-Loop",
            Description = "Recurring agent jobs with schedule, next run, run-now and an accessible on/off switch.",
            RazorSnippet = "<TwRoutinesList Items=\"_routines\" OnToggled=\"Save\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Items", "List<Routine>", "required", "Routines to show."),
                new ComponentParameterDoc("OnToggled", "EventCallback<Routine>", "null", "Enabled state changed."),
                new ComponentParameterDoc("OnRunNow", "EventCallback<Routine>", "null", "Run now pressed."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwMemoryList",
            Category = "Agent Ops & Human-in-the-Loop",
            Description = "What the agent remembers, with source and date, searchable, each with a Forget action.",
            RazorSnippet = "<TwMemoryList Items=\"_memories\" OnForget=\"Forget\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Items", "List<Memory>", "required", "Remembered facts."),
                new ComponentParameterDoc("OnForget", "EventCallback<Memory>", "null", "A memory was forgotten."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwGrantList",
            Category = "Agent Ops & Human-in-the-Loop",
            Description = "Table of tools an agent may use, each with an access level (Ask / Read / Write) and Revoke.",
            RazorSnippet = "<TwGrantList Items=\"_grants\" OnChanged=\"Save\" OnRevoked=\"Revoke\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Items", "List<Grant>", "required", "Granted tools and levels."),
                new ComponentParameterDoc("OnChanged", "EventCallback<Grant>", "null", "Access level changed."),
                new ComponentParameterDoc("OnRevoked", "EventCallback<Grant>", "null", "Grant revoked."),
                new ComponentParameterDoc("Caption", "string", "\"Tools the agent may use\"", "Table caption for assistive tech."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwLiveScreen",
            Category = "Agent Ops & Human-in-the-Loop",
            Description = "The agent's screen with its pointer, a working/in-control badge and Take control; reports normalised click coordinates while you drive.",
            RazorSnippet = "<TwLiveScreen ImageUrl=\"@_frame\" Url=\"https://portal.test\" @bind-Controlling=\"_driving\" OnScreenClick=\"Click\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ImageUrl", "string?", "null", "Latest captured frame."),
                new ComponentParameterDoc("Url", "string?", "null", "Page the agent is on."),
                new ComponentParameterDoc("CursorX", "double?", "null", "Agent pointer X, 0..1."),
                new ComponentParameterDoc("CursorY", "double?", "null", "Agent pointer Y, 0..1."),
                new ComponentParameterDoc("Controlling", "bool", "false", "Human is in control."),
                new ComponentParameterDoc("OnScreenClick", "EventCallback<(double X,double Y)>", "null", "Click while controlling, as 0..1 fractions."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwAgentOrb",
            Category = "Agent Ops & Human-in-the-Loop",
            Description = "Animated agent avatar whose colour, ring and status dot follow the run state, with the state also exposed as text.",
            RazorSnippet = "<TwAgentOrb Name=\"Refund Desk\" Status=\"@_session.Status\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Name", "string", "\"Agent\"", "Agent name (initial and accessible name)."),
                new ComponentParameterDoc("Status", "AgentRunStatus", "Idle", "Run state."),
                new ComponentParameterDoc("Size", "string", "\"md\"", "sm, md or lg."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwRedactedValue",
            Category = "Agent Ops & Human-in-the-Loop",
            Description = "Shows agent arguments and results with credentials masked; a deliberate Reveal is needed to see the raw value.",
            RazorSnippet = "<TwRedactedValue Value=\"@call.ArgsJson\" Label=\"Arguments\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "string", "required", "Raw JSON or text."),
                new ComponentParameterDoc("Label", "string", "\"Value\"", "Heading."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwChatSuggestions",
            Category = "Chat & Generative UI",
            Description = "Suggested follow-up prompts as buttons, with a loading skeleton.",
            RazorSnippet = "<TwChatSuggestions Items=\"_ideas\" OnPick=\"s => _prompt = s\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Items", "IReadOnlyList<string>", "required", "Suggestion texts."),
                new ComponentParameterDoc("OnPick", "EventCallback<string>", "null", "A suggestion was chosen."),
                new ComponentParameterDoc("Loading", "bool", "false", "Show placeholders."),
                new ComponentParameterDoc("Disabled", "bool", "false", "Disable while a run is active."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwFeedbackButtons",
            Category = "Chat & Generative UI",
            Description = "Thumbs up / down on a response, toggling off on a second press, with an optional reason box after thumbs-down.",
            RazorSnippet = "<TwFeedbackButtons @bind-Value=\"_rating\" OnReason=\"SaveReason\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "FeedbackRating", "None", "None, Up or Down."),
                new ComponentParameterDoc("AskReason", "bool", "true", "Ask what went wrong after Down."),
                new ComponentParameterDoc("OnReason", "EventCallback<string>", "null", "Reason text committed."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwAttachmentQueue",
            Category = "Chat & Generative UI",
            Description = "Files staged on a message with count, size and duplicate validation and per-file removal.",
            RazorSnippet = "<TwAttachmentQueue @bind-Items=\"_files\" MaxFiles=\"5\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Items", "List<AttachmentInfo>", "[]", "Staged files."),
                new ComponentParameterDoc("MaxFiles", "int", "5", "Maximum number of files."),
                new ComponentParameterDoc("MaxBytes", "long", "5 MB", "Maximum size per file."),
                new ComponentParameterDoc("Accept", "string?", "null", "Accepted types."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwMessageScroller",
            Category = "Chat & Generative UI",
            Description = "Chat transcript container that follows new content and offers Jump to latest after you scroll up.",
            RazorSnippet = "<TwMessageScroller Height=\"24rem\">...messages...</TwMessageScroller>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Messages."),
                new ComponentParameterDoc("Label", "string", "\"Conversation\"", "Log region name."),
                new ComponentParameterDoc("Height", "string?", "null", "CSS max-height."),
                new ComponentParameterDoc("BodyClass", "string?", "null", "Classes for the scrolling body."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwVoiceInput",
            Category = "Chat & Generative UI",
            Description = "Push-to-talk microphone with a live level meter; reports the clip and can transcribe it through your own speech-to-text function.",
            RazorSnippet = "<TwVoiceInput Transcribe=\"SpeechToText\" OnTranscript=\"t => _prompt = t\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("OnRecorded", "EventCallback<VoiceClip>", "null", "Clip recorded."),
                new ComponentParameterDoc("Transcribe", "Func<VoiceClip, Task<string>>?", "null", "Speech-to-text function."),
                new ComponentParameterDoc("OnTranscript", "EventCallback<string>", "null", "Transcribed text."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwLiveWaveform",
            Category = "Chat & Generative UI",
            Description = "Bar level meter for live audio; maps any number of levels onto any number of bars.",
            RazorSnippet = "<TwLiveWaveform Levels=\"_levels\" Active=\"_recording\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Levels", "IReadOnlyList<float>", "[]", "Levels from 0 to 1."),
                new ComponentParameterDoc("Active", "bool", "false", "Live (true) or flat (false)."),
                new ComponentParameterDoc("Bars", "int", "24", "Number of bars."),
                new ComponentParameterDoc("AriaLabel", "string", "\"Audio level\"", "Accessible name."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwQuestionnaire",
            Category = "Chat & Generative UI",
            Description = "Structured questions an agent asks, one at a time with progress, single or multiple choice and optional free text.",
            RazorSnippet = "<TwQuestionnaire Questions=\"_questions\" OnSubmit=\"Answer\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Questions", "IReadOnlyList<Question>", "required", "Questions with options."),
                new ComponentParameterDoc("OnSubmit", "EventCallback<IReadOnlyDictionary<string, IReadOnlyList<string>>>", "null", "Answers by question id."),
                new ComponentParameterDoc("Title", "string", "\"Questions from the agent\"", "Region name."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwA2uiSurface",
            Category = "Chat & Generative UI",
            Description = "Renders an agent-described interface (A2UI v0.9): streamed components bound to a data model, with button actions returned to the app.",
            RazorSnippet = "<TwA2uiSurface Surface=\"_surface\" OnAction=\"HandleAction\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Surface", "A2uiSurface", "required", "Model fed with createSurface / updateComponents / updateDataModel messages."),
                new ComponentParameterDoc("OnAction", "EventCallback<A2uiUserAction>", "null", "Button action with resolved context."),
                new ComponentParameterDoc("Label", "string", "\"Agent-generated interface\"", "Region name."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwMcpAppFrame",
            Category = "Chat & Generative UI",
            Description = "Hosts an MCP app in a sandboxed iframe with an opaque origin, a locked-down CSP and a postMessage bridge.",
            RazorSnippet = "<TwMcpAppFrame Html=\"@_html\" AllowedDomains=\"_cdns\" OnMessage=\"FromApp\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Html", "string", "required", "Widget HTML."),
                new ComponentParameterDoc("Title", "string", "\"App\"", "Frame title."),
                new ComponentParameterDoc("Height", "string", "\"16rem\"", "Frame height."),
                new ComponentParameterDoc("AllowedDomains", "IReadOnlyList<string>?", "null", "Extra origins permitted by the CSP."),
                new ComponentParameterDoc("OnMessage", "EventCallback<string>", "null", "JSON the app posted to the host."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwSpinner",
            Category = "Buttons, Forms & Controls",
            Description = "Busy indicator with an announced label; stops spinning under reduced motion.",
            RazorSnippet = "<TwSpinner Label=\"Saving\" ShowLabel=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Size", "string", "\"md\"", "sm, md or lg."),
                new ComponentParameterDoc("Label", "string", "\"Loading\"", "Announced label."),
                new ComponentParameterDoc("ShowLabel", "bool", "false", "Show the label visibly."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwButtonGroup",
            Category = "Buttons, Forms & Controls",
            Description = "Joins buttons into one group with collapsed borders and rounded ends, horizontal or vertical.",
            RazorSnippet = "<TwButtonGroup Label=\"View\"><button>Day</button><button>Week</button></TwButtonGroup>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Buttons."),
                new ComponentParameterDoc("Label", "string?", "null", "Group name."),
                new ComponentParameterDoc("Vertical", "bool", "false", "Stack vertically."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwRadioGroup",
            Category = "Buttons, Forms & Controls",
            Description = "Generic native radio group with legend, option descriptions, disabled options and error text.",
            RazorSnippet = "<TwRadioGroup TValue=\"string\" Legend=\"Plan\" Items=\"_plans\" @bind-Value=\"_plan\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Items", "IReadOnlyList<Option>", "required", "Value, Label, Description, Disabled."),
                new ComponentParameterDoc("Value", "TValue?", "default", "Selected value."),
                new ComponentParameterDoc("Legend", "string?", "null", "Group label."),
                new ComponentParameterDoc("Error", "string?", "null", "Validation message."),
                new ComponentParameterDoc("Horizontal", "bool", "false", "Lay out in a row."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwTextArea",
            Category = "Buttons, Forms & Controls",
            Description = "Multi-line input with label, hint, error, character counter and auto-grow.",
            RazorSnippet = "<TwTextArea Label=\"Notes\" MaxLength=\"280\" AutoGrow=\"true\" @bind-Value=\"_notes\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "string?", "null", "Text."),
                new ComponentParameterDoc("Label", "string?", "null", "Label."),
                new ComponentParameterDoc("MaxLength", "int", "0", "Max characters (0 = none)."),
                new ComponentParameterDoc("AutoGrow", "bool", "false", "Grow between MinRows and MaxRows."),
                new ComponentParameterDoc("Error", "string?", "null", "Validation message."),
                new ComponentParameterDoc("Hint", "string?", "null", "Help text."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwCalendar",
            Category = "Advanced Forms & Inputs",
            Description = "Month calendar following the ARIA date-grid pattern: arrow / Home / End / PageUp / PageDown keys, min and max, culture-aware.",
            RazorSnippet = "<TwCalendar @bind-Value=\"_date\" Min=\"_from\" Max=\"_to\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "DateOnly?", "null", "Selected date."),
                new ComponentParameterDoc("Min", "DateOnly?", "null", "Earliest selectable date."),
                new ComponentParameterDoc("Max", "DateOnly?", "null", "Latest selectable date."),
                new ComponentParameterDoc("FirstDayOfWeek", "DayOfWeek", "Sunday", "First column."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwTimePicker",
            Category = "Advanced Forms & Inputs",
            Description = "Accessible hour / minute / AM-PM picker built from native selects, 12 or 24 hour, with a minute step.",
            RazorSnippet = "<TwTimePicker @bind-Value=\"_time\" MinuteStep=\"15\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "TimeOnly?", "null", "Selected time."),
                new ComponentParameterDoc("Use24Hour", "bool", "false", "24 hour clock."),
                new ComponentParameterDoc("MinuteStep", "int", "5", "Minute granularity (1-30)."),
                new ComponentParameterDoc("Label", "string?", "null", "Group label."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwScheduler",
            Category = "Scheduling & Planning",
            Description = "Day, Week and Month scheduler with overlap-aware columns, click-to-create slots, drag-and-drop moves and an Alt+arrow keyboard alternative.",
            RazorSnippet = "<TwScheduler Appointments=\"_appts\" @bind-Date=\"_date\" @bind-View=\"_view\" OnSlotClick=\"Create\" OnAppointmentMoved=\"Save\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Appointments", "IReadOnlyList<Appointment>", "[]", "Id, Title, Start, End, Color."),
                new ComponentParameterDoc("Date", "DateOnly", "today", "Focused date."),
                new ComponentParameterDoc("View", "SchedulerView", "Week", "Day, Week or Month."),
                new ComponentParameterDoc("StartHour", "int", "8", "First visible hour."),
                new ComponentParameterDoc("EndHour", "int", "18", "Last visible hour."),
                new ComponentParameterDoc("SlotMinutes", "int", "30", "Slot length."),
                new ComponentParameterDoc("FirstDayOfWeek", "DayOfWeek", "Monday", "First column."),
                new ComponentParameterDoc("OnSlotClick", "EventCallback<DateTime>", "null", "An empty slot was clicked."),
                new ComponentParameterDoc("OnAppointmentClick", "EventCallback<Appointment>", "null", "An appointment was clicked."),
                new ComponentParameterDoc("OnAppointmentMoved", "EventCallback<Appointment>", "null", "After a drag or keyboard move; the appointment carries its new times."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwGantt",
            Category = "Scheduling & Planning",
            Description = "Project timeline with a task table, day-scaled bars, progress fill, finish-to-start dependency arrows, a today line and conflict detection.",
            RazorSnippet = "<TwGantt Tasks=\"_tasks\" DayWidth=\"24\" OnTaskClick=\"Select\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Tasks", "IReadOnlyList<GanttTask>", "required", "Id, Name, Start, End, Progress (0-1), DependsOn."),
                new ComponentParameterDoc("DayWidth", "int", "28", "Pixels per day."),
                new ComponentParameterDoc("SelectedId", "string?", "null", "Selected task."),
                new ComponentParameterDoc("OnTaskClick", "EventCallback<GanttTask>", "null", "A bar was clicked."),
                new ComponentParameterDoc("Label", "string", "\"Project timeline\"", "Region name."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwTreeList",
            Category = "Enterprise Data & Tables",
            Description = "Hierarchical table (treegrid): nests rows by parent key, expand/collapse with Left/Right arrows, per-level sorting, single selection.",
            RazorSnippet = "<TwTreeList TItem=\"Dept\" Items=\"_depts\" KeySelector=\"d => d.Id\" ParentSelector=\"d => d.ParentId\" Columns=\"_cols\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Items", "IReadOnlyList<TItem>", "required", "Flat list with parent keys."),
                new ComponentParameterDoc("KeySelector", "Func<TItem,string>", "required", "Row key."),
                new ComponentParameterDoc("ParentSelector", "Func<TItem,string?>", "required", "Parent key (missing parents become roots)."),
                new ComponentParameterDoc("Columns", "IReadOnlyList<Column>", "required", "Header, Value, Sortable."),
                new ComponentParameterDoc("SelectedKey", "string?", "null", "Selected row."),
                new ComponentParameterDoc("InitiallyExpanded", "bool", "false", "Start expanded."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwToolbar",
            Category = "Layout & Application Shell",
            Description = "Command toolbar with a roving tab stop, toggle buttons, separators and an overflow menu for items past MaxVisible.",
            RazorSnippet = "<TwToolbar Items=\"_items\" MaxVisible=\"5\" OnItemClick=\"Run\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Items", "IReadOnlyList<ToolbarItem>", "required", "Button, Toggle or Separator items."),
                new ComponentParameterDoc("MaxVisible", "int", "0", "Items beyond this go into the More menu (0 = all)."),
                new ComponentParameterDoc("OnItemClick", "EventCallback<ToolbarItem>", "null", "Item clicked."),
                new ComponentParameterDoc("Label", "string", "\"Toolbar\"", "Accessible name."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwWindow",
            Category = "Layout & Application Shell",
            Description = "Floating non-modal window: drag by the title bar or use arrow keys (Shift farther, Ctrl resizes); minimize, maximize, close; last touched stays on top.",
            RazorSnippet = "<TwWindow Title=\"Inspector\" @bind-Visible=\"_show\" X=\"120\" Y=\"90\">...</TwWindow>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "required", "Title and accessible name."),
                new ComponentParameterDoc("Visible", "bool", "true", "Shown or closed."),
                new ComponentParameterDoc("X", "int", "80", "Left in px."),
                new ComponentParameterDoc("Y", "int", "80", "Top in px."),
                new ComponentParameterDoc("Width", "int", "420", "Width in px."),
                new ComponentParameterDoc("Height", "int", "300", "Height in px."),
                new ComponentParameterDoc("OnMoved", "EventCallback<(int X,int Y)>", "null", "Position after a move."),
                new ComponentParameterDoc("CloseOnEscape", "bool", "true", "Escape closes."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwLoadingPanel",
            Category = "Layout & Application Shell",
            Description = "Wraps a region and covers it with a busy overlay; the region is aria-busy and its content is inert while loading.",
            RazorSnippet = "<TwLoadingPanel Visible=\"_busy\" Message=\"Saving…\">...</TwLoadingPanel>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Visible", "bool", "false", "Show the overlay."),
                new ComponentParameterDoc("Message", "string", "\"Loading…\"", "Announced text."),
                new ComponentParameterDoc("Blur", "bool", "true", "Soften the covered content."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwStackLayout",
            Category = "Layout & Application Shell",
            Description = "One-dimensional flex layout with named gap, alignment, justification, wrapping and reverse order.",
            RazorSnippet = "<TwStackLayout Horizontal=\"true\" Gap=\"4\" Justify=\"between\">...</TwStackLayout>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Horizontal", "bool", "false", "Row instead of column."),
                new ComponentParameterDoc("Gap", "int", "3", "Step 0-6."),
                new ComponentParameterDoc("Align", "string", "\"stretch\"", "start, center, end, stretch, baseline."),
                new ComponentParameterDoc("Justify", "string", "\"start\"", "start, center, end, between, around, evenly."),
                new ComponentParameterDoc("Wrap", "bool", "false", "Wrap items."),
                new ComponentParameterDoc("Reverse", "bool", "false", "Reverse order."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwFormLayout",
            Category = "Layout & Application Shell",
            Description = "Responsive form grid: one column on phones, up to four from the lg breakpoint. Holds TwFormGroup and TwFormItem.",
            RazorSnippet = "<TwFormLayout Columns=\"2\">...</TwFormLayout>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Columns", "int", "2", "1 to 4."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Groups and items."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwFormGroup",
            Category = "Layout & Application Shell",
            Description = "Titled, optionally collapsible section of a form that spans the full layout width.",
            RazorSnippet = "<TwFormGroup Title=\"Billing\" Collapsible=\"true\">...</TwFormGroup>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "required", "Group title."),
                new ComponentParameterDoc("Collapsible", "bool", "false", "Show a toggle."),
                new ComponentParameterDoc("InitiallyCollapsed", "bool", "false", "Start collapsed."),
                new ComponentParameterDoc("Columns", "int", "2", "Inner columns."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwFormItem",
            Category = "Layout & Application Shell",
            Description = "A label, control, hint and error wired together: label points at the field, required is announced, errors use role=alert.",
            RazorSnippet = "<TwFormItem Label=\"Email\" FieldId=\"email\" Required=\"true\" Hint=\"We never share it\"><input id=\"email\" /></TwFormItem>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Label", "string", "required", "Label text."),
                new ComponentParameterDoc("FieldId", "string", "generated", "Id of the control inside."),
                new ComponentParameterDoc("Required", "bool", "false", "Mark required."),
                new ComponentParameterDoc("Hint", "string?", "null", "Help text."),
                new ComponentParameterDoc("Error", "string?", "null", "Validation message (replaces the hint)."),
                new ComponentParameterDoc("LabelBeside", "bool", "false", "Label beside the field on wide screens."),
                new ComponentParameterDoc("ColSpan", "int", "1", "Columns spanned, 1-4."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwListBox",
            Category = "Advanced Forms & Inputs",
            Description = "Searchable list with checkboxes or radios, Select all (applies to the filtered view) and a live selection count.",
            RazorSnippet = "<TwListBox TItem=\"string\" Items=\"_names\" @bind-Selected=\"_picked\" Label=\"Teams\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Items", "IReadOnlyList<TItem>", "required", "Items to list."),
                new ComponentParameterDoc("TextSelector", "Func<TItem,string>?", "ToString", "Display text."),
                new ComponentParameterDoc("Selected", "IReadOnlyList<TItem>", "[]", "Selected items."),
                new ComponentParameterDoc("Multiple", "bool", "true", "Checkboxes (true) or radios (false)."),
                new ComponentParameterDoc("Label", "string", "\"Items\"", "Region name."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwDropDownBox",
            Category = "Advanced Forms & Inputs",
            Description = "A field that opens a panel hosting any content (a grid, tree or form); closes on Escape, outside click or from the content.",
            RazorSnippet = "<TwDropDownBox @bind-Open=\"_open\" Text=\"@_choice\">@context ...</TwDropDownBox>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ChildContent", "RenderFragment<TwDropDownBox>", "empty", "Panel content; the component is the context so it can call CloseAsync()."),
                new ComponentParameterDoc("Text", "string?", "null", "Shown in the field."),
                new ComponentParameterDoc("Placeholder", "string", "\"Select…\"", "Shown when Text is empty."),
                new ComponentParameterDoc("Open", "bool", "false", "Panel open."),
                new ComponentParameterDoc("Disabled", "bool", "false", "Disable the field."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwHtmlEditor",
            Category = "Advanced Forms & Inputs",
            Description = "Rich-text editor: bold, italic, underline, headings, lists, quotes and links. Output is rebuilt from an allowlist in the browser and pasted text is plain.",
            RazorSnippet = "<TwHtmlEditor @bind-Value=\"_html\" Label=\"Message\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "string?", "null", "Sanitized HTML."),
                new ComponentParameterDoc("Label", "string", "\"Rich text\"", "Accessible name."),
                new ComponentParameterDoc("MinHeight", "string", "\"10rem\"", "Editing area height."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwRangeSelector",
            Category = "Advanced Forms & Inputs",
            Description = "Pick a window over a series with an area preview and two range inputs that cannot cross; reports both indices.",
            RazorSnippet = "<TwRangeSelector Values=\"_series\" Labels=\"_months\" @bind-Start=\"_from\" @bind-End=\"_to\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Values", "IReadOnlyList<double>", "required", "Series for the preview."),
                new ComponentParameterDoc("Labels", "IReadOnlyList<string>?", "null", "Point labels."),
                new ComponentParameterDoc("Start", "int", "0", "First index."),
                new ComponentParameterDoc("End", "int", "last", "Last index."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwBarGauge",
            Category = "Visualizations & Gauges",
            Description = "Concentric 270-degree bars, one per value, with a legend carrying the exact numbers.",
            RazorSnippet = "<TwBarGauge Items=\"_items\" Unit=\"%\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Items", "IReadOnlyList<GaugeItem>", "required", "Label and Value."),
                new ComponentParameterDoc("Min", "double", "0", "Scale minimum."),
                new ComponentParameterDoc("Max", "double", "100", "Scale maximum."),
                new ComponentParameterDoc("Unit", "string", "\"\"", "Suffix shown in the legend."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwMfaAuthSuite",
            Category = "Security & Compliance",
            Description = "Enterprise-grade multi-factor authentication suite supporting TOTP authenticator app QR setup, SMS/Email 6-digit OTP with auto-advancing segmented inputs, FIDO2 / WebAuthn biometric passkeys, and single-use emergency recovery codes.",
            RazorSnippet = "<TwMfaAuthSuite OnEnrollmentChanged=\"(isEnrolled) => { /* Handle state */ }\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes."),
                new ComponentParameterDoc("OnEnrollmentChanged", "EventCallback<bool>", "null", "Callback fired when user enrolls or updates 2FA status.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwPrizeWheel",
            Category = "Gamification & Celebrations",
            Description = "Interactive SVG spinning fortune prize wheel with realistic deceleration physics, top pointer tick indicator, customizable prize wedges, celebratory confetti burst, and winner announcement banner for team morale boosts and party giveaways.",
            RazorSnippet = "<TwPrizeWheel OnPrizeWon=\"(prize) => { /* Handle prize */ }\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes."),
                new ComponentParameterDoc("Title", "string", "\"Spin to Win!\"", "Component header title."),
                new ComponentParameterDoc("Prizes", "List<PrizeItem>", "Default 8 wedges", "List of prize wedges with titles, emojis, colors, and descriptions (inspired by MyWheel)."),
                new ComponentParameterDoc("OnPrizeWon", "EventCallback<PrizeItem>", "null", "Fired upon completion of deceleration on the winning wedge.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwCelebrationEditor",
            Category = "Gamification & Celebrations",
            Description = "Visual workbench for configuring canvas particle celebrations and confetti bursts. Sliders for particle count, velocity, spread angle, gravity, and origin height with instant live testing and copyable C#/Razor markup.",
            RazorSnippet = "<TwCelebrationEditor />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwRewardsCenter",
            Category = "Enterprise & Workplace Operations",
            Description = "Employee perks and recognition rewards catalog with category filtering (Gift Cards, Swag, Workplace Flexibility, Learning, Wellness), points wallet ledger, search bar, and digital voucher redemption modal.",
            RazorSnippet = "<TwRewardsCenter />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwScrollCraft",
            Category = "Animation & Micro-Interactions",
            Description = "Scroll-driven object animation container with parallax depth layers, focal zone reading fades, and pinned illustration scrubbers linked to viewport progress.",
            RazorSnippet = "<TwScrollCraft Title=\"Content Scroll Scrubbing\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"Content Scroll Scrubbing\"", "Stage title."),
                new ComponentParameterDoc("Steps", "List<ScrollStep>", "Default 4 steps", "List of steps with title, description, badge, and icon."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwMediaRecorder",
            Category = "Hardware & Media Interop",
            Description = "In-browser microphone and HD webcam media recorder with live SVG audio frequency visualizer, recording status pulse, VU meters, playback preview via TwMediaPlayer, and local file download.",
            RazorSnippet = "<TwMediaRecorder OnRecordingCompleted=\"(duration) => { /* Handle duration */ }\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes."),
                new ComponentParameterDoc("OnRecordingCompleted", "EventCallback<int>", "null", "Fired when user completes a recording with the elapsed seconds.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwFinancialChart",
            Category = "Financial & Analytical Intelligence",
            Description = "High-frequency Candlestick and Area financial charting engine with real-time volume histogram, 20-period moving average line, crosshair hover HUD inspector, timeframe range presets (1W, 1M, 3M, YTD, 1Y, ALL), and zero-JS vector SVG rendering.",
            RazorSnippet = "<TwFinancialChart Ticker=\"BLZR\" CompanyName=\"Blazor Technologies Inc.\" Exchange=\"NASDAQ\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Ticker", "string", "\"BLZR\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("CompanyName", "string", "\"Blazor Technologies Inc.\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Exchange", "string", "\"NASDAQ\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Data", "List<FinancialCandle>?", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwTreemap",
            Category = "Financial & Analytical Intelligence",
            Description = "Multi-dimensional area treemap and valuation heatmap visualizing hierarchical assets, portfolio market caps, and performance returns with squarified 2D tiling, dynamic drill-down breadcrumb, and color ramps.",
            RazorSnippet = "<TwTreemap Title=\"Market Valuation Heatmap & Treemap\" Subtitle=\"Hierarchical squarified area map of enterprise assets and percentage price shifts\" CustomClass=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"Market Valuation Heatmap & Treemap\"", "Header title or accessible label."),
                new ComponentParameterDoc("Subtitle", "string", "\"Hierarchical squarified area map of enterprise assets and percentage price shifts\"", "Header title or accessible label."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Items", "List<TreemapItem>?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnItemClick", "EventCallback<TreemapItem>", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwPivotGrid",
            Category = "Financial & Analytical Intelligence",
            Description = "Enterprise OLAP cross-tabulation matrix supporting dynamic row/column field pivoting, value aggregations (Sum, Average, Count), interactive field drop drawer, and hierarchical subtotal and grand total calculations.",
            RazorSnippet = "<TwPivotGrid Title=\"Enterprise Financial Pivot Grid\" Subtitle=\"Multi-dimensional cross-tabulation matrix with real-time grouping & aggregation\" CustomClass=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"Enterprise Financial Pivot Grid\"", "Header title or accessible label."),
                new ComponentParameterDoc("Subtitle", "string", "\"Multi-dimensional cross-tabulation matrix with real-time grouping & aggregation\"", "Header title or accessible label."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Records", "List<PivotRecord>?", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwIconPicker",
            Category = "Developer Studio & Content Creation",
            Description = "Interactive search-and-select icon & emoji explorer with live filtering, outline/solid toggles, size previews, and one-click Razor component snippet copying.",
            RazorSnippet = "<TwIconPicker Title=\"Open Source Vector Icons & Emoji Studio\" Subtitle=\"Zero-dependency pure C# WASM vector icons and emojis for enterprise dashboards and forms\" CustomClass=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"Open Source Vector Icons & Emoji Studio\"", "Header title or accessible label."),
                new ComponentParameterDoc("Subtitle", "string", "\"Zero-dependency pure C# WASM vector icons and emojis for enterprise dashboards and forms\"", "Header title or accessible label."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwIcon",
            Category = "Developer Studio & Content Creation",
            Description = "Zero-dependency pure C# WASM inline vector SVG icon engine supporting 100+ curated icons (Heroicons & Lucide paths), customizable sizes, stroke widths, outline/solid modes, and semantic color inheritance.",
            RazorSnippet = "<TwIcon Name=\"sparkles\" Size=\"ComponentSize.Md\" StrokeWidth=\"1.75\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Name", "string", "\"sparkles\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Size", "ComponentSize", "ComponentSize.Md", "Component sizing scale (Xs, Sm, Md, Lg)."),
                new ComponentParameterDoc("IsSolid", "bool", "false", "Configures component behavior or styling."),
                new ComponentParameterDoc("StrokeWidth", "double", "1.75", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string", "\"\"", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwEmoji",
            Category = "Developer Studio & Content Creation",
            Description = "Modern emoji component with shortcode mapping (:rocket:, :fire:, :sparkles:), Unicode glyph parsing, hover scale physics, and click-to-copy clipboard integration.",
            RazorSnippet = "<TwEmoji Size=\"ComponentSize.Md\" AnimateOnHover=\"true\" Class=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Shortcode", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Glyph", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Size", "ComponentSize", "ComponentSize.Md", "Component sizing scale (Xs, Sm, Md, Lg)."),
                new ComponentParameterDoc("AnimateOnHover", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("CopyOnClick", "bool", "false", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Class", "string", "\"\"", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwMarkdownEditor",
            Category = "Developer Studio & Content Creation",
            Description = "Enterprise-grade dual Markdown and HTML editor featuring live pure C# WASM typography preview, split/edit/preview modes, toolbar actions (headings, lists, bold, italics, code, tables), fullscreen Zen focus, and media drawer supporting image, video, audio, and link embedding.",
            RazorSnippet = "<TwMarkdownEditor Value=\"\" CustomClass=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("Value", "string", "\"\"", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("ValueChanged", "EventCallback<string>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwDockManager",
            Category = "Developer Studio & Content Creation",
            Description = "Desktop-grade multi-pane docking studio layout engine featuring Solution Explorer tree, multi-tab central document workspace, bottom diagnostic terminal output, and collapsible properties inspector.",
            RazorSnippet = "<TwDockManager CustomClass=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwSpreadsheet",
            Category = "Developer Studio & Content Creation",
            Description = "Spreadsheet calculation engine with alphanumeric grid coordinates (A1..E10), formula bar, formula evaluation (=SUM, =AVG, arithmetic), direct cell inline editing, and formatted total readouts.",
            RazorSnippet = "<TwSpreadsheet Title=\"Enterprise Spreadsheet Matrix\" Subtitle=\"Excel-compatible cell matrix with live arithmetic, range aggregations, and formula bar\" CustomClass=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"Enterprise Spreadsheet Matrix\"", "Header title or accessible label."),
                new ComponentParameterDoc("Subtitle", "string", "\"Excel-compatible cell matrix with live arithmetic, range aggregations, and formula bar\"", "Header title or accessible label."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwOrgChart",
            Category = "Developer Studio & Content Creation",
            Description = "Hierarchical organization tree visualizer with pure vector SVG orthogonal connector lines, search highlight filter, recursive branch expand/collapse toggles, and A-/A+ canvas zoom controls.",
            RazorSnippet = "<TwOrgChart Title=\"Enterprise Organization Tree\" Subtitle=\"Executive leadership structure, reporting hierarchies, and departmental clusters\" CustomClass=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"Enterprise Organization Tree\"", "Header title or accessible label."),
                new ComponentParameterDoc("Subtitle", "string", "\"Executive leadership structure, reporting hierarchies, and departmental clusters\"", "Header title or accessible label."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Root", "OrgNode?", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwUserRegistrationForm",
            Category = "Task-Purpose Enterprise Forms",
            Description = "Turnkey enterprise user registration form featuring full name, username, corporate email, role selector, real-time password strength meter, terms consent, and federated SSO options.",
            RazorSnippet = "<TwUserRegistrationForm Title=\"Create Your Account\" Subtitle=\"Deploy enterprise WebAssembly apps with instant Tailwind CSS parity\" ShowSocialLogins=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"Create Your Account\"", "Header title or accessible label."),
                new ComponentParameterDoc("Subtitle", "string", "\"Deploy enterprise WebAssembly apps with instant Tailwind CSS parity\"", "Header title or accessible label."),
                new ComponentParameterDoc("ShowSocialLogins", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnRegistered", "EventCallback<RegistrationModel>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnSsoRequested", "EventCallback<string>", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwUserEditForm",
            Category = "Task-Purpose Enterprise Forms",
            Description = "Comprehensive profile and security management form featuring interactive avatar selection, corporate role attributes, 2FA toggle, session management, and audited account deactivation.",
            RazorSnippet = "<TwUserEditForm CustomClass=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Model", "UserProfileModel?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnSaved", "EventCallback<UserProfileModel>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnDeactivate", "EventCallback", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwLoginForm",
            Category = "Task-Purpose Enterprise Forms",
            Description = "Split-screen enterprise authentication form featuring curated Unsplash architectural imagery, inspiring technical quotes, federated SSO buttons, password visibility toggle, and remember-me persistence.",
            RazorSnippet = "<TwLoginForm Title=\"Sign in to Enterprise Hub\" Subtitle=\"Access mission-critical dashboards, design tokens, and components\" ImageUrl=\"https://images.unsplash.com/photo-1517694712202-14dd9538aa97?auto=format&fit=crop&w=1200&q=80\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"Sign in to Enterprise Hub\"", "Header title or accessible label."),
                new ComponentParameterDoc("Subtitle", "string", "\"Access mission-critical dashboards, design tokens, and components\"", "Header title or accessible label."),
                new ComponentParameterDoc("ImageUrl", "string", "\"https://images.unsplash.com/photo-1517694712202-14dd9538aa97?auto=format&fit=crop&w=1200&q=80\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("QuoteText", "string", "\"Simplicity is prerequisite for reliability.\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("QuoteAuthor", "string", "\"Edsger W. Dijkstra\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("QuoteCitation", "string", "\"Turing Award Laureate\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowSocialLogins", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("AllowRememberMe", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnLogin", "EventCallback<LoginCredentials>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnSsoLogin", "EventCallback<string>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnForgotPassword", "EventCallback", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwTermsOfUseForm",
            Category = "Task-Purpose Enterprise Forms",
            Description = "Executive legal terms agreement viewer with section jump pills, PDF export, review progress tracking, signatory name/company binding, and tamper-evident SHA-256 hash stamp.",
            RazorSnippet = "<TwTermsOfUseForm Title=\"Enterprise Terms of Service\" Version=\"v3.2\" LastUpdated=\"October 2026\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"Enterprise Terms of Service\"", "Header title or accessible label."),
                new ComponentParameterDoc("Version", "string", "\"v3.2\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("LastUpdated", "string", "\"October 2026\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnAccepted", "EventCallback<TermsAcceptanceModel>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnDeclined", "EventCallback", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwCookieConsent",
            Category = "Task-Purpose Enterprise Forms",
            Description = "GDPR and CCPA compliant cookie preference manager featuring strictly necessary lock, customizable functional/telemetry/marketing toggles, and instant preferences persistence.",
            RazorSnippet = "<TwCookieConsent Title=\"Enterprise Cookie & Privacy Settings\" Description=\"We use minimal first-party cookies to secure user sessions, maintain UI state, and measure performance latency. We never sell your personal data.\" CustomClass=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"Enterprise Cookie & Privacy Settings\"", "Header title or accessible label."),
                new ComponentParameterDoc("Description", "string", "\"We use minimal first-party cookies to secure user sessions, maintain UI state, and measure performance latency. We never sell your personal data.\"", "Secondary descriptive caption."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnSaved", "EventCallback<CookiePreferencesModel>", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwPrivacyPolicyForm",
            Category = "Task-Purpose Enterprise Forms",
            Description = "Enterprise privacy charter featuring zero-CDN data guarantees and an automated self-service Data Subject Request (DSR) portal for machine-readable JSON/ZIP exports and audited account erasure.",
            RazorSnippet = "<TwPrivacyPolicyForm Title=\"Enterprise Privacy & Data Governance\" CustomClass=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"Enterprise Privacy & Data Governance\"", "Header title or accessible label."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnExportRequested", "EventCallback<string>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnErasureRequested", "EventCallback", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwAboutMissionForm",
            Category = "Task-Purpose Enterprise Forms",
            Description = "Enterprise vision and architectural charter banner with live performance metrics, engineering pillars, leadership team cards, and an interactive executive consultation inquiry form.",
            RazorSnippet = "<TwAboutMissionForm MissionHeading=\"Empowering Modern Enterprises with Zero-Latency Blazor UI\" MissionStatement=\"Our mission is to build the world's most performant, secure, and developer-delightful component library for .NET 10+ WebAssembly, bridging modern Tailwind CSS agility with enterprise-grade reliability and zero third-party telemetry.\" CustomClass=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("MissionHeading", "string", "\"Empowering Modern Enterprises with Zero-Latency Blazor UI\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("MissionStatement", "string", "\"Our mission is to build the world's most performant, secure, and developer-delightful component library for .NET 10+ WebAssembly, bridging modern Tailwind CSS agility with enterprise-grade reliability and zero third-party telemetry.\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnInquirySubmitted", "EventCallback<ContactInquiryModel>", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwPledgeForm",
            Category = "Task-Purpose Enterprise Forms",
            Description = "Official Code of Conduct, security compliance, and supply chain integrity pledge form with three sworn adherence principles, signatory details, and digital signature capture.",
            RazorSnippet = "<TwPledgeForm Title=\"Enterprise Engineering & Ethics Pledge\" Subtitle=\"Legally affirmed compliance with software supply chain integrity and responsible AI\" CustomClass=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"Enterprise Engineering & Ethics Pledge\"", "Header title or accessible label."),
                new ComponentParameterDoc("Subtitle", "string", "\"Legally affirmed compliance with software supply chain integrity and responsible AI\"", "Header title or accessible label."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnPledgeAffirmed", "EventCallback<PledgeModel>", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwAppShell",
            Category = "Layout & Application Shell",
            Description = "The root application shell supporting four dynamic layout variants: Sidebar (mobile drawer + desktop sidebar), TopNav (horizontal bar with mobile sheet), CompactRail (icon rail), and Stacked (single column).",
            RazorSnippet = "<TwAppShell Layout=\"LayoutVariant.Sidebar\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Layout", "LayoutVariant", "LayoutVariant.Sidebar", "Configures component behavior or styling."),
                new ComponentParameterDoc("BrandContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("HeaderActions", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("SidebarContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("SidebarFooter", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("TopNavContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("RailContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("BottomNavContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwContainer",
            Category = "Layout & Application Shell",
            Description = "Responsive max-width container wrapper with automatic padding and breakpoint clamping (sm, md, lg, xl, 2xl, full).",
            RazorSnippet = "<TwContainer MaxWidth=\"7xl\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("MaxWidth", "string", "\"7xl\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwGrid",
            Category = "Layout & Application Shell",
            Description = "Mobile-first responsive grid system with configurable column distributions (1-2-3, 1-2-4, 2-3-4, etc.) and gap scales.",
            RazorSnippet = "<TwGrid Preset=\"1-2-3\" Gap=\"md\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("Preset", "string", "\"1-2-3\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Gap", "string", "\"md\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwSplitter",
            Category = "Layout & Application Shell",
            Description = "Enterprise resizable split layout container supporting horizontal and vertical orientations, draggable divider handle, and one-click collapse/reset buttons.",
            RazorSnippet = "<TwSplitter Orientation=\"SplitOrientation.Horizontal\" SplitPercentage=\"40\" MinPercentage=\"15\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Orientation", "SplitOrientation", "SplitOrientation.Horizontal", "Configures component behavior or styling."),
                new ComponentParameterDoc("SplitPercentage", "double", "40", "Configures component behavior or styling."),
                new ComponentParameterDoc("MinPercentage", "double", "15", "Configures component behavior or styling."),
                new ComponentParameterDoc("MaxPercentage", "double", "85", "Configures component behavior or styling."),
                new ComponentParameterDoc("MinHeight", "int", "280", "Configures component behavior or styling."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("PaneOne", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("PaneTwo", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("SplitPercentageChanged", "EventCallback<double>", "null", "Callback invoked when state changes or item is selected."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwBottomNav",
            Category = "Layout & Application Shell",
            Description = "Fixed application bottom navigation bar for touch devices with active state indicators, notification badges, and haptic tap styling.",
            RazorSnippet = "<TwBottomNav Items=\"Array.Empty<NavItem>()\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Items", "IReadOnlyList<NavItem>", "Array.Empty<NavItem>()", "Configures component behavior or styling."),
                new ComponentParameterDoc("ActiveId", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnSelect", "EventCallback<string>", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "ThemeProvider",
            Category = "Theme & Palette Customization",
            Description = "Cascading state provider managing light, dark, and system color mode preferences with localStorage synchronization and zero-FOUC initialization.",
            RazorSnippet = "<ThemeProvider>\n    <span>Content</span>\n</ThemeProvider>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
            }
        },
        new ComponentDocItem
        {
            Name = "ThemeSwitcher",
            Category = "Theme & Palette Customization",
            Description = "Segmented mode selector with accent swatches for quick switching between Light, Dark, and OS System appearance.",
            RazorSnippet = "<ThemeSwitcher ShowAccentPicker=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Compact", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowAccentPicker", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwThemeSelector",
            Category = "Theme & Palette Customization",
            Description = "Modern comprehensive color dropdown selector featuring preset palette swatches and 16M+ custom hex color picker integration.",
            RazorSnippet = "<TwThemeSelector Alignment=\"right\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Compact", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Alignment", "string", "\"right\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwThemeSwitcher",
            Category = "Theme & Palette Customization",
            Description = "Compact pill switcher for toggling between dark, light, and system themes.",
            RazorSnippet = "<TwThemeSwitcher ShowAccentPicker=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Compact", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowAccentPicker", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwColorPicker",
            Category = "Theme & Palette Customization",
            Description = "Interactive 16M+ hex and RGB color picker with preview swatch, hex validation, and opacity support.",
            RazorSnippet = "<TwColorPicker Value=\"#6366f1\" Label=\"Pick Accent Color\" ShowValueBadge=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "string", "\"#6366f1\"", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("ValueChanged", "EventCallback<string>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Label", "string?", "\"Pick Accent Color\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowValueBadge", "bool", "true", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("AllowCustom", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowCopy", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("Inline", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwCodeFontModifier",
            Category = "Code & Syntax Studio",
            Description = "Enterprise code editor toolbar with interactive font sizer (A-/A+), monospace font switcher (JetBrains, Fira Code, Cascadia), ligatures toggle, line height stepper, word wrap, and syntax theme presets.",
            RazorSnippet = "<TwCodeFontModifier Title=\"Code Editor & Font Toolbar\" FontSize=\"13\" FontFamily=\"'JetBrains Mono', monospace\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"Code Editor & Font Toolbar\"", "Header title or accessible label."),
                new ComponentParameterDoc("FontSize", "int", "13", "Component sizing scale (Xs, Sm, Md, Lg)."),
                new ComponentParameterDoc("FontSizeChanged", "EventCallback<int>", "null", "Component sizing scale (Xs, Sm, Md, Lg)."),
                new ComponentParameterDoc("FontFamily", "string", "\"'JetBrains Mono', monospace\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("LineHeight", "string", "\"leading-normal\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("EnableLigatures", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("WordWrap", "bool", "false", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowLineNumbers", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("ThemePreset", "string", "\"emerald\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("SampleCode", "string?", "@\"// High-performance Blazor Tailwind Engine\npublic sealed record ComponentSpec(string Name, string Category)\n{\n    public bool HasDarkModeSupport => true", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnSettingsChanged", "EventCallback<CodeFontSettings>", "null", "Callback invoked when state changes or item is selected."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwCodeTabs",
            Category = "Code & Syntax Studio",
            Description = "Multi-file tabbed code viewer with window controls, active file tabs, language icons, line numbers, and one-click clipboard copy.",
            RazorSnippet = "<TwCodeTabs Tabs=\"Array.Empty<CodeTab>()\" ShowLineNumbers=\"true\" AllowCopy=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Tabs", "IReadOnlyList<CodeTab>", "Array.Empty<CodeTab>()", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowLineNumbers", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("AllowCopy", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwCodeDiff",
            Category = "Code & Syntax Studio",
            Description = "Git-style visual code diff viewer highlighting additions and deletions with side-by-side line numbering, syntax color coding, and stats.",
            RazorSnippet = "<TwCodeDiff FileName=\"diff.razor\" Lines=\"Array.Empty<DiffLine>()\" AllowCopy=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("FileName", "string", "\"diff.razor\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Lines", "IReadOnlyList<DiffLine>", "Array.Empty<DiffLine>()", "Configures component behavior or styling."),
                new ComponentParameterDoc("OldCode", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("NewCode", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("AllowCopy", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwSnippet",
            Category = "Code & Syntax Studio",
            Description = "macOS terminal CLI command snippet container with prompt badges, window controls, and one-click copy transition.",
            RazorSnippet = "<TwSnippet Code=\"\" Prefix=\"$\" ShowHeader=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Code", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Prefix", "string", "\"$\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Title", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("ShowHeader", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowWindowControls", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowCopyButton", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwCodeBlock",
            Category = "Code & Syntax Studio",
            Description = "Syntax code viewer with language badges, line numbers, and copy-to-clipboard button.",
            RazorSnippet = "<TwCodeBlock Code=\"string.Empty\" Language=\"razor\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Code", "string", "string.Empty", "Configures component behavior or styling."),
                new ComponentParameterDoc("Language", "string?", "\"razor\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("FileName", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwFilterBuilder",
            Category = "Advanced Forms & Inputs",
            Description = "Visual query and filter builder supporting nested boolean AND/OR criteria trees, comparison operators, typed values, and LINQ/SQL predicate generation.",
            RazorSnippet = "<TwFilterBuilder Title=\"Visual Query & Filter Builder\" RootGroup=\"new()\" AvailableFields=\"new()\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"Visual Query & Filter Builder\"", "Header title or accessible label."),
                new ComponentParameterDoc("RootGroup", "FilterGroup", "new()", "Configures component behavior or styling."),
                new ComponentParameterDoc("AvailableFields", "List<FieldDefinition>", "new()", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowPreview", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnFilterChanged", "EventCallback<FilterGroup>", "null", "Callback invoked when state changes or item is selected."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwDropzone",
            Category = "Advanced Forms & Inputs",
            Description = "Enterprise multi-file drag-and-drop ingestion zone with animated focus border, file extension badges, individual upload progress bars, and queue management.",
            RazorSnippet = "<TwDropzone AllowMultiple=\"true\" AcceptedExtensions=\".png, .jpg, .pdf, .zip, .cs, .json\" MaxFileSizeMb=\"25\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("AllowMultiple", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("AcceptedExtensions", "string", "\".png, .jpg, .pdf, .zip, .cs, .json\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("MaxFileSizeMb", "int", "25", "Component sizing scale (Xs, Sm, Md, Lg)."),
                new ComponentParameterDoc("Files", "List<DropzoneFile>", "new()", "Configures component behavior or styling."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("FilesChanged", "EventCallback<List<DropzoneFile>>", "null", "Callback invoked when state changes or item is selected."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwOtpInput",
            Category = "Advanced Forms & Inputs",
            Description = "Segmented 6-digit one-time password / PIN verification input with auto-advance, backspace navigation, mobile numeric keypad, paste support, and dot masking.",
            RazorSnippet = "<TwOtpInput Length=\"6\" Value=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Length", "int", "6", "Configures component behavior or styling."),
                new ComponentParameterDoc("Value", "string", "\"\"", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("ValueChanged", "EventCallback<string>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("OnCompleted", "EventCallback<string>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Masked", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Disabled", "bool", "null", "Disables user interaction and applies muted opacity styling."),
                new ComponentParameterDoc("HasError", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ErrorMessage", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Label", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("HelperText", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwPasswordStrength",
            Category = "Advanced Forms & Inputs",
            Description = "Interactive password input with toggleable eye visibility, 4-stage color-coded entropy bar (Weak, Fair, Good, Strong), and real-time live requirements checklist.",
            RazorSnippet = "<TwPasswordStrength Value=\"\" Label=\"Password\" Placeholder=\"Enter a secure password...\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "string", "\"\"", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("ValueChanged", "EventCallback<string>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Label", "string", "\"Password\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Placeholder", "string", "\"Enter a secure password...\"", "Input placeholder hint."),
                new ComponentParameterDoc("ShowChecklist", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("Disabled", "bool", "null", "Disables user interaction and applies muted opacity styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwCombobox",
            Category = "Advanced Forms & Inputs",
            Description = "Searchable autocomplete dropdown with real-time filtering, keyboard arrow navigation, enter-to-select, clear button, and empty filter fallback.",
            RazorSnippet = "<TwCombobox Value=\"\" Items=\"Array.Empty<ComboboxItem>()\" Label=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "string", "\"\"", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("ValueChanged", "EventCallback<string>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Items", "IReadOnlyList<ComboboxItem>", "Array.Empty<ComboboxItem>()", "Configures component behavior or styling."),
                new ComponentParameterDoc("Label", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Placeholder", "string", "\"Select or search option...\"", "Input placeholder hint."),
                new ComponentParameterDoc("Disabled", "bool", "null", "Disables user interaction and applies muted opacity styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwTagsInput",
            Category = "Advanced Forms & Inputs",
            Description = "Interactive token tag creator that adds chips on Enter/Comma, supports backspace removal, removable badge pills, and max tags limit.",
            RazorSnippet = "<TwTagsInput Tags=\"new()\" Label=\"\" Placeholder=\"Type tag and press Enter...\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Tags", "List<string>", "new()", "Configures component behavior or styling."),
                new ComponentParameterDoc("TagsChanged", "EventCallback<List<string>>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Label", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Placeholder", "string", "\"Type tag and press Enter...\"", "Input placeholder hint."),
                new ComponentParameterDoc("HelperText", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("MaxTags", "int?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("AllowDuplicates", "bool", "false", "Configures component behavior or styling."),
                new ComponentParameterDoc("Disabled", "bool", "null", "Disables user interaction and applies muted opacity styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwInputGroup",
            Category = "Advanced Forms & Inputs",
            Description = "Composite input field with seamless prepended and appended text, icon slots, and dropdown buttons (currency, protocol, domain).",
            RazorSnippet = "<TwInputGroup Value=\"\" Label=\"\" Type=\"text\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "string", "\"\"", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("ValueChanged", "EventCallback<string>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Label", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Type", "string", "\"text\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Placeholder", "string", "\"\"", "Input placeholder hint."),
                new ComponentParameterDoc("PrependText", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("AppendText", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("PrependContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("AppendContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("HelperText", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ErrorMessage", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("HasError", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Disabled", "bool", "null", "Disables user interaction and applies muted opacity styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwPill",
            Category = "Typography, Badges & Keys",
            Description = "Modern pill badge supporting glowing pulse dots, mini avatars, counter badges, dismissible close buttons, and semantic variants (Soft, Solid, Outline, Glow).",
            RazorSnippet = "<TwPill Variant=\"Soft\" Color=\"Brand\" Size=\"md\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Text", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("Variant", "string", "\"Soft\"", "Semantic visual variant (e.g. Primary, Secondary, Outline)."),
                new ComponentParameterDoc("Color", "string", "\"Brand\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Size", "string", "\"md\"", "Component sizing scale (Xs, Sm, Md, Lg)."),
                new ComponentParameterDoc("ShowDot", "bool", "false", "Configures component behavior or styling."),
                new ComponentParameterDoc("PulseDot", "bool", "false", "Configures component behavior or styling."),
                new ComponentParameterDoc("AvatarUrl", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Icon", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Count", "int?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Removable", "bool", "false", "Configures component behavior or styling."),
                new ComponentParameterDoc("Clickable", "bool", "false", "Configures component behavior or styling."),
                new ComponentParameterDoc("Selected", "bool", "false", "Configures component behavior or styling."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnClick", "EventCallback", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("OnDismiss", "EventCallback", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwPillGroup",
            Category = "Typography, Badges & Keys",
            Description = "Segmented pill filter collection for single or multi-select filtering with smooth active transitions.",
            RazorSnippet = "<TwPillGroup Items=\"new()\" SelectedValues=\"new()\" CustomClass=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Items", "List<PillItem>", "new()", "Configures component behavior or styling."),
                new ComponentParameterDoc("SelectedValues", "List<string>", "new()", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("SelectedValuesChanged", "EventCallback<List<string>>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("MultiSelect", "bool", "false", "Configures component behavior or styling."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwBadge",
            Category = "Typography, Badges & Keys",
            Description = "Status and counter badge supporting semantic colors (Primary, Success, Warning, Danger, Info), rounded pill styling, and optional pulsing status dots.",
            RazorSnippet = "<TwBadge Variant=\"SemanticVariant.Default\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("Variant", "SemanticVariant", "SemanticVariant.Default", "Semantic visual variant (e.g. Primary, Secondary, Outline)."),
                new ComponentParameterDoc("ShowDot", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwNotificationBadge",
            Category = "Typography, Badges & Keys",
            Description = "Floating count badge designed to overlay icons or avatars for unread alerts and incoming message counters.",
            RazorSnippet = "<TwNotificationBadge Visible=\"true\" MaxCount=\"99\" Variant=\"SemanticVariant.Danger\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("Visible", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("DotOnly", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowPing", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Count", "int", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("MaxCount", "int", "99", "Configures component behavior or styling."),
                new ComponentParameterDoc("Variant", "SemanticVariant", "SemanticVariant.Danger", "Semantic visual variant (e.g. Primary, Secondary, Outline)."),
                new ComponentParameterDoc("Placement", "BadgePlacement", "BadgePlacement.TopRight", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwChip",
            Category = "Typography, Badges & Keys",
            Description = "Interactive tag chip with optional leading icon, selectable toggle state, and dismissible close button.",
            RazorSnippet = "<TwChip Variant=\"SemanticVariant.Default\" Size=\"ComponentSize.Sm\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Text", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("LeadingIcon", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("AvatarContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Variant", "SemanticVariant", "SemanticVariant.Default", "Semantic visual variant (e.g. Primary, Secondary, Outline)."),
                new ComponentParameterDoc("Size", "ComponentSize", "ComponentSize.Sm", "Component sizing scale (Xs, Sm, Md, Lg)."),
                new ComponentParameterDoc("Outlined", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Selectable", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Selected", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("SelectedChanged", "EventCallback<bool>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Removable", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnRemove", "EventCallback", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnClick", "EventCallback", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwChipGroup",
            Category = "Typography, Badges & Keys",
            Description = "Container for managing groups of filter chips with single-select or multi-select modes.",
            RazorSnippet = "<TwChipGroup>\n    <span>Content</span>\n</TwChipGroup>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwKbd",
            Category = "Typography, Badges & Keys",
            Description = "3D-embossed tactile keyboard shortcut badge with automatic modifier symbol mapping (⌘, ⌥, ⇧, ⌃, ↵, ⎋, ←, →).",
            RazorSnippet = "<TwKbd Size=\"md\" Variant=\"default\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Key", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Size", "string", "\"md\"", "Component sizing scale (Xs, Sm, Md, Lg)."),
                new ComponentParameterDoc("Variant", "string", "\"default\"", "Semantic visual variant (e.g. Primary, Secondary, Outline)."),
                new ComponentParameterDoc("Tooltip", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwDivider",
            Category = "Typography, Badges & Keys",
            Description = "Subtle horizontal or vertical divider with optional centered text or icon badge.",
            RazorSnippet = "<TwDivider>\n    <span>Content</span>\n</TwDivider>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Text", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwDateRangePicker",
            Category = "Buttons, Forms & Controls",
            Description = "Dual-month calendar popover with quick range presets (Today, 7 Days, 30 Days, This Month, YTD), continuous selection range highlighting, and manual date input.",
            RazorSnippet = "<TwDateRangePicker Label=\"Date Range Selection\" StartDate=\"DateTime.Today.AddDays(-14)\" EndDate=\"DateTime.Today\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Label", "string?", "\"Date Range Selection\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("StartDate", "DateTime", "DateTime.Today.AddDays(-14)", "Configures component behavior or styling."),
                new ComponentParameterDoc("EndDate", "DateTime", "DateTime.Today", "Configures component behavior or styling."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwButton",
            Category = "Buttons, Forms & Controls",
            Description = "Tactile interactive button featuring 44px touch targets, loading spinner state, semantic variants, and mobile full-width responsive expansion.",
            RazorSnippet = "<TwButton Variant=\"ButtonVariant.Primary\" Size=\"ComponentSize.Md\" Type=\"button\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("LeadingIcon", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("TrailingIcon", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Variant", "ButtonVariant", "ButtonVariant.Primary", "Semantic visual variant (e.g. Primary, Secondary, Outline)."),
                new ComponentParameterDoc("Size", "ComponentSize", "ComponentSize.Md", "Component sizing scale (Xs, Sm, Md, Lg)."),
                new ComponentParameterDoc("Type", "string", "\"button\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Disabled", "bool", "null", "Disables user interaction and applies muted opacity styling."),
                new ComponentParameterDoc("Loading", "bool", "null", "Displays animated loading spinner indicator."),
                new ComponentParameterDoc("ResponsiveFullWidth", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("FullWidth", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
                new ComponentParameterDoc("OnClick", "EventCallback<MouseEventArgs>", "null", "Callback invoked when state changes or item is selected."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwInput",
            Category = "Buttons, Forms & Controls",
            Description = "Accessible text input with validation error states, helper text slots, leading/trailing icons, and clear button.",
            RazorSnippet = "<TwInput Type=\"text\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Label", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Value", "string?", "null", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("ValueChanged", "EventCallback<string?>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Placeholder", "string?", "null", "Input placeholder hint."),
                new ComponentParameterDoc("Type", "string", "\"text\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("HelperText", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ErrorText", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Required", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Disabled", "bool", "null", "Disables user interaction and applies muted opacity styling."),
                new ComponentParameterDoc("PrefixContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("SuffixContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwSelect",
            Category = "Buttons, Forms & Controls",
            Description = "Styled select dropdown with floating labels, option groups, and dark mode contrast.",
            RazorSnippet = "<TwSelect>\n    <span>Content</span>\n</TwSelect>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Label", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Value", "string?", "null", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("ValueChanged", "EventCallback<string?>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("HelperText", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Disabled", "bool", "null", "Disables user interaction and applies muted opacity styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwSwitch",
            Category = "Buttons, Forms & Controls",
            Description = "Accessible toggle switch with smooth spring transitions and screen-reader label binding.",
            RazorSnippet = "<TwSwitch />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Checked", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("CheckedChanged", "EventCallback<bool>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Label", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Description", "string?", "null", "Secondary descriptive caption."),
                new ComponentParameterDoc("Disabled", "bool", "null", "Disables user interaction and applies muted opacity styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwCheckbox",
            Category = "Buttons, Forms & Controls",
            Description = "Touch-friendly checkbox with animated checkmark and focus ring styling.",
            RazorSnippet = "<TwCheckbox />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Checked", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("CheckedChanged", "EventCallback<bool>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Label", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Description", "string?", "null", "Secondary descriptive caption."),
                new ComponentParameterDoc("Disabled", "bool", "null", "Disables user interaction and applies muted opacity styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwSlider",
            Category = "Buttons, Forms & Controls",
            Description = "Touch-friendly range slider with draggable thumb, track fill, and live value bubble readout.",
            RazorSnippet = "<TwSlider Value=\"50\" Min=\"0\" Max=\"100\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "double", "50", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("ValueChanged", "EventCallback<double>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Min", "double", "0", "Configures component behavior or styling."),
                new ComponentParameterDoc("Max", "double", "100", "Configures component behavior or styling."),
                new ComponentParameterDoc("Step", "double", "1", "Configures component behavior or styling."),
                new ComponentParameterDoc("Label", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ValueUnit", "string?", "\"%\"", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("ShowValue", "bool", "true", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("Disabled", "bool", "null", "Disables user interaction and applies muted opacity styling."),
                new ComponentParameterDoc("HelperText", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwNumeric",
            Category = "Buttons, Forms & Controls",
            Description = "Precise numeric stepper input with increment/decrement buttons, min/max clamping, and step intervals.",
            RazorSnippet = "<TwNumeric Min=\"0\" Max=\"100\" Step=\"1\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "double", "null", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("ValueChanged", "EventCallback<double>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Label", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("HelperText", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Prefix", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Suffix", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Min", "double", "0", "Configures component behavior or styling."),
                new ComponentParameterDoc("Max", "double", "100", "Configures component behavior or styling."),
                new ComponentParameterDoc("Step", "double", "1", "Configures component behavior or styling."),
                new ComponentParameterDoc("Required", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwDatePicker",
            Category = "Buttons, Forms & Controls",
            Description = "Accessible date picker with calendar popover, quick range presets, and international date formatting.",
            RazorSnippet = "<TwDatePicker Placeholder=\"Select date...\" DateFormat=\"MMM dd, yyyy\" AllowClear=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "DateTime?", "null", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("ValueChanged", "EventCallback<DateTime?>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Label", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Placeholder", "string", "\"Select date...\"", "Input placeholder hint."),
                new ComponentParameterDoc("DateFormat", "string", "\"MMM dd, yyyy\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("AllowClear", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("Required", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwFileUpload",
            Category = "Buttons, Forms & Controls",
            Description = "Drag-and-drop file upload zone with file size validation, mime type filtering, and upload progress visualization.",
            RazorSnippet = "<TwFileUpload Label=\"Upload Documents or Media\" Multiple=\"true\" Accept=\".png,.jpg,.jpeg,.svg,.pdf,.zip\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Label", "string?", "\"Upload Documents or Media\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("HelperText", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Required", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Multiple", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("Accept", "string", "\".png,.jpg,.jpeg,.svg,.pdf,.zip\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("MaxFileSize", "long", "10 * 1024 * 1024", "Component sizing scale (Xs, Sm, Md, Lg)."),
                new ComponentParameterDoc("FilesChanged", "EventCallback<IReadOnlyList<IBrowserFile>>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwSegmented",
            Category = "Buttons, Forms & Controls",
            Description = "iOS-style segmented control pill for switching between mutually exclusive views with sliding background highlight.",
            RazorSnippet = "<TwSegmented Items=\"Array.Empty<SegmentItem>()\" Value=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Items", "IReadOnlyList<SegmentItem>", "Array.Empty<SegmentItem>()", "Configures component behavior or styling."),
                new ComponentParameterDoc("Value", "string", "\"\"", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("ValueChanged", "EventCallback<string>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("FullWidth", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwRating",
            Category = "Buttons, Forms & Controls",
            Description = "Interactive star rating component supporting full and half-star ratings with hover preview and read-only display mode.",
            RazorSnippet = "<TwRating Value=\"4\" MaxStars=\"5\" ShowValueText=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "int", "4", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("ValueChanged", "EventCallback<int>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("MaxStars", "int", "5", "Configures component behavior or styling."),
                new ComponentParameterDoc("ReadOnly", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowValueText", "bool", "true", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwCard",
            Category = "Cards, Grids & Bento Systems",
            Description = "Modular card container with header, body, and footer slots, subtle hover lift, and glassmorphic borders.",
            RazorSnippet = "<TwCard>\n    <span>Content</span>\n</TwCard>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("Subtitle", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("Hoverable", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("NoPadding", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("HeaderContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("FooterContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwBentoGrid",
            Category = "Cards, Grids & Bento Systems",
            Description = "Modern asymmetrical CSS grid for showcasing features with radial mouse-following spotlight gradients.",
            RazorSnippet = "<TwBentoGrid Cols=\"3\" GapClass=\"gap-4 sm:gap-6\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Cols", "int", "3", "Configures component behavior or styling."),
                new ComponentParameterDoc("GapClass", "string", "\"gap-4 sm:gap-6\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwBentoCard",
            Category = "Cards, Grids & Bento Systems",
            Description = "Card item for Bento grids with customizable column/row spans, glowing borders, and feature badge slots.",
            RazorSnippet = "<TwBentoCard ColSpan=\"1\" RowSpan=\"1\" ShowDotPattern=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ColSpan", "int", "1", "Configures component behavior or styling."),
                new ComponentParameterDoc("RowSpan", "int", "1", "Configures component behavior or styling."),
                new ComponentParameterDoc("Title", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("Description", "string?", "null", "Secondary descriptive caption."),
                new ComponentParameterDoc("Icon", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Badge", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("Footer", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowDotPattern", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("SpotlightColor", "string", "\"rgba(99, 102, 241, 0.12)\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwStatCard",
            Category = "Cards, Grids & Bento Systems",
            Description = "SaaS metric KPI card with sparklines, percentage change indicators, and target progress bars.",
            RazorSnippet = "<TwStatCard Title=\"\" Value=\"\" IsTrendPositive=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"\"", "Header title or accessible label."),
                new ComponentParameterDoc("Value", "string", "\"\"", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("Subtitle", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("TrendText", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("IsTrendPositive", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("ProgressValue", "int?", "null", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("IconContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("IconBgClass", "string", "\"bg-brand-50 dark:bg-brand-500/10\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwKpiMetric",
            Category = "Cards, Grids & Bento Systems",
            Description = "Compact micro-metric card with live animated count-up and cubic ease-out transitions.",
            RazorSnippet = "<TwKpiMetric Title=\"string.Empty\" Value=\"string.Empty\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "string.Empty", "Header title or accessible label."),
                new ComponentParameterDoc("Value", "string", "string.Empty", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("Subtitle", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("ChangePercentage", "double?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("SparklineData", "IReadOnlyList<double>?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Color", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwMediaCard",
            Category = "Cards, Grids & Bento Systems",
            Description = "Rich media card with 16:9 aspect ratio image header, category badges, author avatar, and reading time.",
            RazorSnippet = "<TwMediaCard Title=\"\" ImageAlt=\"Media cover\" AuthorInitials=\"AM\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"\"", "Header title or accessible label."),
                new ComponentParameterDoc("Description", "string?", "null", "Secondary descriptive caption."),
                new ComponentParameterDoc("ImageUrl", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ImageAlt", "string", "\"Media cover\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("CategoryBadge", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("AuthorName", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("AuthorInitials", "string", "\"AM\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("DateText", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Tags", "IReadOnlyList<string>?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ActionContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnClick", "EventCallback<MouseEventArgs>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwFeatureCard",
            Category = "Cards, Grids & Bento Systems",
            Description = "Service feature card with gradient background icon, bold headline, and subtle hover scale.",
            RazorSnippet = "<TwFeatureCard Title=\"\" Description=\"\" LinkText=\"Explore feature\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"\"", "Header title or accessible label."),
                new ComponentParameterDoc("Description", "string", "\"\"", "Secondary descriptive caption."),
                new ComponentParameterDoc("BadgeText", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("LinkText", "string?", "\"Explore feature\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("IconGradientClass", "string", "\"bg-gradient-to-tr from-brand-600 to-indigo-500\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("IconContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnClick", "EventCallback<MouseEventArgs>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwSpotlightCard",
            Category = "Cards, Grids & Bento Systems",
            Description = "Interactive card that calculates cursor coordinates to project a dynamic radial spotlight beam across its surface.",
            RazorSnippet = "<TwSpotlightCard SpotlightColor=\"rgba(99, 102, 241, 0.15)\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("SpotlightColor", "string", "\"rgba(99, 102, 241, 0.15)\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwPricingCard",
            Category = "Cards, Grids & Bento Systems",
            Description = "SaaS subscription pricing tier card with popular badge highlight, price per interval, and feature checklist.",
            RazorSnippet = "<TwPricingCard PlanName=\"Pro\" Price=\"$29\" Interval=\"mo\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("PlanName", "string", "\"Pro\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Description", "string?", "null", "Secondary descriptive caption."),
                new ComponentParameterDoc("Price", "string", "\"$29\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Interval", "string", "\"mo\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("IsPopular", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Features", "IReadOnlyList<string>?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("CtaText", "string", "\"Get Started\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnClick", "EventCallback<MouseEventArgs>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwPricingTable",
            Category = "Cards, Grids & Bento Systems",
            Description = "Full billing cycle matrix comparing Free, Pro, and Enterprise tiers with monthly/annual toggle.",
            RazorSnippet = "<TwPricingTable Tiers=\"new()\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Tiers", "List<PricingTier>", "new()", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnSelectTier", "EventCallback<PricingTier>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwKanbanBoard",
            Category = "Enterprise Data & Tables",
            Description = "Interactive workflow Kanban board with customizable columns, WIP limit thresholds, task cards, priority indicators, subtask checklists, and lane transition buttons.",
            RazorSnippet = "<TwKanbanBoard Title=\"Sprint Workflow Kanban\" Columns=\"new()\" CustomClass=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"Sprint Workflow Kanban\"", "Header title or accessible label."),
                new ComponentParameterDoc("Columns", "List<KanbanColumn>", "new()", "Configures component behavior or styling."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnCardMoved", "EventCallback<KanbanCard>", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwDataGrid",
            Category = "Enterprise Data & Tables",
            Description = "Enterprise data grid: sortable headers (keyboard accessible, Shift adds a second sort), search and per-column filters, grouping with counts and summaries, footer totals, master-detail rows, inline editing with validation, column chooser, CSV export, saved layout, frozen column, conditional formatting, selection, paging and mobile cards.",
            RazorSnippet = "<TwDataGrid TItem=\"Order\" Items=\"_orders\" Label=\"Orders\" Selectable=\"true\" AllowGrouping=\"true\" AllowColumnChooser=\"true\" AllowExport=\"true\" StateKey=\"orders\">\n    <Columns>\n        <TwDataGridColumn TItem=\"Order\" Title=\"Customer\" Property=\"o => o.Customer\" Editable=\"true\" EditValue=\"SetCustomer\" />\n        <TwDataGridColumn TItem=\"Order\" Title=\"Amount\" Property=\"o => o.Amount\" Summary=\"GridSummary.Sum\" SummaryFormat=\"C0\" />\n    </Columns>\n</TwDataGrid>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Items", "IEnumerable<TItem>", "empty", "Records to show."),
                new ComponentParameterDoc("Columns", "RenderFragment?", "null", "TwDataGridColumn definitions."),
                new ComponentParameterDoc("Label", "string", "\"Data grid\"", "Accessible table caption."),
                new ComponentParameterDoc("Selectable", "bool", "false", "Row checkboxes and a select-all for the page."),
                new ComponentParameterDoc("ShowGlobalSearch", "bool", "true", "Search box over all columns."),
                new ComponentParameterDoc("SearchPredicate", "Func<TItem,string,bool>?", "null", "Custom search logic."),
                new ComponentParameterDoc("AllowColumnFiltering", "bool", "true", "Per-column filter row."),
                new ComponentParameterDoc("AllowGrouping", "bool", "false", "Group by list; group headers show a count and summaries."),
                new ComponentParameterDoc("GroupBy", "string?", "null", "Title of the column to group by initially."),
                new ComponentParameterDoc("AllowColumnChooser", "bool", "false", "Columns menu to show and hide columns."),
                new ComponentParameterDoc("AllowExport", "bool", "false", "Export CSV of all filtered rows and visible columns (formula-safe)."),
                new ComponentParameterDoc("ExportFileName", "string", "\"export.csv\"", "Download name."),
                new ComponentParameterDoc("DetailTemplate", "RenderFragment<TItem>?", "null", "Expandable detail row under each record."),
                new ComponentParameterDoc("RowClassSelector", "Func<TItem,string?>?", "null", "Conditional formatting for a row."),
                new ComponentParameterDoc("StickyHeader", "bool", "false", "Keep the header in view."),
                new ComponentParameterDoc("MaxHeight", "string?", "null", "CSS max-height; the body scrolls, the header stays."),
                new ComponentParameterDoc("StateKey", "string?", "null", "Remember sort, search, filters, grouping, page size and visible columns in localStorage."),
                new ComponentParameterDoc("RowSaved", "EventCallback<TItem>", "null", "An edited row passed validation and was applied."),
                new ComponentParameterDoc("ActionsTemplate", "RenderFragment<TItem>?", "null", "Per-row action buttons."),
                new ComponentParameterDoc("ActionsTitle", "string", "\"Actions\"", "Actions column header."),
                new ComponentParameterDoc("MobileCardTemplate", "RenderFragment<TItem>?", "null", "Card layout on phones."),
                new ComponentParameterDoc("BatchActionsTemplate", "RenderFragment<IReadOnlyList<TItem>>?", "null", "Buttons shown while rows are selected."),
                new ComponentParameterDoc("EmptyText", "string", "\"No records found matching your criteria.\"", "Shown when nothing matches."),
                new ComponentParameterDoc("ShowPagination", "bool", "true", "Page controls."),
                new ComponentParameterDoc("PageSize", "int", "5", "Rows per page."),
                new ComponentParameterDoc("PageSizeOptions", "int[]", "5,10,20,50", "Choices in the page-size list."),
                new ComponentParameterDoc("ShowPageSizeSelector", "bool", "true", "Page-size list."),
                new ComponentParameterDoc("CurrentPage", "int", "1", "Current page (two-way with CurrentPageChanged)."),
                new ComponentParameterDoc("CurrentPageChanged", "EventCallback<int>", "null", "Page changed."),
                new ComponentParameterDoc("SelectionChanged", "EventCallback<IReadOnlyList<TItem>>", "null", "Selection changed."),
                new ComponentParameterDoc("Class", "string?", "null", "Custom Tailwind CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwDataGridColumn",
            Category = "Enterprise Data & Tables",
            Description = "Column definition for TwDataGrid: binding, sorting, filtering, width, alignment, templates, footer and group summary, grouping and chooser options, frozen column, inline editing with validation, and conditional cell formatting.",
            RazorSnippet = "<TwDataGridColumn TItem=\"Order\" Title=\"Amount\" Property=\"o => o.Amount\" Align=\"right\" Summary=\"GridSummary.Sum\" SummaryFormat=\"C0\" Editable=\"true\" InputType=\"number\" EditValue=\"SetAmount\" Validate=\"Positive\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"\"", "Header text; also the column key for saved state."),
                new ComponentParameterDoc("Property", "Func<TItem,object?>?", "null", "Display value (also used for sort, filter, group, summary, export)."),
                new ComponentParameterDoc("SortBy", "Func<TItem,object?>?", "null", "Sort value when it differs from the display value."),
                new ComponentParameterDoc("Sortable", "bool", "true", "Header is a sort button."),
                new ComponentParameterDoc("Filterable", "bool", "true", "Shows a filter box."),
                new ComponentParameterDoc("Summary", "GridSummary", "None", "Count, Sum, Average, Min or Max in the footer and group headers."),
                new ComponentParameterDoc("SummaryFormat", "string?", "null", ".NET numeric format, for example C0 or N1."),
                new ComponentParameterDoc("Groupable", "bool", "true", "Offered in Group by."),
                new ComponentParameterDoc("Hideable", "bool", "true", "Can be hidden in the chooser (the last visible column never can)."),
                new ComponentParameterDoc("Frozen", "bool", "false", "Pinned at the start edge while scrolling sideways (use on the first column)."),
                new ComponentParameterDoc("Editable", "bool", "false", "Shows an input while its row is edited."),
                new ComponentParameterDoc("EditValue", "Action<TItem,string>?", "null", "Applies the edited text to the item on save."),
                new ComponentParameterDoc("EditText", "Func<TItem,string>?", "display value", "Initial edit text."),
                new ComponentParameterDoc("Validate", "Func<string,string?>?", "null", "Returns an error message to block the save."),
                new ComponentParameterDoc("InputType", "string", "\"text\"", "text, number, date or email."),
                new ComponentParameterDoc("CellClassSelector", "Func<TItem,string?>?", "null", "Conditional formatting for a cell."),
                new ComponentParameterDoc("Width", "string?", "null", "CSS width."),
                new ComponentParameterDoc("MinWidth", "string?", "null", "CSS min-width."),
                new ComponentParameterDoc("MaxWidth", "string?", "null", "CSS max-width."),
                new ComponentParameterDoc("Align", "string", "\"left\"", "left, center or right."),
                new ComponentParameterDoc("CellTemplate", "RenderFragment<TItem>?", "null", "Custom cell."),
                new ComponentParameterDoc("HeaderTemplate", "RenderFragment?", "null", "Custom header."),
                new ComponentParameterDoc("FilterTemplate", "RenderFragment?", "null", "Custom filter control."),
                new ComponentParameterDoc("FilterPlaceholder", "string?", "null", "Filter box hint."),
                new ComponentParameterDoc("HeaderClass", "string?", "null", "Extra header classes."),
                new ComponentParameterDoc("CellClass", "string?", "null", "Extra cell classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwDataTable",
            Category = "Enterprise Data & Tables",
            Description = "Generic table component that automatically transforms into touch-friendly stacked cards on mobile screens.",
            RazorSnippet = "<TwDataTable Items=\"Array.Empty<TItem>()\" RowTemplate=\"default!\" EmptyText=\"No records found\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Items", "IReadOnlyList<TItem>", "Array.Empty<TItem>()", "Configures component behavior or styling."),
                new ComponentParameterDoc("TableHeader", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("RowTemplate", "RenderFragment<TItem>", "default!", "Configures component behavior or styling."),
                new ComponentParameterDoc("MobileTemplate", "RenderFragment<TItem>?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("EmptyText", "string", "\"No records found\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwPagination",
            Category = "Enterprise Data & Tables",
            Description = "Responsive pagination control showing Prev/Next on mobile devices and numbered page pills on desktop.",
            RazorSnippet = "<TwPagination CurrentPage=\"1\" TotalPages=\"1\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("CurrentPage", "int", "1", "Configures component behavior or styling."),
                new ComponentParameterDoc("TotalPages", "int", "1", "Configures component behavior or styling."),
                new ComponentParameterDoc("CurrentPageChanged", "EventCallback<int>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwTree",
            Category = "Enterprise Data & Tables",
            Description = "Hierarchical tree view component with expandable/collapsible nodes and selection tracking.",
            RazorSnippet = "<TwTree>\n    <span>Content</span>\n</TwTree>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("SelectedId", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("SelectedIdChanged", "EventCallback<string>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwTreeItem",
            Category = "Enterprise Data & Tables",
            Description = "Individual node item inside TwTree with custom icons and nested child rendering.",
            RazorSnippet = "<TwTreeItem Id=\"Guid.NewGuid().ToString(N)\" Text=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Id", "string", "Guid.NewGuid().ToString(\"N\")", "Configures component behavior or styling."),
                new ComponentParameterDoc("Text", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Badge", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("InitiallyExpanded", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("IconContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwSparkline",
            Category = "Visualizations & Gauges",
            Description = "Zero-dependency pure C# inline SVG micro-chart engine supporting Line, Area, Bar, and Win/Loss modes for table cells and executive KPI widgets.",
            RazorSnippet = "<TwSparkline Type=\"SparklineType.Line\" Points=\"new() { 12, 18, 14, 22, 19, 28, 25, 34, 30, 42 }\" Width=\"110\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Type", "SparklineType", "SparklineType.Line", "Configures component behavior or styling."),
                new ComponentParameterDoc("Points", "List<double>", "new() { 12, 18, 14, 22, 19, 28, 25, 34, 30, 42 }", "Configures component behavior or styling."),
                new ComponentParameterDoc("Width", "int", "110", "Configures component behavior or styling."),
                new ComponentParameterDoc("Height", "int", "28", "Configures component behavior or styling."),
                new ComponentParameterDoc("StrokeWidth", "double", "2.0", "Configures component behavior or styling."),
                new ComponentParameterDoc("Color", "string", "\"brand\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowMinMaxDots", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowValueBadge", "bool", "false", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwProgressAdvanced",
            Category = "Visualizations & Gauges",
            Description = "Advanced enterprise progress visualization supporting 5 styles: Multi-segment stacked bar with tooltips, SVG circular radial ring, flowing striped animated bar, discrete LED audio-meter bar, and milestone step tracker.",
            RazorSnippet = "<TwProgressAdvanced Type=\"ProgressType.Stacked\" Value=\"65\" ShowHeader=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Type", "ProgressType", "ProgressType.Stacked", "Configures component behavior or styling."),
                new ComponentParameterDoc("Title", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("Label", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Value", "double", "65", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("ShowHeader", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowLegend", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("Color", "string", "\"brand\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Segments", "List<ProgressSegment>", "new()\n    {\n        new ProgressSegment { Label = \"Code & Engine\", Percentage = 42, ColorClass = \"bg-brand-500\" },\n        new ProgressSegment { Label = \"Assets & Icons\", Percentage = 28, ColorClass = \"bg-emerald-500\" },\n        new ProgressSegment { Label = \"Cached Data\", Percentage = 18, ColorClass = \"bg-amber-500\" },\n        new ProgressSegment { Label = \"Free Memory\", Percentage = 12, ColorClass = \"bg-slate-300 dark:bg-slate-700\" }\n    }", "Configures component behavior or styling."),
                new ComponentParameterDoc("RadialSize", "int", "100", "Component sizing scale (Xs, Sm, Md, Lg)."),
                new ComponentParameterDoc("RadialStrokeWidth", "int", "8", "Configures component behavior or styling."),
                new ComponentParameterDoc("RadialContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("TotalSegments", "int", "15", "Configures component behavior or styling."),
                new ComponentParameterDoc("Steps", "List<string>", "new() { \"Initialize\", \"Validate\", \"Compile\", \"Deploy\" }", "Configures component behavior or styling."),
                new ComponentParameterDoc("CurrentStep", "int", "2", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwChart",
            Category = "Visualizations & Gauges",
            Description = "Native C# inline SVG visualization engine for donut charts, sparklines, and bar charts with zero external JS dependencies.",
            RazorSnippet = "<TwChart Type=\"ChartType.Donut\" CenterValue=\"100%\" CenterLabel=\"Total\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Type", "ChartType", "ChartType.Donut", "Configures component behavior or styling."),
                new ComponentParameterDoc("Title", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("Subtitle", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("CenterValue", "string", "\"100%\"", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("CenterLabel", "string", "\"Total\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("DataItems", "IReadOnlyList<ChartDataItem>", "Array.Empty<ChartDataItem>()", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwGauge",
            Category = "Visualizations & Gauges",
            Description = "Zero-JS radial speedometer gauge rendering pure inline SVG arcs with stroke-dasharray calculations and CSS transitions.",
            RazorSnippet = "<TwGauge Value=\"72\" Min=\"0\" Max=\"100\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "double", "72", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("Min", "double", "0", "Configures component behavior or styling."),
                new ComponentParameterDoc("Max", "double", "100", "Configures component behavior or styling."),
                new ComponentParameterDoc("Unit", "string", "\"%\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Label", "string?", "\"Efficiency\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Sublabel", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Color", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwProgressBar",
            Category = "Visualizations & Gauges",
            Description = "Semantic progress bar with animated stripes, smooth transitions, and percentage labels.",
            RazorSnippet = "<TwProgressBar ShowLabel=\"true\" Variant=\"SemanticVariant.Primary\" Size=\"ComponentSize.Md\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "int", "null", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("Label", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowLabel", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("Animated", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Variant", "SemanticVariant", "SemanticVariant.Primary", "Semantic visual variant (e.g. Primary, Secondary, Outline)."),
                new ComponentParameterDoc("Size", "ComponentSize", "ComponentSize.Md", "Component sizing scale (Xs, Sm, Md, Lg)."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwCountUp",
            Category = "Visualizations & Gauges",
            Description = "Smooth numeric counter animation with customizable duration, prefix/suffix, and easing.",
            RazorSnippet = "<TwCountUp StartValue=\"0\" EndValue=\"100\" DurationMs=\"1200\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("StartValue", "double", "0", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("EndValue", "double", "100", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("DurationMs", "int", "1200", "Configures component behavior or styling."),
                new ComponentParameterDoc("Decimals", "int", "0", "Configures component behavior or styling."),
                new ComponentParameterDoc("Prefix", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Suffix", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("AutoStart", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwAnimate",
            Category = "Visualizations & Gauges",
            Description = "Declarative CSS animation wrapper providing fade, zoom, bounce, and slide keyframes.",
            RazorSnippet = "<TwAnimate Type=\"AnimationType.FadeInUp\" DurationMs=\"400\" DelayMs=\"0\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Type", "AnimationType", "AnimationType.FadeInUp", "Configures component behavior or styling."),
                new ComponentParameterDoc("DurationMs", "int", "400", "Configures component behavior or styling."),
                new ComponentParameterDoc("DelayMs", "int", "0", "Configures component behavior or styling."),
                new ComponentParameterDoc("Infinite", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnHover", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Play", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwTransition",
            Category = "Visualizations & Gauges",
            Description = "Smooth enter/leave transition container for mounting and unmounting Blazor components.",
            RazorSnippet = "<TwTransition Show=\"true\" Enter=\"transition duration-300 ease-out\" EnterFrom=\"opacity-0 scale-95 -translate-y-2\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Show", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("Enter", "string", "\"transition duration-300 ease-out\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("EnterFrom", "string", "\"opacity-0 scale-95 -translate-y-2\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("EnterTo", "string", "\"opacity-100 scale-100 translate-y-0\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Leave", "string", "\"transition duration-200 ease-in\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("LeaveFrom", "string", "\"opacity-100 scale-100 translate-y-0\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("LeaveTo", "string", "\"opacity-0 scale-95 -translate-y-2\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
                new ComponentParameterDoc("Style", "string?", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwMediaPlayer",
            Category = "Media, Scanner & Signature",
            Description = "Custom video and audio player with chapter markers, scrubber seek, playback speed selector (0.5x - 2x), Picture-in-Picture, volume control, and audio visualizer.",
            RazorSnippet = "<TwMediaPlayer Src=\"string.Empty\" Type=\"MediaKind.Video\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Src", "string", "string.Empty", "Configures component behavior or styling."),
                new ComponentParameterDoc("Poster", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Title", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("Subtitle", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("Type", "MediaKind", "MediaKind.Video", "Configures component behavior or styling."),
                new ComponentParameterDoc("Loop", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Chapters", "List<MediaChapter>?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwQrCode",
            Category = "Media, Scanner & Signature",
            Description = "Vector SVG and PNG QR code generator supporting custom dot shapes (Square, Rounded, Circle), error correction levels, and center icon branding.",
            RazorSnippet = "<TwQrCode Value=\"https://tailwindcss.com\" Size=\"200\" ForegroundColor=\"#0f172a\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "string", "\"https://tailwindcss.com\"", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("Title", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("Subtitle", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("Size", "int", "200", "Component sizing scale (Xs, Sm, Md, Lg)."),
                new ComponentParameterDoc("ForegroundColor", "string", "\"#0f172a\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("BackgroundColor", "string", "\"#ffffff\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("DotShape", "QrDotShape", "QrDotShape.Rounded", "Configures component behavior or styling."),
                new ComponentParameterDoc("CenterIcon", "QrCenterIcon", "QrCenterIcon.None", "Configures component behavior or styling."),
                new ComponentParameterDoc("EccLevel", "QrEccLevel", "QrEccLevel.Q", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowActions", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwBarcode",
            Category = "Media, Scanner & Signature",
            Description = "High-density Code 128 1D vector barcode generator rendering sharp scalable SVG bars with human-readable text.",
            RazorSnippet = "<TwBarcode Value=\"SKU-984210\" BarHeight=\"64\" BarWidthMultiplier=\"2\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "string", "\"SKU-984210\"", "Primary data value or two-way bound model property."),
                new ComponentParameterDoc("Title", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("Subtitle", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("BarHeight", "int", "64", "Configures component behavior or styling."),
                new ComponentParameterDoc("BarWidthMultiplier", "int", "2", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowText", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("QuietZone", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("ForegroundColor", "string", "\"#0f172a\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("BackgroundColor", "string", "\"#ffffff\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowActions", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwBarcodeScanner",
            Category = "Media, Scanner & Signature",
            Description = "Live camera video and file barcode scanner supporting QR codes, Code 128, EAN, UPC, and Data Matrix with audio beep feedback.",
            RazorSnippet = "<TwBarcodeScanner />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("OnScan", "EventCallback<BarcodeScanResult>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwSignaturePad",
            Category = "Media, Scanner & Signature",
            Description = "Digital canvas signature pad with smooth Bezier curve drawing, undo/redo history, background transparency, and export to PNG and SVG.",
            RazorSnippet = "<TwSignaturePad Height=\"220\" ShowGuide=\"true\" PenColor=\"#0f172a\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("Subtitle", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("Height", "int", "220", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowGuide", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("PenColor", "string", "\"#0f172a\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("StrokeWidth", "double", "2.5", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnSave", "EventCallback<string>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwImageCompare",
            Category = "Media, Scanner & Signature",
            Description = "Interactive Before/After visual comparison slider with touch/pointer dragging, glassmorphic center handle, keyboard navigation, and dual badges.",
            RazorSnippet = "<TwImageCompare BeforeSrc=\"\" AfterSrc=\"\" BeforeAlt=\"Before image\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("BeforeSrc", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("AfterSrc", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("BeforeAlt", "string", "\"Before image\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("AfterAlt", "string", "\"After image\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("BeforeLabel", "string", "\"Before\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("AfterLabel", "string", "\"After\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Position", "double", "50.0", "Configures component behavior or styling."),
                new ComponentParameterDoc("PositionChanged", "EventCallback<double>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwRibbon",
            Category = "Navigation & Steppers",
            Description = "Award-winning enterprise Ribbon navigation bar featuring tabbed strips, Quick Access Toolbar (QAT), grouped action clusters, large/small buttons, and collapsible drawer.",
            RazorSnippet = "<TwRibbon Tabs=\"new()\" ActiveTabIndex=\"0\" CustomClass=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Tabs", "List<RibbonTab>", "new()", "Configures component behavior or styling."),
                new ComponentParameterDoc("ActiveTabIndex", "int", "0", "Configures component behavior or styling."),
                new ComponentParameterDoc("IsCollapsed", "bool", "false", "Configures component behavior or styling."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnActionClicked", "EventCallback<string>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("ActiveTabIndexChanged", "EventCallback<int>", "null", "Callback invoked when state changes or item is selected."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwFloatingDock",
            Category = "Navigation & Steppers",
            Description = "Modern floating glassmorphism dock navigation bar with hover magnification scale, active pill indicators, notification badges, and smooth tooltips.",
            RazorSnippet = "<TwFloatingDock Items=\"new()\" ActiveId=\"home\" Position=\"bottom\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Items", "List<DockItem>", "new()", "Configures component behavior or styling."),
                new ComponentParameterDoc("ActiveId", "string", "\"home\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Position", "string", "\"bottom\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnItemSelected", "EventCallback<string>", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwTabs",
            Category = "Navigation & Steppers",
            Description = "Horizontal scrollable tabs with pill or underline variants and badge counters.",
            RazorSnippet = "<TwTabs Tabs=\"Array.Empty<TabDefinition>()\" Variant=\"underline\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Tabs", "IReadOnlyList<TabDefinition>", "Array.Empty<TabDefinition>()", "Configures component behavior or styling."),
                new ComponentParameterDoc("ActiveTabKey", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ActiveTabKeyChanged", "EventCallback<string>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Variant", "string", "\"underline\"", "Semantic visual variant (e.g. Primary, Secondary, Outline)."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwSlidingTabs",
            Category = "Navigation & Steppers",
            Description = "Fluid tab strip with an animated gliding background pill that follows the active selection.",
            RazorSnippet = "<TwSlidingTabs Tabs=\"new()\" ActiveTabId=\"string.Empty\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Tabs", "List<TabItem>", "new()", "Configures component behavior or styling."),
                new ComponentParameterDoc("ActiveTabId", "string", "string.Empty", "Configures component behavior or styling."),
                new ComponentParameterDoc("ActiveTabIdChanged", "EventCallback<string>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwBreadcrumb",
            Category = "Navigation & Steppers",
            Description = "Accessible breadcrumb trail with custom separators and truncated intermediate paths.",
            RazorSnippet = "<TwBreadcrumb Items=\"Array.Empty<BreadcrumbItem>()\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Items", "IReadOnlyList<BreadcrumbItem>", "Array.Empty<BreadcrumbItem>()", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwStepper",
            Category = "Navigation & Steppers",
            Description = "Multi-step onboarding wizard with completed step indicators and compact mobile layout.",
            RazorSnippet = "<TwStepper Steps=\"Array.Empty<StepItem>()\" CurrentStep=\"1\" Clickable=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Steps", "IReadOnlyList<StepItem>", "Array.Empty<StepItem>()", "Configures component behavior or styling."),
                new ComponentParameterDoc("CurrentStep", "int", "1", "Configures component behavior or styling."),
                new ComponentParameterDoc("CurrentStepChanged", "EventCallback<int>", "null", "Callback invoked when state changes or item is selected."),
                new ComponentParameterDoc("Clickable", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwTimeline",
            Category = "Navigation & Steppers",
            Description = "Vertical activity timeline container for audit logs, delivery tracking, and project milestones.",
            RazorSnippet = "<TwTimeline Position=\"TimelinePosition.Start\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Position", "TimelinePosition", "TimelinePosition.Start", "Configures component behavior or styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwTimelineItem",
            Category = "Navigation & Steppers",
            Description = "Individual timeline entry with status dot, icon, timestamp, and card description.",
            RazorSnippet = "<TwTimelineItem Variant=\"SemanticVariant.Primary\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("Subtitle", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("Timestamp", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Variant", "SemanticVariant", "SemanticVariant.Primary", "Semantic visual variant (e.g. Primary, Secondary, Outline)."),
                new ComponentParameterDoc("Pulse", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("IsLast", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("DotContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwScrollToTop",
            Category = "Navigation & Steppers",
            Description = "Floating smooth scroll-to-top button with scroll position threshold and mobile safe-area clearance.",
            RazorSnippet = "<TwScrollToTop Variant=\"ScrollToTopVariant.Vertical\" Label=\"TOP\" ShowProgress=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Variant", "ScrollToTopVariant", "ScrollToTopVariant.Vertical", "Semantic visual variant (e.g. Primary, Secondary, Outline)."),
                new ComponentParameterDoc("Label", "string", "\"TOP\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowProgress", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("Threshold", "int", "150", "Configures component behavior or styling."),
                new ComponentParameterDoc("AlwaysVisible", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("PositionClass", "string", "\"bottom-36 sm:bottom-24 right-4 sm:right-6\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwSpeedDial",
            Category = "Navigation & Steppers",
            Description = "Floating action button that expands into a radial or vertical speed-dial menu of quick actions.",
            RazorSnippet = "<TwSpeedDial Actions=\"Array.Empty<SpeedDialAction>()\" PositionClass=\"bottom-20 sm:bottom-8 left-4 sm:left-8\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Actions", "IReadOnlyList<SpeedDialAction>", "Array.Empty<SpeedDialAction>()", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnActionSelected", "EventCallback<string>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("PositionClass", "string", "\"bottom-20 sm:bottom-8 left-4 sm:left-8\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwTour",
            Category = "Navigation & Steppers",
            Description = "Interactive guided walkthrough coaching tour with spotlight backdrop, step popovers, and automatic restart lifecycle.",
            RazorSnippet = "<TwTour Steps=\"Array.Empty<TourStep>()\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("IsOpen", "bool", "null", "Two-way bindable visibility flag."),
                new ComponentParameterDoc("IsOpenChanged", "EventCallback<bool>", "null", "Two-way bindable visibility flag."),
                new ComponentParameterDoc("Steps", "IReadOnlyList<TourStep>", "Array.Empty<TourStep>()", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnCompleted", "EventCallback", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwContextMenu",
            Category = "Overlays & Popovers",
            Description = "Desktop-grade right-click context menu and anchored flyout callout featuring nested submenus, keyboard shortcut badges, icons, and click-outside dismissal.",
            RazorSnippet = "<TwContextMenu Items=\"new()\" PreventDefaultContextMenu=\"true\" CustomClass=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Items", "List<ContextMenuItem>", "new()", "Configures component behavior or styling."),
                new ComponentParameterDoc("PreventDefaultContextMenu", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnItemSelected", "EventCallback<string>", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwModal",
            Category = "Overlays & Popovers",
            Description = "Accessible dialog that presents as a bottom sheet on mobile screens and a centered modal dialog on desktop.",
            RazorSnippet = "<TwModal MaxWidth=\"md\" CloseOnBackdropClick=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("IsOpen", "bool", "null", "Two-way bindable visibility flag."),
                new ComponentParameterDoc("IsOpenChanged", "EventCallback<bool>", "null", "Two-way bindable visibility flag."),
                new ComponentParameterDoc("Title", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("Description", "string?", "null", "Secondary descriptive caption."),
                new ComponentParameterDoc("MaxWidth", "string", "\"md\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("CloseOnBackdropClick", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("HeaderContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("FooterContent", "RenderFragment?", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwDrawer",
            Category = "Overlays & Popovers",
            Description = "Off-canvas sliding drawer supporting Left, Right, and Bottom slide orientations with backdrop blur.",
            RazorSnippet = "<TwDrawer Placement=\"DrawerPlacement.Right\" Width=\"max-w-md\" CloseOnBackdropClick=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("IsOpen", "bool", "null", "Two-way bindable visibility flag."),
                new ComponentParameterDoc("IsOpenChanged", "EventCallback<bool>", "null", "Two-way bindable visibility flag."),
                new ComponentParameterDoc("Title", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("Subtitle", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("Placement", "DrawerPlacement", "DrawerPlacement.Right", "Configures component behavior or styling."),
                new ComponentParameterDoc("Width", "string", "\"max-w-md\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("CloseOnBackdropClick", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("FooterContent", "RenderFragment?", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwDropdown",
            Category = "Overlays & Popovers",
            Description = "Click-outside popover menu with keyboard navigation and position alignment (Left, Right, Center).",
            RazorSnippet = "<TwDropdown Alignment=\"right\" CloseOnSelect=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("TriggerContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("Alignment", "string", "\"right\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("CloseOnSelect", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwTooltip",
            Category = "Overlays & Popovers",
            Description = "Hover and tap tooltip popover with auto-flipping placement and arrow indicator.",
            RazorSnippet = "<TwTooltip Text=\"\" Placement=\"top\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Text", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Placement", "string", "\"top\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwAccordion",
            Category = "Overlays & Popovers",
            Description = "Collapsible accordion container with single-expand or multi-expand modes.",
            RazorSnippet = "<TwAccordion>\n    <span>Content</span>\n</TwAccordion>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("AllowMultiple", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwAccordionItem",
            Category = "Overlays & Popovers",
            Description = "Individual accordion panel with smooth height transition and animated chevron.",
            RazorSnippet = "<TwAccordionItem Title=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"\"", "Header title or accessible label."),
                new ComponentParameterDoc("InitiallyOpen", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwChatBot",
            Category = "AI Chatbot & Reasoning",
            Description = "Intelligent conversational chatbot interface with streaming message bubbles, thinking progress checklists, and code snippet copying.",
            RazorSnippet = "<TwChatBot BotName=\"NexusAI\" SelectedModel=\"Gemini 2.0 Flash\" StarterPrompts=\"new List<StarterPrompt>\n    {\n        new(Explain Architecture, Explain how mobile-first Blazor WASM and TailwindCSS work together with zero FOUC.),\n        new(Generate Code, Write a reusable C# Blazor component for animated radial progress with Tailwind classes.),\n        new(Deep Reasoning Demo, Solve a complex algorithm step-by-step with multi-stage reasoning checklist progress.),\n        new(Compare Performance, Compare Blazor WebAssembly AOT compilation vs JIT performance on mobile phones.)\n    }\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("BotName", "string", "\"NexusAI\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("SelectedModel", "string", "\"Gemini 2.0 Flash\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowCloseButton", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnClose", "EventCallback", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
                new ComponentParameterDoc("StarterPrompts", "IReadOnlyList<StarterPrompt>", "new List<StarterPrompt>\n    {\n        new(\"Explain Architecture\", \"Explain how mobile-first Blazor WASM and TailwindCSS work together with zero FOUC.\"),\n        new(\"Generate Code\", \"Write a reusable C# Blazor component for animated radial progress with Tailwind classes.\"),\n        new(\"Deep Reasoning Demo\", \"Solve a complex algorithm step-by-step with multi-stage reasoning checklist progress.\"),\n        new(\"Compare Performance\", \"Compare Blazor WebAssembly AOT compilation vs JIT performance on mobile phones.\")\n    }", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwChatWidget",
            Category = "AI Chatbot & Reasoning",
            Description = "Floating launcher button that expands into an interactive AI assistant window (or bottom sheet on mobile).",
            RazorSnippet = "<TwChatWidget BotName=\"NexusAI\" SelectedModel=\"Gemini 2.0 Flash\" PositionClass=\"bottom-20 sm:bottom-6 right-4 sm:right-6\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("BotName", "string", "\"NexusAI\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("SelectedModel", "string", "\"Gemini 2.0 Flash\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("PositionClass", "string", "\"bottom-20 sm:bottom-6 right-4 sm:right-6\"", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwAiReasoningProgress",
            Category = "AI Chatbot & Reasoning",
            Description = "Multi-phase thinking checklist component displaying animated progress indicators during AI reasoning tasks.",
            RazorSnippet = "<TwAiReasoningProgress IsThinking=\"true\" DurationSeconds=\"2.4\" ThinkingLabel=\"Reasoning step-by-step...\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("IsThinking", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("DurationSeconds", "double", "2.4", "Configures component behavior or styling."),
                new ComponentParameterDoc("ThinkingLabel", "string", "\"Reasoning step-by-step...\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("ReasoningText", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Steps", "IReadOnlyList<ReasoningStep>?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("DefaultExpanded", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwMarquee",
            Category = "Showcase & Micro-Interactions",
            Description = "Infinite continuous horizontal scrolling ticker with CSS keyframes, pause-on-hover, and edge gradient masks.",
            RazorSnippet = "<TwMarquee DurationSeconds=\"25\" PauseOnHover=\"true\" FadeEdges=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("DurationSeconds", "int", "25", "Configures component behavior or styling."),
                new ComponentParameterDoc("Reverse", "bool", "false", "Configures component behavior or styling."),
                new ComponentParameterDoc("PauseOnHover", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("FadeEdges", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
                new ComponentParameterDoc("TrackClass", "string?", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwConfetti",
            Category = "Showcase & Micro-Interactions",
            Description = "Canvas physics celebratory particle explosion system with gravity, air resistance, and 3D rotation.",
            RazorSnippet = "<TwConfetti ParticleCount=\"85\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("FullScreen", "bool", "false", "Configures component behavior or styling."),
                new ComponentParameterDoc("AutoFire", "bool", "false", "Configures component behavior or styling."),
                new ComponentParameterDoc("ParticleCount", "int", "85", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwSkeleton",
            Category = "Showcase & Micro-Interactions",
            Description = "Animated shimmer placeholder for cards, tables, text, and avatars during asynchronous data loading.",
            RazorSnippet = "<TwSkeleton Variant=\"text\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Variant", "string", "\"text\"", "Semantic visual variant (e.g. Primary, Secondary, Outline)."),
                new ComponentParameterDoc("Width", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Height", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwEmptyState",
            Category = "Showcase & Micro-Interactions",
            Description = "Illustrated zero-data state placeholder with title, description, and call-to-action button.",
            RazorSnippet = "<TwEmptyState Title=\"No items found\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"No items found\"", "Header title or accessible label."),
                new ComponentParameterDoc("Description", "string?", "null", "Secondary descriptive caption."),
                new ComponentParameterDoc("IconContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ActionContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwBanner",
            Category = "Showcase & Micro-Interactions",
            Description = "Full-width announcement banner with dismiss button and action link.",
            RazorSnippet = "<TwBanner Text=\"string.Empty\" Dismissible=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Text", "string", "string.Empty", "Configures component behavior or styling."),
                new ComponentParameterDoc("BadgeText", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ActionText", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnAction", "EventCallback", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Dismissible", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("IsSticky", "bool", "false", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwAlert",
            Category = "Showcase & Micro-Interactions",
            Description = "Dismissible inline feedback alert with semantic variants (Success, Warning, Danger, Info).",
            RazorSnippet = "<TwAlert Variant=\"SemanticVariant.Info\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string?", "null", "Header title or accessible label."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child content or nested markup rendered inside the component slot."),
                new ComponentParameterDoc("Variant", "SemanticVariant", "SemanticVariant.Info", "Semantic visual variant (e.g. Primary, Secondary, Outline)."),
                new ComponentParameterDoc("Dismissible", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnDismiss", "EventCallback", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Additional custom Tailwind CSS utility classes."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwToastContainer",
            Category = "Showcase & Micro-Interactions",
            Description = "Fixed container managing reactive floating toast alert notifications.",
            RazorSnippet = "<TwToastContainer />",
            Parameters = new List<ComponentParameterDoc>
            {
            }
        },
        new ComponentDocItem
        {
            Name = "TwAguiCanvas",
            Category = "Other Components",
            Description = "Reusable Blazor WebAssembly component styled with Tailwind CSS.",
            RazorSnippet = "<TwAguiCanvas Class=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Schema", "AguiWidgetSchema?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnActionTriggered", "EventCallback<AguiActionItem>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string", "\"\"", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwAguiStudio",
            Category = "Other Components",
            Description = "Reusable Blazor WebAssembly component styled with Tailwind CSS.",
            RazorSnippet = "<TwAguiStudio Class=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Class", "string", "\"\"", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwAvatar",
            Category = "Other Components",
            Description = "Reusable Blazor WebAssembly component styled with Tailwind CSS.",
            RazorSnippet = "<TwAvatar Initials=\"UI\" Alt=\"Avatar\" Size=\"ComponentSize.Md\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ImageUrl", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Initials", "string", "\"UI\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Alt", "string", "\"Avatar\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Size", "ComponentSize", "ComponentSize.Md", "Configures component behavior or styling."),
                new ComponentParameterDoc("StatusColor", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwAvatarGroup",
            Category = "Other Components",
            Description = "Reusable Blazor WebAssembly component styled with Tailwind CSS.",
            RazorSnippet = "<TwAvatarGroup Avatars=\"new()\" Max=\"4\" Size=\"ComponentSize.Md\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Avatars", "List<AvatarItem>", "new()", "Configures component behavior or styling."),
                new ComponentParameterDoc("Max", "int", "4", "Configures component behavior or styling."),
                new ComponentParameterDoc("Size", "ComponentSize", "ComponentSize.Md", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwCarousel",
            Category = "Other Components",
            Description = "Reusable Blazor WebAssembly component styled with Tailwind CSS.",
            RazorSnippet = "<TwCarousel AutoCycle=\"true\" IntervalSeconds=\"4\" ShowArrows=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ActiveIndex", "int", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ActiveIndexChanged", "EventCallback<int>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("AutoCycle", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("IntervalSeconds", "int", "4", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowArrows", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowIndicators", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwCarouselSlide",
            Category = "Other Components",
            Description = "Reusable Blazor WebAssembly component styled with Tailwind CSS.",
            RazorSnippet = "<TwCarouselSlide>\n    <span>Content</span>\n</TwCarouselSlide>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string?", "null", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwCommandPalette",
            Category = "Other Components",
            Description = "Reusable Blazor WebAssembly component styled with Tailwind CSS.",
            RazorSnippet = "<TwCommandPalette Items=\"new()\" Placeholder=\"Type a command, component name, or search...\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("IsOpen", "bool", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("IsOpenChanged", "EventCallback<bool>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Items", "List<CommandPaletteItem>", "new()", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnItemSelected", "EventCallback<CommandPaletteItem>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Placeholder", "string", "\"Type a command, component name, or search...\"", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwCompanyPartyForm",
            Category = "Other Components",
            Description = "Reusable Blazor WebAssembly component styled with Tailwind CSS.",
            RazorSnippet = "<TwCompanyPartyForm Model=\"new()\" Rsvp=\"new()\" CustomClass=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Model", "PartyModel", "new()", "Configures component behavior or styling."),
                new ComponentParameterDoc("Rsvp", "GuestRsvpModel", "new()", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnPartySaved", "EventCallback<PartyModel>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnRsvpSubmitted", "EventCallback<GuestRsvpModel>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwCompanyPlanForm",
            Category = "Other Components",
            Description = "Reusable Blazor WebAssembly component styled with Tailwind CSS.",
            RazorSnippet = "<TwCompanyPlanForm Model=\"new()\" CustomClass=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Model", "CompanyPlanModel", "new()", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnPlanSubmitted", "EventCallback<CompanyPlanModel>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwDocsExplorer",
            Category = "Other Components",
            Description = "Reusable Blazor WebAssembly component styled with Tailwind CSS.",
            RazorSnippet = "<TwDocsExplorer />",
            Parameters = new List<ComponentParameterDoc>
            {
            }
        },
        new ComponentDocItem
        {
            Name = "TwEmployeeAwardForm",
            Category = "Other Components",
            Description = "Reusable Blazor WebAssembly component styled with Tailwind CSS.",
            RazorSnippet = "<TwEmployeeAwardForm Model=\"new()\" CustomClass=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Model", "EmployeeAwardModel", "new()", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnAwardNominated", "EventCallback<EmployeeAwardModel>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwFormEngine",
            Category = "Other Components",
            Description = "Reusable Blazor WebAssembly component styled with Tailwind CSS.",
            RazorSnippet = "<TwFormEngine ShowHeader=\"true\" ShowCardWrapper=\"true\" Class=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Schema", "FormSchema?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnSubmit", "EventCallback<FormSubmissionResult>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnChange", "EventCallback<Dictionary<string, object?>>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowHeader", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("ShowCardWrapper", "bool", "true", "Configures component behavior or styling."),
                new ComponentParameterDoc("ReadOnly", "bool", "false", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string", "\"\"", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwFormStudio",
            Category = "Other Components",
            Description = "Reusable Blazor WebAssembly component styled with Tailwind CSS.",
            RazorSnippet = "<TwFormStudio />",
            Parameters = new List<ComponentParameterDoc>
            {
            }
        },
        new ComponentDocItem
        {
            Name = "TwProfitLossForm",
            Category = "Other Components",
            Description = "Reusable Blazor WebAssembly component styled with Tailwind CSS.",
            RazorSnippet = "<TwProfitLossForm Model=\"new()\" CustomClass=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Model", "ProfitLossModel", "new()", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnStatementSaved", "EventCallback<ProfitLossModel>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwSmartFilter",
            Category = "Other Components",
            Description = "Reusable Blazor WebAssembly component styled with Tailwind CSS.",
            RazorSnippet = "<TwSmartFilter Class=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("OnFilterChanged", "EventCallback<Dictionary<string, string>>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string", "\"\"", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwSmartPasteButton",
            Category = "Other Components",
            Description = "Reusable Blazor WebAssembly component styled with Tailwind CSS.",
            RazorSnippet = "<TwSmartPasteButton Class=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("OnApplied", "EventCallback<Dictionary<string, string>>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string", "\"\"", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwSmartSearch",
            Category = "Other Components",
            Description = "Reusable Blazor WebAssembly component styled with Tailwind CSS.",
            RazorSnippet = "<TwSmartSearch SearchQuery=\"\" Placeholder=\"Semantic natural language search (e.g. 'high salary engineers in cloud')...\" Class=\"\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("SearchQuery", "string", "\"\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("SearchQueryChanged", "EventCallback<string>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("OnSearchSubmitted", "EventCallback<string>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Placeholder", "string", "\"Semantic natural language search (e.g. 'high salary engineers in cloud')...\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string", "\"\"", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwSmartTextArea",
            Category = "Other Components",
            Description = "Reusable Blazor WebAssembly component styled with Tailwind CSS.",
            RazorSnippet = "<TwSmartTextArea Value=\"We are thrilled to announce our next-generation WebAssembly architecture which reduces client latency by over 80 percent across all devices.\" Placeholder=\"Start typing and watch smart completions appear...\" Rows=\"4\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Label", "string?", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Value", "string", "\"We are thrilled to announce our next-generation WebAssembly architecture which reduces client latency by over 80 percent across all devices.\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("ValueChanged", "EventCallback<string>", "null", "Configures component behavior or styling."),
                new ComponentParameterDoc("Placeholder", "string", "\"Start typing and watch smart completions appear...\"", "Configures component behavior or styling."),
                new ComponentParameterDoc("Rows", "int", "4", "Configures component behavior or styling."),
                new ComponentParameterDoc("Class", "string", "\"\"", "Configures component behavior or styling."),
            }
        },
        new ComponentDocItem
        {
            Name = "TwMicrCheckReader",
            Category = "Financial & OCR",
            Description = "Check 21 paper check & draft reader powered by 100% local Tesseract WebAssembly, ANSI X9.27 E-13B MICR decoding, 150+ NACHA Bank Routing Directory resolver, Mod-10 Fed routing validation, and ChoETL.NACHA 94-char fixed-width ACH file generation.",
            RazorSnippet = "<TwMicrCheckReader OnCheckScanned=\"HandleCheck\" OnDepositApproved=\"HandleApproved\" OnNachaExported=\"HandleNacha\" OnJsonExported=\"HandleJson\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Class", "string", "\"\"", "Optional CSS classes."),
                new ComponentParameterDoc("OnCheckScanned", "EventCallback<CheckDraftOcrResult>", "null", "Callback fired when a check or draft is scanned."),
                new ComponentParameterDoc("OnInsertMarkdown", "EventCallback<string>", "null", "Callback fired when Markdown ledger table is exported."),
                new ComponentParameterDoc("OnDepositApproved", "EventCallback<CheckDraftOcrResult>", "null", "Callback fired when remote deposit capture (RDC) is approved."),
                new ComponentParameterDoc("OnNachaExported", "EventCallback<string>", "null", "Callback fired when 94-character fixed-width NACHA ACH batch file is exported."),
                new ComponentParameterDoc("OnJsonExported", "EventCallback<string>", "null", "Callback fired when Check 21 JSON payload is exported.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwDocumentOcrStudio",
            Category = "Financial & OCR",
            Description = "Full Form & PDF OCR Studio supporting paper checks, official bank drafts, commercial invoices, and IRS Form W-9 with spatial bounding boxes and Markdown table generation.",
            RazorSnippet = "<TwDocumentOcrStudio OnMarkdownExtracted=\"HandleMarkdown\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Class", "string", "\"\"", "Optional CSS classes."),
                new ComponentParameterDoc("OnMarkdownExtracted", "EventCallback<string>", "null", "Fired when extracted markdown report is copied or exported.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwSmartOcrButton",
            Category = "Smart Components",
            Description = "Drop-in Smart Component button that triggers instant client-side OCR for paper checks, invoices, and PDF forms.",
            RazorSnippet = "<TwSmartOcrButton OnDocumentScanned=\"HandleDoc\" OnCheckScanned=\"HandleCheck\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Class", "string", "\"\"", "Optional CSS classes."),
                new ComponentParameterDoc("OnDocumentScanned", "EventCallback<DocumentOcrResult>", "null", "Fired when a document is scanned and entities are extracted."),
                new ComponentParameterDoc("OnCheckScanned", "EventCallback<CheckDraftOcrResult>", "null", "Fired when a paper check or bank draft is scanned.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwStreamingGrid",
            Category = "Fullstack .NET 10+",
            Description = "High-throughput streaming DataGrid consuming IAsyncEnumerable<TItem> server streams with real-time row pulse animations and 1M row virtualization.",
            RazorSnippet = "<TwStreamingGrid Title=\"Real-time Telemetry\" MaxBufferCapacity=\"5000\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"High-Throughput Streaming Engine (IAsyncEnumerable)\"", "Grid title in header toolbar."),
                new ComponentParameterDoc("Subtitle", "string", "\"Real-time telemetry pipeline consuming asynchronous server streams...\"", "Toolbar subtitle."),
                new ComponentParameterDoc("MaxBufferCapacity", "int", "5000", "Maximum items kept in memory before circular eviction."),
                new ComponentParameterDoc("Class", "string", "\"\"", "Optional CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwHydratedView",
            Category = "Fullstack .NET 10+",
            Description = "Universal zero-flicker SSR hydration container eliminating duplicate database queries and visual layout shift between prerender and client hydration.",
            RazorSnippet = "<TwHydratedView IsRestored=\"@_isRestored\" IsLoading=\"@_loading\"><p>Hydrated Content</p></TwHydratedView>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ChildContent", "RenderFragment", "null", "The hydrated content to render."),
                new ComponentParameterDoc("SkeletonTemplate", "RenderFragment", "null", "Custom skeleton placeholder to show during initial async load."),
                new ComponentParameterDoc("IsLoading", "bool", "false", "Whether the component is actively fetching initial state."),
                new ComponentParameterDoc("IsRestored", "bool", "false", "Whether the component state was restored from SSR prerender cache."),
                new ComponentParameterDoc("ShowHydrationBadge", "bool", "true", "Displays subtle SSR/Hydrated badge indicator in top right."),
                new ComponentParameterDoc("Class", "string", "\"\"", "Optional CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwFullstackForm",
            Category = "Fullstack .NET 10+",
            Description = "Dual-mode enterprise form engine supporting Static SSR Enhanced Form POST (zero JS) and Interactive Auto Client RPC seamlessly.",
            RazorSnippet = "<TwFullstackForm Title=\"Universal Fullstack Form\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"Universal Fullstack Form Engine\"", "Form card title."),
                new ComponentParameterDoc("Subtitle", "string", "\"Dual-mode form handling supporting Static SSR and Interactive Auto...\"", "Form card subtitle."),
                new ComponentParameterDoc("Class", "string", "\"\"", "Optional CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwFullstackHub",
            Category = "Fullstack .NET 10+",
            Description = "Full-stack developer architecture explorer, zero-flicker hydration workbench, and streaming playground for .NET 10+ Web Apps.",
            RazorSnippet = "<TwFullstackHub />",
            Parameters = new List<ComponentParameterDoc>()
        },
        new ComponentDocItem
        {
            Name = "TwUnionBoundary",
            Category = "Fullstack .NET 10+",
            Description = "C# 15 Discriminated Union boundary rendering exhaustive states (Loading, Success, Empty, Failure) with compile-time branch safety.",
            RazorSnippet = "<TwUnionBoundary TItem=\"string\" State=\"@state\"><Success Context=\"d\">@d</Success></TwUnionBoundary>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("State", "UiState<TItem>", "required", "Active algebraic union UI state."),
                new ComponentParameterDoc("Loading", "RenderFragment", "null", "Template rendered during loading state."),
                new ComponentParameterDoc("Success", "RenderFragment<TItem>", "null", "Template rendered on successful state with typed item context."),
                new ComponentParameterDoc("Empty", "RenderFragment", "null", "Template rendered on empty collection or record set."),
                new ComponentParameterDoc("Failure", "RenderFragment<UiState<TItem>.Failure>", "null", "Template rendered on error with failure details context.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwDonutCache",
            Category = "Fullstack .NET 10+",
            Description = "Blazor Donut Caching container that edge-caches 95%+ of static SSR page markup while dynamically perforating personalized donut holes.",
            RazorSnippet = "<TwDonutCache CacheKey=\"catalog\" Tags=\"@([\"catalog\"])\"><DynamicHole>...</DynamicHole></TwDonutCache>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("CacheKey", "string", "\"default_donut\"", "Unique cache key for the static shell segment."),
                new ComponentParameterDoc("Duration", "TimeSpan", "5 minutes", "Cache expiration time-to-live."),
                new ComponentParameterDoc("Tags", "List<string>", "[\"catalog\"]", "Cache invalidation tags for programmatic cache busting."),
                new ComponentParameterDoc("StaticContent", "RenderFragment", "null", "Static markup cached at the edge across users."),
                new ComponentParameterDoc("DynamicHole", "RenderFragment", "null", "Dynamic personalized slot evaluated per-request.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwPostQuantumVault",
            Category = "Fullstack .NET 10+",
            Description = "Post-Quantum Cryptography secure enclave implementing NIST FIPS 203 ML-KEM-768 key encapsulation and zero-trust authenticated encryption.",
            RazorSnippet = "<TwPostQuantumVault />",
            Parameters = new List<ComponentParameterDoc>()
        },
        new ComponentDocItem
        {
            Name = "TwTelemetryDashboard",
            Category = "Fullstack .NET 10+",
            Description = "Native OpenTelemetry (OTel) developer cockpit rendering real-time ActivitySource lifecycle spans, render latency waterfalls, and heap telemetry.",
            RazorSnippet = "<TwTelemetryDashboard />",
            Parameters = new List<ComponentParameterDoc>()
        },
        new ComponentDocItem
        {
            Name = "TwVectorMath",
            Category = "Fullstack .NET 10+",
            Description = "WebAssembly SIMD hardware acceleration and vector math engine evaluating high-throughput financial dot products, vector sums, and euclidean distance norms with comparative scalar telemetry.",
            RazorSnippet = "<TwVectorMath />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Class", "string?", "null", "Optional CSS utility classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwAspireHub",
            Category = "Fullstack .NET 10+",
            Description = ".NET Aspire 11 cloud-native orchestration cockpit with live microservice topology inspector, real-time port health probing, environment variable explorer, structured logs stream, and instant 1-click downloads for executable Program.cs AppHost and docker-compose.yml specifications.",
            RazorSnippet = "<TwAspireHub />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Class", "string?", "null", "Optional CSS utility classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwMaskInput",
            Category = "Forms & Controls",
            Description = "High-performance masked text input with slot validation, prompt guides, and Raw vs WithFormatting modes for Phone, SSN, Credit Card, and custom masks.",
            RazorSnippet = "<TwMaskInput Label=\"Phone\" Mask=\"(###) ###-####\" @bind-Value=\"@phone\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "string", "\"\"", "Two-way bound input value."),
                new ComponentParameterDoc("Mask", "string", "\"(###) ###-####\"", "Mask pattern (# digit, A letter, * alphanumeric)."),
                new ComponentParameterDoc("Prompt", "char", "'_'", "Prompt symbol for unfilled slots."),
                new ComponentParameterDoc("ValueMode", "MaskValueMode", "Raw", "Raw unformatted string vs formatted string with mask literals."),
                new ComponentParameterDoc("Preset", "MaskPreset", "Custom", "Preconfigured mask presets (Phone, Ssn, CreditCard, Date, ZipCode)."),
                new ComponentParameterDoc("Label", "string?", "null", "Input label."),
                new ComponentParameterDoc("PrefixIcon", "string?", "null", "Prefix icon symbol.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwRangeSlider",
            Category = "Forms & Controls",
            Description = "Dual-thumb interactive range slider with Lower/Upper thumb bounds, colored track highlights, ticks, and touch-friendly tap targets.",
            RazorSnippet = "<TwRangeSlider Label=\"Budget\" Min=\"0\" Max=\"1000\" @bind-Lower=\"@minVal\" @bind-Upper=\"@maxVal\" Prefix=\"$\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Lower", "double", "20", "Lower boundary thumb value (two-way bound)."),
                new ComponentParameterDoc("Upper", "double", "80", "Upper boundary thumb value (two-way bound)."),
                new ComponentParameterDoc("Min", "double", "0", "Minimum range limit."),
                new ComponentParameterDoc("Max", "double", "100", "Maximum range limit."),
                new ComponentParameterDoc("Step", "double", "1", "Granular increment step."),
                new ComponentParameterDoc("Prefix", "string?", "null", "Prefix symbol for values ($)."),
                new ComponentParameterDoc("Suffix", "string?", "null", "Suffix symbol for values (%).")
            }
        },
        new ComponentDocItem
        {
            Name = "TwHighlight",
            Category = "Typography & Utilities",
            Description = "Fast text search highlighter supporting case-sensitive matching, whole-word boundaries, match count badges, and Next/Prev navigation.",
            RazorSnippet = "<TwHighlight Text=\"@body\" SearchText=\"@query\" ShowNavigation=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Text", "string", "\"\"", "Full body text to search."),
                new ComponentParameterDoc("SearchText", "string", "\"\"", "Query term to highlight."),
                new ComponentParameterDoc("CaseSensitive", "bool", "false", "Whether search matches exact case."),
                new ComponentParameterDoc("WholeWord", "bool", "false", "Match entire words only."),
                new ComponentParameterDoc("ActiveIndex", "int", "0", "Index of currently focused active match."),
                new ComponentParameterDoc("ShowNavigation", "bool", "true", "Displays Next/Prev navigation bar with match count.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwTileManager",
            Category = "Cards, Grids & Bento Systems",
            Description = "Executive dashboard tile manager with responsive multi-column layouts, 1-click tile focus maximization, minimize collapse, and refresh callbacks.",
            RazorSnippet = "<TwTileManager Title=\"Executive Command\"><TwTile Title=\"ARR\" ColSpan=\"2\">...</TwTile></TwTileManager>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string?", "\"Executive Operations Command\"", "Toolbar title."),
                new ComponentParameterDoc("ColumnCount", "int", "4", "Number of grid columns (Quad, Triple, Dual)."),
                new ComponentParameterDoc("Gap", "string", "\"gap-4\"", "Tailwind gap utility."),
                new ComponentParameterDoc("ShowToolbar", "bool", "true", "Whether to show view switcher toolbar.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwNavDrawer",
            Category = "Navigation & Headers",
            Description = "Enterprise side navigation drawer featuring Mini icon rail (64px) vs Expanded full sidebar (260px) transitions, item badges, and overlay mode.",
            RazorSnippet = "<TwNavDrawer BrandTitle=\"Enterprise\"><TwNavDrawerItem Text=\"Dashboard\" Icon=\"📊\" /></TwNavDrawer>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("BrandTitle", "string", "\"Enterprise Hub\"", "Drawer header brand text."),
                new ComponentParameterDoc("IsMini", "bool", "false", "Mini collapsed icon-only rail mode."),
                new ComponentParameterDoc("IsOpen", "bool", "true", "Drawer open state."),
                new ComponentParameterDoc("IsOverlay", "bool", "false", "Modal overlay mode with backdrop."),
                new ComponentParameterDoc("AllowToggleMini", "bool", "true", "Show collapse/expand toggle button.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwPopover",
            Category = "Overlays & Modals",
            Description = "Anchored contextual floating popover with arrow pointer, 8-way placement, click or hover triggers, keyboard Escape dismiss, and optional header/footer slots.",
            RazorSnippet = "<TwPopover Title=\"Filter Settings\" Placement=\"TwPopoverPlacement.Bottom\"><AnchorContent><button class=\"btn\">Filter</button></AnchorContent><ChildContent><p>Options...</p></ChildContent></TwPopover>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string?", "null", "Optional header title text."),
                new ComponentParameterDoc("Placement", "TwPopoverPlacement", "TwPopoverPlacement.Bottom", "Placement relative to anchor (Top, Bottom, Left, Right, etc.)."),
                new ComponentParameterDoc("Trigger", "TwPopoverTrigger", "TwPopoverTrigger.Click", "Activation trigger mode (Click, Hover, Manual)."),
                new ComponentParameterDoc("ShowArrow", "bool", "true", "Render pointer arrow pointing to anchor."),
                new ComponentParameterDoc("Width", "string", "\"w-72\"", "Flyout container width utility."),
                new ComponentParameterDoc("IsOpen", "bool", "false", "Two-way bindable open state.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwWordBalloonOverlay",
            Category = "Overlays & Modals",
            Description = "Anchored contextual floating Word Balloon and speech bubble overlay with directional pointer tail, 11 shapes, 9 themes, 9 effects, and hover/click/always-visible trigger modes for avatars, metric cards, and action buttons.",
            RazorSnippet = "<TwWordBalloonOverlay Text=\"KABOOM!\" Shape=\"WordBalloonShape.SpeechBubble\" Theme=\"WordBalloonTheme.ComicYellow\" Placement=\"TwPopoverPlacement.Top\"><button class=\"btn\">Target</button></TwWordBalloonOverlay>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Anchor target content."),
                new ComponentParameterDoc("Text", "string", "\"KABOOM!\"", "Primary center word or phrase."),
                new ComponentParameterDoc("Kicker", "string?", "null", "Optional top kicker badge or sub-caption."),
                new ComponentParameterDoc("Shape", "WordBalloonShape", "WordBalloonShape.SpeechBubble", "Backdrop balloon shape geometry."),
                new ComponentParameterDoc("Effect", "WordBalloonEffect", "WordBalloonEffect.Outlined", "Typography text effect."),
                new ComponentParameterDoc("Theme", "WordBalloonTheme", "WordBalloonTheme.ComicYellow", "Color palette gradient theme."),
                new ComponentParameterDoc("Placement", "TwPopoverPlacement", "TwPopoverPlacement.Top", "Placement relative to anchor element."),
                new ComponentParameterDoc("Trigger", "TwPopoverTrigger", "TwPopoverTrigger.Hover", "Trigger activation mode (Hover, Click, Manual)."),
                new ComponentParameterDoc("IsOpen", "bool", "true", "Two-way bindable visibility state."),
                new ComponentParameterDoc("Persistent", "bool", "false", "Keep balloon permanently visible and prevent mouseleave dismissal."),
                new ComponentParameterDoc("Interactive", "bool", "true", "Enable pointer interaction and hover bridge over balloon."),
                new ComponentParameterDoc("CloseDelayMs", "int", "250", "Hover grace delay in milliseconds before closing."),
                new ComponentParameterDoc("AutoResizeBalloon", "bool", "true", "Auto-resize backdrop shape to fit text with safety margins."),
                new ComponentParameterDoc("BalloonScale", "double", "1.30", "Backdrop balloon scale multiplier (0.7x to 2.8x)."),
                new ComponentParameterDoc("PaddingX", "double", "36.0", "Horizontal internal padding."),
                new ComponentParameterDoc("PaddingY", "double", "18.0", "Vertical internal padding."),
                new ComponentParameterDoc("CustomWidth", "double", "0.0", "Manual fixed width override when AutoResizeBalloon is false."),
                new ComponentParameterDoc("CustomHeight", "double", "0.0", "Manual fixed height override when AutoResizeBalloon is false.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwSnackbar",
            Category = "Feedback & Alerts",
            Description = "Actionable bottom floating notification snackbar with action trigger callback, animated progress auto-dismiss countdown bar, pause on hover, and semantic color variants.",
            RazorSnippet = "<TwSnackbar Message=\"Invoice archived\" ActionText=\"Undo\" Variant=\"SemanticVariant.Info\" @bind-IsOpen=\"_showSnack\" OnAction=\"HandleUndo\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Message", "string", "\"\"", "Notification message."),
                new ComponentParameterDoc("ActionText", "string?", "null", "Optional action button text (e.g. Undo, Retry)."),
                new ComponentParameterDoc("DurationMs", "int", "4000", "Auto-dismiss duration in ms (0 for persistent)."),
                new ComponentParameterDoc("ShowProgress", "bool", "true", "Render animated countdown progress bar."),
                new ComponentParameterDoc("Variant", "SemanticVariant", "SemanticVariant.Default", "Color scheme (Default, Info, Success, Warning, Danger)."),
                new ComponentParameterDoc("Position", "TwSnackbarPosition", "TwSnackbarPosition.BottomCenter", "Viewport anchoring position.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwChatStudio",
            Category = "AI & Agentic Workflows",
            Description = "Full-featured modern conversational AI chat studio with multiline growing composer, staged file attachment preview chips, quick reply suggestion pills, feedback reactions (thumbs up/down), copy to clipboard, and live harmonic typing indicator.",
            RazorSnippet = "<TwChatStudio Title=\"AI Enterprise Studio\" Messages=\"_messages\" Suggestions=\"_prompts\" OnMessageSent=\"HandleSend\" OnRegenerate=\"HandleRegen\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"AI Enterprise Studio\"", "Chat studio title."),
                new ComponentParameterDoc("ModelBadge", "string", "\".NET 10+ WASM\"", "Model identification badge."),
                new ComponentParameterDoc("Messages", "List<ChatMessage>", "new()", "Chronological chat message stream."),
                new ComponentParameterDoc("Suggestions", "List<string>", "new()", "Quick reply suggestion chips above composer."),
                new ComponentParameterDoc("IsTyping", "bool", "false", "Show animated harmonic typing indicator."),
                new ComponentParameterDoc("TypingStatusText", "string", "\"AI is thinking...\"", "Typing indicator status message."),
                new ComponentParameterDoc("OnMessageSent", "EventCallback<ChatMessage>", "-", "Invoked when user sends prompt with staged attachments.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwPickList",
            Category = "Buttons, Forms & Controls",
            Description = "Dual-listbox item transfer picker with instant dual-list search filtering, move selected/all transfer actions, target list reordering, and item badges.",
            RazorSnippet = "<TwPickList TItem=\"string\" Source=\"_available\" @bind-Target=\"_selected\" SourceTitle=\"Roles\" TargetTitle=\"Assigned\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Source", "IEnumerable<TItem>?", "null", "Source available items collection."),
                new ComponentParameterDoc("Target", "IList<TItem>", "new List<TItem>()", "Selected target items collection."),
                new ComponentParameterDoc("SourceTitle", "string", "\"Available Items\"", "Left panel header title."),
                new ComponentParameterDoc("TargetTitle", "string", "\"Selected Items\"", "Right panel header title."),
                new ComponentParameterDoc("AllowFiltering", "bool", "true", "Show real-time filter search input on both lists."),
                new ComponentParameterDoc("ShowMoveAll", "bool", "true", "Show transfer all (>> and <<) buttons."),
                new ComponentParameterDoc("ShowReorder", "bool", "true", "Show up/down reorder buttons in target list.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwTimeSpanPicker",
            Category = "Advanced Forms & Inputs",
            Description = "Structured duration and timespan picker with Days, Hours, Minutes, Seconds input blocks, step increments, reset trigger, and quick preset duration pills.",
            RazorSnippet = "<TwTimeSpanPicker Label=\"SLA Duration\" @bind-Value=\"_sla\" ShowPresets=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "TimeSpan", "TimeSpan.Zero", "Two-way bindable duration value."),
                new ComponentParameterDoc("Label", "string?", "null", "Optional form label."),
                new ComponentParameterDoc("AllowDays", "bool", "true", "Whether to show the Days stepper unit."),
                new ComponentParameterDoc("AllowSeconds", "bool", "true", "Whether to show the Seconds stepper unit."),
                new ComponentParameterDoc("ShowPresets", "bool", "true", "Show quick duration preset buttons (15m, 1h, 1d, etc.)."),
                new ComponentParameterDoc("ShowReset", "bool", "true", "Show zero-reset button.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwVirtualKeyboard",
            Category = "Typography, Badges & Keys",
            Description = "On-screen virtual keyboard for kiosks, touch displays, POS terminals, and anti-keylogger secure PIN entry with QWERTY, Numeric, and scrambled PIN modes.",
            RazorSnippet = "<TwVirtualKeyboard @bind-Value=\"_enteredPin\" Mode=\"VirtualKeyboardMode.Pin\" IsPassword=\"true\" OnSubmit=\"HandlePinSubmit\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "string", "\"\"", "Two-way bindable typed text/PIN value."),
                new ComponentParameterDoc("Mode", "VirtualKeyboardMode", "VirtualKeyboardMode.Qwerty", "Key layout mode (Qwerty, Numeric, Pin)."),
                new ComponentParameterDoc("IsPassword", "bool", "false", "Mask display output with bullet points."),
                new ComponentParameterDoc("ShowDisplay", "bool", "true", "Show top value display bar with clear button."),
                new ComponentParameterDoc("AllowModeSwitch", "bool", "true", "Show mode switcher tabs."),
                new ComponentParameterDoc("OnSubmit", "EventCallback", "-", "Callback invoked when Enter / Submit key is pressed.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwWordCloud",
            Category = "Visualizations & Gauges",
            Description = "Pure SVG weighted term and keyword frequency cloud with Archimedean spiral positioning, harmonic color palettes, hover scaling, center word balloon overlay, and click callbacks.",
            RazorSnippet = "<TwWordCloud Words=\"_keywords\" Title=\"Sentiment Keywords\" SurroundingWordCount=\"12\" BalloonShape=\"WordBalloonShape.SpeechBubble\" OnWordClick=\"HandleWordClick\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Words", "IReadOnlyList<WordCloudItem>", "[]", "Weighted words collection with Text and Weight."),
                new ComponentParameterDoc("Title", "string?", "\"Keyword Density & Term Frequency\"", "Chart header title."),
                new ComponentParameterDoc("Width", "int", "640", "SVG viewBox width."),
                new ComponentParameterDoc("Height", "int", "320", "SVG viewBox height."),
                new ComponentParameterDoc("SurroundingWordCount", "int", "11", "Count of surrounding spiral words (0 to 25; 0 renders clean center balloon)."),
                new ComponentParameterDoc("SurroundingWordScale", "double", "1.0", "Font scale multiplier for surrounding words."),
                new ComponentParameterDoc("CenterWordScale", "double", "1.0", "Font scale multiplier for main center word."),
                new ComponentParameterDoc("AutoResizeBalloon", "bool", "true", "Auto-resize center balloon shape to fit word with generous safety padding."),
                new ComponentParameterDoc("BalloonScale", "double", "1.35", "Center balloon backdrop scale multiplier (0.8x to 2.5x)."),
                new ComponentParameterDoc("BalloonPaddingX", "double", "40.0", "Horizontal internal padding."),
                new ComponentParameterDoc("BalloonPaddingY", "double", "20.0", "Vertical internal padding."),
                new ComponentParameterDoc("BalloonCustomWidth", "double", "0.0", "Manual fixed width override when AutoResizeBalloon is false."),
                new ComponentParameterDoc("BalloonCustomHeight", "double", "0.0", "Manual fixed height override when AutoResizeBalloon is false."),
                new ComponentParameterDoc("BalloonShape", "WordBalloonShape", "WordBalloonShape.PillCapsule", "Center balloon backdrop shape geometry."),
                new ComponentParameterDoc("BalloonEffect", "WordBalloonEffect", "WordBalloonEffect.Bold", "Center word typography effect."),
                new ComponentParameterDoc("BalloonTheme", "WordBalloonTheme", "WordBalloonTheme.IndigoViolet", "Color palette gradient theme."),
                new ComponentParameterDoc("ShowSpeedSunburst", "bool", "false", "Render radial action speed rays background overlay."),
                new ComponentParameterDoc("ShowHalftoneDots", "bool", "false", "Render vintage Ben-Day pop-art halftone dots background overlay."),
                new ComponentParameterDoc("ShowActionSparks", "bool", "false", "Render action stars and sparkle glints overlay."),
                new ComponentParameterDoc("ShowSpecularSheen", "bool", "false", "Render glossy specular highlight sheen overlay."),
                new ComponentParameterDoc("ShowScanlineHud", "bool", "false", "Render cyber scanlines and corner HUD brackets overlay."),
                new ComponentParameterDoc("OnWordClick", "EventCallback<WordCloudItem>", "-", "Fired when user clicks any keyword.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwWordBalloonCreator",
            Category = "Visualizations & Gauges",
            Description = "Interactive Vector Word Balloon & Comic Banner Creator studio with 11 backdrop shapes, 9 typography effects, 9 color palettes, surrounding words count/scale sliders, visual graphic overlays (speed sunburst rays, Ben-Day halftone dots, action sparks, specular sheen, cyber scanlines, kicker ribbons), and 1-click clean normal balloon mode.",
            RazorSnippet = "<TwWordBalloonCreator />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Class", "string", "\"\"", "Additional CSS class names.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwSankeyChart",
            Category = "Visualizations & Gauges",
            Description = "Pure SVG flow and migration diagram rendering source/target nodes and cubic bezier ribbons with thickness proportional to flow volume.",
            RazorSnippet = "<TwSankeyChart Nodes=\"_nodes\" Links=\"_links\" Title=\"Pipeline Throughput Flow\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Nodes", "IReadOnlyList<SankeyNode>", "[]", "Nodes collection with Id, Label, and Color."),
                new ComponentParameterDoc("Links", "IReadOnlyList<SankeyLink>", "[]", "Ribbon links collection with SourceId, TargetId, and Value."),
                new ComponentParameterDoc("Title", "string?", "\"Enterprise Pipeline & Resource Flow\"", "Chart header title."),
                new ComponentParameterDoc("Width", "int", "700", "SVG viewBox width."),
                new ComponentParameterDoc("Height", "int", "360", "SVG viewBox height."),
                new ComponentParameterDoc("OnNodeClick", "EventCallback<SankeyNode>", "-", "Callback invoked when a node is clicked.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwFileManager",
            Category = "Cards, Grids & Bento Systems",
            Description = "Enterprise cloud and asset file manager with folder tree navigation, breadcrumbs, grid/table view modes, search, storage quota meter, and upload/delete actions.",
            RazorSnippet = "<TwFileManager Items=\"_files\" CurrentPath=\"/\" OnFileSelected=\"HandleFileSelected\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Items", "List<FileItem>", "new()", "Files and folders collection."),
                new ComponentParameterDoc("CurrentPath", "string", "\"/\"", "Current active directory path."),
                new ComponentParameterDoc("OnFileSelected", "EventCallback<FileItem>", "-", "Fired when a file or folder is selected."),
                new ComponentParameterDoc("OnFileOpened", "EventCallback<FileItem>", "-", "Fired when a file or folder is double-clicked."),
                new ComponentParameterDoc("OnUpload", "EventCallback", "-", "Fired when user clicks Upload button.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwSpatialCard",
            Category = "Cards, Grids & Bento Systems",
            Description = "2027 volumetric 3D card featuring real-time gyroscopic mouse tilt, dynamic specular refraction glare, multi-layer Z-depth elevation, and tactile organic blur.",
            RazorSnippet = "<TwSpatialCard Title=\"Quantum Enclave\" MaxTiltDeg=\"14\">...content...</TwSpatialCard>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string?", "null", "Card title header."),
                new ComponentParameterDoc("Subtitle", "string?", "null", "Card subtitle text."),
                new ComponentParameterDoc("BadgeText", "string?", "null", "Elevated 3D pill badge text."),
                new ComponentParameterDoc("MaxTiltDeg", "double", "12.0", "Maximum rotation angle degrees on tilt."),
                new ComponentParameterDoc("GlowGradient", "string", "\"from-brand-500 to-indigo-600\"", "Tailwind gradient classes for dynamic ambient aura.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwFluidMesh",
            Category = "Visualizations & Gauges",
            Description = "Tactile generative fluid gradient shader with organic motion and real-time psychological emotional state resonance tuning for operator cognitive optimization.",
            RazorSnippet = "<TwFluidMesh @bind-Mood=\"_operatorMood\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Mood", "EmotionalState", "EmotionalState.DeepFocus", "Active emotional and cognitive resonance frequency preset."),
                new ComponentParameterDoc("MoodChanged", "EventCallback<EmotionalState>", "-", "Fired when operator toggles sentiment mood.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwPredictiveIntent",
            Category = "Forms & Inputs",
            Description = "2027 anticipatory state machine demonstrating zero-click UX: gaze focal tracking, tactile biometric pulse sensor, and automated intent action synthesis.",
            RazorSnippet = "<TwPredictiveIntent />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Class", "string", "\"\"", "Optional CSS classes for outer container.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwModernProgress",
            Category = "Progress & Steppers",
            Description = "Cool modern progress indicators: SVG glowing circular rings, dashboard speedometer arcs, multi-segment capsule pill steppers, neon liquid shimmer, and Apple Fitness style triple concentric activity rings.",
            RazorSnippet = "<TwModernProgress Type=\"TwModernProgress.ProgressType.RadialRing\" Value=\"75\" Label=\"Done\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Type", "ProgressType", "ProgressType.RadialRing", "Visual variant: RadialRing, SpeedometerArc, SegmentedCapsule, NeonLiquidGlow, ConcentricMultiRing."),
                new ComponentParameterDoc("Value", "int", "75", "Primary progress percentage (0-100)."),
                new ComponentParameterDoc("SecondaryValue", "int", "60", "Secondary ring percentage for concentric mode."),
                new ComponentParameterDoc("TertiaryValue", "int", "92", "Tertiary ring percentage for concentric mode."),
                new ComponentParameterDoc("Label", "string?", "null", "Text description or phase label."),
                new ComponentParameterDoc("PixelSize", "int", "110", "Diameter size in pixels for ring/arc modes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwUnionStudio",
            Category = "Architecture & State",
            Description = "C# 15 Discriminated Union boundary studio with exhaustive pattern matching, algebraic UI states (Idle, Loading, Success, Error), latency simulation, and retry triggers.",
            RazorSnippet = "<TwUnionStudio />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Optional CSS class modifiers.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwPdfFormFiller",
            Category = "Documents & Forms",
            Description = "Interactive PDF form filler and attestation engine supporting initial stamping, legal full name signing, UTC timestamps, and corporate authority checkboxes.",
            RazorSnippet = "<TwPdfFormFiller />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Optional CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwAguiFormCanvas",
            Category = "Forms & Inputs",
            Description = "AG-UI Adaptive Generative UI form engine with AI confidence percentages, 2-way diff binding, dynamic field schema synthesis, and human-in-the-loop gating.",
            RazorSnippet = "<TwAguiFormCanvas Schema=\"@schema\" OnSubmit=\"HandleSubmit\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Schema", "AguiFormSchema", "null", "The synthesized adaptive form schema."),
                new ComponentParameterDoc("OnSubmit", "EventCallback<Dictionary<string, object>>", "null", "Callback on approved form submission.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwPwaInstallButton",
            Category = "Navigation & Shell",
            Description = "1-Click Progressive Web App (PWA) installation button with native beforeinstallprompt detection, standalone status badges, and cross-platform installation modal.",
            RazorSnippet = "<TwPwaInstallButton />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Class", "string?", "null", "Optional CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwActivityMeter",
            Category = "Data Visualization",
            Description = "High-density system activity meter with pulse rate indicators, status badge, and customizable threshold levels.",
            RazorSnippet = "<TwActivityMeter Value=\"85\" Max=\"100\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Value", "double", "0", "Current numeric activity level."),
                new ComponentParameterDoc("Max", "double", "100", "Maximum activity threshold scale.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwAsyncState",
            Category = "Architecture & State",
            Description = "Declarative async boundary container for handling loading skeletons, error states with retry buttons, empty states, and success content.",
            RazorSnippet = "<TwAsyncState State=\"@state\"><Success>Content</Success></TwAsyncState>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("State", "AsyncOperationState", "-", "The active state enum.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwBirthdayBanner",
            Category = "Gamification & Celebrations",
            Description = "Company-wide birthday alert banner with quick emoji reaction buttons and celebration confetti trigger.",
            RazorSnippet = "<TwBirthdayBanner EmployeeName=\"Jane Doe\" Department=\"Engineering\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("EmployeeName", "string", "\"\"", "Recipient employee name."),
                new ComponentParameterDoc("Department", "string", "\"\"", "Employee department.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwBirthdayCard",
            Category = "Gamification & Celebrations",
            Description = "Role-tailored interactive 3D card with inside jokes, particle reveals, and peer signing wall.",
            RazorSnippet = "<TwBirthdayCard RecipientName=\"Jane Doe\" Role=\"Architect\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("RecipientName", "string", "\"\"", "Card recipient name.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwCelebrationModal",
            Category = "Modals & Overlays",
            Description = "Modal dialog with celebration confetti burst animation, congratulatory header, and reward claim buttons.",
            RazorSnippet = "<TwCelebrationModal @bind-IsOpen=\"isOpen\" Title=\"Congratulations!\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("IsOpen", "bool", "false", "Whether the modal is visible.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwDashboard",
            Category = "Dashboards & Layouts",
            Description = "Executive multi-widget draggable dashboard layout with GridStack.js integration and responsive card snapping.",
            RazorSnippet = "<TwDashboard />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("CustomClass", "string", "\"\"", "Optional CSS classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwNavDrawerItem",
            Category = "Navigation & Shell",
            Description = "Navigation drawer menu item with icon, label, active state indicator, and optional counter badge.",
            RazorSnippet = "<TwNavDrawerItem Icon=\"📊\" Label=\"Analytics\" IsActive=\"true\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Label", "string", "\"\"", "Menu item label text."),
                new ComponentParameterDoc("Icon", "string", "\"\"", "Menu item icon."),
                new ComponentParameterDoc("IsActive", "bool", "false", "Whether the item is currently active.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwTile",
            Category = "Cards & Bento",
            Description = "Compact KPI metric tile with inline trend badge, subtext, and zero-JS sparkline micro-chart.",
            RazorSnippet = "<TwTile Title=\"Active Users\" Value=\"4,820\" Change=\"+12%\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "\"\"", "Tile title text."),
                new ComponentParameterDoc("Value", "string", "\"\"", "Primary metric value text.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwUser",
            Category = "Styling Engine & Multi-Part Primitives",
            Description = "Multi-part user profile badge with avatar/initials, status dot indicator, primary name, secondary handle/role description, and per-slot customization via TwUserSlots.",
            RazorSnippet = "<TwUser Name=\"Elena Rostova\" Description=\"Principal UI Architect\" Initials=\"ER\" Status=\"online\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Name", "string", "\"\"", "User display name."),
                new ComponentParameterDoc("Description", "string?", "null", "Secondary role or handle description."),
                new ComponentParameterDoc("AvatarUrl", "string?", "null", "Optional avatar image URL."),
                new ComponentParameterDoc("Initials", "string?", "null", "Fallback avatar initials."),
                new ComponentParameterDoc("Status", "string?", "null", "Presence status dot (online, busy, away, offline)."),
                new ComponentParameterDoc("Classes", "TwUserSlots?", "null", "Multi-part slot class overrides (Base, Avatar, StatusDot, Wrapper, Name, Description)."),
                new ComponentParameterDoc("Class", "string?", "null", "Root container utility overrides resolved via TwMerge.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwFloatingLabelInput",
            Category = "Styling Engine & Multi-Part Primitives",
            Description = "Animated peer-floating label input supporting Outlined, Filled, and Standard underline variants, multi-part TwFloatingInputSlots, and automatic registration inside a parent TwValidatorGroup.",
            RazorSnippet = "<TwFloatingLabelInput Label=\"Workspace Slug\" Variant=\"FloatingLabelVariant.Outlined\" @bind-Value=\"_slug\" Required=\"true\" MinLength=\"4\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Label", "string", "\"Label\"", "Floating label text."),
                new ComponentParameterDoc("Value", "string?", "null", "Two-way bound input value."),
                new ComponentParameterDoc("Variant", "FloatingLabelVariant", "FloatingLabelVariant.Outlined", "Visual style (Outlined, Filled, Standard)."),
                new ComponentParameterDoc("Status", "ValidationStatus", "ValidationStatus.None", "Explicit validation status when used standalone."),
                new ComponentParameterDoc("Required", "bool", "false", "Marks field as required during validation."),
                new ComponentParameterDoc("MinLength", "int?", "null", "Minimum character length rule."),
                new ComponentParameterDoc("Pattern", "string?", "null", "Regular expression validation pattern."),
                new ComponentParameterDoc("Validator", "Func<string?, string?>?", "null", "Custom validation delegate returning null when valid or an error message."),
                new ComponentParameterDoc("HelperText", "string?", "null", "Neutral helper caption below the input."),
                new ComponentParameterDoc("ErrorMessage", "string?", "null", "Custom error message when validation fails."),
                new ComponentParameterDoc("SuccessMessage", "string?", "null", "Caption displayed when validation succeeds."),
                new ComponentParameterDoc("Classes", "TwFloatingInputSlots?", "null", "Multi-part slot class overrides (Base, Input, Label, HelperText).")
            }
        },
        new ComponentDocItem
        {
            Name = "TwValidatorGroup",
            Category = "Styling Engine & Multi-Part Primitives",
            Description = "Decoupled cascading reactive validation coordinator that tracks child TwFloatingLabelInput instances and exposes ValidateAll(), ResetAll(), ValidCount, InvalidCount, and IsAllValid.",
            RazorSnippet = "<TwValidatorGroup @ref=\"_validatorGroup\"><TwFloatingLabelInput Label=\"Email\" @bind-Value=\"_email\" Required=\"true\" /></TwValidatorGroup>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Child form inputs subscribing to the cascading validation scope."),
                new ComponentParameterDoc("OnStatusChanged", "EventCallback<bool>", "default", "Fires whenever aggregate validation status changes."),
                new ComponentParameterDoc("Class", "string?", "null", "Container CSS utility classes.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwHoverCard",
            Category = "Styling Engine & Multi-Part Primitives",
            Description = "Hover-intent preview card with configurable open/close delays, an invisible cursor bridge so users can move into the floating card without it closing, directional arrow, and TwHoverCardSlots.",
            RazorSnippet = "<TwHoverCard><TriggerContent><span>Hover Profile</span></TriggerContent><ChildContent><p>Rich preview content</p></ChildContent></TwHoverCard>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("TriggerContent", "RenderFragment?", "null", "Inline trigger element that opens the card on hover or focus."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Floating preview card body content."),
                new ComponentParameterDoc("Placement", "TwPopoverPlacement", "TwPopoverPlacement.Bottom", "Floating card placement relative to trigger."),
                new ComponentParameterDoc("OpenDelayMs", "int", "120", "Hover intent delay in milliseconds before opening."),
                new ComponentParameterDoc("CloseDelayMs", "int", "220", "Grace delay in milliseconds before closing after mouse leave."),
                new ComponentParameterDoc("ShowArrow", "bool", "true", "Whether to render the directional pointer arrow."),
                new ComponentParameterDoc("Classes", "TwHoverCardSlots?", "null", "Multi-part slot overrides (Base, Trigger, Content, Arrow).")
            }
        },
        new ComponentDocItem
        {
            Name = "TwSonner",
            Category = "Styling Engine & Multi-Part Primitives",
            Description = "3D stacked toast notification deck with depth-scaled cards, hover/programmatic fan-out expansion, semantic rich colors, and async PushPromiseAsync loading-to-success transitions.",
            RazorSnippet = "<TwSonner @ref=\"_sonner\" Items=\"_toasts\" Expanded=\"false\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Items", "List<SonnerToastItem>", "new()", "Active toast notifications in the stack."),
                new ComponentParameterDoc("Expanded", "bool", "false", "Programmatically expands the 3D toast stack into full list view."),
                new ComponentParameterDoc("MaxVisible", "int", "4", "Maximum number of toasts rendered in the visible stack."),
                new ComponentParameterDoc("RichColors", "bool", "true", "Applies semantic color palettes for Success, Warning, Danger, and Info toasts."),
                new ComponentParameterDoc("OnActionClicked", "EventCallback<SonnerToastItem>", "default", "Callback invoked when a toast action button is clicked.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwMenubar",
            Category = "Styling Engine & Multi-Part Primitives",
            Description = "Desktop-grade horizontal application menubar with click-to-open menus, automatic hover-switching between sibling menus, keyboard shortcut badges, separators, and checkable items.",
            RazorSnippet = "<TwMenubar Menus=\"@_menus\" OnItemSelected=\"HandleCommand\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Menus", "List<MenubarMenu>", "new()", "Top-level menu definitions and their dropdown items."),
                new ComponentParameterDoc("OnItemSelected", "EventCallback<MenubarItem>", "default", "Invoked when a menu item is clicked or toggled."),
                new ComponentParameterDoc("Class", "string?", "null", "Optional root utility class overrides.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwToggleGroup",
            Category = "Styling Engine & Multi-Part Primitives",
            Description = "Single or multiple selection segmented toggle bar with WAI-ARIA aria-pressed states, icon + label items, and Outline/Soft variants.",
            RazorSnippet = "<TwToggleGroup Options=\"@_options\" Multiple=\"true\" @bind-SelectedValues=\"_selected\" Variant=\"ButtonVariant.Soft\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Options", "List<ToggleItem>", "new()", "Toggle button items (Value, Label, Icon, Tooltip, Disabled)."),
                new ComponentParameterDoc("Multiple", "bool", "false", "Allows selecting multiple options simultaneously."),
                new ComponentParameterDoc("Value", "string?", "null", "Selected value in single-select mode."),
                new ComponentParameterDoc("SelectedValues", "HashSet<string>", "new()", "Selected values set in multi-select mode."),
                new ComponentParameterDoc("Variant", "ButtonVariant", "ButtonVariant.Outline", "Visual container variant (Outline or Soft)."),
                new ComponentParameterDoc("Size", "ComponentSize", "ComponentSize.Sm", "Button size (Xs, Sm, Md, Lg).")
            }
        },
        new ComponentDocItem
        {
            Name = "TwMockup",
            Category = "Styling Engine & Multi-Part Primitives",
            Description = "Device and application frame container supporting Browser (with traffic lights and URL bar), Phone (with Dynamic Island notch), Window, and Terminal Code (with line prefixes and highlights) modes.",
            RazorSnippet = "<TwMockup Kind=\"MockupKind.Browser\" Url=\"https://blazortwui.dev\"><p>Preview</p></TwMockup>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Kind", "MockupKind", "MockupKind.Browser", "Mockup frame style (Browser, Phone, Window, Code)."),
                new ComponentParameterDoc("Url", "string", "\"https://blazortwui.dev/studio\"", "URL displayed in Browser mode address bar."),
                new ComponentParameterDoc("Title", "string", "\"BlazorTwUI — Studio\"", "Window/Terminal title bar caption."),
                new ComponentParameterDoc("CodeLines", "List<MockupCodeLine>?", "null", "Terminal command/output lines when Kind is Code."),
                new ComponentParameterDoc("ChildContent", "RenderFragment?", "null", "Viewport body content."),
                new ComponentParameterDoc("Class", "string?", "null", "Root frame utility class overrides resolved via TwMerge.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwSwap",
            Category = "Styling Engine & Multi-Part Primitives",
            Description = "Animated two-state toggle button that transitions between OnContent and OffContent using CSS Rotate, 3D Flip, or Fade animations with WAI-ARIA aria-pressed compliance.",
            RazorSnippet = "<TwSwap @bind-Active=\"_active\" Animation=\"SwapAnimation.Rotate\"><OnContent>☀️</OnContent><OffContent>🌙</OffContent></TwSwap>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Active", "bool", "false", "Two-way bound active state."),
                new ComponentParameterDoc("Animation", "SwapAnimation", "SwapAnimation.Rotate", "Transition style (Rotate, Flip, Fade)."),
                new ComponentParameterDoc("OnContent", "RenderFragment?", "null", "Content rendered when Active is true."),
                new ComponentParameterDoc("OffContent", "RenderFragment?", "null", "Content rendered when Active is false."),
                new ComponentParameterDoc("Class", "string?", "null", "Button utility class overrides.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwChatBubble",
            Category = "Styling Engine & Multi-Part Primitives",
            Description = "Semantic start/end aligned conversation bubble with avatar initials, sender name, timestamp header, delivery status footer, and semantic color variants.",
            RazorSnippet = "<TwChatBubble Sender=\"System\" Initials=\"BT\" Timestamp=\"14:32\" Message=\"Deployment ready!\" FooterStatus=\"Delivered\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Sender", "string?", "null", "Sender display name."),
                new ComponentParameterDoc("Timestamp", "string?", "null", "Message timestamp caption."),
                new ComponentParameterDoc("Initials", "string?", "null", "Avatar initials."),
                new ComponentParameterDoc("Message", "string?", "null", "Message text content."),
                new ComponentParameterDoc("FooterStatus", "string?", "null", "Delivery or read-receipt status text."),
                new ComponentParameterDoc("IsEnd", "bool", "false", "Right-aligns the chat bubble for outgoing messages."),
                new ComponentParameterDoc("Variant", "SemanticVariant", "SemanticVariant.Default", "Bubble color variant (Default, Primary, Success, Warning, Danger, Info).")
            }
        },
        new ComponentDocItem
        {
            Name = "TwCompactStatCard",
            Category = "Page Templates & UI Building Blocks",
            Description = "Minimalist enterprise KPI metric card with uppercase title, large numeric value, percentage delta badge (+18.4%), and pure native mathematical SVG sparklines.",
            RazorSnippet = "<TwCompactStatCard Title=\"Gross Volume\" Value=\"$14.8M\" Delta=\"+18.4%\" SparklineData=\"@_inflowTrend\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "string.Empty", "Stat card headline label."),
                new ComponentParameterDoc("Value", "string", "string.Empty", "Stat value formatted for display."),
                new ComponentParameterDoc("Caption", "string?", "null", "Optional descriptive subtitle."),
                new ComponentParameterDoc("Delta", "string?", "null", "Percentage or numerical delta text."),
                new ComponentParameterDoc("IsPositive", "bool", "true", "Whether the delta is positive (green) or negative (red)."),
                new ComponentParameterDoc("SparklineData", "List<double>?", "null", "List of data points to render as an inline SVG sparkline."),
                new ComponentParameterDoc("SparklineColor", "string", "#10b981", "Stroke color for the SVG sparkline polyline.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwLandingHero",
            Category = "Page Templates & UI Building Blocks",
            Description = "Kinetic split marketing hero with headline, animated announcement pill, action buttons, social proof avatar stack, and floating preview card with realistic shadow.",
            RazorSnippet = "<TwLandingHero Headline=\"One Platform. Infinite Enterprise Velocity.\" BadgeText=\"v1.0 Live\">\n    <ActionSlot><button class=\"btn-primary\">Get Started</button></ActionSlot>\n</TwLandingHero>",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Headline", "string", "string.Empty", "Main hero headline text."),
                new ComponentParameterDoc("BadgeText", "string?", "null", "Pill announcement text at top of hero."),
                new ComponentParameterDoc("Description", "string?", "null", "Hero supporting copy."),
                new ComponentParameterDoc("ActionSlot", "RenderFragment?", "null", "Slot for primary and secondary action buttons."),
                new ComponentParameterDoc("SocialProofSlot", "RenderFragment?", "null", "Slot for avatar ratings or metrics."),
                new ComponentParameterDoc("PreviewSlot", "RenderFragment?", "null", "Elevated preview card or screenshot slot.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwPricingSection",
            Category = "Page Templates & UI Building Blocks",
            Description = "3-tier interactive pricing matrix with monthly/annual billing switcher (20% discount calculation), highlighted popular plan, and feature checklists.",
            RazorSnippet = "<TwPricingSection Title=\"Simple, Transparent Pricing\" OnSelectPlan=\"HandlePlanSelection\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "Predictable Plans for Every Stage", "Pricing section main heading."),
                new ComponentParameterDoc("Subtitle", "string?", "null", "Supporting description text."),
                new ComponentParameterDoc("BadgeText", "string", "Simple Pricing", "Category eyebrow label."),
                new ComponentParameterDoc("Plans", "List<PricingPlan>", "Default 3 plans", "Collection of pricing tier plans."),
                new ComponentParameterDoc("OnSelectPlan", "EventCallback<PricingPlan>", "null", "Callback triggered when a user selects a plan.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwPromptComposer",
            Category = "Page Templates & UI Building Blocks",
            Description = "Auto-expanding AI prompt box with suggestion chips, file attachment trigger, voice dictation toggle, and dual-plane execution hooks.",
            RazorSnippet = "<TwPromptComposer Placeholder=\"Ask an enterprise agent...\" SuggestedPrompts=\"@_prompts\" OnSend=\"HandleSend\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Placeholder", "string", "Ask an enterprise agent...", "Input placeholder text."),
                new ComponentParameterDoc("Rows", "int", "3", "Initial textarea row height."),
                new ComponentParameterDoc("IsLoading", "bool", "false", "Shows spinning loading state when processing."),
                new ComponentParameterDoc("SuggestedPrompts", "List<string>?", "null", "Clickable starter prompt pills."),
                new ComponentParameterDoc("OnSend", "EventCallback<string>", "null", "Event callback invoked when prompt is submitted.")
            }
        },
        new ComponentDocItem
        {
            Name = "TwActivityTimeline",
            Category = "Page Templates & UI Building Blocks",
            Description = "Vertical audit activity stream with status dots (Info, Success, Warning, Danger), timestamps, descriptions, and metadata badges.",
            RazorSnippet = "<TwActivityTimeline Title=\"Live Audit Trail\" Events=\"@_events\" />",
            Parameters = new List<ComponentParameterDoc>
            {
                new ComponentParameterDoc("Title", "string", "Audit & Event Activity", "Timeline card title."),
                new ComponentParameterDoc("Subtitle", "string?", "null", "Supporting subtitle text."),
                new ComponentParameterDoc("Events", "List<TimelineEvent>", "empty", "List of timeline events to display.")
            }
        },
        new ComponentDocItem
        {
            Name = "FintechAnalyticsDashboard",
            Category = "Page Templates & UI Building Blocks",
            Description = "Turnkey Fintech and Payment Treasury Cockpit featuring real-time settlement KPIs, multi-series cash flow line chart, treasury liquidity donut, and clearance ledger table.",
            RazorSnippet = "<FintechAnalyticsDashboard />",
            Parameters = new List<ComponentParameterDoc>()
        },
        new ComponentDocItem
        {
            Name = "AiOperationsTokenMonitor",
            Category = "Page Templates & UI Building Blocks",
            Description = "Turnkey AI Operations & Control Plane with real-time token throughput KPIs, latency percentiles, human-in-the-loop approval gates, prompt composer, and live execution trail.",
            RazorSnippet = "<AiOperationsTokenMonitor />",
            Parameters = new List<ComponentParameterDoc>()
        },
        new ComponentDocItem
        {
            Name = "SaaSProductLanding",
            Category = "Page Templates & UI Building Blocks",
            Description = "Turnkey SaaS product marketing page assembling kinetic hero, bento feature matrix, 3-tier pricing matrix, and customer testimonials.",
            RazorSnippet = "<SaaSProductLanding />",
            Parameters = new List<ComponentParameterDoc>()
        },
        new ComponentDocItem
        {
            Name = "TemplatesGallery",
            Category = "Page Templates & UI Building Blocks",
            Description = "Interactive preview gallery with live viewport frames (Desktop, Tablet, Mobile) for browsing and testing Page Templates and UI Building Blocks.",
            RazorSnippet = "<TemplatesGallery />",
            Parameters = new List<ComponentParameterDoc>()
        }
    };

    public static int TotalCount => _items.Count;
    public int TotalComponentCount => _items.Count;
    public IReadOnlyList<ComponentDocItem> GetAll() => _items;
    public IReadOnlyList<string> GetCategories() => _items.Select(i => i.Category).Distinct().ToList();
    public ComponentDocItem? GetByName(string name) => _items.FirstOrDefault(i => i.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase));
}
