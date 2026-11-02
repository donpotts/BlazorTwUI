"""
Browser E2E for the data grid: keyboard sorting, multi-sort, summaries, grouping, master-detail, inline editing with validation,
column chooser, CSV export (downloaded and inspected), saved state across a reload, frozen column, mobile cards, axe-core, screenshots.

    dotnet run --urls http://127.0.0.1:5199        # or any host serving the app
    python tests/e2e/grid_e2e.py [base_url]
"""
import os
import re
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


def money(text):
    return int(re.sub(r"[^\d]", "", text) or 0)


def shot(page, name, selector="[data-testid=grid-demo]"):
    path = os.path.join(SHOTS, f"grid-{name}.png")
    page.locator(selector).first.screenshot(path=path)
    print("     screenshot:", path)


def open_grid(page):
    page.goto(BASE, wait_until="networkidle", timeout=60000)
    page.locator("[data-testid=jump-bar] >> text=Smart AGUI").click()
    page.locator("[data-testid=subtab-grid]").click()
    page.locator("[data-testid=grid]").wait_for(timeout=15000)
    page.locator("[data-testid=grid-row]").first.wait_for(timeout=15000)


def amounts(page):
    return [money(c.inner_text()) for c in page.locator("[data-testid=grid-row] td[data-col=Amount]").all()]


require_200(BASE + "/", "app host")

