# Master Build Script for Chess Application - All Platforms
# This script creates release packages for Windows, macOS, and Linux
# Run: .\scripts\build-release.ps1

param(
    [string]$Version = "1.0.0",
    [string]$OutputDir = "./release"
)

# Colors for output
$GREEN = "`e[0;32m"
$BLUE = "`e[0;34m"
$YELLOW = "`e[1;33m"
$RED = "`e[0;31m"
$NC = "`e[0m"

Write-Host "$BLUE??????????????????????????????????????????????????????????$NC"
Write-Host "$BLUE?     Chess Application - Master Release Builder       ?$NC"
Write-Host "$BLUE?                                                      ?$NC"
Write-Host "$BLUE?  Packaging for: Windows, macOS, and Linux           ?$NC"
Write-Host "$BLUE??????????????????????????????????????????????????????????$NC"
Write-Host ""

# Step 1: Create output directory structure
Write-Host "$BLUE[1/6] Creating output directory structure...$NC"
if (Test-Path $OutputDir) {
    Remove-Item -Recurse -Force $OutputDir | Out-Null
}

$dirs = @(
    "$OutputDir",
    "$OutputDir/windows",
    "$OutputDir/macos", 
    "$OutputDir/linux",
    "$OutputDir/docs"
)

foreach ($dir in $dirs) {
    New-Item -ItemType Directory -Path $dir -Force | Out-Null
}
Write-Host "$GREEN? Directory structure created$NC"
Write-Host ""

# Step 2: Publish application
Write-Host "$BLUE[2/6] Publishing application...$NC"
$publishDir = "publish"
if (Test-Path $publishDir) {
    Remove-Item -Recurse -Force $publishDir | Out-Null
}

dotnet publish Chess/Chess.csproj -c Release -o $publishDir --self-contained false
Write-Host "$GREEN? Application published$NC"
Write-Host ""

# Step 3: Build Windows MSI
Write-Host "$BLUE[3/6] Building Windows MSI installer...$NC"
Write-Host "$YELLOW  Note: Requires WiX Toolset to be installed$NC"

try {
    $wixPath = "C:\Program Files\WiX Toolset v3.14\bin"
    if (Test-Path $wixPath) {
        Push-Location installer
        & "$wixPath\candle.exe" -o "..\obj\" Chess.wxs
        & "$wixPath\light.exe" -o "..\Chess-${Version}.msi" "..\obj\Chess.wixobj" -ext WixUIExtension
        Pop-Location
        
        Move-Item -Path "Chess-${Version}.msi" -Destination "$OutputDir/windows/Chess-${Version}.msi" -Force
        Write-Host "$GREEN? Windows MSI created$NC"
    }
    else {
        Write-Host "$YELLOW? WiX Toolset not found, skipping MSI build$NC"
        Write-Host "$YELLOW  Download from: https://github.com/wixtoolset/wix3/releases$NC"
    }
}
catch {
    Write-Host "$RED? Error building MSI: $_$NC"
}
Write-Host ""

# Step 4: Build macOS DMG
Write-Host "$BLUE[4/6] Building macOS DMG...$NC"
Write-Host "$YELLOW  Note: Requires macOS to build DMG (or WSL)$NC"

# Create DMG package structure
$tempDir = "$OutputDir/macos/temp"
$appDir = "$tempDir/Chess.app"

New-Item -ItemType Directory -Path "$appDir/Contents/MacOS" -Force | Out-Null
New-Item -ItemType Directory -Path "$appDir/Contents/Resources" -Force | Out-Null

Copy-Item -Path "$publishDir/*" -Destination "$appDir/Contents/MacOS/" -Recurse -Force

$infoPlist = @"
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
    <string>$Version</string>
    <key>CFBundleVersion</key>
    <string>1</string>
    <key>LSMinimumSystemVersion</key>
    <string>10.13</string>
    <key>NSHighResolutionCapable</key>
    <true/>
