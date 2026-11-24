import os
import glob
import fitz
import win32print
import win32ui
import win32con
from PIL import Image, ImageWin

def print_pdf(pdf_path, printer_name):
    print(f"\n--- Printing: {os.path.basename(pdf_path)} to {printer_name} ---")
    doc = fitz.open(pdf_path)
    total_pages = len(doc)
    print(f"Document has {total_pages} page(s).")

    hdc = win32ui.CreateDC()
    hdc.CreatePrinterDC(printer_name)

    printable_w = hdc.GetDeviceCaps(win32con.HORZRES)
    printable_h = hdc.GetDeviceCaps(win32con.VERTRES)
    dpi_x = hdc.GetDeviceCaps(win32con.LOGPIXELSX)
    dpi_y = hdc.GetDeviceCaps(win32con.LOGPIXELSY)

    job_title = f"BlazorTailwindUI - {os.path.basename(pdf_path)}"
    hdc.StartDoc(job_title)

    try:
        # Match printer DPI (360 DPI for Epson ET-3850)
        zoom_x = dpi_x / 72.0
        zoom_y = dpi_y / 72.0
        mat = fitz.Matrix(zoom_x, zoom_y)

        for page_num in range(total_pages):
            page = doc[page_num]
            pix = page.get_pixmap(matrix=mat, alpha=False)
            img = Image.frombytes("RGB", [pix.width, pix.height], pix.samples)

            # Determine fit within printable rectangle with 0.15 in margin
            margin_x = int(0.15 * dpi_x)
            margin_y = int(0.15 * dpi_y)
            avail_w = printable_w - (2 * margin_x)
            avail_h = printable_h - (2 * margin_y)

            scale = min(avail_w / img.width, avail_h / img.height)
            target_w = int(img.width * scale)
            target_h = int(img.height * scale)

            dest_x = margin_x + (avail_w - target_w) // 2
            dest_y = margin_y + (avail_h - target_h) // 2

            hdc.StartPage()
            dib = ImageWin.Dib(img)
            dib.draw(hdc.GetHandleOutput(), (dest_x, dest_y, dest_x + target_w, dest_y + target_h))
            hdc.EndPage()
            print(f"  ✓ Page {page_num + 1}/{total_pages} rasterized & sent to spooler ({target_w}x{target_h} px)")

        hdc.EndDoc()
        print(f"✓ Completed print job: {os.path.basename(pdf_path)}")
    except Exception as e:
        print(f"Error printing {pdf_path}: {e}")
        try:
            hdc.AbortDoc()
        except:
            pass
        raise
    finally:
        del hdc
        doc.close()

def main():
    printer_name = win32print.GetDefaultPrinter()
    print(f"Target Default Printer: {printer_name}")

    sample_dir = r"C:\repos\BlazorTailwindUI\docs\sample_pdfs"
    pdf_files = sorted(glob.glob(os.path.join(sample_dir, "*.pdf")))

    if not pdf_files:
        print(f"No PDF files found in {sample_dir}")
        return

    print(f"Found {len(pdf_files)} sample PDF(s) to print:")
    for f in pdf_files:
        print(f" - {os.path.basename(f)}")

    for pdf in pdf_files:
        print_pdf(pdf, printer_name)

    print("\n🎉 ALL 5 SAMPLE PDF DOCUMENTS SUCCESSFULLY SENT TO PRINTER SPOOLER!")

if __name__ == "__main__":
    main()
