#!/bin/bash

# Universal Build Script for Chess Application
# Builds installers for all platforms (macOS, Windows, Linux)

set -e

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

VERSION="1.0.0"
BUILD_DIR="build"
PUBLISH_DIR="publish"

echo -e "${BLUE}"
cat << "EOF"
????????????????????????????????????????????????????????????
?         Chess Application - Universal Builder            ?
?                                                          ?
?  Builds installers for:                                 ?
?    • macOS (DMG)                                        ?
?    • Windows (MSI)                                      ?
?    • Linux (DEB + AppImage)                             ?
????????????????????????????????????????????????????????????
EOF
echo -e "${NC}"

# Step 1: Publish application
echo -e "${BLUE}[1/5] Publishing application...${NC}"
mkdir -p "${PUBLISH_DIR}"
dotnet publish Chess/Chess.csproj -c Release -o "${PUBLISH_DIR}" --self-contained false

# Step 2: Detect platform
echo -e "${BLUE}[2/5] Detecting platform...${NC}"
if [[ "$OSTYPE" == "linux-gnu"* ]]; then
    PLATFORM="linux"
    echo -e "${GREEN}? Linux detected${NC}"
elif [[ "$OSTYPE" == "darwin"* ]]; then
    PLATFORM="macos"
    echo -e "${GREEN}? macOS detected${NC}"
elif [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "cygwin" ]]; then
    PLATFORM="windows"
    echo -e "${GREEN}? Windows detected${NC}"
else
    echo -e "${RED}? Unsupported platform: $OSTYPE${NC}"
    exit 1
fi

# Step 3: Create installers based on platform
echo -e "${BLUE}[3/5] Creating installers...${NC}"

mkdir -p "${BUILD_DIR}"

case "$PLATFORM" in
    macos)
        echo -e "${YELLOW}Building macOS DMG...${NC}"
        chmod +x scripts/build-macos-dmg.sh
        ./scripts/build-macos-dmg.sh
        echo -e "${GREEN}? macOS DMG created${NC}"
        ;;
    windows)
        echo -e "${YELLOW}Building Windows MSI...${NC}"
        # This requires WiX Toolset
        if ! command -v candle &> /dev/null; then
            echo -e "${RED}??  WiX Toolset not installed${NC}"
            echo "Please install from: https://github.com/wixtoolset/wix3/releases"
        else
            chmod +x installer/build-windows-msi.bat
            ./installer/build-windows-msi.bat
            echo -e "${GREEN}? Windows MSI created${NC}"
        fi
        ;;
    linux)
        echo -e "${YELLOW}Building Linux packages...${NC}"
        
        # Build DEB
        echo -e "${YELLOW}  - Building DEB package...${NC}"
        chmod +x scripts/build-linux-deb.sh
        ./scripts/build-linux-deb.sh
        echo -e "${GREEN}  ? DEB created${NC}"
        
        # Build AppImage
        echo -e "${YELLOW}  - Building AppImage...${NC}"
        chmod +x scripts/build-linux-appimage.sh
        ./scripts/build-linux-appimage.sh
        echo -e "${GREEN}  ? AppImage created${NC}"
        ;;
esac

# Step 4: Generate checksums
echo -e "${BLUE}[4/5] Generating checksums...${NC}"
find . -name "Chess*.dmg" -o -name "Chess*.msi" -o -name "chess*.deb" -o -name "Chess*.AppImage" 2>/dev/null | while read file; do
    if [ -f "$file" ]; then
        sha256sum "$file" > "$file.sha256"
        echo -e "${GREEN}? Checksum: $file${NC}"
    fi
done

# Step 5: Summary
echo -e "${BLUE}[5/5] Build complete!${NC}"
echo ""
echo -e "${GREEN}???????????????????????????????????????????????????????${NC}"
echo -e "${GREEN}Build Summary${NC}"
echo -e "${GREEN}???????????????????????????????????????????????????????${NC}"
echo ""

echo "?? Installers created:"
find . -name "Chess*.dmg" -o -name "Chess*.msi" -o -name "chess*.deb" -o -name "Chess*.AppImage" 2>/dev/null | while read file; do
    if [ -f "$file" ]; then
        size=$(du -h "$file" | cut -f1)
        echo -e "  ${GREEN}?${NC} $file ($size)"
    fi
done

echo ""
echo "?? Checksums:"
find . -name "*.sha256" 2>/dev/null | while read file; do
    if [ -f "$file" ]; then
        echo -e "  ${GREEN}?${NC} $file"
    fi
done

echo ""
echo -e "${YELLOW}Next steps:${NC}"
echo "  1. Test installers on respective platforms"
echo "  2. Upload to GitHub Releases"
echo "  3. Update documentation"
echo ""
echo "?? See INSTALLATION_GUIDE.md for detailed instructions"
echo ""