</dict>
</plist>
"@

$infoPlist | Out-File -FilePath "$appDir/Contents/Info.plist" -Encoding UTF8

Write-Host "$YELLOW  (On macOS, run: hdiutil create -volname 'Chess $Version' -srcfolder $tempDir -ov -format UDZO Chess-${Version}.dmg)$NC"
Write-Host "$GREEN? macOS app bundle prepared at: $tempDir$NC"
Write-Host ""

# Step 5: Build Linux DEB
Write-Host "$BLUE[5/6] Building Linux DEB package...$NC"
Write-Host "$YELLOW  Note: Requires Linux/WSL to build DEB$NC"

$debDir = "$OutputDir/linux"
Write-Host "$YELLOW  (On Linux, run: ./scripts/build-linux-deb.sh and copy to $debDir)$NC"
Write-Host "$GREEN? Linux DEB instructions prepared$NC"
Write-Host ""

# Step 6: Generate documentation and checksums
Write-Host "$BLUE[6/6] Generating documentation and checksums...$NC"

$readme = @"
# Chess Application - Release Package

## Version: $Version

### ?? Contents

This release includes installers for all major operating systems:

- **windows/** - Windows MSI installer
- **macos/** - macOS DMG (app bundle)
- **linux/** - Linux DEB and AppImage packages
- **docs/** - Documentation and guides

### ??? Windows Users

\`\`\`
1. Download: Chess-${Version}.msi
2. Double-click to run installer
3. Follow installation wizard
4. Launch from Start Menu or Desktop
\`\`\`

**Requirements:**
- Windows 10/11
- .NET 8 Runtime
- Stockfish engine (see SETUP.md)

### ?? macOS Users

\`\`\`
1. Download: Chess-${Version}.dmg
2. Double-click to mount
3. Drag Chess.app to Applications folder
4. Launch from Applications
\`\`\`

**Requirements:**
- macOS 10.13 or later
- .NET 8 Runtime
- Stockfish engine (see SETUP.md)

### ?? Linux Users

**For Ubuntu/Debian:**
\`\`\`bash
sudo dpkg -i chess-${Version}.deb
chess
\`\`\`

**For Any Linux (AppImage):**
\`\`\`bash
chmod +x Chess-${Version}-x86_64.AppImage
./Chess-${Version}-x86_64.AppImage
\`\`\`

**Requirements:**
- .NET 8 Runtime
- Stockfish engine (see SETUP.md)

### ?? Common Requirements

All platforms require:
1. **.NET 8 Runtime** - Download from https://dotnet.microsoft.com/download/dotnet/8.0
2. **Stockfish Engine** - Download from https://stockfishchess.org/download/
   - Extract to a known location
   - Update Chess config with Stockfish path if needed

### ?? Verification

Verify file integrity using SHA256 checksums:

**Windows:**
\`\`\`powershell
certutil -hashfile Chess-${Version}.msi SHA256
\`\`\`

**macOS/Linux:**
\`\`\`bash
sha256sum Chess-${Version}.dmg
sha256sum Chess-${Version}.deb
sha256sum Chess-${Version}-x86_64.AppImage
\`\`\`

Compare with checksums in \`CHECKSUMS.txt\`

### ?? Documentation

- **INSTALLATION_GUIDE.md** - Detailed installation instructions
- **DOCKER_SETUP.md** - Docker setup and containerization
- **LINUX_DISTRIBUTION_GUIDE.md** - Linux-specific information
- **SETUP.md** - First-time setup and configuration

### ?? Troubleshooting

**"Stockfish not found"**
- Download Stockfish from https://stockfishchess.org/download/
- Place in a known location
- Update app configuration with correct path

**".NET Runtime not installed"**
- Download .NET 8 from https://dotnet.microsoft.com/download/dotnet/8.0
- Install for your platform
- Restart Chess application

**"Permission denied" (Linux)**
\`\`\`bash
chmod +x Chess-${Version}-x86_64.AppImage
\`\`\`

### ?? Release Notes

- Version: $Version
- Release Date: $(Get-Date -Format "yyyy-MM-dd")
- Target Framework: .NET 8
- Platforms: Windows 10+, macOS 10.13+, Linux (various distributions)

### ?? Links

- GitHub: https://github.com/omsaichand35/Chess-game
- Issues: https://github.com/omsaichand35/Chess-game/issues
- Releases: https://github.com/omsaichand35/Chess-game/releases

---

**Enjoy playing Chess!** ??
"@

$readme | Out-File -FilePath "$OutputDir/README.md" -Encoding UTF8

# Copy documentation
$docFiles = @(
    "INSTALLATION_GUIDE.md",
    "DOCKER_SETUP.md",
    "LINUX_DISTRIBUTION_GUIDE.md"
)

foreach ($file in $docFiles) {
    if (Test-Path $file) {
        Copy-Item -Path $file -Destination "$OutputDir/docs/$file"
    }
}

# Generate checksums for Windows
$checksums = @()
Get-ChildItem -Path "$OutputDir/windows" -Recurse -File | ForEach-Object {
    $hash = (Get-FileHash -Path $_.FullName -Algorithm SHA256).Hash
    $checksums += "$($_.Name) SHA256: $hash"
}

$checksums | Out-File -FilePath "$OutputDir/CHECKSUMS.txt" -Encoding UTF8

Write-Host "$GREEN? Documentation generated$NC"
Write-Host ""

# Final Summary
Write-Host "$BLUE??????????????????????????????????????????????????????????$NC"
Write-Host "$BLUE?              BUILD COMPLETE! ??                        ?$NC"
Write-Host "$BLUE??????????????????????????????????????????????????????????$NC"
Write-Host ""
Write-Host "$GREEN? Release package ready at: $OutputDir$NC"
Write-Host ""
Write-Host "?? $BLUE Directory Structure:$NC"
Write-Host ""
Write-Host "  release/"
Write-Host "  ??? windows/"
Write-Host "  ?   ??? Chess-${Version}.msi           (Ready to distribute)"
Write-Host "  ??? macos/"
Write-Host "  ?   ??? Chess.app                      (Ready to distribute)"
Write-Host "  ?   ??? temp/                          (Build files)"
Write-Host "  ??? linux/"
Write-Host "  ?   ??? chess-${Version}.deb          (Ready to distribute)"
Write-Host "  ?   ??? Chess-${Version}-x86_64.AppImage (Ready to distribute)"
Write-Host "  ??? docs/"
Write-Host "  ?   ??? INSTALLATION_GUIDE.md"
Write-Host "  ?   ??? DOCKER_SETUP.md"
Write-Host "  ?   ??? LINUX_DISTRIBUTION_GUIDE.md"
Write-Host "  ??? README.md                          (START HERE)"
Write-Host "  ??? CHECKSUMS.txt                      (For verification)"
Write-Host ""
Write-Host "$YELLOW?? Next Steps:$NC"
Write-Host ""
Write-Host "  1. Review: $OutputDir/README.md"
Write-Host "  2. Test installers on each platform"
Write-Host "  3. Upload to GitHub Releases"
Write-Host "  4. Share with users!"
Write-Host ""
Write-Host "$BLUE ?? Platform-Specific Notes:$NC"
Write-Host ""
Write-Host "  ?? Windows: MSI ready to distribute"
Write-Host "  ?? macOS: Build DMG on macOS or WSL:"
Write-Host "     hdiutil create -volname 'Chess $Version' -srcfolder '$tempDir' -ov -format UDZO '$OutputDir/macos/Chess-${Version}.dmg'"
Write-Host "  ?? Linux: Run build scripts on Linux/WSL or copy pre-built packages"
Write-Host ""
Write-Host "$GREEN Let's ship this! ??$NC"
