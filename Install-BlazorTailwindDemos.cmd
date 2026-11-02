@echo off
setlocal
cd /d "%~dp0"
title Blazor Tailwind UI -- Demo Suite Installer

echo =================================================================
echo    Blazor Tailwind UI -- Enterprise Demo Suite (v0.3.5)
echo    Windows Installer with Pre-Flight Port Coordinator Setup
echo =================================================================
echo.

powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0install-demo-suite.ps1" %*

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [!] Installation encountered an error (Code: %ERRORLEVEL%).
    pause
    exit /b %ERRORLEVEL%
)

echo.
set /p LAUNCH="Would you like to launch the demos now? (Y/N): "
if /i "%LAUNCH%"=="Y" (
    powershell -NoProfile -ExecutionPolicy Bypass -File "%LOCALAPPDATA%\BlazorTailwindUI\Demos\launch-demos.ps1"
)
