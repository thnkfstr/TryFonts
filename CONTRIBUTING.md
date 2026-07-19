# Contributing to Try Fonts

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (or later LTS)
- Git
- Windows or macOS (Linux is not a supported target but the Core library and tests build there)

## Local setup

```sh
git clone https://github.com/ben-adams1/TryFonts
cd TryFonts
dotnet restore TryFonts.sln
```

## Common commands

| Task | Command |
|------|---------|
| Build all | `dotnet build TryFonts.sln` |
| Run tests | `dotnet test TryFonts.sln` |
| Run with hot-reload | `dotnet watch run --project src/TryFonts.App` |
| Check formatting | `dotnet format TryFonts.sln --verify-no-changes` |
| Fix formatting | `dotnet format TryFonts.sln` |
| Windows EXE | `pwsh tools/build-windows.ps1` |
| macOS DMGs | `bash tools/build-macos.sh` |

## Performance testing

To stress-test the virtualized list without needing 5,000 installed fonts, launch with the `--synthetic-fonts` flag:

```sh
dotnet run --project src/TryFonts.App -- --synthetic-fonts 5000
```

This appends 5,000 synthetic font records to whatever is actually installed. Synthetic mode is development-only and must never appear in normal user-facing UI.

## Project layout

```
src/TryFonts.Core/       Domain models and pure logic (no UI dependency)
src/TryFonts.App/        Avalonia desktop UI
  App.axaml / .cs        Application entry, style resources, converters
  MainWindow.axaml / .cs Main window layout and keyboard shortcuts
  ViewModels/            CommunityToolkit.Mvvm view models
  Services/              Font discovery (SkiaSharp) and settings (JSON)
  Converters/            AXAML value converters
  Assets/                Icons
tests/TryFonts.Core.Tests/  xunit tests; no UI, no SkiaSharp dependency
legacy/                  Old WinForms code (reference only)
tools/                   Build + packaging scripts
.github/workflows/       Baseline, platform compatibility, and release pipelines
```

## Architecture decisions

**Avalonia UI** was chosen over WinForms / WPF to support both Windows and macOS from a single codebase. SkiaSharp (a transitive Avalonia dependency) is used for cross-platform font enumeration via `SKFontManager`.

**ListBox + VirtualizingStackPanel** provides real UI virtualization — only rows near the viewport have live controls. This replaces the original manual batch-append loop.

**CommunityToolkit.Mvvm** source generators produce boilerplate-free observable properties and relay commands.

**Settings** are stored in `%APPDATA%\TryFonts\settings.json` (Windows) or `~/Library/Application Support/TryFonts/settings.json` (macOS). The app never writes to its install directory.

**Preview text is never persisted.** Every launch starts with the base sample string. This is a hard requirement enforced by tests.

## Adding a test

Add test files to `tests/TryFonts.Core.Tests/`. Tests must not depend on Avalonia, SkiaSharp, or any installed fonts. Use `SyntheticFontDataGenerator` to produce test data.

## Pull requests

- Keep Core and App cleanly separated — no Avalonia references in Core.
- Add tests for any new filtering, sorting, or settings behavior.
- Do not commit `.vs/`, `bin/`, `obj/`, or `*.pfx` files (the `.gitignore` handles this).
- Update `CHANGELOG.md` with a brief description of the change.
