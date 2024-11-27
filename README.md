# Try Fonts



Preview any text string across all fonts installed on your computer. Supports bold, italic, font size, search, and filtering. Works completely offline.

**License:** GPL-3.0 · **Author:** Ben Adams · **Platform:** Windows, macOS



---

## Download

Download one file for your platform from the [Releases](../../releases) page.

| Platform | File | Notes |
|----------|------|-------|
| Windows  | `TryFonts-windows-x64-{version}.exe` | Portable. Double-click to run. No installer needed. |
| macOS (Apple Silicon) | `TryFonts-macos-arm64-{version}.dmg` | Open the DMG, drag to Applications. |
| macOS (Intel) | `TryFonts-macos-x64-{version}.dmg` | Open the DMG, drag to Applications. |

### .NET not required

Each download is self-contained and includes everything it needs to run.

### Offline / privacy

Try Fonts makes no network requests at any time. It reads only the fonts installed on your system. No analytics, no telemetry, no accounts.

### Signing status

Current releases are **unsigned**. This means:

- **Windows:** SmartScreen will warn on first run. Click "More info" → "Run anyway."
- **macOS:** Gatekeeper will block the app on first launch. Right-click → Open → Open.

Signing hooks are built into the release pipeline and will be enabled once certificates are in place. See [`docs/signing.md`](docs/signing.md) for the plan.

---

## Build from source

Requirements: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later.

```sh
# Clone
git clone https://github.com/ben-adams1/TryFonts
cd TryFonts

# Build and test
dotnet restore TryFonts.sln
dotnet build   TryFonts.sln --configuration Release
dotnet test    TryFonts.sln --configuration Release

# Run (development mode)
dotnet run --project src/TryFonts.App

# Produce Windows EXE (run on Windows or in Windows CI)
pwsh tools/build-windows.ps1

# Produce macOS DMGs (run on macOS)
bash tools/build-macos.sh
```

See [CONTRIBUTING.md](CONTRIBUTING.md) for full setup instructions.

---

## Features

- Virtualized list — smooth scrolling through 5,000+ fonts without stalls
- Live search with Contains / Starts with modes
- Bold and italic toggles; style availability shown per font
- Built-in preview text presets (base sample, alphabet, typography, Latin extended, …)
- Persists font size, bold/italic, search mode, and sort between sessions
- Preview text always resets to the base sample on launch
- Window size and position restored across sessions

---

## Project layout

```
src/
  TryFonts.Core/      Platform-independent models and services
  TryFonts.App/       Avalonia UI (Windows + macOS)
tests/
  TryFonts.Core.Tests/ Unit tests (no UI dependency)
legacy/               Original WinForms versions (reference only)
docs/                 User and maintainer documentation
tools/                Build and packaging scripts
.github/workflows/    CI and release pipelines
```