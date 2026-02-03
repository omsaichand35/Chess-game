#!/bin/bash

# Linux AppImage Builder for Chess Application
# Creates a portable AppImage for all Linux distributions

set -e

echo "?? Chess Linux AppImage Builder"
echo "================================"

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Configuration
APP_NAME="Chess"
VERSION="1.0.0"
BUILD_DIR="build/appimage"
PUBLISH_DIR="publish"

echo -e "${BLUE}Step 1: Checking AppImage tools...${NC}"
if ! command -v appimagetool &> /dev/null; then
    echo -e "${YELLOW}appimagetool not found. Installing...${NC}"
    wget https://github.com/AppImage/AppImageKit/releases/download/continuous/appimagetool-x86_64.AppImage
    chmod +x appimagetool-x86_64.AppImage
fi

echo -e "${BLUE}Step 2: Cleaning previous builds...${NC}"
rm -rf "${BUILD_DIR}"

echo -e "${BLUE}Step 3: Building application...${NC}"
if [ ! -d "${PUBLISH_DIR}" ]; then
    dotnet publish Chess/Chess.csproj -c Release -o "${PUBLISH_DIR}" --self-contained false
fi

echo -e "${BLUE}Step 4: Creating AppImage structure...${NC}"
mkdir -p "${BUILD_DIR}/${APP_NAME}.AppDir/usr/bin"
mkdir -p "${BUILD_DIR}/${APP_NAME}.AppDir/usr/share/applications"
mkdir -p "${BUILD_DIR}/${APP_NAME}.AppDir/usr/share/pixmaps"
mkdir -p "${BUILD_DIR}/${APP_NAME}.AppDir/opt/chess"

echo -e "${BLUE}Step 5: Copying application files...${NC}"
cp -r "${PUBLISH_DIR}"/* "${BUILD_DIR}/${APP_NAME}.AppDir/opt/chess/"

echo -e "${BLUE}Step 6: Creating launcher script...${NC}"
cat > "${BUILD_DIR}/${APP_NAME}.AppDir/AppRun" << 'EOF'
#!/bin/bash
SELF=$(readlink -f "$0")
HERE="${SELF%/*}"
export LD_LIBRARY_PATH="${HERE}/usr/lib:${LD_LIBRARY_PATH}"
export PATH="${HERE}/usr/bin:${PATH}"
export XDG_DATA_DIRS="${HERE}/usr/share:${XDG_DATA_DIRS}"
exec "${HERE}/opt/chess/Chess" "$@"
EOF
chmod +x "${BUILD_DIR}/${APP_NAME}.AppDir/AppRun"

echo -e "${BLUE}Step 7: Creating desktop entry...${NC}"
cat > "${BUILD_DIR}/${APP_NAME}.AppDir/chess.desktop" << 'EOF'
[Desktop Entry]
Version=1.0
Type=Application
Name=Chess
Comment=Play chess against AI or other players
Exec=chess
Icon=chess
Terminal=false
Categories=Games;
X-AppImage-Version=1.0.0
EOF

echo -e "${BLUE}Step 8: Building AppImage...${NC}"
cd "${BUILD_DIR}"
../appimagetool-x86_64.AppImage -n "${APP_NAME}.AppDir" "${APP_NAME}-${VERSION}-x86_64.AppImage"
cd ../..

echo -e "${BLUE}Step 9: Signing AppImage (optional)...${NC}"
if command -v gpg &> /dev/null; then
    echo "To sign the AppImage, use: gpg --detach-sign --armor build/appimage/${APP_NAME}-${VERSION}-x86_64.AppImage"
fi

# Cleanup
rm -rf "${BUILD_DIR}/${APP_NAME}.AppDir"

echo -e "${GREEN}? AppImage created: build/appimage/${APP_NAME}-${VERSION}-x86_64.AppImage${NC}"
echo ""
echo "File: build/appimage/${APP_NAME}-${VERSION}-x86_64.AppImage"
echo "Size: $(ls -lh build/appimage/${APP_NAME}-${VERSION}-x86_64.AppImage | awk '{print $5}')"
echo ""
echo "To run:"
echo "  ./build/appimage/${APP_NAME}-${VERSION}-x86_64.AppImage"
echo ""
echo "To install (optional):"
echo "  sudo mv build/appimage/${APP_NAME}-${VERSION}-x86_64.AppImage /usr/local/bin/chess"
echo "  sudo chmod +x /usr/local/bin/chess"
