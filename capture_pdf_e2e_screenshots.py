import os
import time
import sys
from playwright.sync_api import sync_playwright

sys.stdout.reconfigure(encoding='utf-8')

screenshots_dir = r"C:\Users\donpo\.gemini\antigravity\brain\27e04055-7eb7-457e-ac4c-93c7084425b7\screenshots"
docs_images_dir = r"C:\repos\BlazorTailwindUI\docs\images"
os.makedirs(screenshots_dir, exist_ok=True)
os.makedirs(docs_images_dir, exist_ok=True)

with sync_playwright() as p:
    browser = p.chromium.launch(headless=True)
    # Standard 1920x1080 viewport, unzoomed 1:1 pixel rendering with device_scale_factor=2 for razor-sharp text
    context = browser.new_context(viewport={"width": 1920, "height": 1080}, device_scale_factor=2)
    page = context.new_page()

    print("Navigating to http://127.0.0.1:5128 ...", flush=True)
    page.goto("http://127.0.0.1:5128", wait_until="networkidle")
    time.sleep(2)

    # Click Core Components tab
    page.locator("button:has-text('Core Components')").first.click()
    time.sleep(2)

    # Scroll to PDF studio
    studio = page.locator("div.rounded-2xl:has-text('PDF Form Filler, Contract Signer')").first
    studio.scroll_into_view_if_needed()
    time.sleep(1)

    # 1. Signer View (Page 1)
    print("Capturing 01_signer_view_msa.png...", flush=True)
    shot1 = os.path.join(screenshots_dir, "01_signer_view_msa.png")
    shot1_docs = os.path.join(docs_images_dir, "01_signer_view_msa.png")
    page.screenshot(path=shot1, full_page=False)
    page.screenshot(path=shot1_docs, full_page=False)
    print("  Done 01", flush=True)

    # 2. Signature Adoption Modal (Navigate to Page 2 where Sign Here is located)
    print("Capturing 02_signature_adoption_modal.png...", flush=True)
    studio.locator("button:has-text('Page 2')").first.click()
    time.sleep(1)
    
    # Click button containing SIGN HERE to open signature adoption modal
    studio.locator("button:has-text('SIGN HERE')").first.click()
    time.sleep(1.2)
    
    shot2 = os.path.join(screenshots_dir, "02_signature_adoption_modal.png")
    shot2_docs = os.path.join(docs_images_dir, "02_signature_adoption_modal.png")
    page.screenshot(path=shot2, full_page=False)
    page.screenshot(path=shot2_docs, full_page=False)
    print("  Done 02", flush=True)

    # Close modal
    page.locator("div.fixed.inset-0 button:has-text('✕')").first.click()
    time.sleep(1)

    # 3. Form Maker Mode
    print("Capturing 03_form_maker_palette.png...", flush=True)
    studio.locator("button:has-text('Form Maker Mode')").first.click()
    time.sleep(1)
    shot3 = os.path.join(screenshots_dir, "03_form_maker_palette.png")
    shot3_docs = os.path.join(docs_images_dir, "03_form_maker_palette.png")
    page.screenshot(path=shot3, full_page=False)
    page.screenshot(path=shot3_docs, full_page=False)
    print("  Done 03", flush=True)

    # Switch back to Signer View
    studio.locator("button:has-text('Signer View')").first.click()
    time.sleep(1)

    # Pre-fill rest of data
    studio.locator("button:has-text('Pre-fill Data')").first.click()
    time.sleep(1)

    # 4. Flatten & Stamp PDF (Merged Stream)
    print("Capturing 04_flattened_stamped_stream.png...", flush=True)
    studio.locator("button:has-text('Flatten & Stamp')").first.click()
    time.sleep(1.5)
    shot4 = os.path.join(screenshots_dir, "04_flattened_stamped_stream.png")
    shot4_docs = os.path.join(docs_images_dir, "04_flattened_stamped_stream.png")
    page.screenshot(path=shot4, full_page=False)
    page.screenshot(path=shot4_docs, full_page=False)
    print("  Done 04", flush=True)

    # Unflatten to allow template change
    studio.locator("button:has-text('Unflatten')").first.click()
    time.sleep(1)

    # 5. Template selection: Commercial Invoice
    print("Capturing 05_commercial_invoice_template.png...", flush=True)
    select_elem = studio.locator("select").first
    select_elem.select_option("invoice")
    time.sleep(1.5)
    studio.locator("button:has-text('Page 1')").first.click()
    time.sleep(1)
    shot5 = os.path.join(screenshots_dir, "05_commercial_invoice_template.png")
    shot5_docs = os.path.join(docs_images_dir, "05_commercial_invoice_template.png")
    page.screenshot(path=shot5, full_page=False)
    page.screenshot(path=shot5_docs, full_page=False)
    print("  Done 05", flush=True)

    # 6. Watermark and Bates Numbering on NDA template
    print("Capturing 06_watermark_and_bates.png...", flush=True)
    select_elem.select_option("nda")
    time.sleep(1.5)
    shot6 = os.path.join(screenshots_dir, "06_watermark_and_bates.png")
    shot6_docs = os.path.join(docs_images_dir, "06_watermark_and_bates.png")
    page.screenshot(path=shot6, full_page=False)
    page.screenshot(path=shot6_docs, full_page=False)
    print("  Done 06", flush=True)

    # 7. Scalar API Reference
    print("Capturing 07_scalar_api_reference.png...", flush=True)
    page.goto("http://127.0.0.1:5128/scalar/v1", wait_until="networkidle")
    time.sleep(3)
    shot7 = os.path.join(screenshots_dir, "07_scalar_api_reference.png")
    shot7_docs = os.path.join(docs_images_dir, "07_scalar_api_reference.png")
    page.screenshot(path=shot7, full_page=False)
    page.screenshot(path=shot7_docs, full_page=False)
    print("  Done 07", flush=True)

    browser.close()
    print("ALL 7 HIGH-RESOLUTION UNZOOMED SCREENSHOTS CAPTURED SUCCESSFULLY!", flush=True)
