import os
import sys
import time

sys.stdout.reconfigure(encoding='utf-8')
from playwright.sync_api import sync_playwright

SCREENSHOTS_DIR = r"C:\Users\donpo\.gemini\antigravity\brain\27e04055-7eb7-457e-ac4c-93c7084425b7\screenshots"
os.makedirs(SCREENSHOTS_DIR, exist_ok=True)

CHROME_PATH = r"C:\Program Files\Google\Chrome\Application\chrome.exe"
BASE_URL = "http://localhost:5128"

def run():
    print("=" * 70)
    print("STARTING COMPREHENSIVE E2E SCREENSHOT VALIDATION FOR BLAZORTWUI")
    print(f"Target: {BASE_URL}")
    print(f"Screenshots folder: {SCREENSHOTS_DIR}")
    print("=" * 70)

    with sync_playwright() as p:
        browser = p.chromium.launch(
            executable_path=CHROME_PATH,
            headless=True,
            args=["--disable-web-security", "--allow-running-insecure-content", "--no-sandbox"]
        )
        context = browser.new_context(
            viewport={"width": 1600, "height": 1100},
            device_scale_factor=2
        )
        page = context.new_page()

        # Step 1: Open app and switch to Showcase tab
        print("\n[Step 1] Loading application and navigating to Tailwind Showcase tab...")
        page.goto(BASE_URL, wait_until="networkidle", timeout=30000)
        time.sleep(4)

        showcase_tab = page.locator('nav[aria-label="Tabs"] button:has-text("Tailwind Showcase")').first
        if showcase_tab.is_visible():
            showcase_tab.click()
            time.sleep(2)

        page.wait_for_selector("text=Next-Gen Modern UI Patterns", timeout=15000)
        hero_shot = os.path.join(SCREENSHOTS_DIR, "01_showcase_hero_with_templates_entry.png")
        page.screenshot(path=hero_shot, full_page=False)
        print("  --> Saved: 01_showcase_hero_with_templates_entry.png")

        # Step 2: Switch to "UI Templates" tab
        print("\n[Step 2] Navigating to 'UI Templates' tab...")
        templates_tab = page.locator('nav[aria-label="Tabs"] button:has-text("UI Templates")').first
        templates_tab.click()
        time.sleep(2)

        page.wait_for_selector("text=Enterprise Templates Gallery", timeout=15000)
        gallery_shot = os.path.join(SCREENSHOTS_DIR, "02_templates_gallery_overview.png")
        page.screenshot(path=gallery_shot, full_page=False)
        print("  --> Saved: 02_templates_gallery_overview.png")

        # Step 3: Validate Fintech Analytics Dashboard
        print("\n[Step 3] Validating Fintech & Payments Cockpit...")
        page.wait_for_selector("text=Payment & Settlement Treasury", timeout=10000)
        page.wait_for_selector("text=$14.82M", timeout=5000)
        page.wait_for_selector("text=$9.45M", timeout=5000)
        page.wait_for_selector("text=Daily Cash Flow & Settlement Volume", timeout=5000)
        page.wait_for_selector("text=Recent Clearance Records", timeout=5000)

        fintech_shot = os.path.join(SCREENSHOTS_DIR, "03_fintech_treasury_cockpit_e2e.png")
        page.screenshot(path=fintech_shot, full_page=False)
        print("  --> Saved: 03_fintech_treasury_cockpit_e2e.png")

        # Scroll to view ledger table and donut
        page.locator("text=Recent Clearance Records").first.scroll_into_view_if_needed()
        time.sleep(1)
        fintech_ledger_shot = os.path.join(SCREENSHOTS_DIR, "03b_fintech_settlement_ledger_e2e.png")
        page.screenshot(path=fintech_ledger_shot, full_page=False)
        print("  --> Saved: 03b_fintech_settlement_ledger_e2e.png")

        # Step 4: Validate AI Agent Operations & Telemetry
        print("\n[Step 4] Switching to AI Agent Operations & Telemetry...")
        page.locator("button:has-text('AI Agent Operations & Telemetry')").first.click()
        time.sleep(2)
        page.wait_for_selector("text=Autonomous Agent Telemetry", timeout=10000)
        page.wait_for_selector("text=248k /s", timeout=5000)
        page.wait_for_selector("text=48 ms", timeout=5000)
        page.wait_for_selector("text=Direct Agent Prompt & Tool Dispatch", timeout=5000)

        ai_shot = os.path.join(SCREENSHOTS_DIR, "04_ai_operations_control_plane_e2e.png")
        page.screenshot(path=ai_shot, full_page=False)
        print("  --> Saved: 04_ai_operations_control_plane_e2e.png")

        # Type an enterprise command into the AI Prompt Composer
        composer_input = page.locator("textarea").first
        if composer_input.is_visible():
            composer_input.fill("Auto-balance EUR/USD settlement reserves with max 0.02% slippage threshold.")
            time.sleep(1)

        # Scroll down to prompt composer & live execution trail
        page.locator("text=Live Agent Execution Trail").first.scroll_into_view_if_needed()
        time.sleep(1)
        ai_composer_shot = os.path.join(SCREENSHOTS_DIR, "04b_ai_prompt_composer_and_timeline_e2e.png")
        page.screenshot(path=ai_composer_shot, full_page=False)
        print("  --> Saved: 04b_ai_prompt_composer_and_timeline_e2e.png")

        # Step 5: Validate SaaS Product Marketing Suite & Pricing Panels
        print("\n[Step 5] Switching to SaaS Product Marketing Suite...")
        page.locator("text=Enterprise Templates Gallery").first.scroll_into_view_if_needed()
        time.sleep(1)
        page.locator("button:has-text('SaaS Product Marketing Suite')").first.click()
        time.sleep(2)
        page.wait_for_selector("text=One Platform. Infinite Enterprise Velocity.", timeout=10000)
        page.wait_for_selector("text=Engineered for Production Scale", timeout=5000)

        saas_shot = os.path.join(SCREENSHOTS_DIR, "05_saas_product_landing_e2e.png")
        page.screenshot(path=saas_shot, full_page=False)
        print("  --> Saved: 05_saas_product_landing_e2e.png")

        # Scroll down to pricing section
        print("  --> Validating SaaS Pricing Panels (Annual Billing)...")
        page.locator("text=Simple, Transparent Enterprise Licensing").first.scroll_into_view_if_needed()
        time.sleep(1)

        # Ensure Annual billing is active and capture
        annual_btn = page.locator("button:has-text('Annual billing')").first
        if annual_btn.is_visible():
            annual_btn.click()
            time.sleep(1)

        saas_pricing_shot = os.path.join(SCREENSHOTS_DIR, "05b_saas_pricing_matrix_e2e.png")
        page.screenshot(path=saas_pricing_shot, full_page=False)
        print("  --> Saved: 05b_saas_pricing_matrix_e2e.png")

        # Test Monthly billing toggle and capture to prove zero layout shift
        print("  --> Validating SaaS Pricing Panels (Monthly Billing)...")
        monthly_btn = page.locator("button:has-text('Monthly billing')").first
        if monthly_btn.is_visible():
            monthly_btn.click()
            time.sleep(1)
            saas_monthly_shot = os.path.join(SCREENSHOTS_DIR, "05c_saas_pricing_monthly_e2e.png")
            page.screenshot(path=saas_monthly_shot, full_page=False)
            print("  --> Saved: 05c_saas_pricing_monthly_e2e.png")

            # Toggle back to Annual
            annual_btn.click()
            time.sleep(1)

        # Step 6: Validate Building Blocks (5) Category
        print("\n[Step 6] Switching to Building Blocks category...")
        page.locator("text=Enterprise Templates Gallery").first.scroll_into_view_if_needed()
        time.sleep(1)
        blocks_cat_btn = page.locator("button:has-text('Building Blocks (5)')").first
        if blocks_cat_btn.is_visible():
            blocks_cat_btn.click()
            time.sleep(2)
            page.wait_for_selector("text=Accelerate Enterprise Blazor Development", timeout=10000)
            blocks_shot = os.path.join(SCREENSHOTS_DIR, "06_building_blocks_interactive_suite_e2e.png")
            page.screenshot(path=blocks_shot, full_page=False)
            print("  --> Saved: 06_building_blocks_interactive_suite_e2e.png")

            # Click Compact KPI Cards block
            page.locator("button:has-text('Compact KPI Cards')").first.click()
            time.sleep(1)
            cards_shot = os.path.join(SCREENSHOTS_DIR, "06b_building_blocks_compact_kpi_cards_e2e.png")
            page.screenshot(path=cards_shot, full_page=False)
            print("  --> Saved: 06b_building_blocks_compact_kpi_cards_e2e.png")

        # Step 7: Validate Responsive Viewport Frames
        print("\n[Step 7] Testing Responsive Mobile Frame Mode...")
        # Switch back to Page Templates -> Fintech Cockpit
        page.locator("button:has-text('Page Templates (3)')").first.click()
        time.sleep(1)
        mobile_btn = page.locator("button[title='Mobile Preview']").first
        if mobile_btn.is_visible():
            mobile_btn.click()
            time.sleep(2)
            mobile_shot = os.path.join(SCREENSHOTS_DIR, "07_templates_responsive_mobile_frame_e2e.png")
            page.screenshot(path=mobile_shot, full_page=False)
            print("  --> Saved: 07_templates_responsive_mobile_frame_e2e.png")

        browser.close()
        print("\n" + "=" * 70)
        print("E2E SCREENSHOT VALIDATION FINISHED SUCCESSFULLY!")
        print("=" * 70)

if __name__ == "__main__":
    run()
