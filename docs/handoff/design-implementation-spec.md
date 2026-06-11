# Try Fonts Rebuild Design And Implementation Spec

## Mission

Rebuild Try Fonts as a releasable offline desktop tool for previewing arbitrary text across large local font libraries. The finished product must feel instant while scrolling through thousands of fonts and must be easy for nontechnical users to download and run.

## Non-Negotiable Outcomes

- End users can download one file per platform.
- Windows users get a strong-preference artifact that runs directly as one portable `.exe`.
- macOS users get one downloaded file that installs or opens the app locally, such as `.dmg` or another standard macOS single-file container. A true one-file double-click app is preferred only if it does not degrade platform behavior.
- The app works completely offline after download.
- The app never phones home, loads remote assets, or requires an account.
- The UX remains smooth with at least 5,000 discoverable fonts.
- Scrolling must use real UI virtualization. Do not fake this with manual batch append logic.
- The implementation must be maintainable as an OSS project, with build, test, and release commands documented and automated.

## Starting Context

The current repo contains two WinForms versions under `Original/` and `New/`. Treat them as behavioral reference only, not as the foundation to patch.

Known current issues to eliminate:

- WinForms and `System.Drawing` make the app Windows-only.
- The project files are ignored by the root `.gitignore`, so a clean clone is not reliably buildable.
- Generated `.vs`, `bin`, `obj`, publish output, and temporary signing material are mixed into the source tree.
- The current lazy loading creates controls manually and clears/recreates the list on common edits.
- Font objects are disposed while still assigned to labels.
- The current package target is `net6.0-windows`, which is out of support.
- The current publish output is large and Windows-only.

## Technology Decision

Build the new app in C# with Avalonia UI unless a short spike proves a blocker that cannot be solved cleanly.

Required architecture:

- `src/TryFonts.App`: desktop UI.
- `src/TryFonts.Core`: platform-independent application logic.
- `tests/TryFonts.Core.Tests`: unit tests for filtering, sorting, glyph sample presets, restart defaults, settings, and non-UI behavior.
- `tests/TryFonts.App.Tests` or equivalent UI automation harness if practical.
- `docs/`: user and maintainer docs.
- `tools/`: repeatable local scripts only when commands become long or fragile.

Target modern supported .NET. Prefer the current LTS at implementation time. Do not target an end-of-support runtime.

Do not use WinForms, WPF, or `System.Drawing.Common` for the rebuilt cross-platform app.

## Product Scope

### Primary Screen

The first screen is the actual font browser, not a landing page.

Required controls:

- Text input for preview text.
- Preset preview text selector.
- Font size control.
- Bold toggle.
- Italic toggle.
- Search input.
- Search mode selector: `Contains`, `Starts with`.
- Sort selector: at minimum `Name A-Z` and `Name Z-A`.
- Font count display showing visible fonts and total discovered fonts.
- Virtualized preview list.

Required row content:

- Preview text rendered in the row's font family.
- Font family name.
- Availability indicators for regular, bold, italic, bold italic when the platform can determine them reliably.
- Clear fallback styling when a requested style is unavailable.

### Preview Text

Include at least these built-in presets:

- Base sample: `*The quick brown fox jumps over 10 of the 2,345 lazy dogs @ the farm - starting with #6 & costing $7 (plus $0.89 tax?)!`
- Alphabet and digits.
- Uppercase/lowercase contrast.
- Punctuation and symbols.
- Typography sample with quotes, apostrophes, dashes, currency, fractions, and common math symbols.
- Multilingual Latin sample with accented characters.

Users can type custom text during a session. On every fresh app launch, the preview text field must start with the base sample string above, not the prior custom text.

### Filtering

Filtering must be case-insensitive.

Search behavior:

- Empty search shows all fonts.
- `Contains` matches anywhere in the family name.
- `Starts with` matches from the beginning of the family name.
- Filtering updates after a short debounce.
- Filtering must not reset user-entered text.
- Filtering should preserve scroll position when possible; if not possible, reset to top intentionally and consistently.

### Performance

The list must be virtualized by the UI framework. Only rows near the viewport should have live controls.

Performance requirements on a machine with at least 5,000 fonts:

- Initial usable UI appears in under 1 second.
- Font discovery may continue asynchronously, but controls must remain responsive.
- Search results update in under 150 ms after debounce for 5,000 fonts.
- Scrolling remains visually smooth at high wheel or trackpad speed.
- No long UI-thread stalls above 50 ms during normal scrolling.
- Memory usage remains stable after repeated scrolling from top to bottom and back.

If the local machine does not have 5,000 installed fonts, implement a development/test mode that can synthesize font records while reusing real installed font families for rendering where possible. Synthetic mode is for performance testing only and must not be exposed as a confusing normal user feature.

