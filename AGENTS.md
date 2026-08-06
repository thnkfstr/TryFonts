# TryFonts Agent Instructions

TryFonts is a cross-platform Avalonia desktop application for previewing locally installed fonts.
Read `README.md` and `CONTRIBUTING.md` before substantial work.

## Boundaries

- Preserve the application's offline-only privacy model: no network requests, analytics,
  telemetry, accounts, or remote font services.
- Keep platform-independent behavior in `src/TryFonts.Core`; do not introduce Avalonia or
  SkiaSharp dependencies into the Core project.
- Tests under `tests/TryFonts.Core.Tests` must not depend on a graphical environment, installed
  fonts, Avalonia, or SkiaSharp. Use `SyntheticFontDataGenerator` for font data.
- Linux supports Core development and tests, but Windows and macOS packaging must be validated on
  their respective platforms.
- Do not commit signing certificates, private keys, certificate passwords, or notarization
  credentials. Release signing uses environment-provided secrets described in `docs/signing.md`.
- Update `CHANGELOG.md` for user-visible changes.
- Check `git status --short --branch` before editing and before committing.

## Validation

Use the repository's documented commands:

```sh
dotnet restore TryFonts.sln
dotnet build TryFonts.sln --configuration Release
dotnet test TryFonts.sln --configuration Release
dotnet format TryFonts.sln --verify-no-changes
```

Build distributable artifacts only through the platform-specific scripts in `tools/`.
