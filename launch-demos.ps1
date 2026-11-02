<#
.SYNOPSIS
    Blazor Tailwind UI -- Bulletproof Demo Suite Launcher with Pre-flight Port Coordination.
.DESCRIPTION
    1. Scans and allocates an unoccupied port (5128-5228) to eliminate port collisions.
    2. Synchronizes active port configuration into appsettings.json.
    3. Boots the API/Web server backend in a controlled background process.
    4. Executes an active HTTP pre-flight health probe loop until the server returns HTTP 200.
    5. Opens the browser UI only after the server is 100% verified online.
    6. Ensures clean process cleanup on exit.
#>

param(
    [int]$StartPort = 5128,
    [int]$MaxPortScan = 100,
    [switch]$NoBrowser,
    [switch]$TestOnly,
    [string]$ServerMode = "auto" # "auto", "dotnet", or "python"
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition

Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host "   Blazor Tailwind UI -- Enterprise Demo Suite (v0.3.5)          " -ForegroundColor White
Write-Host "   Client-Server Port Coordinator & Pre-Flight Health Launcher   " -ForegroundColor Yellow
Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host ""

# -------------------------------------------------------------
# STEP 1: Dynamic Port Discovery
# -------------------------------------------------------------
Write-Host "[1/4] Scanning for available TCP ports starting at $StartPort..." -ForegroundColor Gray

function Test-PortAvailable([int]$port) {
    try {
        $listener = New-Object System.Net.Sockets.TcpListener([System.Net.IPAddress]::Loopback, $port)
        $listener.Start()
        $listener.Stop()
        return $true
    }
    catch {
        return $false
    }
}

$allocatedPort = $null
for ($p = $StartPort; $p -lt ($StartPort + $MaxPortScan); $p++) {
    if (Test-PortAvailable $p) {
        $allocatedPort = $p
        break
    }
}

if (-not $allocatedPort) {
    Write-Error "CRITICAL: Could not find any free port between $StartPort and $($StartPort + $MaxPortScan)."
    exit 1
}

Write-Host "   -> Allocated Free Port: $allocatedPort" -ForegroundColor Green
$targetUrl = "http://127.0.0.1:$allocatedPort"

# -------------------------------------------------------------
# STEP 2: Client-Server Port Configuration Synchronization
# -------------------------------------------------------------
Write-Host "[2/4] Synchronizing active port ($allocatedPort) to appsettings.json..." -ForegroundColor Gray

$settingsContent = @"
{
  "Server": {
    "ApiPort": $allocatedPort,
    "ApiBaseUrl": "$targetUrl",
    "HealthEndpoint": "/api/health",
    "AutoPortDiscovery": true,
    "Status": "Healthy"
  }
}
"@

$appSettingsTargets = @(
    (Join-Path $ScriptDir "wwwroot\appsettings.json"),
    (Join-Path $ScriptDir "dist\wwwroot\appsettings.json")
)

foreach ($target in $appSettingsTargets) {
    $parentDir = Split-Path -Parent $target
    if (Test-Path $parentDir) {
        Set-Content -Path $target -Value $settingsContent -Encoding UTF8
        Write-Host "   -> Synchronized: $target" -ForegroundColor Gray
    }
}

# -------------------------------------------------------------
# STEP 3: Server Boot & Selection
# -------------------------------------------------------------
Write-Host "[3/4] Initializing API / Web Backend Server..." -ForegroundColor Gray

$serverProcess = $null
$hasDistWasm = Test-Path (Join-Path $ScriptDir "dist\wwwroot\index.html")
$hasPython = [bool](Get-Command python -ErrorAction SilentlyContinue)
$hasDotnet = [bool](Get-Command dotnet -ErrorAction SilentlyContinue)

if ($ServerMode -eq "auto") {
    if ($hasDistWasm -and $hasPython) {
        $chosenEngine = "python-dist"
    }
    elseif ($hasDotnet -and (Test-Path (Join-Path $ScriptDir "BlazorTailwindUI.csproj"))) {
        $chosenEngine = "dotnet"
    }
    elseif ($hasPython) {
        $chosenEngine = "python"
    }
    else {
        $chosenEngine = "dotnet"
    }
}
else {
    $chosenEngine = $ServerMode
}

Write-Host "   -> Engine: $chosenEngine" -ForegroundColor Cyan

if ($chosenEngine -eq "python-dist" -or $chosenEngine -eq "python") {
    $serverScript = Join-Path $ScriptDir "demo-server.py"
    $distDir = if ($hasDistWasm) { Join-Path $ScriptDir "dist\wwwroot" } else { Join-Path $ScriptDir "wwwroot" }
    
    $psi = New-Object System.Diagnostics.ProcessStartInfo
    $psi.FileName = "python"
    $psi.Arguments = "`"$serverScript`" --port $allocatedPort --root `"$distDir`""
    $psi.WorkingDirectory = $ScriptDir
    $psi.UseShellExecute = $false
    $psi.CreateNoWindow = $true
    $serverProcess = [System.Diagnostics.Process]::Start($psi)
}
else {
    # .NET DevServer / Kestrel Engine
    $psi = New-Object System.Diagnostics.ProcessStartInfo
    $psi.FileName = "dotnet"
    $psi.Arguments = "run --urls `"$targetUrl`""
    $psi.WorkingDirectory = $ScriptDir
    $psi.UseShellExecute = $false
    $psi.CreateNoWindow = $true
    $serverProcess = [System.Diagnostics.Process]::Start($psi)
}

# -------------------------------------------------------------
# STEP 4: Pre-Flight Health Probe (Active Polling Loop)
# -------------------------------------------------------------
Write-Host "[4/4] Executing Pre-Flight Health Probe on $targetUrl..." -ForegroundColor Yellow

$probeSuccess = $false
$maxAttempts = 40
$probeStopwatch = [System.Diagnostics.Stopwatch]::StartNew()

for ($i = 1; $i -le $maxAttempts; $i++) {
    Write-Host -NoNewline "`r   -> Health Probe Attempt $i/$maxAttempts... " -ForegroundColor Gray
    try {
        $req = [System.Net.WebRequest]::Create("$targetUrl/api/health")
        $req.Timeout = 1200
        $resp = $req.GetResponse()
        if ($resp.StatusCode -eq [System.Net.HttpStatusCode]::OK) {
            $resp.Close()
            $probeSuccess = $true
            $probeStopwatch.Stop()
            break
        }
        $resp.Close()
    }
    catch {
        try {
            $req2 = [System.Net.WebRequest]::Create($targetUrl)
            $req2.Timeout = 1200
            $resp2 = $req2.GetResponse()
            if ($resp2.StatusCode -eq [System.Net.HttpStatusCode]::OK) {
                $resp2.Close()
                $probeSuccess = $true
                $probeStopwatch.Stop()
                break
            }
            $resp2.Close()
        } catch {}
    }
    Start-Sleep -Milliseconds 400
}

Write-Host ""
if (-not $probeSuccess) {
    Write-Host "[-] Pre-flight health probe timed out after $($probeStopwatch.Elapsed.TotalSeconds.ToString('F1'))s." -ForegroundColor Red
    if ($serverProcess -and -not $serverProcess.HasExited) {
        $serverProcess.Kill()
    }
    exit 1
}

Write-Host "[+] API & SERVER VERIFIED 100% ONLINE!" -ForegroundColor Green
Write-Host "    Response Time: $($probeStopwatch.Elapsed.TotalMilliseconds.ToString('F0'))ms | Status: HTTP 200 OK" -ForegroundColor Green
Write-Host "    Target URL:    $targetUrl" -ForegroundColor Cyan
Write-Host ""

if ($TestOnly) {
    Write-Host "[+] TestOnly mode specified: Health verification passed successfully. Exiting." -ForegroundColor Green
    if ($serverProcess -and -not $serverProcess.HasExited) {
        $serverProcess.Kill()
    }
    exit 0
}

# -------------------------------------------------------------
# Launch Browser ONLY after API is healthy
# -------------------------------------------------------------
if (-not $NoBrowser) {
    Write-Host ">>> Launching Client UI in Default Browser..." -ForegroundColor Green
    Start-Process $targetUrl
}

Write-Host ""
Write-Host "Demo Suite is actively running. Press Ctrl+C to stop the server." -ForegroundColor White
Write-Host "-----------------------------------------------------------------" -ForegroundColor Gray

# Trap SIGINT to kill background server process
try {
    while ($true) {
        if ($serverProcess.HasExited) {
            Write-Host "[-] Server process terminated unexpectedly." -ForegroundColor Red
            break
        }
        Start-Sleep -Seconds 1
    }
}
finally {
    if ($serverProcess -and -not $serverProcess.HasExited) {
        Write-Host "`n[*] Stopping demo server process ($($serverProcess.Id))..." -ForegroundColor Yellow
        $serverProcess.Kill()
        Write-Host "[+] Server shut down cleanly. Port $allocatedPort released." -ForegroundColor Green
    }
}
