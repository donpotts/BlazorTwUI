import subprocess
import json
import os

def test_audit():
    server_path = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'server.js')
    proc = subprocess.Popen(
        ['node', server_path],
        stdin=subprocess.PIPE,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        text=True,
        encoding='utf-8'
    )

    def send_request(req):
        proc.stdin.write(json.dumps(req) + '\n')
        proc.stdin.flush()
        return json.loads(proc.stdout.readline())

    send_request({"jsonrpc": "2.0", "id": 1, "method": "initialize", "params": {}})

    # Audit compliant code
    res = send_request({
        "jsonrpc": "2.0",
        "id": 2,
        "method": "tools/call",
        "params": {
            "name": "blazortw_audit_razor",
            "arguments": {
                "razorCode": '<TwButton Variant="ButtonVariant.Primary" Class="py-2.5 px-4 bg-white dark:bg-slate-900 text-slate-900 dark:text-white">Save</TwButton>'
            }
        }
    })
    text = res['result']['content'][0]['text']
    assert "0 issues found" in text
    print("Compliant code passed audit cleanly.")

    # Audit code with issues
    res = send_request({
        "jsonrpc": "2.0",
        "id": 3,
        "method": "tools/call",
        "params": {
            "name": "blazortw_audit_razor",
            "arguments": {
                "razorCode": '<button class="bg-white">Click</button> <!-- legacy-ui test -->'
            }
        }
    })
    text = res['result']['content'][0]['text']
    assert "Potential Issues" in text
    print("Violations correctly flagged by audit.")

    # Test scaffold tool
    res = send_request({
        "jsonrpc": "2.0",
        "id": 4,
        "method": "tools/call",
        "params": {
            "name": "blazortw_scaffold_component",
            "arguments": {
                "componentName": "TwMicrCheckReader",
                "includeCodeBlock": True
            }
        }
    })
    text = res['result']['content'][0]['text']
    assert "TwMicrCheckReader" in text and "@code" in text
    print("Scaffolding tool generates Razor and C# code.")

    proc.terminate()
    print("AUDIT & SCAFFOLDING TESTS PASSED!")

if __name__ == '__main__':
    test_audit()
