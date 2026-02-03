#!/bin/bash

# Simple Chess Application Launcher for macOS/Linux
# This script runs the Chess application

echo ""
echo "========================================"
echo "       Chess Application Launcher"
echo "========================================"
echo ""

# Check if publish folder exists
if [ ! -d "publish" ]; then
    echo "ERROR: publish folder not found!"
    echo "Please make sure you extracted the Chess application correctly."
    echo ""
    read -p "Press Enter to exit..."
    exit 1
fi

# Check if Chess executable exists
if [ ! -f "publish/Chess" ]; then
    echo "ERROR: Chess executable not found in publish folder!"
    echo ""
    read -p "Press Enter to exit..."
    exit 1
fi

# Check if .NET 8 Runtime is installed
if ! command -v dotnet &> /dev/null; then
    echo "WARNING: .NET 8 Runtime not found!"
    echo ""
    echo "You need to install .NET 8 Runtime to run Chess."
    echo "Download from: https://dotnet.microsoft.com/download/dotnet/8.0"
    echo ""
    read -p "Press Enter to exit..."
    exit 1
fi

echo "Running Chess Application..."
echo ""

# Make executable if not already
chmod +x publish/Chess

# Launch the application
cd publish
./Chess

if [ $? -eq 0 ]; then
    echo "Chess has been launched successfully!"
else
    echo "ERROR: Failed to launch Chess."
    echo "Please make sure .NET 8 Runtime is installed."
fi

cd ..
