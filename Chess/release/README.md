# Chess Application - Release Package

## Version: 1.0.0

### ?? Contents

This release includes installers for all major operating systems:

- **windows/** - Windows MSI installer
- **macos/** - macOS DMG (app bundle)
- **linux/** - Linux DEB and AppImage packages
- **docs/** - Documentation and guides

### ??? Windows Users

\\\
1. Download: Chess-1.0.0.msi
2. Double-click to run installer
3. Follow installation wizard
4. Launch from Start Menu or Desktop
\\\

**Requirements:**
- Windows 10/11
- .NET 8 Runtime
- Stockfish engine (see SETUP.md)

### ?? macOS Users

\\\
1. Download: Chess-1.0.0.dmg
2. Double-click to mount
3. Drag Chess.app to Applications folder
4. Launch from Applications
\\\

**Requirements:**
- macOS 10.13 or later
- .NET 8 Runtime
- Stockfish engine (see SETUP.md)

### ?? Linux Users

**For Ubuntu/Debian:**
\\\ash
sudo dpkg -i chess-1.0.0.deb
chess
\\\

**For Any Linux (AppImage):**
\\\ash
chmod +x Chess-1.0.0-x86_64.AppImage
./Chess-1.0.0-x86_64.AppImage
\\\

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
\\\powershell
certutil -hashfile Chess-1.0.0.msi SHA256
\\\

**macOS/Linux:**
\\\ash
sha256sum Chess-1.0.0.dmg
sha256sum Chess-1.0.0.deb
sha256sum Chess-1.0.0-x86_64.AppImage
\\\

Compare with checksums in \CHECKSUMS.txt\

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
\\\ash
chmod +x Chess-1.0.0-x86_64.AppImage
\\\

### ?? Release Notes

- Version: 1.0.0
- Release Date: 2026-02-03
- Target Framework: .NET 8
- Platforms: Windows 10+, macOS 10.13+, Linux (various distributions)

### ?? Links

- GitHub: https://github.com/omsaichand35/Chess-game
- Issues: https://github.com/omsaichand35/Chess-game/issues
- Releases: https://github.com/omsaichand35/Chess-game/releases

---

**Enjoy playing Chess!** ??
