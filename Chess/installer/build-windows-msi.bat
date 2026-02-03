@echo off
REM Windows Installer Builder for Chess Application
REM Requirements: WiX Toolset, .NET 8 SDK

setlocal enabledelayedexpansion

echo.
echo ?? Chess Windows Installer Builder
echo ==================================
echo.

set VERSION=1.0.0
set APP_NAME=Chess
set PUBLISHER=Chess Team
set PUBLISH_DIR=publish
set WIX_DIR=%ProgramFiles%\WiX Toolset v3.14

REM Check if publish directory exists
if not exist "%PUBLISH_DIR%" (
    echo ??  Publish directory not found. Building application...
    dotnet publish Chess/Chess.csproj -c Release -o %PUBLISH_DIR% --self-contained false
)

echo [1/4] Validating WiX installation...
if not exist "%WIX_DIR%" (
    echo ??  WiX Toolset not found!
    echo Please download from: https://github.com/wixtoolset/wix3/releases
    pause
    exit /b 1
)

echo ? WiX found at: %WIX_DIR%

echo.
echo [2/4] Compiling WiX source...
"%WIX_DIR%\bin\candle.exe" -o obj\ installer\Chess.wxs

echo.
echo [3/4] Linking installer...
"%WIX_DIR%\bin\light.exe" -o "%APP_NAME%-%VERSION%.msi" obj\Chess.wixobj -ext WixUIExtension

echo.
echo [4/4] Cleaning up...
rmdir /s /q obj

echo.
echo ? Windows installer created: %APP_NAME%-%VERSION%.msi
echo.

pause
