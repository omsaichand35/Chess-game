# Installation Guide for Chess Application

## ?? Installation Packages Available

### ?? macOS (DMG)
- **File**: `Chess-1.0.0.dmg`
- **Requirements**: macOS 10.13 or later, .NET 8 Runtime
- **Installation**:
  1. Download `Chess-1.0.0.dmg`
  2. Double-click to mount the image
  3. Drag Chess.app to Applications folder
  4. Launch from Applications

### ?? Windows (MSI)
- **File**: `Chess-1.0.0.msi`
- **Requirements**: Windows 10/11, .NET 8 Runtime
- **Installation**:
  1. Download `Chess-1.0.0.msi`
  2. Double-click the installer
  3. Follow the installation wizard
  4. Shortcuts will be created on Desktop and Start Menu

### ?? Linux (DEB Package)
- **File**: `chess-1.0.0.deb`
- **Requirements**: Debian/Ubuntu-based systems, .NET 8 Runtime
- **Installation**:
  ```bash
  sudo apt install ./chess-1.0.0.deb
  ```
  Or using dpkg:
  ```bash
  sudo dpkg -i chess-1.0.0.deb
  ```

### ?? Linux (AppImage - Universal)
- **File**: `Chess-1.0.0-x86_64.AppImage`
- **Requirements**: Any 64-bit Linux distribution, .NET 8 Runtime
- **Installation**:
  ```bash
  chmod +x Chess-1.0.0-x86_64.AppImage
  ./Chess-1.0.0-x86_64.AppImage
  ```
  Or for system-wide install:
  ```bash
  sudo mv Chess-1.0.0-x86_64.AppImage /usr/local/bin/chess
  sudo chmod +x /usr/local/bin/chess
  chess
  ```

---

## ?? Building Installers from Source

### Prerequisites for All Platforms
```bash
git clone https://github.com/omsaichand35/Chess-game.git
cd Chess
dotnet --version  # Must be 8.0 or higher
```

### Build macOS DMG

**Requirements**:
- macOS 10.13 or later
- Xcode Command Line Tools

**Steps**:
```bash
# Build and publish
dotnet publish Chess/Chess.csproj -c Release -o publish --self-contained false

# Create DMG
chmod +x scripts/build-macos-dmg.sh
./scripts/build-macos-dmg.sh

# Result: Chess-1.0.0.dmg
```

### Build Windows MSI

**Requirements**:
- Windows 10/11
- WiX Toolset 3.14 ([Download](https://github.com/wixtoolset/wix3/releases))
- .NET 8 SDK

**Steps**:
```powershell
# Build and publish
dotnet publish Chess/Chess.csproj -c Release -o publish --self-contained false

# Create MSI
.\installer\build-windows-msi.bat

# Result: Chess-1.0.0.msi
```

### Build Linux DEB Package

**Requirements**:
- Debian/Ubuntu-based system
- .NET 8 SDK
- Build tools: `sudo apt-get install build-essential`

**Steps**:
```bash
# Build and publish
dotnet publish Chess/Chess.csproj -c Release -o publish --self-contained false

# Create DEB
chmod +x scripts/build-linux-deb.sh
./scripts/build-linux-deb.sh

# Result: chess-1.0.0.deb
```

### Build Linux AppImage (Universal)

**Requirements**:
- Any Linux distribution with .NET 8 SDK
- AppImageKit: Will be downloaded automatically

**Steps**:
```bash
# Build and publish
dotnet publish Chess/Chess.csproj -c Release -o publish --self-contained false

# Create AppImage
chmod +x scripts/build-linux-appimage.sh
./scripts/build-linux-appimage.sh

# Result: build/appimage/Chess-1.0.0-x86_64.AppImage
```

---

## ?? System Requirements

### Minimum
- **CPU**: Dual-core 2.0 GHz
- **RAM**: 2 GB
- **Storage**: 500 MB free space
- **.NET Runtime**: 8.0 or later

### Recommended
- **CPU**: Quad-core 2.4 GHz
- **RAM**: 4 GB
- **Storage**: 1 GB free space
- **.NET Runtime**: 8.0 latest
- **GPU**: Dedicated GPU (optional, for better performance)

### Platform-Specific

**macOS**:
- macOS 10.13 (High Sierra) or later
- For Apple Silicon (M1/M2): Requires .NET 8 ARM64 build

**Windows**:
- Windows 10 (Build 1909) or later
- Windows 11 recommended

**Linux**:
- Ubuntu 18.04 LTS or later
- Debian 9 or later
- Any systemd-based distribution
- Wayland or X11 display server

---

## ?? Troubleshooting Installation

### "Application not found" on macOS
```bash
# Check if app is in Applications
ls -la ~/Applications/Chess.app

# Grant execute permissions
chmod +x ~/Applications/Chess.app/Contents/MacOS/Chess

# Open with Finder -> Applications -> Right-click -> Open
```

### "Missing .NET Runtime" Error
**All Platforms**:
1. Download .NET 8 Runtime: https://dotnet.microsoft.com/download/dotnet/8.0
2. Install runtime for your platform
3. Verify installation:
   ```bash
   dotnet --version
   ```

### "Permission Denied" on Linux
```bash
# Make executable
chmod +x chess-1.0.0.deb
chmod +x Chess-1.0.0-x86_64.AppImage

# Run with sudo if needed
sudo ./Chess-1.0.0-x86_64.AppImage
```

### "Stockfish not found" Error
See [DOCKER_SETUP.md](DOCKER_SETUP.md) for Stockfish installation instructions

---

## ?? Distribution & Sharing

### GitHub Releases
1. Create release on GitHub
2. Upload DMG, MSI, and DEB/AppImage files
3. Add checksums:
   ```bash
   md5sum Chess-1.0.0.dmg > Chess-1.0.0.dmg.md5
   sha256sum Chess-1.0.0.dmg >> Chess-1.0.0.dmg.sha256
   ```

### Automatic Updates (Future Enhancement)
Consider using:
- **macOS**: Sparkle framework
- **Windows**: WinSparkle or Update system
- **Linux**: APT repository or AppImage update feature

---

## ?? Security Notes

- All installers are self-contained
- No admin access required (except Linux system-wide install)
- Verify checksums before installation
- Consider code signing for production releases

---

## ?? Installer Details

### DMG (macOS)
- Universal binary support (Intel & Apple Silicon)
- Code signing ready
- Size: ~200 MB compressed
- Installation time: 2-5 seconds

### MSI (Windows)
- Per-machine installation
- Desktop & Start Menu shortcuts
- Program Files integration
- Uninstaller included
- Size: ~180 MB compressed
- Installation time: 10-30 seconds

### DEB (Linux)
- Standard Debian package
- System integration (Desktop menu entry)
- Auto-update capable
- Size: ~150 MB compressed
- Installation time: 5-15 seconds

### AppImage (Linux)
- No installation required
- Portable across distributions
- Self-contained runtime
- Size: ~250 MB
- Just run and play!

---

## ? Verification Checklist

After installation, verify:
- [ ] Application launches successfully
- [ ] Main window displays correctly
- [ ] Can start new games
- [ ] Chess engine loads (if available)
- [ ] Online features work (if network available)
- [ ] Uninstall works cleanly

---

For issues or support, visit: https://github.com/omsaichand35/Chess-game/issues
