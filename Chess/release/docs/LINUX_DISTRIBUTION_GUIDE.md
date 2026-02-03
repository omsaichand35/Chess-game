# How to Share Chess with Linux Friends

## ?? Recommended Options (Easiest to Hardest)

### Option 1: **DEB Package** (Ubuntu/Debian) ? EASIEST
Perfect for: Ubuntu, Debian, Linux Mint, Pop!_OS

**Steps for your friend:**
```bash
# 1. Download the .deb file you send them
# 2. Open file manager and double-click the .deb file
# 3. Click "Install" in the Software Center
# 4. Launch from Applications menu
```

Or via terminal:
```bash
sudo apt install ~/Downloads/chess-1.0.0.deb
chess
```

**How to send:**
- Build: `./scripts/build-linux-deb.sh`
- Send file: `chess-1.0.0.deb`
- Size: ~150 MB

---

### Option 2: **AppImage** (Any Linux) ? UNIVERSAL
Perfect for: Any 64-bit Linux (works everywhere!)

**Steps for your friend:**
```bash
# 1. Download the .AppImage file you send
# 2. Right-click ? Properties ? Make Executable
# 3. Double-click to run (or ./Chess-1.0.0-x86_64.AppImage in terminal)
```

**How to send:**
- Build: `./scripts/build-linux-appimage.sh`
- Send file: `Chess-1.0.0-x86_64.AppImage`
- Size: ~250 MB
- **Advantage**: Works on ANY Linux distro!

---

### Option 3: **Docker** ? MODERN APPROACH
Perfect for: Friends comfortable with Docker

**Steps for your friend:**
```bash
# 1. Install Docker: https://docs.docker.com/install/
# 2. Clone your repository
git clone https://github.com/omsaichand35/Chess-game.git
cd Chess

# 3. Run with Docker
docker-compose up
```

**Advantages:**
- No installation needed
- Works on any Linux with Docker
- Isolated environment
- Easy updates (just pull latest code)

---

### Option 4: **Build from Source**
For developer friends who want to compile themselves

**Steps:**
```bash
git clone https://github.com/omsaichand35/Chess-game.git
cd Chess/Chess
dotnet build -c Release
dotnet run
```

**Requirements:**
- .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
- Stockfish engine: https://stockfishchess.org/download/

---

## ?? Quick Comparison

| Option | Ease | Size | Works Everywhere | Setup Time |
|--------|------|------|------------------|------------|
| **DEB** | ????? | 150 MB | Ubuntu/Debian | 30 seconds |
| **AppImage** | ????? | 250 MB | Any Linux | 30 seconds |
| **Docker** | ???? | 500 MB | Any w/ Docker | 2 minutes |
| **Source** | ??? | 100 MB | Any | 5 minutes |

---

## ?? Step-by-Step: Building & Sharing

### 1. Build the Installers
```bash
cd C:\Users\omsai\source\repos\Chess

# Publish application
dotnet publish Chess/Chess.csproj -c Release -o publish --self-contained false

# Build DEB (if on Linux VM/WSL)
chmod +x scripts/build-linux-deb.sh
./scripts/build-linux-deb.sh
# Result: chess-1.0.0.deb

# Build AppImage (if on Linux VM/WSL)
chmod +x scripts/build-linux-appimage.sh
./scripts/build-linux-appimage.sh
# Result: build/appimage/Chess-1.0.0-x86_64.AppImage
```

### 2. Share via GitHub Releases (Recommended)
```bash
# Create GitHub release
git tag v1.0.0
git push origin v1.0.0

# Upload files via GitHub UI:
# 1. Go to: https://github.com/omsaichand35/Chess-game/releases
# 2. Click "Create a new release"
# 3. Tag: v1.0.0
# 4. Upload: chess-1.0.0.deb + Chess-1.0.0-x86_64.AppImage
# 5. Publish
```

### 3. Share via Direct Download
**For friend to download:**
```bash
# DEB
wget https://github.com/omsaichand35/Chess-game/releases/download/v1.0.0/chess-1.0.0.deb
sudo apt install ./chess-1.0.0.deb

# AppImage
wget https://github.com/omsaichand35/Chess-game/releases/download/v1.0.0/Chess-1.0.0-x86_64.AppImage
chmod +x Chess-1.0.0-x86_64.AppImage
./Chess-1.0.0-x86_64.AppImage
```

---

## ?? MY RECOMMENDATION

**For your Linux friend, send them:**

1. **First choice**: `chess-1.0.0.deb` (if they use Ubuntu/Debian)
   - Simplest: double-click to install
   - System integration
   - Auto-updates through apt

2. **Fallback**: `Chess-1.0.0-x86_64.AppImage` (works on any Linux)
   - No installation needed
   - Just download and run
   - Perfect if they use Fedora, Arch, etc.

3. **Instructions to send them:**
```
Hi! Here's how to install Chess on Linux:

## For Ubuntu/Debian:
1. Download: chess-1.0.0.deb
2. Double-click and click "Install"
3. Search for "Chess" in Applications

## For other Linux:
1. Download: Chess-1.0.0-x86_64.AppImage
2. Right-click ? Properties ? Make Executable
3. Double-click to play!

Both require:
- .NET 8 Runtime (auto-installed on DEB)
- Stockfish engine (download from https://stockfishchess.org/download/)
```

---

## ?? File Sizes & Storage

| File | Size | Compressed |
|------|------|-----------|
| chess-1.0.0.deb | ~150 MB | ~70 MB (zip) |
| Chess-1.0.0-x86_64.AppImage | ~250 MB | ~120 MB (zip) |
| .NET Runtime | Included | - |
| Stockfish | ~50 MB | Separate download |

---

## ? Pre-Flight Checklist for Your Friend

Before running on Linux:
- [ ] Download installer (DEB or AppImage)
- [ ] For DEB: `sudo apt-get update && sudo apt-get install libfontconfig1 libfreetype6`
- [ ] Download Stockfish engine
- [ ] Extract to `~/.stockfish/stockfish` or `/usr/local/bin/stockfish`
- [ ] Update `StockfishEngine.cs` with correct path (if needed)

---

## ?? Alternative: Docker for Easy Sharing

If your friend just wants to run it without installation:

```dockerfile
# Send this simplified setup to your friend:

# 1. Clone the repo
git clone https://github.com/omsaichand35/Chess-game.git
cd Chess

# 2. Run with Docker
docker-compose up

# That's it! No other setup needed.
```

---

## ?? Support Links for Your Friend

- **Stockfish**: https://stockfishchess.org/download/
- **.NET Runtime**: https://dotnet.microsoft.com/download/dotnet/8.0
- **AppImage Info**: https://appimage.org/
- **Docker**: https://docs.docker.com/install/
