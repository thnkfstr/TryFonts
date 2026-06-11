# Try Fonts — Manual Smoke Test Checklist

Run this checklist on both Windows and macOS before tagging a release.
Mark each item ✅ pass, ❌ fail (with notes), or ⏭ skipped with reason.

---

## Environment setup

- [ ] Machine has no internet connection during this test run
- [ ] Record: OS version, machine arch, number of installed fonts
- [ ] Record: App version / build date / RID being tested

---

## 1. Launch

| # | Step | Expected | Result |
|---|------|----------|--------|
| 1.1 | Double-click the release artifact (`.exe` on Windows, open `.dmg` and launch on macOS) | App opens without installing .NET or any other dependency | |
| 1.2 | Watch the toolbar during launch | UI is usable within 1 second; font list may still populate | |
| 1.3 | Wait for font list to fully load | Font count display shows total discovered fonts | |
| 1.4 | Check network monitor (e.g., Activity Monitor / Resource Monitor) | Zero outbound connections from TryFonts.exe | |

---

## 2. Preview text

| # | Step | Expected | Result |
|---|------|----------|--------|
| 2.1 | Read the preview text field on fresh launch | Contains the base sample string starting with `*The quick brown fox…` | |
| 2.2 | Type "Sphinx of black quartz" into the preview text field | All visible font rows update to show that text | |
| 2.3 | Verify font names remain visible in each row | Names in the right-hand column are not overwritten | |
| 2.4 | Open the Preset dropdown | At least 6 preset names appear | |
| 2.5 | Select "Alphabet & digits" preset | Preview text changes to the alphabet/digit string | |
| 2.6 | Close and reopen the app | Preview text resets to the base sample, not "Sphinx…" | |

---

## 3. Font list / virtualization

| # | Step | Expected | Result |
|---|------|----------|--------|
| 3.1 | Scroll rapidly from top to bottom of the list using the mouse wheel | Scrolling remains smooth; no visible freeze | |
| 3.2 | Scroll back to the top | List still shows correct fonts; no duplicates or gaps | |
| 3.3 | Open Task Manager / Activity Monitor during fast scrolling | Memory usage is stable (no continuous growth) | |
| 3.4 | (Optional) Launch with `--synthetic-fonts 5000` | Font count increases by 5,000; scrolling remains smooth | |

---

## 4. Search and filtering

| # | Step | Expected | Result |
|---|------|----------|--------|
| 4.1 | Press `/` or `Ctrl+F` | Focus moves to search box | |
| 4.2 | Type a font name that exists (e.g., "Arial" on Windows) | Matching fonts appear within ~150 ms | |
| 4.3 | Verify Contains mode: search "ial" | "Arial" and any font containing "ial" are shown | |
| 4.4 | Switch mode to "Starts with", search "ari" | Only fonts starting with "ari" shown | |
| 4.5 | Search is case-insensitive | "arial", "ARIAL", "ArIaL" all match the same fonts | |
| 4.6 | Clear search with `Esc` | All fonts return; search box empties | |
| 4.7 | Empty search shows all fonts | Font count returns to total | |
| 4.8 | Verify preview text is unchanged after searching | Text in rows is still what the user typed | |

---

## 5. Controls

| # | Step | Expected | Result |
|---|------|----------|--------|
| 5.1 | Increase font size to 48 | All visible rows resize; no freeze | |
| 5.2 | Decrease font size to 12 | Rows shrink; list remains scrollable | |
| 5.3 | Enable Bold toggle | Visible rows attempt bold; rows without bold are marked | |
| 5.4 | Enable Italic toggle | Visible rows attempt italic or bold-italic | |
| 5.5 | Disable both Bold and Italic | Rows return to regular rendering | |
| 5.6 | Change sort to "Name Z–A" | List reverses; last alphabetical font is now first | |
| 5.7 | Press `Ctrl+L` | Focus moves to preview text field; text is selected | |

---

## 6. Settings persistence

| # | Step | Expected | Result |
|---|------|----------|--------|
| 6.1 | Set font size to 32, enable Bold, set sort to Z–A | Note settings | |
| 6.2 | Close and reopen the app | Font size is 32, Bold is enabled, sort is Z–A | |
| 6.3 | Preview text is the base sample (not any custom text from last session) | ✓ Confirmed | |
| 6.4 | Resize the window; close and reopen | Window restores to approximately the same size | |

---

## 7. Style availability indicators

| # | Step | Expected | Result |
|---|------|----------|--------|
| 7.1 | Find a font known to have all four styles (e.g., Arial on Windows) | R, B, I, BI labels are all fully opaque | |
| 7.2 | Find a font with only Regular (e.g., a symbol font) | B, I, BI labels are dimmed (≈25% opacity) | |
| 7.3 | Enable Bold for a font that lacks it | Row renders in closest available style; BI label is dim | |

---

## 8. Accessibility / resizing

| # | Step | Expected | Result |
|---|------|----------|--------|
| 8.1 | Resize the window to its minimum size | Controls wrap or compress; no clipping of essential controls | |
| 8.2 | Resize to full screen | Font list expands to fill the space | |
| 8.3 | Set OS display scaling to 150% and relaunch (Windows) | App renders crisply; text is not blurry | |

---

## Notes

Record any failures, unexpected behaviors, or observations here.

```
OS:
Version:
Arch:
Installed fonts:
Build:
Notes:
```
