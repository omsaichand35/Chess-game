# Chess Application - End User Setup Guide

## ?? Quick Start (Choose Your Operating System)

### ?? Windows Users
```
1. Download: Chess-1.0.0.msi
2. Double-click the file
3. Follow the installation wizard
4. Click "Finish"
5. Chess appears on your Desktop and Start Menu
6. Double-click Chess to play!
```

### ?? macOS Users
```
1. Download: Chess-1.0.0.dmg
2. Double-click to mount the disk image
3. Drag "Chess.app" to your Applications folder
4. Open Applications folder
5. Double-click Chess to play!
```

### ?? Linux Users (Ubuntu/Debian)
```
1. Download: chess-1.0.0.deb
2. Double-click to open in Software Center
3. Click "Install"
4. Enter your password if prompted
5. Search for "Chess" in Applications
6. Click to play!
```

### ?? Linux Users (Any Distribution)
```
1. Download: Chess-1.0.0-x86_64.AppImage
2. Right-click ? Properties ? Permissions ? Make Executable
3. Double-click to play!
```

---

## ?? System Requirements

### All Platforms Need:
- ? **.NET 8 Runtime** (Free, from Microsoft)
- ? **Stockfish Chess Engine** (Free, open source)

### Platform-Specific Requirements

**Windows 10/11:**
- 500 MB free disk space
- 2 GB RAM

**macOS 10.13+:**
- 500 MB free disk space
- 2 GB RAM

**Linux:**
- 500 MB free disk space
- 2 GB RAM
- X11 or Wayland display server

---

## ?? Installing Prerequisites

### Step 1: Install .NET 8 Runtime

**Windows:**
1. Go to: https://dotnet.microsoft.com/download/dotnet/8.0
2. Click ".NET 8.0" ? "Runtime" ? "Windows x64"
3. Download and run the installer
4. Follow the setup wizard
5. Restart your computer

**macOS:**
1. Go to: https://dotnet.microsoft.com/download/dotnet/8.0
2. Click ".NET 8.0" ? "Runtime" ? "macOS ARM64" or "x64" (check your Mac)
3. Download and run the installer
4. Follow the setup wizard
5. Restart your computer

**Linux (Ubuntu/Debian):**
```bash
# Add Microsoft repository
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x ./dotnet-install.sh
./dotnet-install.sh --channel 8.0
```

### Step 2: Install Stockfish Engine

**All Platforms:**
1. Go to: https://stockfishchess.org/download/
2. Download for your operating system
3. Extract to a known location:
   - **Windows**: `C:\Program Files\Stockfish\`
   - **macOS**: `/usr/local/bin/stockfish/`
   - **Linux**: `/usr/local/bin/` or `~/.local/bin/`

**Verify Installation:**

**Windows (PowerShell):**
```powershell
stockfish --version
```

**macOS/Linux:**
```bash
which stockfish
stockfish --version
```

---

## ?? Playing Chess

### Game Modes

1. **Play vs Computer** ??
   - Play against AI
   - Choose difficulty: Easy to Expert
   - Play as White or Black

2. **Play vs Friend** ??
   - Local multiplayer on same computer
   - Take turns with another player
   - Board rotates after each move

3. **Play Online** ??
   - Play with someone on another computer
   - Host a game or join a friend
   - Enter IP address and port

4. **AI vs AI** ??
   - Watch two AI engines play
   - Choose names and difficulty
   - Learn from AI strategies

---

## ?? Troubleshooting

### "Stockfish not found" Error

**Solution:**
1. Make sure Stockfish is installed and executable
2. On Linux/macOS, make executable:
   ```bash
   chmod +x /path/to/stockfish
   ```
3. Verify path:
   ```bash
   which stockfish  # Linux/macOS
   where stockfish  # Windows
   ```
4. If not in PATH, update Chess config with full path

### ".NET Runtime not installed" Error

**Solution:**
1. Download .NET 8 from: https://dotnet.microsoft.com/download/dotnet/8.0
2. Install for your operating system
3. Verify installation:
   ```bash
   dotnet --version
   ```
4. Restart Chess application

### "Permission Denied" (Linux/macOS)

**Solution:**
```bash
# Make executable
chmod +x ~/Downloads/Chess-1.0.0-x86_64.AppImage

# Or for DMG on macOS
chmod +x /Applications/Chess.app/Contents/MacOS/Chess
```

### "Cannot find library" Error

**Solution - Linux:**
```bash
# Install required libraries
sudo apt-get install libfontconfig1 libfreetype6 libx11-6 libgl1
```

### Game Crashes

**Solution:**
1. Ensure .NET 8 runtime is installed
2. Verify Stockfish is working:
   ```bash
   stockfish
   quit
   ```
3. Check system resources (disk space, RAM)
4. Try reinstalling the application

---

## ?? Using the Application

### Main Menu
- **Play vs Computer** - Solo gameplay with difficulty selection
- **Play vs Friend** - Local two-player game
- **Play Online** - Network multiplayer
- **AI vs AI** - Watch AI engines compete

### Difficulty Levels
- ?? **Easy** - Great for learning
- ?? **Medium** - Balanced challenge
- ?? **Hard** - Challenging gameplay
- ? **Expert** - Maximum difficulty

### Board Display
- Yellow squares = Last move
- Green squares = Legal move options
- Red border = Selected piece
- Material advantage shown at top

---

## ?? Features

? Full chess rule implementation
? Multiple difficulty levels
? Online multiplayer support
? AI vs AI mode
? Move highlights and suggestions
? Game state tracking
? Captured pieces display
? Win/draw/loss statistics

---

## ?? Performance Tips

1. **Easier=Faster**: Lower difficulty = quicker AI responses
2. **First Move**: Initial moves may take a moment to calculate
3. **Network Play**: Requires stable internet connection
4. **Offline Modes**: AI modes work without internet

---

## ?? File Locations

**Windows:**
- Installed to: `C:\Program Files\Chess\`
- Config: `%APPDATA%\Chess\`

**macOS:**
- Installed to: `/Applications/Chess.app`
- Config: `~/.config/Chess/`

**Linux:**
- DEB: `/opt/chess/`
- AppImage: Anywhere (portable)
- Config: `~/.config/Chess/`

---

## ? FAQ

**Q: Can I play online with my friends?**
A: Yes! Use "Play Online" mode. You need your friend's IP address and port number.

**Q: Is there a time limit per move?**
A: No, the AI takes as long as needed for your selected difficulty.

**Q: Can I undo moves?**
A: Currently, moves cannot be undone. Plan your moves carefully!

**Q: Does it work offline?**
A: Yes! Only "Play Online" requires internet. Local and AI modes work offline.

**Q: Where's my game saved?**
A: Games are tracked for statistics but not saved for later replay.

**Q: Can I change the difficulty mid-game?**
A: Start a new game with different difficulty settings.

---

## ?? Support

If you encounter issues:

1. Check this guide first
2. Verify .NET 8 and Stockfish are installed
3. Try reinstalling the application
4. Report bugs at: https://github.com/omsaichand35/Chess-game/issues

---

## ?? Additional Resources

- **Official GitHub**: https://github.com/omsaichand35/Chess-game
- **.NET 8 Documentation**: https://docs.microsoft.com/dotnet/core/
- **Stockfish Documentation**: https://stockfishchess.org/
- **Chess Rules**: https://www.chess.com/terms/chess-rules

---

**Enjoy your Chess experience! ??**

For questions or issues, visit the GitHub repository: https://github.com/omsaichand35/Chess-game
