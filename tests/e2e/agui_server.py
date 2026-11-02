"""
A small real AG-UI agent over server-sent events, for exercising SseAgentTransport end to end.

    python tests/e2e/agui_server.py [port]      # default 5300

POST /agui/run       streams AG-UI events (text/event-stream). Stops at the gated tool call and waits.
POST /agui/decision  the human's decision for a gated call; the run continues only after it arrives.
GET  /agui/_log      what the server received (decisions and run requests), for assertions.

CORS is open because the Blazor app is served from another origin.
"""
import json
import sys
import threading
import time
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer

PORT = int(sys.argv[1]) if len(sys.argv) > 1 else 5300
decisions = {}
log = {"runs": [], "decisions": []}
lock = threading.Lock()


def sse(handler, event):
    handler.wfile.write(f"data: {json.dumps(event)}\n\n".encode())
    handler.wfile.flush()


def chunks(text, size=10):
    return [text[i:i + size] for i in range(0, len(text), size)]


class Handler(BaseHTTPRequestHandler):
    protocol_version = "HTTP/1.1"

    def log_message(self, *args):
        pass

    def _cors(self):
        self.send_header("Access-Control-Allow-Origin", "*")
        self.send_header("Access-Control-Allow-Headers", "content-type, accept")
        self.send_header("Access-Control-Allow-Methods", "GET, POST, OPTIONS")

    def do_OPTIONS(self):
        self.send_response(204)
        self._cors()
        self.send_header("Content-Length", "0")
        self.end_headers()

    def do_GET(self):
        if self.path.startswith("/agui/_log"):
            body = json.dumps(log).encode()
            self.send_response(200)
            self._cors()
            self.send_header("Content-Type", "application/json")
            self.send_header("Content-Length", str(len(body)))
            self.end_headers()
            self.wfile.write(body)
        else:
            self.send_response(404)
            self._cors()
            self.send_header("Content-Length", "0")
            self.end_headers()

    def do_POST(self):
        length = int(self.headers.get("Content-Length") or 0)
        payload = json.loads(self.rfile.read(length) or b"{}")
        if self.path == "/agui/decision":
            with lock:
                decisions[payload["toolCallId"]] = payload
                log["decisions"].append(payload)
            self.send_response(204)
            self._cors()
            self.send_header("Content-Length", "0")
            self.end_headers()
        elif self.path == "/agui/run":
            self.run(payload)
        else:
            self.send_response(404)
            self._cors()
            self.send_header("Content-Length", "0")
            self.end_headers()

    def run(self, payload):
        log["runs"].append(payload)
        self.send_response(200)
        self._cors()
        self.send_header("Content-Type", "text/event-stream")
        self.send_header("Cache-Control", "no-cache")
        self.send_header("Connection", "close")
        self.end_headers()

        amount = (payload.get("state") or {}).get("refund.amount", "249.00")
        sse(self, {"type": "RUN_STARTED", "threadId": payload.get("threadId"), "runId": payload.get("runId")})
        for c in chunks("Checking the order with the live agent. "):
            sse(self, {"type": "TEXT_MESSAGE_CONTENT", "messageId": "m1", "delta": c})
            time.sleep(0.02)

        sse(self, {"type": "TOOL_CALL_START", "toolCallId": "live_lookup", "toolCallName": "lookup_order"})
        sse(self, {"type": "TOOL_CALL_ARGS", "toolCallId": "live_lookup", "delta": '{"orderId":"4821"}'})
        sse(self, {"type": "TOOL_CALL_END", "toolCallId": "live_lookup"})
        sse(self, {"type": "TOOL_CALL_RESULT", "toolCallId": "live_lookup", "content": "Order 4821: Walnut desk, $249.00"})
        sse(self, {"type": "STATE_SNAPSHOT", "snapshot": {"order.id": "4821", "refund.amount": "249.00", "refund.status": "proposed"}})

        sse(self, {"type": "TOOL_CALL_START", "toolCallId": "live_refund", "toolCallName": "issue_refund",
                   "risk": "high", "requiresApproval": True})
        sse(self, {"type": "TOOL_CALL_ARGS", "toolCallId": "live_refund", "delta": json.dumps({"orderId": "4821", "amount": amount})})
        sse(self, {"type": "TOOL_CALL_END", "toolCallId": "live_refund"})

        # The agent does nothing further until the human decides.
        deadline = time.time() + 60
        decision = None
        while time.time() < deadline and decision is None:
            with lock:
                decision = decisions.pop("live_refund", None)
            time.sleep(0.05)

        if decision and decision.get("approved"):
            final = amount
            if decision.get("editedArgs"):
                final = json.loads(decision["editedArgs"]).get("amount", amount)
            sse(self, {"type": "TOOL_CALL_RESULT", "toolCallId": "live_refund", "content": f"Refund of ${final} issued by the live agent."})
            sse(self, {"type": "STATE_DELTA", "delta": [{"op": "replace", "path": "/refund.status", "value": "issued"},
                                                       {"op": "replace", "path": "/refund.amount", "value": final}]})
            for c in chunks(f"Done. ${final} refunded."):
                sse(self, {"type": "TEXT_MESSAGE_CONTENT", "messageId": "m2", "delta": c})
        else:
            why = (decision or {}).get("reason") or "no decision"
            sse(self, {"type": "TOOL_CALL_RESULT", "toolCallId": "live_refund", "content": f"Not executed ({why})."})
            sse(self, {"type": "STATE_DELTA", "delta": [{"op": "replace", "path": "/refund.status", "value": "blocked"}]})
            sse(self, {"type": "TEXT_MESSAGE_CONTENT", "messageId": "m2", "delta": "Understood, I did not refund."})
        sse(self, {"type": "RUN_FINISHED"})
        self.close_connection = True


if __name__ == "__main__":
    print(f"AG-UI test agent on http://127.0.0.1:{PORT}/agui/run", flush=True)
    ThreadingHTTPServer(("127.0.0.1", PORT), Handler).serve_forever()
