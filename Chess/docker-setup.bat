@echo off
REM Chess Docker Setup Script for Windows

setlocal enabledelayedexpansion

echo.
echo ?? Chess Docker Setup - Windows
echo ================================
echo.

REM Create directories for volumes if they don't exist
echo Creating volume directories...
if not exist ".\stockfish" mkdir .\stockfish
if not exist ".\chess-data" mkdir .\chess-data

REM Check if Stockfish exists
if not exist ".\stockfish\stockfish.exe" (
    echo.
    echo ??  Stockfish not found. Please download it from:
    echo https://stockfishchess.org/download/
    echo.
    echo Extract and place the stockfish executable in:
    echo %cd%\stockfish\stockfish.exe
    echo.
    pause
) else (
    echo ? Stockfish found
)

REM Check Docker installation
where docker >nul 2>nul
if errorlevel 1 (
    echo ??  Docker is not installed or not in PATH
    echo Please install Docker Desktop from: https://www.docker.com/products/docker-desktop
    pause
    exit /b 1
)

echo ? Docker is installed
echo.

REM Choose mode
echo Choose deployment mode:
echo 1 - Development
echo 2 - Production
set /p mode="Enter choice [1-2]: "

if "%mode%"=="1" (
    echo.
    echo Building development image...
    docker-compose build
    echo.
    echo Starting Chess in development mode...
    docker-compose up
) else if "%mode%"=="2" (
    echo.
    echo Building production image...
    docker-compose -f docker-compose.prod.yml build
    echo.
    echo Starting Chess in production mode...
    docker-compose -f docker-compose.prod.yml up -d
    echo.
    echo ? Chess is running in production mode
    echo View logs with: docker-compose -f docker-compose.prod.yml logs -f
) else (
    echo Invalid choice
    exit /b 1
)

endlocal
