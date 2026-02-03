#!/bin/bash

# Linux DEB Package Builder for Chess Application
# Creates a distributable .deb package for Debian/Ubuntu systems

set -e

echo "?? Chess Linux DEB Builder"
echo "=========================="

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Configuration
APP_NAME="chess"
VERSION="1.0.0"
MAINTAINER="Chess Team <support@chess-game.local>"
HOMEPAGE="https://github.com/omsaichand35/Chess"
BUILD_DIR="build/deb"
PUBLISH_DIR="publish"

echo -e "${BLUE}Step 1: Cleaning previous builds...${NC}"
rm -rf "${BUILD_DIR}"

echo -e "${BLUE}Step 2: Building application...${NC}"
if [ ! -d "${PUBLISH_DIR}" ]; then
    dotnet publish Chess/Chess.csproj -c Release -o "${PUBLISH_DIR}" --self-contained false
fi

echo -e "${BLUE}Step 3: Creating DEB package structure...${NC}"
mkdir -p "${BUILD_DIR}/${APP_NAME}-${VERSION}/DEBIAN"
mkdir -p "${BUILD_DIR}/${APP_NAME}-${VERSION}/usr/bin"
mkdir -p "${BUILD_DIR}/${APP_NAME}-${VERSION}/usr/share/applications"
mkdir -p "${BUILD_DIR}/${APP_NAME}-${VERSION}/usr/share/pixmaps"
mkdir -p "${BUILD_DIR}/${APP_NAME}-${VERSION}/opt/chess"

echo -e "${BLUE}Step 4: Copying application files...${NC}"
cp -r "${PUBLISH_DIR}"/* "${BUILD_DIR}/${APP_NAME}-${VERSION}/opt/chess/"

echo -e "${BLUE}Step 5: Creating launcher script...${NC}"
cat > "${BUILD_DIR}/${APP_NAME}-${VERSION}/usr/bin/chess" << 'EOF'
#!/bin/bash
cd /opt/chess
./Chess "$@"
EOF
chmod +x "${BUILD_DIR}/${APP_NAME}-${VERSION}/usr/bin/chess"

echo -e "${BLUE}Step 6: Creating desktop entry...${NC}"
cat > "${BUILD_DIR}/${APP_NAME}-${VERSION}/usr/share/applications/chess.desktop" << 'EOF'
[Desktop Entry]
Version=1.0
Type=Application
Name=Chess
Comment=Play chess against AI or other players
Exec=chess
Icon=chess
Terminal=false
Categories=Games;
EOF

echo -e "${BLUE}Step 7: Creating DEBIAN/control file...${NC}"
mkdir -p "${BUILD_DIR}/${APP_NAME}-${VERSION}/DEBIAN"
cat > "${BUILD_DIR}/${APP_NAME}-${VERSION}/DEBIAN/control" << EOF
Package: ${APP_NAME}
Version: ${VERSION}
Architecture: amd64
Maintainer: ${MAINTAINER}
Homepage: ${HOMEPAGE}
Description: Chess Game with AI opponent
 Play chess against the Stockfish engine or other players online.
 Features include multiple difficulty levels, threefold repetition detection,
 and online multiplayer support.
Depends: libfontconfig1, libfreetype6, libx11-6, dotnet-runtime-8.0
EOF

echo -e "${BLUE}Step 8: Creating DEBIAN/postinst script...${NC}"
cat > "${BUILD_DIR}/${APP_NAME}-${VERSION}/DEBIAN/postinst" << 'EOF'
#!/bin/bash
set -e

# Make sure the binary is executable
chmod +x /opt/chess/Chess

# Update desktop database
update-desktop-database /usr/share/applications || true

exit 0
EOF
chmod +x "${BUILD_DIR}/${APP_NAME}-${VERSION}/DEBIAN/postinst"

echo -e "${BLUE}Step 9: Creating DEBIAN/postrm script...${NC}"
cat > "${BUILD_DIR}/${APP_NAME}-${VERSION}/DEBIAN/postrm" << 'EOF'
#!/bin/bash
set -e

# Update desktop database on removal
update-desktop-database /usr/share/applications || true

exit 0
EOF
chmod +x "${BUILD_DIR}/${APP_NAME}-${VERSION}/DEBIAN/postrm"

echo -e "${BLUE}Step 10: Building DEB package...${NC}"
cd "${BUILD_DIR}"
dpkg-deb --build "${APP_NAME}-${VERSION}"

echo -e "${BLUE}Step 11: Copying DEB to root...${NC}"
cp "${APP_NAME}-${VERSION}.deb" "../../${APP_NAME}-${VERSION}.deb"

echo -e "${BLUE}Step 12: Signing package (optional)...${NC}"
if command -v dpkg-sig &> /dev/null; then
    echo "Skipping GPG signing. To sign, use: dpkg-sig -k <key-id> -s builder ${APP_NAME}-${VERSION}.deb"
fi

# Cleanup
cd ../../
rm -rf "${BUILD_DIR}"

echo -e "${GREEN}? DEB package created: ${APP_NAME}-${VERSION}.deb${NC}"
echo ""
echo "File: ${APP_NAME}-${VERSION}.deb"
echo "Size: $(ls -lh ${APP_NAME}-${VERSION}.deb | awk '{print $5}')"
echo ""
echo "To install:"
echo "  sudo dpkg -i ${APP_NAME}-${VERSION}.deb"
echo ""
echo "Or using apt:"
echo "  sudo apt install ./${APP_NAME}-${VERSION}.deb"
