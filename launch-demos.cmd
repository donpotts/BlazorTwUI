@echo off
setlocal
cd /d "%~dp0"
title Blazor Tailwind UI -- Demo Suite Launcher

echo =================================================================
echo    Blazor Tailwind UI -- Enterprise Demo Suite (v0.3.5)
echo    Client-Server Port Coordinator ^& Pre-Flight Health Launcher
echo =================================================================
echo.

powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0launch-demos.ps1" %*

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [!] Launcher exited with error code %ERRORLEVEL%.
    pause
)
