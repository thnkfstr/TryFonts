# Legacy Implementations

This directory preserves the two original WinForms versions of Try Fonts for historical reference.

| Directory | Description |
|-----------|-------------|
| `Original/` | First WinForms implementation (flat project structure, `net6.0-windows`) |
| `New/` | Second WinForms iteration with lazy-load scroll and debounced search |

**These are reference only.** The active codebase is in `src/` and `tests/`. Do not add features here.

Known issues in both legacy versions (addressed in the rebuild):
- Windows-only (`net6.0-windows`, WinForms, `System.Drawing`)
- Project files excluded from git by the old root `.gitignore`
- Generated `.vs/`, `bin/`, `obj/` mixed into the source tree
- Font objects disposed while still assigned to labels
- List rebuilt from scratch on every search/size/style change (no real virtualization)
- Temporary signing key (`*.pfx`) committed to source
