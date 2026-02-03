# ?? Complete Build & Distribution Guide

## ?? What You'll Get

After running the build scripts, you'll have a **`release/`** folder containing:

```
release/
??? windows/
?   ??? Chess-1.0.0.msi          ? Windows users download this
??? macos/
?   ??? Chess-1.0.0.dmg          ? macOS users download this
??? linux/
?   ??? chess-1.0.0.deb          ? Ubuntu/Debian users
?   ??? Chess-1.0.0-x86_64.AppImage  ? Any Linux user
??? docs/
?   ??? INSTALLATION_GUIDE.md
?   ??? DOCKER_SETUP.md
?   ??? LINUX_DISTRIBUTION_GUIDE.md
??? README.md                     ? Users read this first!
??? CHECKSUMS.txt                 ? For security verification
```

---

## ?? How to Build

### **Windows (PowerShell)**

```powershell
# Run the master build script
.\scripts\build-release.ps1 -Version "1.0.0" -OutputDir "./release"

# This creates:
# - Windows MSI installer
# - macOS app bundle (for DMG conversion on macOS)
# - Documentation
# - Checksums
```

**Requirements:**
- .NET 8 SDK
- WiX Toolset (for MSI, optional)
- PowerShell

### **Linux/macOS (Bash)**

```bash
# Run the master build script
chmod +x scripts/build-release.sh
./scripts/build-release.sh 1.0.0 ./release

# This creates:
# - macOS DMG (if on macOS)
# - Linux DEB package
# - Linux AppImage
# - Documentation
# - Checksums
```

**Requirements:**
- .NET 8 SDK
- hdiutil (macOS, for DMG)
- dpkg-deb (Linux, for DEB)
- appimagetool (Linux, for AppImage)

---

## ?? Distribution Steps

### Step 1: Build Everything
```bash
./scripts/build-release.sh
# or
.\scripts\build-release.ps1
```

### Step 2: Test All Installers
- ? Test MSI on Windows
- ? Test DMG on macOS
- ? Test DEB on Ubuntu/Debian
- ? Test AppImage on various Linux distros

### Step 3: Verify Checksums
```bash
sha256sum -c CHECKSUMS.txt
```

### Step 4: Create GitHub Release

```bash
cd release/

# Tag the release
git tag v1.0.0
git push origin v1.0.0

# Create release on GitHub and upload:
# - windows/Chess-1.0.0.msi
# - macos/Chess-1.0.0.dmg
# - linux/chess-1.0.0.deb
# - linux/Chess-1.0.0-x86_64.AppImage
# - CHECKSUMS.txt
# - README.md
```

### Step 5: Share the Link
Users access from: `https://github.com/omsaichand35/Chess-game/releases`

---

## ? For End Users

### Everything They Need to Know

**File sent to users:**
- One `.msi` file (Windows)
- One `.dmg` file (macOS)
- One `.deb` file (Linux - Ubuntu/Debian)
- One `.AppImage` file (Linux - Any distro)

**What they do:**
1. Download the file for their OS
2. Double-click it
3. Follow the installer
4. Launch the app
5. Install Stockfish engine (one-time setup)
6. Play!

**That's it!** No complex setup needed.

---

## ?? File Details

### Windows MSI
- **Size**: ~50-70 MB
- **Installation Time**: 10-30 seconds
- **Features**: Desktop shortcut, Start Menu entry, Add/Remove Programs
- **Uninstall**: Easy (Add/Remove Programs or installer)

### macOS DMG
- **Size**: ~80-100 MB
- **Installation Time**: 5-10 seconds
- **Features**: Drag-and-drop to Applications
- **Uninstall**: Delete from Applications folder

### Linux DEB
- **Size**: ~60-80 MB
- **Installation Time**: 10-20 seconds
- **Features**: System integration, graphical installer
- **Uninstall**: `sudo apt remove chess`

### Linux AppImage
- **Size**: ~100-150 MB
- **Installation Time**: 0 seconds (no installation)
- **Features**: Portable, works everywhere
- **Uninstall**: Delete the file

---

## ?? Quick Reference

| Aspect | Windows | macOS | Linux |
|--------|---------|-------|-------|
| **Installer** | MSI | DMG | DEB/AppImage |
| **Install Time** | 10-30s | 5-10s | 10-20s |
| **Size** | 50-70 MB | 80-100 MB | 60-150 MB |
| **Setup** | Automatic | Drag-drop | Auto/Manual |
| **Uninstall** | Easy | Delete app | apt remove |
| **User Friendly** | ????? | ????? | ???? |

---

## ?? Security & Verification

### Checksums
All files are SHA256 hashed in `CHECKSUMS.txt`:
```
Chess-1.0.0.msi SHA256: abc123...
Chess-1.0.0.dmg SHA256: def456...
chess-1.0.0.deb SHA256: ghi789...
```

Users can verify:
```bash
sha256sum -c CHECKSUMS.txt
```

### Code Signing (Optional)
For production:
- **Windows**: Sign MSI with certificate
- **macOS**: Notarize DMG with Apple
- **Linux**: GPG sign packages

---

## ?? Pre-Release Checklist

- [ ] All installers build successfully
- [ ] Tested MSI on Windows 10/11
- [ ] Tested DMG on macOS
- [ ] Tested DEB on Ubuntu/Debian
- [ ] Tested AppImage on various Linux
- [ ] Checksums verified
- [ ] Documentation is current
- [ ] README is user-friendly
- [ ] GitHub release page created
- [ ] All files uploaded to release
- [ ] Share link with users

---

## ?? User Instructions (What to Send)

### For Windows Users
```
1. Download Chess-1.0.0.msi
2. Double-click to install
3. Click "Finish"
4. Chess is ready to play!
```

### For macOS Users
```
1. Download Chess-1.0.0.dmg
2. Open the image
3. Drag Chess.app to Applications
4. Launch from Applications folder
```

### For Linux Users
**Ubuntu/Debian:**
```bash
sudo dpkg -i chess-1.0.0.deb
chess
```

**Any Linux:**
```bash
chmod +x Chess-1.0.0-x86_64.AppImage
./Chess-1.0.0-x86_64.AppImage
```

---

## ?? Troubleshooting Guide

### Build Fails
- Ensure .NET 8 SDK is installed
- Check WiX Toolset is in PATH (Windows)
- Verify `publish` folder exists

### Installer Won't Run
- Download correct version for your OS
- Check system requirements
- Try downloading again (corrupted file)

### App Won't Launch
- Install .NET 8 Runtime
- Install Stockfish engine
- Check file permissions (Linux)

### Stockfish Not Found
- Download from https://stockfishchess.org
- Extract to standard location
- Update app configuration

---

## ?? Support Resources

**For Developers:**
- GitHub: https://github.com/omsaichand35/Chess-game
- Issues: https://github.com/omsaichand35/Chess-game/issues

**For End Users:**
- SETUP.md (included)
- INSTALLATION_GUIDE.md (included)
- GitHub Releases page

---

## ?? You're Ready!

Your Chess application is now production-ready and can be easily distributed to any user on any platform!

### Next Steps:
1. Run: `./scripts/build-release.sh` (or PowerShell equivalent)
2. Test all installers
3. Create GitHub release
4. Share the link with users
5. Watch them enjoy Chess! ??

---

**Everything is automated. Users just need to click and play!** ??
