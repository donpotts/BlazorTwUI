import subprocess
import json
import sys
import os

def run_test():
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
        raw = json.dumps(req) + '\n'
        proc.stdin.write(raw)
        proc.stdin.flush()
        line = proc.stdout.readline()
        return json.loads(line)

    print("1. Testing 'initialize'...")
    res = send_request({
        "jsonrpc": "2.0",
        "id": 1,
        "method": "initialize",
        "params": {}
    })
    assert res['result']['serverInfo']['name'] == 'blazor-tw-mcp'
    print("   -> Success! Server initialized:", res['result']['serverInfo'])

    print("2. Testing 'tools/list'...")
    res = send_request({
        "jsonrpc": "2.0",
        "id": 2,
        "method": "tools/list",
        "params": {}
    })
    tools = res['result']['tools']
    print(f"   -> Success! Found {len(tools)} tools: {[t['name'] for t in tools]}")

    print("3. Testing 'tools/call' for 'blazortw_search_components' (query: 'micr')...")
    res = send_request({
        "jsonrpc": "2.0",
        "id": 3,
        "method": "tools/call",
        "params": {
            "name": "blazortw_search_components",
            "arguments": { "query": "micr" }
        }
    })
    text = res['result']['content'][0]['text']
    assert "TwMicrCheckReader" in text
    print("   -> Success! Search returned TwMicrCheckReader.")

    print("4. Testing 'tools/call' for 'blazortw_validate_micr' (valid check line)...")
    res = send_request({
        "jsonrpc": "2.0",
        "id": 4,
        "method": "tools/call",
        "params": {
            "name": "blazortw_validate_micr",
            "arguments": { "micrLine": "⑆121000358⑆ 0123456789⑈ 1042" }
        }
    })
    text = res['result']['content'][0]['text']
    assert "Valid Mod-10 Checksum" in text
    print("   -> Success! Validated routing 121000358 with Mod-10 Checksum.")

    print("5. Testing 'tools/call' for 'blazortw_validate_micr' (invalid check line)...")
    res = send_request({
        "jsonrpc": "2.0",
        "id": 5,
        "method": "tools/call",
        "params": {
            "name": "blazortw_validate_micr",
            "arguments": { "micrLine": "⑆123456789⑆ 0011223344⑈ 9901" }
        }
    })
    text = res['result']['content'][0]['text']
    assert "FAILED Checksum" in text
    print("   -> Success! Correctly flagged invalid routing checksum.")

    print("6. Testing 'tools/call' for 'blazortw_get_component_api' ('TwDataGrid')...")
    res = send_request({
        "jsonrpc": "2.0",
        "id": 6,
        "method": "tools/call",
        "params": {
            "name": "blazortw_get_component_api",
            "arguments": { "componentName": "TwDataGrid" }
        }
    })
    text = res['result']['content'][0]['text']
    assert "Parameter Specifications" in text
    print("   -> Success! Component API specification retrieved.")

    print("7. Testing 'tools/call' for 'blazortw_list_forms'...")
    res = send_request({
        "jsonrpc": "2.0",
        "id": 7,
        "method": "tools/call",
        "params": {
            "name": "blazortw_list_forms",
            "arguments": {}
        }
    })
    text = res['result']['content'][0]['text']
    assert "100 Standard Enterprise Forms" in text
    print("   -> Success! 100 enterprise forms listed.")

    print("8. Testing 'resources/list'...")
    res = send_request({
        "jsonrpc": "2.0",
        "id": 8,
        "method": "resources/list",
        "params": {}
    })
    res_list = res['result']['resources']
    print(f"   -> Success! Found {len(res_list)} resources.")

    proc.terminate()
    print("\nALL MCP SERVER TESTS PASSED 100%!")

if __name__ == '__main__':
    run_test()
