#!/bin/bash

# macOS DMG Creator for Chess Application
# This script creates a distributable DMG file for macOS

set -e

echo "?? Chess macOS DMG Builder"
echo "=========================="

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Configuration
APP_NAME="Chess"
VERSION="1.0.0"
DMG_NAME="Chess-${VERSION}.dmg"
TEMP_DIR="/tmp/chess-build"
APP_DIR="${TEMP_DIR}/${APP_NAME}.app"
BUILD_OUTPUT="./publish"

echo -e "${BLUE}Step 1: Creating app bundle structure...${NC}"
rm -rf "${TEMP_DIR}"
mkdir -p "${TEMP_DIR}"

# Create macOS app bundle
mkdir -p "${APP_DIR}/Contents/MacOS"
mkdir -p "${APP_DIR}/Contents/Resources"

echo -e "${BLUE}Step 2: Copying application files...${NC}"
cp -r "${BUILD_OUTPUT}"/* "${APP_DIR}/Contents/MacOS/"

echo -e "${BLUE}Step 3: Creating Info.plist...${NC}"
cat > "${APP_DIR}/Contents/Info.plist" << 'EOF'
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
    <key>NSPrincipalClass</key>
    <string>NSApplication</string>
</dict>
</plist>
EOF

echo -e "${BLUE}Step 4: Creating launcher script...${NC}"
cat > "${APP_DIR}/Contents/MacOS/launcher.sh" << 'EOF'
#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$DIR"
./Chess
EOF

chmod +x "${APP_DIR}/Contents/MacOS/launcher.sh"
chmod +x "${APP_DIR}/Contents/MacOS/Chess"

echo -e "${BLUE}Step 5: Creating DMG...${NC}"

# Create DMG
hdiutil create \
    -volname "Chess ${VERSION}" \
    -srcfolder "${TEMP_DIR}" \
    -ov \
    -format UDZO \
    "${DMG_NAME}"

echo -e "${GREEN}? DMG created: ${DMG_NAME}${NC}"

# Cleanup
rm -rf "${TEMP_DIR}"

echo -e "${GREEN}? macOS installation package ready!${NC}"
echo ""
echo "File: ${DMG_NAME}"
echo "Size: $(du -h ${DMG_NAME} | cut -f1)"
