import asyncio
import os
import edge_tts

OUTPUT_DIR = r"C:\Users\donpo\.gemini\antigravity\brain\27e04055-7eb7-457e-ac4c-93c7084425b7\videos\audio_tts"
os.makedirs(OUTPUT_DIR, exist_ok=True)

SCRIPTS = {
    "reel1_interactive_signer.mp3": (
        "Welcome to the Blazor Tailwind PDF Studio. Experience frictionless digital contract execution. "
        "In Signer View, users interact with intuitive, pulsing field anchors. "
        "Opening the Signature Adoption modal unlocks authentic calligraphic cursive script styles including Dancing Script and Great Vibes. "
        "With one click, your signature is legally adopted with instant UTC timestamping, ready for enterprise execution under the ESIGN Act."
    ),
    "reel2_form_maker_palette.mp3": (
        "Switch to Form Maker Studio Mode to design custom document workflows in real time. "
        "Our modular anchor palette lets you drop dynamic fields anywhere on the page, including signature zones, "
        "section initials, automatic date-time stamps, custom text fields, and compliance checkboxes. "
        "Every anchor supports fine-grained role assignment and responsive multi-page coordinate mapping."
    ),
    "reel3_vector_flattening_seal.mp3": (
        "Security and compliance are built into the core. "
        "With one click on Flatten and Stamp, all interactive form widgets are purged and converted into a permanent, "
        "static ISO 32000-1 content stream. "
        "Embedded vectors and calligraphic signatures are permanently baked into the PDF binary, "
        "sealed with a cryptographic SHA-256 integrity hash for unalterable legal archiving."
    ),
    "reel4_enterprise_templates_bates.mp3": (
        "Accelerate enterprise workflows with pre-built, production-ready templates. "
        "Seamlessly toggle between Master Services Agreements, commercial invoices with automated line-item calculations, "
        "mutual non-disclosure agreements, and completion certificates. "
        "Enable automated diagonal watermarking and sequential Bates numbering for litigation-grade document preparation."
    ),
    "reel5_minimal_api_scalar.mp3": (
        "Built for true cross-platform ubiquity. "
        "Our high-performance .NET Minimal API engine is fully documented with Scalar OpenAPI at slash scalar slash v one. "
        "With zero third-party commercial dependencies and pure MIT licensing, "
        "ready-to-use client adapters bridge seamlessly to React, React Native Expo, MudBlazor, and .NET MAUI Blazor Hybrid."
    )
}

VOICE = "en-US-JennyNeural"

async def generate_single(filename, text):
    path = os.path.join(OUTPUT_DIR, filename)
    print(f"Generating TTS narration: {filename}...")
    communicate = edge_tts.Communicate(text, VOICE, rate="+3%", pitch="+0Hz")
    await communicate.save(path)
    print(f"  -> Generated {path} ({os.path.getsize(path)} bytes)")

async def main():
    tasks = [generate_single(fname, text) for fname, text in SCRIPTS.items()]
    await asyncio.gather(*tasks)
    print("All 5 neural TTS narration tracks generated successfully!")

if __name__ == "__main__":
    asyncio.run(main())
