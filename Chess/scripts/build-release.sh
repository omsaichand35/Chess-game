#!/bin/bash

# Master Build Script for Chess Application - All Platforms
# This script creates release packages for Windows, macOS, and Linux
# Run: chmod +x scripts/build-release.sh && ./scripts/build-release.sh

set -e

VERSION="${1:-1.0.0}"
OUTPUT_DIR="${2:-./release}"

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

echo -e "${BLUE}??????????????????????????????????????????????????????????${NC}"
echo -e "${BLUE}?     Chess Application - Master Release Builder       ?${NC}"
echo -e "${BLUE}?                                                      ?${NC}"
echo -e "${BLUE}?  Packaging for: Windows, macOS, and Linux           ?${NC}"
echo -e "${BLUE}??????????????????????????????????????????????????????????${NC}"
echo ""

# Step 1: Create output directory structure
echo -e "${BLUE}[1/6] Creating output directory structure...${NC}"
rm -rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR"/{windows,macos,linux,docs}
echo -e "${GREEN}? Directory structure created${NC}"
echo ""

# Step 2: Publish application
echo -e "${BLUE}[2/6] Publishing application...${NC}"
PUBLISH_DIR="publish"
rm -rf "$PUBLISH_DIR"
dotnet publish Chess/Chess.csproj -c Release -o "$PUBLISH_DIR" --self-contained false
echo -e "${GREEN}? Application published${NC}"
echo ""

# Step 3: Build macOS DMG
echo -e "${BLUE}[3/6] Building macOS DMG...${NC}"
TEMP_DIR="$OUTPUT_DIR/macos/temp"
APP_DIR="$TEMP_DIR/Chess.app"

mkdir -p "$APP_DIR/Contents/MacOS"
mkdir -p "$APP_DIR/Contents/Resources"

cp -r "$PUBLISH_DIR"/* "$APP_DIR/Contents/MacOS/"

cat > "$APP_DIR/Contents/Info.plist" << 'EOF'
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleDevelopmentRegion</key>
    <string>en</string>
    <key>CFBundleExecutable</key>
    <string>Chess</string>
    <key>CFBundleIdentifier</key>
    <string>com.chess.app</string>
    <key>CFBundleInfoDictionaryVersion</key>
    <string>6.0</string>
    <key>CFBundleName</key>
    <string>Chess</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleShortVersionString</key>
    <string>1.0.0</string>
    <key>CFBundleVersion</key>
    <string>1</string>
    <key>LSMinimumSystemVersion</key>
    <string>10.13</string>
    <key>NSHighResolutionCapable</key>
    <true/>
</dict>
</plist>
EOF

chmod +x "$APP_DIR/Contents/MacOS/Chess"

if command -v hdiutil &> /dev/null; then
    hdiutil create \
        -volname "Chess $VERSION" \
        -srcfolder "$TEMP_DIR" \
        -ov \
        -format UDZO \
        "$OUTPUT_DIR/macos/Chess-${VERSION}.dmg"
    echo -e "${GREEN}? macOS DMG created${NC}"
else
    echo -e "${YELLOW}? hdiutil not found (requires macOS), app bundle prepared${NC}"
fi
echo ""

# Step 4: Build Linux DEB
echo -e "${BLUE}[4/6] Building Linux DEB package...${NC}"
if command -v dpkg-deb &> /dev/null; then
    chmod +x scripts/build-linux-deb.sh
    ./scripts/build-linux-deb.sh
    if [ -f "chess-${VERSION}.deb" ]; then
        mv "chess-${VERSION}.deb" "$OUTPUT_DIR/linux/"
        echo -e "${GREEN}? Linux DEB created${NC}"
    fi
else
    echo -e "${YELLOW}? dpkg not found, skipping DEB build${NC}"
fi
echo ""

# Step 5: Build Linux AppImage
echo -e "${BLUE}[5/6] Building Linux AppImage...${NC}"
if command -v appimagetool &> /dev/null || [ -f "appimagetool-x86_64.AppImage" ]; then
    chmod +x scripts/build-linux-appimage.sh
    ./scripts/build-linux-appimage.sh
    if [ -f "build/appimage/Chess-${VERSION}-x86_64.AppImage" ]; then
        mv "build/appimage/Chess-${VERSION}-x86_64.AppImage" "$OUTPUT_DIR/linux/"
        echo -e "${GREEN}? Linux AppImage created${NC}"
    fi
else
    echo -e "${YELLOW}? appimagetool not found, skipping AppImage build${NC}"
fi
echo ""

# Step 6: Generate documentation
echo -e "${BLUE}[6/6] Generating documentation and checksums...${NC}"

# Copy documentation
for file in INSTALLATION_GUIDE.md DOCKER_SETUP.md LINUX_DISTRIBUTION_GUIDE.md; do
    if [ -f "$file" ]; then
        cp "$file" "$OUTPUT_DIR/docs/"
    fi
done

# Generate checksums
cd "$OUTPUT_DIR"
find . -type f ! -name "CHECKSUMS.txt" ! -name "README.md" | while read file; do
    sha256sum "$file" >> CHECKSUMS.txt
done
cd - > /dev/null

echo -e "${GREEN}? Documentation generated${NC}"
echo ""

# Final Summary
echo -e "${BLUE}??????????????????????????????????????????????????????????${NC}"
echo -e "${BLUE}?              BUILD COMPLETE! ??                        ?${NC}"
echo -e "${BLUE}??????????????????????????????????????????????????????????${NC}"
echo ""
echo -e "${GREEN}? Release package ready at: $OUTPUT_DIR${NC}"
echo ""
echo -e "?? ${BLUE}Directory Structure:${NC}"
echo ""
echo "  release/"
echo "  ??? windows/"
echo "  ?   ??? Chess-${VERSION}.msi                    (Ready to distribute)"
echo "  ??? macos/"
echo "  ?   ??? Chess-${VERSION}.dmg                    (Ready to distribute)"
echo "  ?   ??? temp/Chess.app                          (App bundle)"
echo "  ??? linux/"
echo "  ?   ??? chess-${VERSION}.deb                    (Ready to distribute)"
echo "  ?   ??? Chess-${VERSION}-x86_64.AppImage        (Ready to distribute)"
echo "  ??? docs/"
echo "  ?   ??? INSTALLATION_GUIDE.md"
echo "  ?   ??? DOCKER_SETUP.md"
echo "  ?   ??? LINUX_DISTRIBUTION_GUIDE.md"
echo "  ??? README.md                                   (START HERE)"
echo "  ??? CHECKSUMS.txt                              (For verification)"
echo ""
echo -e "${YELLOW}?? Next Steps:${NC}"
echo ""
echo "  1. Review: $OUTPUT_DIR/README.md"
echo "  2. Test installers on each platform"
echo "  3. Upload to GitHub Releases"
echo "  4. Share the release folder link with users!"
echo ""
echo -e "${GREEN}Let's ship this! ??${NC}"
