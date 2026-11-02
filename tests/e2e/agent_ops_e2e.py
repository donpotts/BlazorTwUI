"""
End-to-end check of the Agent Ops module in a real browser, with screenshots.

Needs the app running (default http://127.0.0.1:5199, see README "Testing") and
`npm install` done so node_modules/axe-core exists.

    python tests/e2e/agent_ops_e2e.py [base_url]
"""
import json
import os
import sys

from playwright.sync_api import expect, sync_playwright

BASE = sys.argv[1] if len(sys.argv) > 1 else "http://127.0.0.1:5199"
ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
SHOTS = os.path.join(ROOT, "docs", "images")
AXE = os.path.join(ROOT, "node_modules", "axe-core", "axe.min.js")
os.makedirs(SHOTS, exist_ok=True)

problems = []


def check(condition, message):
    print(("PASS " if condition else "FAIL ") + message)
    if not condition:
        problems.append(message)


def shot(page, name):
    path = os.path.join(SHOTS, f"agent-ops-{name}.png")
    page.locator("[data-testid=agent-console]").screenshot(path=path)
    print("     screenshot:", path)


def state_value(page, key):
    row = page.locator(f"[data-testid=state-row][data-key='{key}']")
    row.wait_for(timeout=15000)
    # The value is an input while the human is in control, plain text otherwise.
    field = row.locator("input")
    return field.input_value() if field.count() else row.locator("[data-testid=state-value]").inner_text()


def open_console(page):
    page.goto(BASE, wait_until="networkidle", timeout=60000)
    page.locator("[data-testid=jump-bar] >> text=Smart AGUI").click()
    page.locator("[data-testid=subtab-agentops]").click()
    page.locator("[data-testid=agent-console]").wait_for(timeout=15000)


def close_theme_menu(page):
    # The dropdown puts a full-screen backdrop behind itself; clicking outside is how a user closes it.
    page.locator("div.fixed.inset-0.z-40").first.click(position={"x": 5, "y": 5})


def toggle_contrast(page):
    """Contrast lives in the theme dropdown: open it, flip the switch, close it again."""
    trigger = page.locator("button[title='Theme & Accent Color Selector']").first
    trigger.click()
    page.locator("[data-testid=contrast-toggle]").first.click()
    close_theme_menu(page)


def run_to_gate(page):
    page.locator("[data-testid=run]").click()
    page.locator("[data-testid=approval-gate]").wait_for(timeout=15000)


def wait_status(page, text, timeout=15000):
    expect(page.locator("[data-testid=run-status]")).to_have_text(text, timeout=timeout)


