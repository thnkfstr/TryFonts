# Try Fonts Rebuild Evaluation Spec

## Mission

Evaluate whether the rebuilt Try Fonts is ready to release as an OSS desktop app. This spec is the QA handoff. The evaluator must verify behavior, packaging, performance, offline operation, and repository health without relying on builder claims.

## Evaluation Inputs

The evaluator needs:

- A clean checkout of the repo.
- The builder's documented build/test commands.
- Windows test machine.
- macOS Apple Silicon test machine.
- macOS Intel test machine or explicit product decision dropping Intel support.
- At least one machine with many installed fonts, or the app's documented synthetic large-list test mode.
- Network control method, such as disabling Wi-Fi/Ethernet or using a firewall rule.
- Release artifacts from the CI/release workflow.

## Pass/Fail Rule

Fail the release if any required check below fails. Cosmetic issues may be marked as non-blocking only when they do not affect installability, offline operation, data safety, accessibility basics, or performance.

## Repository QA

Run from a fresh clone, not from the builder's dirty working tree.

Required checks:

- Source project files are tracked.
- Generated directories such as `.vs`, `bin`, `obj`, publish output, and temporary signing keys are not tracked.
- Old WinForms code is clearly marked as legacy or removed.
- `README.md` explains purpose, install/run, offline behavior, signing status, and build commands.
- `CONTRIBUTING.md` or equivalent explains local setup and tests.
- License file is present and is GPL-3.0.
- CI workflow files are present for Windows and macOS.
- Release workflow is present.
- No committed secrets, certificates, `.pfx`, `.env`, or private keys.

Suggested commands:

```powershell
git status --short --branch
git ls-files
rg -n --hidden --glob '!legacy/**' --glob '!**/bin/**' --glob '!**/obj/**' --glob '!**/.vs/**' "\.pfx|BEGIN PRIVATE KEY|BEGIN RSA PRIVATE KEY|client_secret|password|token"
```

Expected result:

- Working tree is clean before evaluation starts.
- Secret scan has no real secret findings.
- Build files required for a clean clone are present.

## Build QA

Run the documented commands exactly as written.

Required checks:

- Restore succeeds.
- Build succeeds.
- Unit tests pass.
- Analyzer/format checks pass if configured.
- No end-of-support framework warnings.
- No Windows-only API warnings in shared cross-platform code.

Expected result:

- All required commands exit 0.
- Any warnings are either fixed or documented as intentional and non-release-blocking.

## Packaging QA

### Windows

Required artifact:

- `TryFonts-windows-x64.exe`.

Checks:

- Artifact is one user-downloadable file.
- The file is self-contained and runs on a clean Windows machine without installing .NET.
- Double-click launch works.
- App name and icon are correct.
- File version/product metadata are reasonable.
- Signing status is documented.
- If unsigned, first-run SmartScreen behavior is documented in release notes.

### macOS

Required artifacts:

- `TryFonts-macos-arm64.dmg`.
- `TryFonts-macos-x64.dmg`, unless Intel support is explicitly dropped.

Checks:

- Each artifact is one user-downloadable file.
- The app launches from the dmg or after drag-install without installing .NET.
- App name and icon are correct.
- Signing/notarization status is documented.
- If unsigned or unnotarized, first-run Gatekeeper behavior is documented in release notes.

Expected result:

- A normal user can get from download to launched app without building anything or installing a runtime.

## Offline And Privacy QA

Perform this test on every supported platform.

Steps:

1. Download or copy the release artifact to the test machine.
2. Disconnect the machine from the internet.
3. Launch the app.
4. Use every primary feature: preview text, preset, font size, bold, italic, search, sort, scroll.
5. Leave the app open for 5 minutes.

Pass criteria:

- App launches offline.
- App remains usable offline.
- No error appears because internet is unavailable.
- No remote assets are missing.
- Network monitor/firewall logs show no attempted remote calls during normal use.

Fail criteria:

- Runtime download is required.
- Remote web asset is required.
- Telemetry, update check, analytics, or other network call occurs without a deliberate documented opt-in.

## Functional QA

### Font Discovery

Steps:

1. Launch app.
2. Record total discovered font count.
3. Confirm known system fonts appear.
4. Install or enable a test font if practical.
5. Relaunch app.
6. Confirm the test font appears.

Pass criteria:

- Font count is plausible for the machine.
- Known fonts appear.
- Duplicate family names are not shown as repeated indistinguishable rows.
- Discovery failures do not crash the app.

### Preview Rendering

Steps:

1. Enter custom text with letters, numbers, punctuation, and symbols.
2. Select each built-in preset.
3. Change font size from small to large.
4. Toggle bold and italic.
5. Scroll while styles are enabled.

Pass criteria:

- Visible rows update correctly.
- Font family names stay readable.
- Text does not overlap adjacent rows or controls.
- Missing style handling is visible and non-crashing.

### Search And Sort

Use a known set of installed font names where possible.

Checks:

- Empty search shows all fonts.
- `Contains` finds substrings case-insensitively.
- `Starts with` matches prefixes case-insensitively.
- Search debounce feels responsive.
- Sort A-Z and Z-A are deterministic.
- Clearing search restores the full list.

Pass criteria:

- Results match expected font names.
- UI does not freeze during rapid typing.

### Settings

Steps:

1. Change preview text to `Custom launch text`.
2. Change font size, bold/italic, search mode, sort mode, and window size.
3. Close app.
4. Relaunch app.

