"""
End-to-end check of the thread list, the live AG-UI SSE transport, the Agent Kit and the inputs and pickers,
in a real browser with screenshots and an axe-core audit.

    python tests/e2e/agui_server.py 5300          # live test agent
    dotnet run --urls http://127.0.0.1:5199       # the app
    python tests/e2e/agent_kit_e2e.py [base_url] [agui_url]
"""
import json
import os
import sys
import urllib.request

from playwright.sync_api import expect, sync_playwright

BASE = sys.argv[1] if len(sys.argv) > 1 else "http://127.0.0.1:5199"
AGUI = sys.argv[2] if len(sys.argv) > 2 else "http://127.0.0.1:5300"
ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
SHOTS = os.path.join(ROOT, "docs", "images")
AXE = os.path.join(ROOT, "node_modules", "axe-core", "axe.min.js")
os.makedirs(SHOTS, exist_ok=True)
problems = []


def check(condition, message):
    print(("PASS " if condition else "FAIL ") + message)
    if not condition:
        problems.append(message)


def shot(page, selector, name):
    path = os.path.join(SHOTS, f"agent-kit-{name}.png")
    page.locator(selector).first.screenshot(path=path)
    print("     screenshot:", path)


def server_log():
    return json.load(urllib.request.urlopen(AGUI + "/agui/_log"))


def open_ai_tab(page, subtab):
    page.goto(BASE, wait_until="networkidle", timeout=60000)
    page.locator("[data-testid=jump-bar] >> text=Smart AGUI").click()
    page.locator(f"[data-testid=subtab-{subtab}]").click()


def axe_violations(page, scope):
    page.add_script_tag(path=AXE)
    return page.evaluate(
        """async (scope) => {
            const r = await axe.run(scope, { runOnly: ['wcag2a', 'wcag2aa', 'wcag21aa', 'wcag22aa'] });
            return r.violations.map(v => ({ id: v.id, impact: v.impact, help: v.help, nodes: v.nodes.slice(0, 3).map(n => n.target.join(' ')) }));
        }""", scope)


def audit(page, scope, label):
    v = axe_violations(page, scope)
    for x in v:
        print("     axe:", x["impact"], x["id"], "-", x["help"], x["nodes"])
    check(len(v) == 0, f"axe-core: no WCAG 2.x A/AA violations in {label} ({len(v)} found)")


def require_200(url, what, attempts=60):
    """The UI is only started once the API answers 200; otherwise every failure below would be noise."""
    import time
    for _ in range(attempts):
        try:
            if urllib.request.urlopen(url, timeout=3).status == 200:
                print(f"PASS {what} answered 200")
                return
        except Exception:
            pass
        time.sleep(1)
    print(f"FAIL {what} did not answer 200 at {url}; not starting the UI")
    sys.exit(2)


require_200(AGUI + "/agui/_log", "AG-UI API")
require_200(BASE + "/", "app host")