## Font Discovery

Implement a font service behind an interface in `TryFonts.Core`.

Required model:

```csharp
public sealed record FontFamilyInfo(
    string FamilyName,
    IReadOnlySet<FontFaceStyle> AvailableStyles,
    string? SourcePath = null
);
```

`SourcePath` is optional and must not be required for normal operation. Do not expose absolute paths in the default UI unless a deliberate "details" affordance is added.

Font discovery must:

- Enumerate installed system/user fonts on Windows and macOS.
- De-duplicate family names.
- Sort deterministically.
- Fail gracefully if individual fonts cannot be loaded.
- Never block first paint on full discovery.
- Be isolated enough that platform-specific code can be tested or mocked.

If Avalonia exposes adequate installed font enumeration, use it. If not, use small platform-specific adapters:

- Windows: DirectWrite or another supported Windows font API.
- macOS: CoreText via a maintained binding or minimal interop.

Do not parse OS font directories with ad hoc string scanning as the primary discovery mechanism.

## Rendering

Preview rows must use platform font fallback naturally. If a glyph is missing in a font, the app may display platform fallback glyphs, but the UI should allow a future glyph-coverage indicator.

Required behavior:

- Changing font size updates visible rows without rebuilding all records.
- Toggling bold/italic updates visible rows.
- If a selected style does not exist for a family, render using the closest available style and mark the style as unavailable in the row metadata.
- Long preview text wraps or clips cleanly according to a documented UI choice. It must not overlap font names or adjacent rows.

## UX Requirements

The tool should feel utilitarian, focused, and fast.

Layout:

- Top controls in a compact toolbar.
- Preview text field prominent but not oversized.
- Font list takes most of the viewport.
- Avoid decorative hero sections, marketing copy, or nested cards.
- Use restrained styling with clear contrast.
- Support resizing on Windows and macOS.
- Respect OS light/dark mode if straightforward; otherwise ship a clean light theme first.

Keyboard:

- `/` or `Ctrl+F` focuses search.
- `Esc` clears search when search is focused.
- `Ctrl+L` or equivalent focuses preview text.
- Arrow/page scrolling works in the list.
- Tab order is logical.

Accessibility:

- Controls have accessible names.
- Font rows expose family names to screen readers.
- The app is usable at common OS scaling settings.

## Settings

Persist locally:

- Font size.
- Bold/italic state.
- Search mode.
- Sort mode.
- Window size and position when safe.

Do not persist custom preview text. Do not persist a custom-text startup mode. Every launch starts with the base sample string.

Settings must be stored in the user's normal per-user app data location. Do not write settings into the app install directory.

## Distribution

### Windows

Required release artifact:

- `TryFonts-windows-x64.exe`: portable, self-contained, double-click runnable.

Optional later artifact:

- `TryFonts-windows-x64-setup.exe`: installer if code signing, file associations, Start Menu shortcuts, or updates become necessary.

The portable `.exe` is the primary Windows target. It must not require the user to install .NET.

### macOS

Required release artifacts:

- `TryFonts-macos-arm64.dmg`.
- `TryFonts-macos-x64.dmg`, unless Intel Mac support is explicitly dropped.

The `.dmg` must contain the app and clear install affordance. The app must not require the user to install .NET.

### Signing And Trust

Implement release packaging so unsigned local development builds are possible.

For public release:

- Document whether each artifact is signed, notarized, or unsigned.
- Windows signing requires an Authenticode code-signing certificate. If no certificate is available, release notes must clearly warn that SmartScreen may complain.
- macOS public releases should be signed and notarized with an Apple Developer ID certificate. If not available, release notes must clearly explain the first-run warning.
- Signing secrets must never be committed.
- CI must support unsigned builds without secrets and signed builds only when secrets are present.

Definition of done does not require purchasing certificates, but it does require the build pipeline and documentation to make the signing gap explicit and to support signing later without redesign.

## Repository Cleanup Requirements

Before rebuilding, normalize the repo:

- Move old implementations to `legacy/Original` and `legacy/New`, or keep them in place with a clear `legacy/README.md`.
- Remove generated `.vs`, `bin`, `obj`, publish output, and temporary signing material from source control candidates.
- Fix `.gitignore` so source project files are tracked and generated files are ignored.
- Track solution/project files needed for a clean clone build.
- Add `README.md` with user-facing description, build commands, release artifact explanation, offline/privacy statement, and license.
- Add `CONTRIBUTING.md` with local setup and test commands.
- Add `CHANGELOG.md`.
- Keep GPL-3.0. Do not change the license.

## Build And CI