Pass criteria:

- Preview text resets to `*The quick brown fox jumps over 10 of the 2,345 lazy dogs @ the farm - starting with #6 && costing $7 (plus $0.89 tax?)!`.
- Expected non-text settings are restored.
- Bad or missing settings file does not crash the app.
- Settings are stored in a per-user app data location, not beside the executable.

Fail criteria:

- The app restarts with the prior custom preview text.
- The app persists a custom-text startup mode.

### License

Checks:

1. Confirm the repository license is GPL-3.0.
2. Confirm README and package metadata identify GPL-3.0.

Pass criteria:

- The license file is GPL-3.0.
- README and package metadata do not claim a different license.

## Performance QA

The performance target is not merely "acceptable"; the app should feel unusually fast for a font browser.

### Required Scenario

Evaluate with at least 5,000 font records. If the OS does not have that many installed fonts, use the documented synthetic large-list mode.

Measure:

- Time to first usable UI.
- Time until font discovery completes.
- Search latency after debounce.
- Scroll smoothness from top to bottom.
- UI thread stalls.
- Memory before scrolling, after full downward scroll, after returning to top, and after five repeated full scroll cycles.

Pass criteria:

- First usable UI under 1 second.
- Search results update under 150 ms after debounce.
- No normal scrolling interaction produces a UI freeze over 50 ms.
- Memory growth after five full scroll cycles is bounded and explainable.
- The app remains responsive while discovery or filtering runs.

Suggested tooling:

- Built-in app performance logging if provided.
- Windows Performance Recorder or Visual Studio profiler on Windows.
- Instruments on macOS.
- Manual screen recording at high scroll speed for visual review.

Fail criteria:

- The implementation manually creates thousands of row controls at once.
- Scrolling stutters badly with 5,000 records.
- Memory grows continuously after repeated scroll cycles.
- Search blocks the UI thread.

## Accessibility QA

Checks:

- Keyboard-only navigation reaches all controls.
- `Ctrl+F` or documented shortcut focuses search.
- Escape clears search when search is focused.
- Preview text field is reachable and editable by keyboard.
- Font list can be scrolled by keyboard.
- Controls have accessible names.
- Text remains legible at common OS scaling settings.

Pass criteria:

- Basic keyboard and screen-reader navigation works.
- No critical control is mouse-only.

## Visual QA

Check on Windows and macOS:

- Default window size.
- Narrow but supported window size.
- Large display.
- OS scaling at 100%, 150%, and 200% where available.
- Light and dark mode if supported.

Pass criteria:

- Controls do not overlap.
- Preview rows remain readable.
- Font names are not clipped in ordinary cases.
- The list occupies the majority of the app.
- The app looks like a focused desktop tool, not a marketing page.

## Gherkin Evaluation Scenarios

```gherkin
Feature: Clean clone build
  Scenario: Evaluator builds from tracked source
    Given a fresh clone of the repository
    When the evaluator runs the documented restore, build, and test commands
    Then all commands succeed
    And no generated Visual Studio or build output is required from the repo
```

```gherkin
Feature: Windows release artifact
  Scenario: User launches portable Windows executable
    Given a clean Windows machine without the .NET runtime installed
    And the TryFonts-windows-x64.exe artifact
    When the evaluator disconnects the machine from the internet
    And double-clicks the executable
    Then Try Fonts launches
    And the primary UI is usable
    And no internet connection is required
```

```gherkin
Feature: macOS release artifact
  Scenario: User launches macOS app from a single downloaded file
    Given a clean supported macOS machine
    And the matching Try Fonts dmg artifact
    When the evaluator disconnects the machine from the internet
    And opens the artifact
    And launches Try Fonts
    Then Try Fonts launches
    And the primary UI is usable
    And no internet connection is required
```

```gherkin
Feature: High-volume scrolling
  Scenario: Evaluator scrolls through 5000 font records
    Given Try Fonts has at least 5000 font records available
    When the evaluator scrolls rapidly from top to bottom five times
    Then the UI remains responsive
    And no scroll interaction freezes for more than 50 milliseconds
    And memory usage does not grow without bound
```

```gherkin
Feature: Search correctness
  Scenario: Evaluator compares contains and starts-with search
    Given fonts named "Arial", "Arial Rounded MT Bold", and "Georgia" are available
    When search mode is "Contains"
    And the evaluator searches for "round"
    Then only matching font families are shown
    When search mode is "Starts with"
    And the evaluator searches for "ari"
    Then font families beginning with "ari" are shown
```

```gherkin
Feature: Settings persistence
  Scenario: Evaluator verifies settings survive restart without preserving custom text
    Given the evaluator changes preview text, font size, style, search mode, and sort mode
    When the evaluator closes and reopens Try Fonts
    Then the preview text is the base sample string
    And the expected non-text settings are restored
    And the settings file is in the user's app data location
```

## Evaluation Report Template

The evaluator must produce a short report with:

- Commit SHA evaluated.
- Artifact names and hashes.
- Platforms tested.
- Build/test command results.
- Offline test result.
- Packaging result.
- Performance measurements.
- Functional pass/fail summary.
- Accessibility pass/fail summary.
- Blocking issues.
- Non-blocking issues.
- Final recommendation: `release`, `release with notes`, or `do not release`.

Artifact hash command on Windows:

```powershell
Get-FileHash -Algorithm SHA256 <artifact-path>
```