with sync_playwright() as p:
    browser = p.chromium.launch(headless=True)
    context = browser.new_context(viewport={"width": 1500, "height": 1100}, accept_downloads=True)
    page = context.new_page()
    errors = []
    page.on("console", lambda m: errors.append(m.text) if m.type == "error" else None)
    page.on("pageerror", lambda e: errors.append(str(e)))

    open_grid(page)
    page.evaluate("localStorage.removeItem('btui-grid:orders-demo')")
    open_grid(page)

    check(page.locator("[data-testid=grid-row]").count() == 8, "first page shows eight rows")
    check(page.locator("[data-testid=grid-range]").inner_text() == "Showing 1 - 8 of 25", "range text counts all 25 orders")
    check(page.locator("table caption").inner_text() == "Orders", "table has an accessible caption")

    # ---- keyboard sorting ----
    btn = page.locator("[data-testid=grid-sort][data-col=Amount]")
    btn.focus()
    page.keyboard.press("Enter")
    check(page.locator("th[data-col=Amount]").get_attribute("aria-sort") == "ascending", "Enter on a header button sorts ascending (aria-sort)")
    a = amounts(page)
    check(a == sorted(a), f"amounts are ascending on the page ({a[:4]}...)")
    page.keyboard.press("Space")
    check(page.locator("th[data-col=Amount]").get_attribute("aria-sort") == "descending", "Space sorts descending")
    a = amounts(page)
    check(a == sorted(a, reverse=True), "amounts are descending")

    # multi-sort: Region first, then Amount descending inside each region
    page.locator("[data-testid=grid-sort][data-col=Region]").click()
    page.locator("[data-testid=grid-sort][data-col=Amount]").click(modifiers=["Shift"])
    check(page.locator("[data-testid=grid-sort][data-col=Region]").inner_text().strip().endswith("1") or "▲" in page.locator("[data-testid=grid-sort][data-col=Region]").inner_text(), "primary sort is marked")
    regions = [c.inner_text().strip() for c in page.locator("[data-testid=grid-row] td[data-col=Region]").all()]
    check(regions == sorted(regions), f"rows are ordered by region first ({regions[:4]}...)")

    # ---- summaries use all rows, not the page ----
    page.locator("[data-testid=grid-page-size]").select_option("50")
    page.wait_for_function("document.querySelectorAll('[data-testid=grid-row]').length === 25")
    total = sum(amounts(page))
    footer = money(page.locator("[data-testid=grid-summary][data-col=Amount]").inner_text())
    check(footer == total, f"footer Sum equals the sum of all 25 rows ({footer} vs {total})")
    page.locator("[data-testid=grid-page-size]").select_option("8")
    footer8 = money(page.locator("[data-testid=grid-summary][data-col=Amount]").inner_text())
    check(footer8 == total, "footer total does not change with the page size")
    page.locator("[data-testid=grid-sort][data-col=Region]").click()  # reset to a simple state
    page.locator("[data-testid=grid-sort][data-col=Region]").click()
    shot(page, "1-default")

    # ---- grouping ----
    page.locator("[data-testid=grid-page-size]").select_option("50")
    page.locator("[data-testid=grid-group-by]").select_option("Region")
    groups = page.locator("[data-testid=grid-group]")
    check(groups.count() == 4, "grouping by Region shows four group headers")
    counts = [int(re.search(r"\((\d+)\)", g.inner_text()).group(1)) for g in groups.all()]
    check(sum(counts) == 25, f"group counts add up to all rows ({counts})")
    check(page.locator("[data-testid=grid-group-summary]").count() >= 4, "group headers carry the Amount summary")
    first_rows = page.locator("[data-testid=grid-row]").count()
    page.locator("[data-testid=grid-group-toggle]").first.click()
    check(page.locator("[data-testid=grid-row]").count() == first_rows - counts[0], "collapsing a group hides its rows")
    shot(page, "2-grouped")
    page.locator("[data-testid=grid-group-toggle]").first.click()

    # ---- master-detail ----
    page.locator("[data-testid=grid-detail-toggle]").first.click()
    check("was placed on" in page.locator("[data-testid=detail-text]").first.inner_text(), "expanding a row shows its detail")
    ctl = page.locator("[data-testid=grid-detail-toggle]").first.get_attribute("aria-controls")
    check(page.locator(f"#{ctl}").count() == 1, "aria-controls points at the detail row")
    shot(page, "3-detail")
    page.locator("[data-testid=grid-detail-toggle]").first.click()

    # ---- inline editing with validation ----
    page.locator("[data-testid=grid-group-by]").select_option("")
    page.locator("[data-testid=grid-edit]").first.click()
    check(page.locator("[data-testid=grid-edit-input]").count() == 2, "editing shows inputs for the editable columns")
    check(page.locator("[data-testid=grid-edit]").first.is_disabled(), "other rows cannot start editing at the same time")
    page.locator("[data-testid=grid-edit-input][data-col=Customer]").fill("")
    page.locator("[data-testid=grid-edit-input][data-col=Amount]").fill("-5")
    page.locator("[data-testid=grid-save]").click()
    check(page.locator("[data-testid=grid-edit-error]").count() == 2, "invalid values show two errors and block the save")
    check(page.locator("[data-testid=grid-edit-input][data-col=Customer]").get_attribute("aria-invalid") == "true", "invalid input is aria-invalid")
    shot(page, "4-edit-errors")
    page.locator("[data-testid=grid-edit-input][data-col=Customer]").fill("Northwind Traders")
    page.locator("[data-testid=grid-edit-input][data-col=Amount]").fill("7777")
    page.locator("[data-testid=grid-edit-input][data-col=Amount]").press("Enter")
    check("Northwind Traders" in page.locator("[data-testid=grid-saved]").inner_text() and "$7,777" in page.locator("[data-testid=grid-saved]").inner_text(), "Enter saves and reports the row")
    check(page.locator("[data-testid=grid-edit-input]").count() == 0, "the row leaves edit mode")
    page.locator("[data-testid=grid-edit]").first.click()
    page.locator("[data-testid=grid-edit-input][data-col=Customer]").fill("Discarded")
    page.locator("[data-testid=grid-edit-input][data-col=Customer]").press("Escape")
    check("Discarded" not in page.locator("[data-testid=grid]").inner_text(), "Escape cancels the edit")

    # ---- column chooser + saved state across a reload ----
    page.locator("[data-testid=grid-columns-button]").click()
    page.locator("[data-testid=grid-column-toggle][data-col=Category]").uncheck()
    check(page.locator("th[data-col=Category]").count() == 0, "a hidden column disappears")
    check(page.locator("[data-testid=grid-column-toggle][data-col=Order]").is_disabled(), "a column marked not hideable cannot be hidden")
    shot(page, "5-columns-menu")
    page.keyboard.press("Escape")
    page.locator("[data-testid=grid-columns-button]").click()
    page.locator("[data-testid=grid-sort][data-col=Qty]").click()
    page.locator("[data-testid=grid-group-by]").select_option("Status")
    page.locator("[data-testid=grid-search]").fill("o")
    page.wait_for_timeout(300)

    open_grid(page)  # reload
    page.wait_for_function("document.querySelectorAll('th[data-col=Category]').length === 0", timeout=8000)
    check(page.locator("th[data-col=Category]").count() == 0, "reload: the hidden column stays hidden")
    check(page.locator("th[data-col=Qty]").get_attribute("aria-sort") == "ascending", "reload: the sort is restored")
    check(page.locator("[data-testid=grid-group-by]").input_value() == "Status", "reload: the grouping is restored")
    check(page.locator("[data-testid=grid-search]").input_value() == "o", "reload: the search text is restored")

    # ---- export: download and inspect the file ----
    page.locator("[data-testid=grid-search]").fill("")
    page.locator("[data-testid=grid-group-by]").select_option("")
    with page.expect_download() as dl:
        page.locator("[data-testid=grid-export]").click()
    path = dl.value.path()
    text = open(path, encoding="utf-8-sig", newline="").read()
    lines = [l for l in text.split("\r\n") if l]
    check(dl.value.suggested_filename == "orders.csv", "download is named orders.csv")
    check(lines[0] == "Order,Customer,Region,Amount,Qty,Status", f"header lists the visible columns only ({lines[0]})")
    check(len(lines) == 26, f"the file has every order, not just the current page ({len(lines)} lines)")
    check(any(l.startswith('SO-1025,"\'=HYPERLINK(') for l in lines), "a formula-like customer name is neutralised in the CSV")

    # ---- frozen column stays put while scrolling sideways ----
    page.set_viewport_size({"width": 820, "height": 1000})
    page.wait_for_timeout(300)
    box = page.locator("[data-testid=grid-table]").evaluate("e => { const s = e.parentElement; return {sw: s.scrollWidth, cw: s.clientWidth}; }")
    check(box["sw"] > box["cw"], f"the table overflows at 820px so there is something to scroll ({box})")
    holder = page.locator("[data-testid=grid-table]").evaluate("e => e.parentElement.getBoundingClientRect().left")
    page.locator("[data-testid=grid-table]").evaluate("e => e.parentElement.scrollLeft = 400")
    page.wait_for_timeout(200)
    order_x = page.locator("td[data-col=Order]").first.bounding_box()["x"]
    qty_x = page.locator("td[data-col=Qty]").first.bounding_box()["x"]
    check(abs(order_x - holder) < 2, f"the frozen Order column stays pinned to the left edge while scrolled ({order_x:.0f} vs edge {holder:.0f})")
    check(qty_x < holder + 5 or qty_x > order_x, "other columns scroll underneath it")

    # ---- mobile cards ----
    page.set_viewport_size({"width": 390, "height": 900})
    page.wait_for_timeout(300)
    check(not page.locator("[data-testid=grid-table]").is_visible(), "the table is hidden on a phone")
    cards = page.locator("[data-testid=grid] .md\\:hidden > div")
    check(cards.count() >= 1, f"rows appear as cards on a phone ({cards.count()})")
    shot(page, "6-mobile")
    page.set_viewport_size({"width": 1500, "height": 1100})
    page.wait_for_timeout(300)

    # ---- accessibility ----
    page.add_script_tag(path=AXE)
    violations = page.evaluate(
        """async () => {
            const r = await axe.run('[data-testid=grid-demo]', { runOnly: ['wcag2a', 'wcag2aa', 'wcag21aa', 'wcag22aa'] });
            return r.violations.map(v => ({ id: v.id, impact: v.impact, help: v.help, nodes: v.nodes.slice(0, 4).map(n => n.target.join(' ')) }));
        }""")
    for v in violations:
        print("     axe:", v["impact"], v["id"], "-", v["help"], v["nodes"])
    check(len(violations) == 0, f"axe-core: no WCAG 2.x A/AA violations in the grid ({len(violations)} found)")

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
