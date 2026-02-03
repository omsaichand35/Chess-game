# ?? 5-MINUTE QUICK START - Build & Distribute

## For the Impatient Developer ??

### **Step 1: Build Everything (Choose Your OS)**

#### Windows (PowerShell)
```powershell
.\scripts\build-release.ps1 -Version "1.0.0"
```

#### Linux/macOS (Bash)
```bash
chmod +x scripts/build-release.sh
./scripts/build-release.sh
```

### **Step 2: Find Your Installers**
```
release/
??? windows/Chess-1.0.0.msi          ? Windows users get this
??? macos/Chess-1.0.0.dmg            ? macOS users get this  
??? linux/chess-1.0.0.deb            ? Ubuntu/Debian users get this
??? linux/Chess-1.0.0-x86_64.AppImage ? Any Linux user gets this
??? README.md                         ? Everyone reads this first!
```

### **Step 3: Share With Users**

**Option A: GitHub Releases** (RECOMMENDED)
```bash
# Create release on GitHub and upload all files from release/ folder
# Users download from: https://github.com/omsaichand35/Chess-game/releases
```

**Option B: Direct Links**
```
Download Windows:   https://your-site.com/Chess-1.0.0.msi
Download macOS:     https://your-site.com/Chess-1.0.0.dmg
Download Linux:     https://your-site.com/chess-1.0.0.deb
```

### **Step 4: Done! ??**

Users just double-click their installer and play!

---

## ?? What Each User Sees

### Windows User
```
1. Download Chess-1.0.0.msi
2. Double-click
3. Follow wizard
4. Click "Finish"
5. Chess is ready to play!
```

### macOS User  
```
1. Download Chess-1.0.0.dmg
2. Double-click  
3. Drag Chess.app to Applications
4. Play!
```

### Linux User
```bash
# Ubuntu/Debian
sudo dpkg -i chess-1.0.0.deb
chess

# Any Linux
chmod +x Chess-1.0.0-x86_64.AppImage
./Chess-1.0.0-x86_64.AppImage
```

---

## ?? System Requirements (For You to Tell Users)

All platforms need:
- ? .NET 8 Runtime (free from Microsoft)
- ? Stockfish engine (free from stockfishchess.org)

**Installation guides in:** `SETUP.md`

---

## ?? How to Test

```bash
# Test on Windows
# ? Download Chess-1.0.0.msi and double-click

# Test on macOS  
# ? Download Chess-1.0.0.dmg and double-click

# Test on Linux
# ? Download chess-1.0.0.deb and double-click (or AppImage)
```

If everything runs ? You're good! ?

---

## ?? File Structure Reference

```
Your Repo
??? scripts/
?   ??? build-release.ps1    ? Run on Windows
?   ??? build-release.sh     ? Run on Linux/macOS
?   ??? build-linux-deb.sh   ? Auto-called by build-release.sh
?   ??? build-linux-appimage.sh ? Auto-called by build-release.sh
??? release/                 ? Contains your distributable files!
?   ??? windows/Chess-1.0.0.msi
?   ??? macos/Chess-1.0.0.dmg
?   ??? linux/chess-1.0.0.deb
?   ??? linux/Chess-1.0.0-x86_64.AppImage
?   ??? README.md
??? SETUP.md                 ? Users read this
```

---

## ? TL;DR Version

```
BUILD: Run script
WAIT: 5 minutes
CHECK: release/ folder
SHIP: All files ready to distribute!
DONE: Users just click and play! ??
```

---

## ?? If Something Goes Wrong

**Build fails?**
- Ensure .NET 8 SDK is installed
- Run: `dotnet --version`
- Check internet connection

**Installer won't run?**
- Try downloading again (corrupted file)
- Check system requirements match

**App crashes on user's computer?**
- They need .NET 8 Runtime
- They need Stockfish engine
- See SETUP.md

---

## ?? That's It!

Your Chess app is production-ready and can be given to:
- ? Windows users
- ? macOS users  
- ? Linux users (any distro)

No technical knowledge required on their end.
Everything is automated.

**Go ship it!** ??
