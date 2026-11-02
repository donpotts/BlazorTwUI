#!/usr/bin/env python3
"""
BlazorTwUI Standalone High-Performance Demo Server
Features:
- Dynamic port discovery & port reservation
- Full Blazor WebAssembly MIME type mapping (.wasm, .dll, .json, .webmanifest)
- SPA client-side routing fallback to index.html
- Dedicated pre-flight health probe endpoint (/api/health)
- Graceful shutdown handlers
"""

import http.server
import socketserver
import os
import sys
import json
import time
import socket
import argparse
from urllib.parse import urlparse

DEFAULT_START_PORT = 5128
MAX_PORT_ATTEMPTS = 100

MIME_TYPES = {
    ".wasm": "application/wasm",
    ".dll": "application/octet-stream",
    ".json": "application/json",
    ".webmanifest": "application/manifest+json",
    ".js": "application/javascript",
    ".mjs": "application/javascript",
    ".css": "text/css",
    ".html": "text/html",
    ".ico": "image/x-icon",
    ".png": "image/png",
    ".jpg": "image/jpeg",
    ".jpeg": "image/jpeg",
    ".svg": "image/svg+xml",
    ".woff": "font/woff",
    ".woff2": "font/woff2",
    ".ttf": "font/ttf",
    ".otf": "font/otf",
    ".parquet": "application/octet-stream",
    ".pdf": "application/pdf"
}

START_TIME = time.time()

def find_available_port(start_port=DEFAULT_START_PORT, max_attempts=MAX_PORT_ATTEMPTS):
    """Scans and finds the first unbound TCP port."""
    for port in range(start_port, start_port + max_attempts):
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as sock:
            sock.settimeout(0.5)
            result = sock.connect_ex(("127.0.0.1", port))
            if result != 0:
                try:
                    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as bind_sock:
                        bind_sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
                        bind_sock.bind(("127.0.0.1", port))
                        return port
                except OSError:
                    continue
    return None

class BlazorWasmRequestHandler(http.server.SimpleHTTPRequestHandler):
    def end_headers(self):
        self.send_header("Access-Control-Allow-Origin", "*")
        self.send_header("Access-Control-Allow-Methods", "GET, POST, OPTIONS")
        self.send_header("Access-Control-Allow-Headers", "*")
        if self.path.endswith("appsettings.json") or self.path == "/" or self.path.endswith("index.html"):
            self.send_header("Cache-Control", "no-cache, no-store, must-revalidate")
        http.server.SimpleHTTPRequestHandler.end_headers(self)

    def do_OPTIONS(self):
        self.send_response(200)
        self.end_headers()

    def do_GET(self):
        parsed = urlparse(self.path)
        path = parsed.path

        if path in ("/api/health", "/healthz", "/api/probe"):
            self.send_response(200)
            self.send_header("Content-Type", "application/json")
            payload = json.dumps({
                "status": "Healthy",
                "uptimeSeconds": round(time.time() - START_TIME, 2),
                "port": self.server.server_address[1],
                "engine": "BlazorTwUI-ServerPortBridge",
                "version": "0.3.5"
            }).encode("utf-8")
            self.send_header("Content-Length", str(len(payload)))
            self.end_headers()
            self.wfile.write(payload)
            return

        local_path = self.translate_path(self.path)
        if not os.path.exists(local_path) and not os.path.splitext(path)[1]:
            self.path = "/index.html"

        return http.server.SimpleHTTPRequestHandler.do_GET(self)

    def guess_type(self, path):
        ext = os.path.splitext(path)[1].lower()
        if ext in MIME_TYPES:
            return MIME_TYPES[ext]
        return super().guess_type(path)

    def log_message(self, format, *args):
        if "api/health" not in args[0]:
            sys.stdout.write(f"[{time.strftime('%H:%M:%S')}] {format % args}\n")
            sys.stdout.flush()

class ThreadedHTTPServer(socketserver.ThreadingMixIn, http.server.HTTPServer):
    daemon_threads = True
    allow_reuse_address = True

def resolve_root_dir(candidate):
    script_dir = os.path.dirname(os.path.abspath(__file__))
    candidates = [
        candidate,
        os.path.join(script_dir, candidate),
        os.path.join(script_dir, "dist", "wwwroot"),
        os.path.join(script_dir, "wwwroot"),
        script_dir,
        os.getcwd()
    ]
    for c in candidates:
        if os.path.isdir(c) and os.path.exists(os.path.join(c, "index.html")):
            return os.path.abspath(c)
    for c in candidates:
        if os.path.isdir(c):
            return os.path.abspath(c)
    return os.path.abspath(".")

def run(root_dir="dist/wwwroot", requested_port=None):
    resolved_root = resolve_root_dir(root_dir)
    os.chdir(resolved_root)
    print(f"[*] Serving web demo assets from: {resolved_root}")

    port = requested_port
    if not port or port <= 0:
        port = find_available_port(DEFAULT_START_PORT)
        if not port:
            print("[-] Error: Unable to find an available port between 5128 and 5228.")
            sys.exit(1)

    appsettings_path = "appsettings.json"
    try:
        settings_data = {
            "Server": {
                "ApiPort": port,
                "ApiBaseUrl": f"http://127.0.0.1:{port}",
                "HealthEndpoint": "/api/health",
                "AutoPortDiscovery": True,
                "Status": "Healthy"
            }
        }
        with open(appsettings_path, "w", encoding="utf-8") as f:
            json.dump(settings_data, f, indent=2)
        print(f"[+] Synchronized active port {port} into {appsettings_path}")
    except Exception as e:
        print(f"[!] Warning: Could not update appsettings.json: {e}")

    try:
        httpd = ThreadedHTTPServer(("127.0.0.1", port), BlazorWasmRequestHandler)
    except OSError as e:
        print(f"[-] Port {port} binding failed: {e}. Retrying with next available port...")
        port = find_available_port(port + 1)
        httpd = ThreadedHTTPServer(("127.0.0.1", port), BlazorWasmRequestHandler)

    print(f"[+] API & Web Server listening on: http://127.0.0.1:{port}")
    print(f"[+] Health probe endpoint: http://127.0.0.1:{port}/api/health")
    print("[+] Ready for UI client connections.")
    sys.stdout.flush()

    try:
        httpd.serve_forever()
    except KeyboardInterrupt:
        print("\n[*] Server stopping cleanly...")
        httpd.shutdown()
        httpd.server_close()
        print("[+] Server stopped.")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="BlazorTwUI Standalone Demo Server")
    parser.add_argument("--port", type=int, default=0, help="Specific port to bind (default: auto-find starting at 5128)")
    parser.add_argument("--root", type=str, default="dist/wwwroot", help="Directory containing WASM assets")
    args = parser.parse_args()

    run(root_dir=args.root, requested_port=args.port)