Required local commands:

- Restore dependencies.
- Build all projects.
- Run all tests.
- Run formatting/analyzer checks.
- Produce Windows release artifact.
- Produce macOS release artifacts in CI.

Required CI jobs:

- Windows build/test.
- macOS build/test.
- Release packaging workflow that can run manually.
- Artifact upload for each platform.

Do not require internet at app runtime. Build-time dependency restore is acceptable.

## Testing Requirements For Builder

The builder must add tests for:

- Font name filtering.
- Starts-with vs contains behavior.
- Case-insensitive search.
- Stable sorting.
- Settings persistence model.
- Restart default behavior: preview text resets to the base sample string on every launch.
- Preview preset selection.
- Handling unavailable styles.
- Synthetic large-list performance data generation.

The builder must also provide a repeatable manual smoke checklist for Windows and macOS.

## Definition Of Done

The implementation is done only when all items below are true:

- A clean clone can build from tracked files.
- Old generated artifacts are removed or quarantined under documented legacy folders.
- `dotnet build` or the documented equivalent succeeds.
- All tests pass.
- The app starts with no internet connection.
- The app discovers installed fonts on Windows.
- The app discovers installed fonts on macOS.
- The app can display and scroll a virtualized list of at least 5,000 font records.
- Search, sort, size, bold, italic, and preview text changes work without UI stalls.
- Windows release packaging emits one portable self-contained `.exe`.
- macOS release packaging emits one `.dmg` per supported architecture.
- README explains install/run behavior, offline/privacy behavior, and signing status.
- CI builds and tests Windows and macOS.
- CI can produce unsigned release artifacts.
- Signing/notarization hooks are documented and ready for future secrets.
- QA has enough scripts/docs to run the evaluation spec without reverse-engineering the app.

## Gherkin Acceptance Criteria

```gherkin
Feature: Offline launch
  Scenario: Windows user runs the portable app
    Given a Windows user has downloaded TryFonts-windows-x64.exe
    And the machine has no internet connection
    When the user double-clicks the file
    Then Try Fonts opens without installing .NET
    And no remote network request is required

  Scenario: macOS user installs from a single downloaded file
    Given a macOS user has downloaded the Try Fonts dmg for their architecture
    And the machine has no internet connection
    When the user opens the dmg and launches the app
    Then Try Fonts opens without installing .NET
    And no remote network request is required
```

```gherkin
Feature: Font browsing performance
  Scenario: User scrolls through a very large font list
    Given the app has at least 5000 font records available
    When the user scrolls rapidly from the top to the bottom of the list
    Then scrolling remains visually smooth
    And the UI does not freeze
    And memory usage remains stable after returning to the top
```

```gherkin
Feature: Preview text
  Scenario: User enters custom preview text
    Given Try Fonts is open
    When the user types "Sphinx of black quartz, judge my vow!" into the preview text field
    Then visible font rows render that exact text
    And the font family names remain visible
```

```gherkin
Feature: Font filtering
  Scenario: Contains search
    Given Try Fonts has discovered fonts named "Arial", "Arial Rounded MT Bold", and "Georgia"
    And search mode is "Contains"
    When the user searches for "round"
    Then "Arial Rounded MT Bold" is visible
    And "Arial" is not visible
    And "Georgia" is not visible

  Scenario: Starts with search
    Given Try Fonts has discovered fonts named "Arial", "Arial Rounded MT Bold", and "Georgia"
    And search mode is "Starts with"
    When the user searches for "ari"
    Then "Arial" is visible
    And "Arial Rounded MT Bold" is visible
    And "Georgia" is not visible
```

```gherkin
Feature: Style selection
  Scenario: User toggles bold and italic
    Given Try Fonts is open
    When the user enables bold
    And the user enables italic
    Then visible rows render using bold italic when available
    And rows without bold italic indicate the unavailable style without crashing
```

```gherkin
Feature: Settings persistence
  Scenario: User settings are restored without preserving custom text
    Given the user changed preview text to "Custom launch text"
    And the user changed font size, search mode, and sort mode
    When the user closes and reopens Try Fonts
    Then the preview text is "*The quick brown fox jumps over 10 of the 2,345 lazy dogs @ the farm - starting with #6 && costing $7 (plus $0.89 tax?)!"
    And the prior font size, search mode, and sort mode are restored
    And the app remains fully offline
```

```gherkin
Feature: Release artifacts
  Scenario: Release workflow produces user-downloadable files
    Given the release workflow has completed
    Then a Windows portable exe artifact exists
    And a macOS arm64 dmg artifact exists
    And a macOS x64 dmg artifact exists unless Intel support is explicitly dropped
    And each artifact has documented signing status
```