with sync_playwright() as p:
    browser = p.chromium.launch(headless=True)
    context = browser.new_context(viewport={"width": 1500, "height": 1100}, accept_downloads=True)
    page = context.new_page()
    errors = []
    page.on("console", lambda m: errors.append(m.text) if m.type == "error" else None)
    page.on("pageerror", lambda e: errors.append(str(e)))

    # ---------- 1. Threads + suggestions + feedback on the simulated agent ----------
    open_ai_tab(page, "agentops")
    page.locator("[data-testid=agent-console]").wait_for(timeout=15000)

    page.locator("[data-testid=chat-suggestion]").nth(2).click()
    check(page.locator("[data-testid=prompt]").input_value() == "Check the status of order 4821.", "suggestion chip fills the prompt")
    page.locator("[data-testid=prompt]").fill("Please refund order 4821, the desk arrived with a cracked leg.")
    page.locator("[data-testid=run]").click()
    page.locator("[data-testid=approval-gate]").wait_for(timeout=15000)
    check(page.locator("[data-testid=agent-orb]").first.get_attribute("aria-label").endswith("waiting for approval"), "orb shows waiting-for-approval as text")
    page.locator("[data-testid=approve]").click()
    expect(page.locator("[data-testid=run-status]")).to_have_text("Finished", timeout=15000)

    check(page.locator("[data-testid=thread-item]").count() == 1, "the run was saved as a thread")
    title = page.locator("[data-testid=thread-title]").first.inner_text()
    check(title.startswith("Please refund order 4821"), f"thread was titled from the first message ({title!r})")

    page.locator("[data-testid=feedback-down]").first.click()
    check(page.locator("[data-testid=feedback-down]").first.get_attribute("aria-pressed") == "true", "thumbs-down is pressed")
    page.locator("[data-testid=feedback-reason]").fill("slow")
    page.locator("[data-testid=feedback-down]").first.click()
    check(page.locator("[data-testid=feedback-down]").first.get_attribute("aria-pressed") == "false", "second press clears feedback")

    # Reload: the thread survives (localStorage), selecting it restores the transcript.
    open_ai_tab(page, "agentops")
    page.locator("[data-testid=thread-item]").first.wait_for(timeout=15000)
    check(page.locator("[data-testid=thread-item]").count() == 1, "thread survived a page reload")
    page.locator("[data-testid=thread-select]").first.click()
    page.locator("[data-testid=thread-history]").first.wait_for(timeout=5000)
    check(page.locator("[data-testid=thread-history]").count() == 2, "opening the thread restores the saved transcript")

    # rename / pin / archive
    page.locator("[data-testid=thread-rename]").first.click()
    page.locator("[data-testid=thread-rename-input]").fill("Cracked desk refund")
    page.locator("[data-testid=thread-rename-save]").click()
    check(page.locator("[data-testid=thread-title]").first.inner_text() == "Cracked desk refund", "thread renamed")
    page.locator("[data-testid=thread-pin]").first.click()
    check(page.locator("[data-testid=thread-item]").first.get_attribute("data-pinned") == "true", "thread pinned")
    page.locator("[data-testid=thread-new]").click()
    page.locator("[data-testid=thread-search]").fill("cracked")
    check(page.locator("[data-testid=thread-item]").count() == 1, "search narrows the list")
    page.locator("[data-testid=thread-search]").fill("")
    shot(page, "[data-testid=agent-console]", "1-threads")

    # ---------- 2. Live AG-UI over SSE, with the server waiting at the gate ----------
    page.locator("input[type=radio]").nth(1).check()
    page.locator("[data-testid=agui-url]").fill(AGUI + "/agui/run")
    page.locator("[data-testid=agui-connect]").click()
    expect(page.locator("[data-testid=transport-label]")).to_contain_text("Live AG-UI agent", timeout=10000)
    before = len(server_log()["decisions"])

    page.locator("[data-testid=prompt]").fill("Refund order 4821 via the live agent")
    page.locator("[data-testid=run]").click()
    page.locator("[data-testid=approval-gate]").wait_for(timeout=15000)
    check(server_log()["runs"][-1]["messages"][0]["content"] == "Refund order 4821 via the live agent", "server received the prompt over HTTP")
    check(len(server_log()["decisions"]) == before, "no decision reached the server while the gate is open")
    check(page.locator("[data-testid=tool-call][data-tool=issue_refund]").count() == 1, "live tool call is shown")
    shot(page, "[data-testid=agent-console]", "2-live-gate")

    page.locator("textarea[id^=gate-args-]").fill('{"orderId":"4821","amount":"120.00"}')
    page.locator("[data-testid=approve]").click()
    expect(page.locator("[data-testid=run-status]")).to_have_text("Finished", timeout=15000)
    d = server_log()["decisions"][-1]
    check(d["approved"] and json.loads(d["editedArgs"])["amount"] == "120.00", "server got the approval with the edited $120.00 arguments")
    check("Refund of $120.00 issued by the live agent" in page.locator("[data-testid=tool-call][data-tool=issue_refund]").inner_text(), "live agent executed the EDITED amount")

    page.locator("[data-testid=run]").click()
    page.locator("[data-testid=approval-gate]").wait_for(timeout=15000)
    page.locator("input[id^=gate-reason-]").fill("over policy limit")
    page.locator("[data-testid=reject]").click()
    expect(page.locator("[data-testid=run-status]")).to_have_text("Finished", timeout=15000)
    d = server_log()["decisions"][-1]
    check((not d["approved"]) and d["reason"] == "over policy limit", "server got the rejection with its reason")
    check("Not executed (over policy limit)" in page.locator("[data-testid=tool-call][data-tool=issue_refund]").inner_text(), "live agent honoured the rejection")

    # Unreachable server: a readable failure, not a hang.
    page.locator("[data-testid=agui-url]").fill("http://127.0.0.1:5399/agui/run")
    page.locator("[data-testid=agui-connect]").click()
    page.locator("[data-testid=run]").click()
    expect(page.locator("[data-testid=run-status]")).to_have_text("Failed", timeout=15000)
    check(True, "unreachable AG-UI endpoint ends the run as Failed")

    # ---------- 3. Agent Kit ----------
    open_ai_tab(page, "agentkit")
    page.locator("[data-testid=agentkit-demo]").wait_for(timeout=15000)
    kit = "[data-testid=agentkit-demo]"

    # A2UI: edit the streamed form, press Send, see the resolved action
    page.locator("[data-a2ui=TextField] input").fill("75.00")
    page.locator("[data-id=reason] [data-option=late]").check()
    page.locator("[data-a2ui=CheckBox] input").uncheck()
    page.locator("[data-a2ui=Button]").click()
    out = page.locator("[data-testid=redacted-text]").first.inner_text()
    check('"amount": "75.00"' in out and '"reason": "late"' in out and '"notify": "false"' in out, "A2UI action carries the edited values")
    shot(page, kit + " section[aria-labelledby=gen-ui]", "3-a2ui")

    # MCP app: click inside the sandboxed iframe, host receives the postMessage
    frame = page.frame_locator("[data-testid=mcp-frame]")
    frame.locator("#b").click()
    expect(page.locator("[data-testid=mcp-received]")).to_contain_text('"type":"reviewed"', timeout=5000)
    check(page.locator("[data-testid=mcp-frame]").get_attribute("sandbox") == "allow-scripts allow-forms", "MCP frame has no allow-same-origin")
    check(page.evaluate("document.querySelector('[data-testid=mcp-frame]').contentDocument") is None, "host cannot read into the sandboxed frame")

    # Handoff + live screen
    page.locator("[data-testid=handoff-take]").click()
    check(page.locator("[data-testid=handoff-driving]").is_visible(), "handoff: driving state shown")
    check(page.locator("[data-testid=live-screen]").get_attribute("data-controlling") == "true", "live screen: take-over sets controlling")
    page.locator("[data-testid=screen-surface]").click(position={"x": 100, "y": 50})
    page.wait_for_function("document.querySelector('[data-testid=last-click]').textContent !== 'none'")
    check(page.locator("[data-testid=last-click]").inner_text() != "none", "live screen reports a click while controlling")
    page.locator("[data-testid=handoff-return]").click()
    check(page.locator("[data-testid=handoff-resolved]").is_visible(), "handoff: hand back resolves the panel")
    shot(page, kit + " section[aria-labelledby=handoff]", "4-handoff")

    # Secrets stay masked
    cmd = page.locator("[data-testid=command-output]").inner_text()
    check("eyJhbGciOiJIUzI1NiJ9" not in cmd and "[redacted]" in cmd, "command output masks the bearer token")

    # Grants, memory, routines
    page.locator("[data-testid=grant-level]").nth(1).select_option("Write")
    check(page.locator("[data-testid=grant-log]").inner_text() == "Stripe refunds set to Write", "grant level change reaches the host")
    page.locator("[data-testid=grant-revoke]").nth(0).click()
    check(page.locator("[data-testid=grant]").count() == 2, "grant revoked")
    page.locator("[data-testid=memory-search]").fill("email")
    check(page.locator("[data-testid=memory-item]").count() == 1, "memory search filters")
    page.locator("[data-testid=memory-forget]").click()
    check(page.locator("[data-testid=memory-empty]").count() == 1, "memory forgotten")
    page.locator("[data-testid=routine-toggle]").nth(0).click()
    check(page.locator("[data-testid=routine]").nth(0).get_attribute("data-enabled") == "false", "routine switched off")
    shot(page, kit + " section[aria-labelledby=control]", "5-control")

    # Chat pieces
    scroller = page.locator("[data-testid=scroller-body]")
    for _ in range(8):
        page.locator("[data-testid=chat-add]").click()
    check(scroller.evaluate("e => e.scrollHeight - e.scrollTop - e.clientHeight") < 30, "scroller follows new messages")
    scroller.evaluate("e => { e.scrollTop = 0; }")
    page.locator("[data-testid=scroller-jump]").wait_for(timeout=3000)
    page.locator("[data-testid=scroller-jump]").click()
    check(scroller.evaluate("e => e.scrollHeight - e.scrollTop - e.clientHeight") < 30, "Jump to latest returns to the end")
    page.locator("[data-testid=chat-suggestion]").nth(0).click()
    check(page.locator("[data-testid=picked]").inner_text() == "Show the audit trail", "chat suggestion reports its text")

    big = os.path.join(ROOT, "tests", "e2e", "_big.bin")
    small = os.path.join(ROOT, "tests", "e2e", "_small.txt")
    open(big, "wb").write(b"0" * 1100000)
    open(small, "w").write("hello")
    page.locator("[data-testid=attachment-input]").set_input_files([small, big])
    check(page.locator("[data-testid=attachment-item]").count() == 1 and "larger than" in page.locator("[data-testid=attachment-error]").inner_text(), "attachment: small accepted, oversize refused")
    page.locator("[data-testid=attachment-remove]").click()
    check(page.locator("[data-testid=attachment-item]").count() == 0, "attachment removed")
    os.remove(big)
    os.remove(small)

    # Questionnaire
    page.locator("[data-option='Store credit']").check()
    page.locator("[data-testid=questionnaire-next]").click()
    page.locator("[data-option=Email]").check()
    page.locator("[data-testid=questionnaire-other]").fill("Push")
    page.locator("[data-testid=questionnaire-next]").click()
    check(page.locator("[data-testid=answers]").inner_text() == "account=Store credit; notify=Email,Push", "questionnaire answers delivered")

    # Voice: fake mic via Chromium flags is not available on this context, so assert the graceful path.
    page.locator("[data-testid=voice-toggle]").click()
    page.wait_for_timeout(800)
    status = page.locator("[data-testid=voice-status]").inner_text()
    check(status in ("Listening…", "Microphone permission was denied.", "Could not start the microphone.", "This browser has no microphone support."),
          f"voice input starts or fails with a readable message ({status!r})")

    audit(page, kit, "the Agent Kit tab")

    # ---------- 4. Inputs and pickers ----------
    open_ai_tab(page, "inputs")
    page.locator("[data-testid=inputs-demo]").wait_for(timeout=15000)
    tk = "[data-testid=inputs-demo]"

    check(page.locator("[data-testid=cal-day][disabled]").count() >= 4, "calendar disables days before Min")
    first_enabled = page.locator("[data-testid=cal-day]:not([disabled])").first
    first_enabled.click()
    check(page.locator("[data-testid=date-out]").inner_text() != "none", "calendar click selects a date")
    sel = page.locator("[data-testid=cal-day][data-selected=true]").first
    start = sel.get_attribute("data-date")
    sel.focus()
    page.keyboard.press("ArrowRight")
    page.keyboard.press("ArrowDown")
    focused = page.evaluate("document.activeElement.getAttribute('data-date')")
    check(focused is not None and focused != start, f"arrow keys move focus within the calendar ({start} -> {focused})")
    page.keyboard.press("Enter")
    check(page.locator("[data-testid=date-out]").inner_text() == focused, "Enter selects the focused date")
    month_before = page.locator("[data-testid=cal-title]").inner_text()
    page.locator("[data-testid=cal-next]").click()
    check(page.locator("[data-testid=cal-title]").inner_text() != month_before, "next month navigates")

    page.locator("[data-testid=time-hour]").first.select_option("3")
    page.locator("[data-testid=time-minute]").first.select_option("45")
    page.locator("[data-testid=time-period]").first.select_option("PM")
    check(page.locator("[data-testid=time-out]").inner_text() == "15:45", "time picker converts 3:45 PM to 15:45")

    page.locator("[data-testid=radio-option] input").nth(0).check()
    check(page.locator("[data-testid=plan-out]").inner_text() == "free", "radio group selects a value")
    check(page.locator("[data-testid=radio-option] input").nth(2).is_disabled(), "disabled radio option is not selectable")

    ta = page.locator("[data-testid=textarea-input]")
    ta.fill("x" * 115)
    check(page.locator("[data-testid=textarea-count]").inner_text() == "5 left", "text area counts down")
    ta.fill("TODO later")
    check(page.locator("[data-testid=textarea-error]").inner_text() == "Replace the TODO before sending.", "text area shows its error")

    page.locator("[data-testid=button-group] button", has_text="Month").click()
    check(page.locator("[data-testid=range-out]").inner_text() == "Month", "button group selects a range")

    shot(page, tk, "6-inputs")
    audit(page, tk, "the Inputs tab")

    # ---------- 5. Reduced motion: spinner stops ----------
    page.emulate_media(reduced_motion="reduce")
    anim = page.locator("[data-testid=spinner] svg").evaluate("e => getComputedStyle(e).animationName")
    check(anim == "none", f"spinner animation is off under prefers-reduced-motion ({anim})")
    page.emulate_media(reduced_motion="no-preference")

    real = [e for e in errors if "favicon" not in e.lower() and "5399" not in e and "Failed to load resource" not in e
            and "ERR_CONNECTION_REFUSED" not in e and "Access-Control" not in e and "microphone" not in e.lower() and "Permission" not in e]
    check(len(real) == 0, f"no unexpected console errors ({real[:3]})")
    browser.close()

print()
if problems:
    print(f"{len(problems)} FAILED:")
    for m in problems:
        print("  -", m)
    sys.exit(1)
print("ALL CHECKS PASSED")
