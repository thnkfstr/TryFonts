# Changelog

All notable changes to Try Fonts are documented here.
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

---

## [Unreleased]

### Added

- **Full rebuild in C# + Avalonia UI** targeting Windows and macOS from a single codebase.
- Cross-platform font discovery via SkiaSharp `SKFontManager` (DirectWrite on Windows, CoreText on macOS).
- `TryFonts.Core` library with no UI dependencies, fully unit-tested.
- Virtualized font list using `ListBox` + `VirtualizingStackPanel` — only visible rows have live controls.
- Six built-in preview text presets: base sample, alphabet, upper/lower, punctuation, typography, and Latin extended.
- Per-font style availability indicators (R / B / I / BI) in each row.
- Settings persistence (font size, bold, italic, search mode, sort mode, window geometry) in user app-data directory via JSON. Preview text is intentionally not persisted.
- `--synthetic-fonts <n>` developer flag for performance testing without needing thousands of installed fonts.
- GitHub Actions CI (Windows + macOS build/test on every push) and a manually-triggered release workflow producing portable EXE and DMG artifacts.
- `tools/build-windows.ps1` and `tools/build-macos.sh` for local release builds.
- Keyboard shortcuts: `/` or `Ctrl+F` → search, `Ctrl+L` → preview text, `Esc` → clear search.
- `legacy/` directory preserving the two original WinForms implementations for reference.

### Removed

- WinForms dependency — app now runs on macOS without Wine or emulation.
- `System.Drawing.Common` — replaced by SkiaSharp font enumeration and Avalonia rendering.
- Manual batch-append lazy loading — replaced by real UI virtualization.
- Font object disposal bug — controls no longer hold disposed Font handles.
- `net6.0-windows` target (end of support) — now targets `net8.0` (LTS).
- Generated `.vs/`, `bin/`, `obj/` artifacts from git tracking.
- Temporary signing key (`*.pfx`) from git history.

---

## [Legacy: New v1] — 2024

WinForms rebuild with debounced search and lazy-load scrolling. Windows only.

## [Legacy: Original v1] — 2024

Initial WinForms implementation. Windows only.

[Unreleased]: https://github.com/ben-adams1/TryFonts/compare/HEAD...HEAD
