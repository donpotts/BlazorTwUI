"""
Browser E2E for the Planning & Enterprise tab: scheduler, Gantt, tree list, range selector and gauge, toolbar, window, loading panel,
form layout with pickers, and the rich-text editor (including hostile input). Screenshots plus an axe-core audit.

    dotnet run --urls http://127.0.0.1:5199          # or any host serving the app
    python tests/e2e/enterprise_e2e.py [base_url]
"""
import os
import sys
import time
import urllib.request

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


def require_200(url, what, attempts=60):
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


def shot(page, selector, name):
    path = os.path.join(SHOTS, f"enterprise-{name}.png")
    page.locator(selector).first.screenshot(path=path)
    print("     screenshot:", path)


require_200(BASE + "/", "app host")

with sync_playwright() as p:
    browser = p.chromium.launch(headless=True)
    context = browser.new_context(viewport={"width": 1500, "height": 1100})
    page = context.new_page()
    errors = []
    page.on("console", lambda m: errors.append(m.text) if m.type == "error" else None)
    page.on("pageerror", lambda e: errors.append(str(e)))

    page.goto(BASE, wait_until="networkidle", timeout=60000)
    page.locator("[data-testid=jump-bar] >> text=Smart AGUI").click()
    page.locator("[data-testid=subtab-enterprise]").click()
    page.locator("[data-testid=enterprise-demo]").wait_for(timeout=15000)
    demo = "[data-testid=enterprise-demo]"

    # ---- Scheduler ----
    check(page.locator("[data-testid=sch-col]").count() == 7, "scheduler week view shows seven day columns")
    appt = page.locator("[data-testid=sch-appt]", has_text="Customer call")
    before = appt.get_attribute("data-start")
    target = page.locator("[data-testid=sch-slot]").nth(20)
    appt.drag_to(target)
    page.wait_for_function("document.querySelector('[data-testid=sched-out]').textContent.includes('moved')", timeout=5000)
    after = page.locator("[data-testid=sch-appt]", has_text="Customer call").get_attribute("data-start")
    check(after != before, f"drag and drop moved the appointment ({before} -> {after})")

    page.locator("[data-testid=sch-appt]", has_text="Planning").focus()
    page.keyboard.press("Alt+ArrowRight")
    check("Planning moved" in page.locator("[data-testid=sched-out]").inner_text(), "Alt+arrow moves an appointment from the keyboard")

    page.locator("[data-testid=sch-slot][data-start$=\"T16:00\"]").last.click()
    check("created" in page.locator("[data-testid=sched-out]").inner_text(), "clicking an empty slot creates an event")

    page.locator("[data-testid=sch-view][data-view=month]").click()
    check(page.locator("[data-testid=sch-cell]").count() >= 35, "month view shows the month grid")
    page.locator("[data-testid=sch-view][data-view=week]").click()
    shot(page, "[aria-labelledby=e-sch]", "1-scheduler")

    # ---- Gantt ----
    check(page.locator("[data-testid=gantt-bar]").count() == 5, "gantt shows five task bars")
    check(page.locator("[data-testid=gantt-arrow]").count() == 5, "gantt draws dependency arrows (finish-to-start)")
    check(page.locator("[data-testid=gantt-conflict]").count() == 1, "gantt flags the one task that starts before its predecessor finishes")
    page.locator("[data-testid=gantt-bar][data-id=design]").click()
    check(page.locator("[data-testid=gantt-out]").inner_text() == "Design", "clicking a bar selects the task")
    shot(page, "[aria-labelledby=e-gantt]", "2-gantt")

    # ---- Tree list ----
    rows = page.locator("[data-testid=tree-row]")
    check(rows.count() == 9, "tree list starts expanded with nine rows")
    page.locator("[data-testid=tree-collapse-all]").click()
    check(rows.count() == 1, "collapse all leaves the root")
    page.locator("[data-key=co]").focus()
    page.keyboard.press("ArrowRight")
    check(rows.count() == 4, "Right arrow expands the focused row")
    page.locator("[data-testid=tree-sort][data-col=Headcount]").click()
    first_children = [r.get_attribute("data-key") for r in page.locator("[data-testid=tree-row]").all()][1:]
    check(first_children == ["fin", "ops", "eng"], f"sorting orders siblings numerically ({first_children})")
    page.locator("[data-key=eng]").click()
    check(page.locator("[data-testid=tree-out]").inner_text() == "eng", "clicking a row selects it")

    # ---- Range selector + gauge ----
    avg_before = page.locator("[data-testid=bar-gauge-legend]").first.inner_text()
    page.locator("[data-testid=range-end]").fill("11")
    page.wait_for_timeout(300)
    avg_after = page.locator("[data-testid=bar-gauge-legend]").first.inner_text()
    check(avg_before != avg_after, f"changing the range updates the gauge ({avg_before!r} -> {avg_after!r})")
    check("10 of 12" in page.locator("[data-testid=range-summary]").inner_text(), "range summary counts the selected points")
    page.locator("[data-testid=range-start]").fill("11")
    page.wait_for_timeout(200)
    check(page.locator("[data-testid=range-start]").input_value() == "11" and page.locator("[data-testid=range-end]").input_value() == "11", "start cannot pass end")

    # ---- Toolbar ----
    check(page.locator("[data-testid=toolbar-item]").count() == 3, "toolbar shows items up to MaxVisible")
    page.locator("[data-testid=toolbar-more]").click()
    check(page.locator("[data-testid=toolbar-overflow-item]").count() == 3, "the rest go into the overflow menu")
    page.locator("[data-testid=toolbar-overflow-item][data-id=export]").click()
    check(page.locator("[data-testid=tool-out]").inner_text() == "Export clicked", "overflow item runs its command")
    page.locator("[data-testid=toolbar-item][data-id=bold]").click()
    check(page.locator("[data-testid=toolbar-item][data-id=bold]").get_attribute("aria-pressed") == "true", "toggle button is pressed")

    # ---- Window ----
    page.locator("[data-testid=open-window]").click()
    win = page.locator("[data-testid=window]")
    win.wait_for(timeout=5000)
    bar = page.locator("[data-testid=window-titlebar]")
    box = bar.bounding_box()
    page.mouse.move(box["x"] + 60, box["y"] + 15)
    page.mouse.down()
    page.mouse.move(box["x"] + 260, box["y"] + 115, steps=8)
    page.mouse.up()
    page.wait_for_function("document.querySelector('[data-testid=win-pos]').textContent !== '140,140'", timeout=5000)
    check(page.locator("[data-testid=win-pos]").inner_text() != "140,140", f"dragging the title bar moves the window ({page.locator('[data-testid=win-pos]').inner_text()})")
    bar.focus()
    page.keyboard.press("ArrowRight")
    page.keyboard.press("Escape")
    check(win.count() == 0, "Escape closes the window")
    shot(page, "[aria-labelledby=e-tool]", "3-toolbar-window")

    # ---- Loading panel ----
    page.locator("[data-testid=toggle-busy]").click()
    check(page.locator("[data-testid=loading-overlay]").is_visible(), "loading overlay appears")
    check(page.locator("[data-testid=loading-panel]").get_attribute("aria-busy") == "true", "region is aria-busy")
    inert = page.locator("[data-testid=covered-button]").evaluate("e => !!e.closest('[inert]')")
    check(inert, "covered content is inert (cannot be tabbed into)")
    page.locator("[data-testid=toggle-busy]").click()

    # ---- Form layout, dropdown box, list box ----
    page.locator("[data-testid=f-name]").fill("")
    check(page.locator("[data-testid=form-item-error]").count() == 1, "an empty name shows the field error")
    page.locator("[data-testid=f-name]").fill("Dana")
    check(page.locator("label[for=f-name]").inner_text().startswith("Name"), "label is bound to its input")
    page.locator("[data-testid=dropdown-trigger]").click()
    page.locator("[data-testid=dropdown-panel] [data-testid=listbox-item][data-text='Priya Nair']").click()
    check(page.locator("[data-testid=owner-out]").inner_text() == "Priya Nair", "picking inside the drop-down panel sets the value")
    check(page.locator("[data-testid=dropdown-panel]").count() == 0, "the panel closes after the pick")
    page.locator("[data-testid=listbox-search]").last.fill("s")
    check(page.locator("[data-testid=listbox-items]").last.locator("li").count() == 3, "list box search filters the list (Support, Success, Sales)")
    page.locator("[data-testid=listbox-search]").last.fill("")
    page.locator("[data-testid=form-group-toggle]").click()
    check(page.locator("[data-testid=form-group]").get_attribute("data-open") == "false", "form group collapses")
    page.locator("[data-testid=form-group-toggle]").click()
    shot(page, "[aria-labelledby=e-form]", "4-form")

    # ---- HTML editor: formatting and hostile input ----
    surface = page.locator("[data-testid=editor-surface]")
    out = page.locator("[data-testid=html-out]")
    check("quarterly review" in out.inner_text(), "the bound output starts with the sample text")
    surface.click()
    page.keyboard.press("Control+A")
    page.keyboard.type("Plain words")
    page.keyboard.press("Control+A")
    page.locator("[data-cmd=bold]").click()
    page.wait_for_function("document.querySelector('[data-testid=html-out]').textContent.includes('Plain words')", timeout=5000)
    page.wait_for_timeout(400)
    check(any(t in out.inner_text() for t in ("<b>Plain words</b>", "<strong>Plain words</strong>")), f"bold wraps the selection ({out.inner_text()[:60]!r})")

    hostile = '<p onclick="alert(1)">hi</p><script>window.__pwned=1</script><img src=x onerror="window.__pwned=2"><a href="javascript:alert(3)">bad</a><a href="https://ok.example/x" style="color:red">ok</a><iframe src="//evil"></iframe>'
    cleaned = page.evaluate("(h) => twEnterprise.sanitize(h)", hostile)
    check("<script" not in cleaned and "onclick" not in cleaned and "onerror" not in cleaned and "<img" not in cleaned and "<iframe" not in cleaned, f"sanitizer strips scripts, handlers, images and frames ({cleaned})")
    check('href="javascript' not in cleaned and 'href="https://ok.example/x"' in cleaned and "style=" not in cleaned, "sanitizer keeps only safe links and drops styles")
    check('rel="noopener noreferrer nofollow"' in cleaned, "kept links are noopener")
    # Hostile markup set from the host goes through the same sanitizer before it touches the DOM.
    page.evaluate("(h) => twEnterprise.setEditorHtml(document.querySelector('[data-testid=editor-surface]'), h)", hostile)
    live = surface.inner_html()
    check("<script" not in live and "<img" not in live and "onclick" not in live and "javascript:" not in live, f"host-supplied markup is sanitized before it is rendered ({live[:80]!r})")
    surface.dispatch_event("input")
    page.wait_for_timeout(500)
    shown = out.inner_text()
    check("script" not in shown.lower() and "onerror" not in shown and "javascript:" not in shown, "hostile input never reaches the bound value")
    check(page.evaluate("window.__pwned === undefined"), "no injected script ran")
    page.evaluate("() => { const dt = new DataTransfer(); dt.setData('text/html', '<b>x</b><script>window.__pwned=3</script>'); dt.setData('text/plain', 'pasted'); document.querySelector('[data-testid=editor-surface]').dispatchEvent(new ClipboardEvent('paste', {clipboardData: dt, bubbles: true, cancelable: true})); }")
    check(page.evaluate("window.__pwned === undefined"), "pasting HTML does not execute or insert it")

    page.locator("[data-testid=editor-link-toggle]").click()
    page.locator("[data-testid=editor-url]").fill("javascript:alert(1)")
    page.locator("[data-testid=editor-link-apply]").click()
    check(page.locator("[data-testid=editor-link-error]").count() == 1, "a javascript: link is refused with a message")
    shot(page, "[aria-labelledby=e-html]", "5-editor")

    # ---- Accessibility + reduced motion ----
    page.add_script_tag(path=AXE)
    violations = page.evaluate(
        """async (scope) => {
            const r = await axe.run(scope, { runOnly: ['wcag2a', 'wcag2aa', 'wcag21aa', 'wcag22aa'] });
            return r.violations.map(v => ({ id: v.id, impact: v.impact, help: v.help, nodes: v.nodes.slice(0, 4).map(n => n.target.join(' ')) }));
        }""", demo)
    for v in violations:
        print("     axe:", v["impact"], v["id"], "-", v["help"], v["nodes"])
    check(len(violations) == 0, f"axe-core: no WCAG 2.x A/AA violations on the tab ({len(violations)} found)")

    real = [e for e in errors if "favicon" not in e.lower() and "Failed to load resource" not in e]
    check(len(real) == 0, f"no console errors ({real[:3]})")
    browser.close()

print()
if problems:
    print(f"{len(problems)} FAILED:")
    for m in problems:
        print("  -", m)
    sys.exit(1)
print("ALL CHECKS PASSED")
