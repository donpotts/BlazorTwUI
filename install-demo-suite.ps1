<#
.SYNOPSIS
    Installs the Blazor Tailwind UI Enterprise Demo Suite to the local machine.
.DESCRIPTION
    Extracts/copies demo runtime assets to %LOCALAPPDATA%\BlazorTailwindUI\Demos,
    configures port coordinator launchers, creates Desktop and Start Menu shortcuts,
    and generates a clean uninstaller.
#>

param(
    [string]$InstallPath = (Join-Path $env:LOCALAPPDATA "BlazorTailwindUI\Demos"),
    [switch]$NoShortcuts,
    [switch]$Silent
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition

Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host "   Blazor Tailwind UI -- Enterprise Demo Suite Installer        " -ForegroundColor White
Write-Host "   Version: 0.3.5 | Client-Server Port-Guaranteed Distribution  " -ForegroundColor Yellow
Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "[1/5] Target Installation Path: $InstallPath" -ForegroundColor Gray
if (-not (Test-Path $InstallPath)) {
    New-Item -ItemType Directory -Path $InstallPath -Force | Out-Null
    Write-Host "   -> Created installation directory." -ForegroundColor Green
}

# Determine source files
$sourceDir = if (Test-Path (Join-Path $ScriptDir "dist\wwwroot")) {
    Join-Path $ScriptDir "dist\wwwroot"
} elseif (Test-Path (Join-Path $ScriptDir "wwwroot\_framework")) {
    Join-Path $ScriptDir "wwwroot"
} else {
    $ScriptDir
}

Write-Host "[2/5] Copying Demo Suite Assets from $sourceDir..." -ForegroundColor Gray
Copy-Item -Path "$sourceDir\*" -Destination $InstallPath -Recurse -Force

# Copy launchers and scripts
$auxiliaryFiles = @(
    (Join-Path $ScriptDir "demo-server.py"),
    (Join-Path $ScriptDir "launch-demos.ps1"),
    (Join-Path $ScriptDir "launch-demos.cmd")
)

foreach ($f in $auxiliaryFiles) {
    if (Test-Path $f) {
        Copy-Item -Path $f -Destination $InstallPath -Force
        Write-Host "   -> Installed coordinator tool: $(Split-Path -Leaf $f)" -ForegroundColor Gray
    }
}

# Generate dedicated uninstaller
Write-Host "[3/5] Generating uninstaller..." -ForegroundColor Gray
$uninstallerPath = Join-Path $InstallPath "Uninstall-Demos.cmd"
$desktopLnk = Join-Path ([Environment]::GetFolderPath("Desktop")) "Blazor Tailwind UI Demos.lnk"
$startMenuDir = Join-Path ([Environment]::GetFolderPath("Programs")) "Blazor Tailwind UI"
$startMenuLnk = Join-Path $startMenuDir "Blazor Tailwind UI Demos.lnk"

$uninstallScript = @"
@echo off
title Uninstalling Blazor Tailwind UI Demos
echo Removing Blazor Tailwind UI Demos...
if exist "$desktopLnk" del /f /q "$desktopLnk"
if exist "$startMenuLnk" del /f /q "$startMenuLnk"
if exist "$startMenuDir" rmdir /s /q "$startMenuDir"
echo Removing demo installation files...
cd /d "%LOCALAPPDATA%"
rmdir /s /q "$InstallPath"
echo Uninstall completed successfully.
pause
"@
Set-Content -Path $uninstallerPath -Value $uninstallScript -Encoding UTF8

# Create Shortcuts
if (-not $NoShortcuts) {
    Write-Host "[4/5] Creating Windows Desktop and Start Menu Shortcuts..." -ForegroundColor Gray
    $wshShell = New-Object -ComObject WScript.Shell

    $targetCmd = Join-Path $InstallPath "launch-demos.cmd"
    $iconPath = Join-Path $InstallPath "favicon.ico"

    # 1. Desktop Shortcut
    $shortcutDesktop = $wshShell.CreateShortcut($desktopLnk)
    $shortcutDesktop.TargetPath = $targetCmd
    $shortcutDesktop.WorkingDirectory = $InstallPath
    $shortcutDesktop.Description = "Blazor Tailwind UI Enterprise Demo Suite (v0.3.5) with Port Coordinator"
    if (Test-Path $iconPath) {
        $shortcutDesktop.IconLocation = "$iconPath,0"
    }
    $shortcutDesktop.Save()
    Write-Host "   -> Created Desktop Shortcut: $desktopLnk" -ForegroundColor Green

    # 2. Start Menu Shortcut
    if (-not (Test-Path $startMenuDir)) {
        New-Item -ItemType Directory -Path $startMenuDir -Force | Out-Null
    }
    $shortcutStartMenu = $wshShell.CreateShortcut($startMenuLnk)
    $shortcutStartMenu.TargetPath = $targetCmd
    $shortcutStartMenu.WorkingDirectory = $InstallPath
    $shortcutStartMenu.Description = "Blazor Tailwind UI Enterprise Demo Suite"
    if (Test-Path $iconPath) {
        $shortcutStartMenu.IconLocation = "$iconPath,0"
    }
    $shortcutStartMenu.Save()
    Write-Host "   -> Created Start Menu Shortcut: $startMenuLnk" -ForegroundColor Green
}

Write-Host "[5/5] Installation & Pre-Flight Setup Complete!" -ForegroundColor Green
Write-Host ""
Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host " Blazor Tailwind UI Demo Suite is installed and ready to launch!" -ForegroundColor White
Write-Host " Installation Directory: $InstallPath" -ForegroundColor Gray
Write-Host " To start anytime, double-click the desktop shortcut or run:     " -ForegroundColor Gray
Write-Host "   `"$InstallPath\launch-demos.cmd`"" -ForegroundColor Yellow
Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host ""
