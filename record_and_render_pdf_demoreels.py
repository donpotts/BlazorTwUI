import os
import sys
import time
import subprocess
import json
from playwright.sync_api import sync_playwright

sys.stdout.reconfigure(encoding='utf-8')

VIDEOS_DIR = r"C:\Users\donpo\.gemini\antigravity\brain\27e04055-7eb7-457e-ac4c-93c7084425b7\videos"
AUDIO_DIR = os.path.join(VIDEOS_DIR, "audio_tts")
TEMP_REC_DIR = os.path.join(VIDEOS_DIR, "temp_recordings")
os.makedirs(VIDEOS_DIR, exist_ok=True)
os.makedirs(TEMP_REC_DIR, exist_ok=True)

REELS = [
    {
        "id": 1,
        "name": "reel1_interactive_signer",
        "audio": "reel1_interactive_signer.mp3",
        "output": "reel1_interactive_signer_4k.mp4",
        "target_dur": 28.5
    },
    {
        "id": 2,
        "name": "reel2_form_maker_palette",
        "audio": "reel2_form_maker_palette.mp3",
        "output": "reel2_form_maker_palette_4k.mp4",
        "target_dur": 23.5
    },
    {
        "id": 3,
        "name": "reel3_vector_flattening_seal",
        "audio": "reel3_vector_flattening_seal.mp3",
        "output": "reel3_vector_flattening_seal_4k.mp4",
        "target_dur": 24.5
    },
    {
        "id": 4,
        "name": "reel4_enterprise_templates_bates",
        "audio": "reel4_enterprise_templates_bates.mp3",
        "output": "reel4_enterprise_templates_bates_4k.mp4",
        "target_dur": 22.5
    },
    {
        "id": 5,
        "name": "reel5_minimal_api_scalar",
        "audio": "reel5_minimal_api_scalar.mp3",
        "output": "reel5_minimal_api_scalar_4k.mp4",
        "target_dur": 23.5
    }
]

def mux_and_upscale_to_4k(raw_webm_path, audio_path, output_mp4_path, target_duration):
    print(f"\n--- Muxing & Transcoding to 4K UHD: {os.path.basename(output_mp4_path)} ---", flush=True)
    cmd = [
        "ffmpeg", "-y",
        "-i", raw_webm_path,
        "-i", audio_path,
        "-vf", "scale=3840:2160:flags=lanczos",
        "-c:v", "libx264",
        "-preset", "slow",
        "-crf", "18",
        "-pix_fmt", "yuv420p",
        "-r", "60",
        "-c:a", "aac",
        "-b:a", "192k",
        "-af", "loudnorm=I=-16:TP=-1.5:LRA=11",
        "-t", f"{target_duration:.2f}",
        "-movflags", "+faststart",
        output_mp4_path
    ]
    subprocess.run(cmd, check=True)
    print(f"✓ 4K UHD Master Created: {output_mp4_path} ({os.path.getsize(output_mp4_path):,} bytes)", flush=True)

def record_reel_1(context, page, rec_subfolder):
    pass

def record_reel_2(context, page, rec_subfolder):
    pass

def record_reel_3(context, page, rec_subfolder):
    pass

def record_reel_4(context, page, rec_subfolder):
    pass

def record_reel_5(context, page, rec_subfolder):
    print("Recording Reel 5: Cross-Platform Minimal APIs & Scalar OpenAPI...", flush=True)
    start_time = time.time()
    
    page.goto("http://127.0.0.1:5128/scalar/v1/index.html", wait_until="networkidle")
    time.sleep(3.5)
    
    # Smooth scroll through Scalar API Reference
    page.mouse.move(400, 300)
    time.sleep(1.5)
    
    # Click generate endpoint using exact text matching
    try:
        gen_item = page.get_by_text("/api/pdf/generate").first
        if gen_item.count() > 0:
            gen_item.click()
            time.sleep(2.0)
    except Exception as e:
        print(f"Notice gen: {e}")
        
    page.mouse.wheel(0, 400)
    time.sleep(2.5)
    
    try:
        flatten_item = page.get_by_text("/api/pdf/flatten").first
        if flatten_item.count() > 0:
            flatten_item.click()
            time.sleep(2.0)
    except Exception as e:
        print(f"Notice flatten: {e}")
        
    page.mouse.wheel(0, 400)
    time.sleep(2.5)
    
    try:
        combine_item = page.get_by_text("/api/pdf/combine").first
        if combine_item.count() > 0:
            combine_item.click()
            time.sleep(2.0)
    except Exception as e:
        print(f"Notice combine: {e}")
        
    page.mouse.wheel(0, 300)
    time.sleep(2.5)
    
    elapsed = time.time() - start_time
    remaining = max(0, 23.5 - elapsed)
    time.sleep(remaining + 0.5)

def run():
    rec_funcs = [
        record_reel_1,
        record_reel_2,
        record_reel_3,
        record_reel_4,
        record_reel_5
    ]
    
    with sync_playwright() as p:
        for idx, reel in enumerate(REELS):
            output_mp4 = os.path.join(VIDEOS_DIR, reel["output"])
            if os.path.exists(output_mp4) and os.path.getsize(output_mp4) > 1000000:
                print(f"Skipping already rendered reel: {reel['output']} ({os.path.getsize(output_mp4):,} bytes)", flush=True)
                continue
                
            subfolder = os.path.join(TEMP_REC_DIR, f"rec_{reel['id']}")
            os.makedirs(subfolder, exist_ok=True)
            
            # Clean old webms in subfolder
            for f in os.listdir(subfolder):
                if f.endswith(".webm"):
                    try:
                        os.remove(os.path.join(subfolder, f))
                    except Exception:
                        pass
            
            # Unzoomed 1920x1080 recording (1:1 aspect ratio, full-screen canvas edge-to-edge)
            browser = p.chromium.launch(headless=True)
            context = browser.new_context(
                viewport={"width": 1920, "height": 1080},
                record_video_dir=subfolder,
                record_video_size={"width": 1920, "height": 1080}
            )
            page = context.new_page()
            
            # Execute reel actions
            rec_funcs[idx](context, page, subfolder)
            
            # Close to finalize webm video file
            context.close()
            browser.close()
            
            # Locate saved webm
            webm_files = [os.path.join(subfolder, f) for f in os.listdir(subfolder) if f.endswith(".webm")]
            if not webm_files:
                raise RuntimeError(f"No webm recorded for reel {reel['id']}")
            raw_webm = webm_files[0]
            
            audio_path = os.path.join(AUDIO_DIR, reel["audio"])
            
            # Mux with TTS narration & Lanczos 4K upscale
            mux_and_upscale_to_4k(raw_webm, audio_path, output_mp4, reel["target_dur"])
            
    print("\n=======================================================", flush=True)
    print("🎉 ALL 5 4K UHD DEMOREELS RECORDED & RENDERED AWESOME!", flush=True)
    print("=======================================================", flush=True)

if __name__ == "__main__":
    run()