with sync_playwright() as p:
    browser = p.chromium.launch(headless=True)
    context = browser.new_context(viewport={"width": 1500, "height": 1000}, accept_downloads=True)
    page = context.new_page()
    console_errors = []
    page.on("console", lambda m: console_errors.append(m.text) if m.type == "error" else None)
    page.on("pageerror", lambda e: console_errors.append(str(e)))
    # The dev server probes a hot-reload file that only exists under `dotnet watch`. Any other 404 or throw fails the run.
    known_404 = ("hot-reload-settings.json",)
    page.on("response", lambda r: console_errors.append(f"404 {r.url}") if r.status == 404 and not any(k in r.url for k in known_404) else None)

    open_console(page)
    shot(page, "1-idle")

    # --- 1. High-risk call really blocks the agent until a human approves, with edited arguments.
    run_to_gate(page)
    wait_status(page, "Waiting for your approval")
    check(state_value(page, "refund.status") == "proposed", "refund is only 'proposed' while the gate is open")
    check(page.locator("[data-testid=tool-call][data-tool=send_email]").count() == 0, "follow-up action has not run while blocked")
    check(page.locator("[data-testid=run]").is_disabled(), "cannot start a second run while one is blocked")
    shot(page, "2-approval-gate")

    page.locator("textarea[id^=gate-args-]").fill('{"orderId":"4821","amount":"120.00"}')
    page.locator("[data-testid=approve]").click()
    wait_status(page, "Finished")
    check(state_value(page, "refund.status") == "issued", "approved: refund issued")
    check(state_value(page, "refund.amount") == "120.00", "approved with edited arguments: the edited amount ($120.00) was executed")
    check(page.locator("[data-testid=tool-call][data-tool=send_email]").count() == 1, "approved: confirmation email step ran")
    shot(page, "3-approved")

    # --- 2. Rejection reroutes the agent instead of just hiding a button.
    run_to_gate(page)
    page.locator("input[id^=gate-reason-]").fill("over policy limit")
    page.locator("[data-testid=reject]").click()
    wait_status(page, "Finished")
    check(state_value(page, "refund.status") == "blocked", "rejected: refund stays blocked")
    check(page.locator("[data-testid=tool-call][data-tool=send_email]").count() == 0, "rejected: no customer email sent")
    check(page.locator("[data-testid=tool-call][data-tool=create_ticket]").count() == 1, "rejected: agent escalated with a ticket instead")
    check(page.locator("[data-testid=audit-row][data-action='tool.reject']").count() == 1, "rejection is in the audit trail")
    shot(page, "4-rejected")

    # --- 3. Audit export downloads real JSON.
    with page.expect_download() as dl:
        page.locator("[data-testid=audit-export]").click()
    data = json.load(open(dl.value.path(), encoding="utf-8"))
    check(any(e["action"] == "tool.reject" and "over policy limit" in e["detail"] for e in data), "exported JSON contains the human rejection and its reason")

    # --- 4. Takeover pauses the agent; a human state edit changes what it does next.
    page.locator("[data-testid=run]").click()
    # Wait for the NEW run (Run disabled) and its lookup step; the previous run's rows are still on screen until then.
    expect(page.locator("[data-testid=run]")).to_be_disabled()
    page.locator("[data-testid=tool-call][data-tool=lookup_order][data-status=Completed]").wait_for(timeout=15000)
    page.locator("[data-testid=takeover]").click()
    wait_status(page, "Paused: you have control")
    count_paused = page.locator("[data-testid=tool-call]").count()
    page.wait_for_timeout(1500)
    check(page.locator("[data-testid=tool-call]").count() == count_paused, "paused: the agent produced nothing while the human had control")
    shot(page, "5-takeover")
    page.locator("[data-testid=state-row][data-key='refund.amount'] input").fill("75.00")
    page.locator("[data-testid=state-row][data-key='refund.amount'] input").press("Tab")
    page.locator("[data-testid=handback]").click()
    page.locator("[data-testid=approval-gate]").wait_for(timeout=15000)
    gate_args = page.locator("textarea[id^=gate-args-]").input_value()
    check("75.00" in gate_args, f"human state edit ($75.00) flowed into the agent's next tool call (gate args: {gate_args})")
    page.locator("[data-testid=cancel]").click()
    wait_status(page, "Stopped")

    # --- 5. Contrast modes + RTL.
    toggle_contrast(page)
    check(page.evaluate("document.documentElement.getAttribute('data-contrast')") == "high", "contrast toggle sets data-contrast=high on <html>")
    page.locator("[data-testid=run]").click()
    page.locator("[data-testid=approval-gate]").wait_for(timeout=15000)
    shot(page, "6-high-contrast")
    page.evaluate("document.documentElement.setAttribute('dir','rtl')")
    shot(page, "7-rtl-high-contrast")
    page.evaluate("document.documentElement.removeAttribute('dir')")
    toggle_contrast(page)
    check(page.evaluate("document.documentElement.getAttribute('data-contrast')") is None, "toggling again restores standard contrast")
    page.locator("button[title='Theme & Accent Color Selector']").first.click()
    check(page.locator("[data-testid=contrast-toggle]").first.get_attribute("aria-checked") == "false", "switch reports aria-checked=false")
    close_theme_menu(page)

    # forced-colors emulation: the gate must stay operable and visible
    page.emulate_media(forced_colors="active")
    shot(page, "8-forced-colors")
    page.emulate_media(forced_colors="none")

    # --- 6. Accessibility audit (axe-core) of the console at the open gate.
    page.add_script_tag(path=AXE)
    result = page.evaluate(
        """async () => {
            const r = await axe.run('[data-testid=agent-console]', { runOnly: ['wcag2a', 'wcag2aa', 'wcag21aa', 'wcag22aa'] });
            return r.violations.map(v => ({ id: v.id, impact: v.impact, help: v.help, nodes: v.nodes.slice(0, 3).map(n => n.target.join(' ')) }));
        }"""
    )
    for v in result:
        print("     axe:", v["impact"], v["id"], "-", v["help"], v["nodes"])
    check(len(result) == 0, f"axe-core: no WCAG 2.x A/AA violations in the agent console ({len(result)} found)")

    page.locator("[data-testid=cancel]").click()
    # The generic "Failed to load resource" console line carries no URL; the response hook above names real ones.
    real_errors = [e for e in console_errors if "favicon" not in e.lower() and not e.startswith("Failed to load resource")]
    check(len(real_errors) == 0, f"no browser console/page errors ({real_errors[:3]})")
    browser.close()

print()
if problems:
    print(f"{len(problems)} FAILED:")
    for m in problems:
        print("  -", m)
    sys.exit(1)
print("ALL CHECKS PASSED")
