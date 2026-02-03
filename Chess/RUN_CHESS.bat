@echo off
REM Simple Chess Application Launcher for Windows
REM This batch file runs the Chess application

echo.
echo ========================================
echo       Chess Application Launcher
echo ========================================
echo.

REM Check if publish folder exists
if not exist "publish\" (
    echo ERROR: publish folder not found!
    echo Please make sure you extracted the Chess application correctly.
    echo.
    pause
    exit /b 1
)

REM Check if Chess.exe exists
if not exist "publish\Chess.exe" (
    echo ERROR: Chess.exe not found in publish folder!
    echo.
    pause
    exit /b 1
)

REM Check if .NET 8 Runtime is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo WARNING: .NET 8 Runtime not found!
    echo.
    echo You need to install .NET 8 Runtime to run Chess.
    echo Download from: https://dotnet.microsoft.com/download/dotnet/8.0
    echo.
    pause
    exit /b 1
)

echo Running Chess Application...
echo.

REM Launch the application
cd /d publish
start Chess.exe

if %errorlevel% equ 0 (
    echo Chess has been launched successfully!
) else (
    echo ERROR: Failed to launch Chess.
    echo Please make sure .NET 8 Runtime is installed.
)

cd ..
