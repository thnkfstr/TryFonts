# Code Signing Plan

Try Fonts release artifacts are currently **unsigned**. This document describes what
signing would add and how to enable it once certificates are available.

---

## Windows — Authenticode signing

**Effect:** Eliminates SmartScreen "Unknown publisher" warning on first run.

**Requirement:** An Authenticode code-signing certificate from a public CA
(e.g., DigiCert, Sectigo) or an EV certificate.

**How to enable:**

1. Obtain a certificate. Export it as a password-protected `.pfx` file.
2. Add two repository secrets in GitHub → Settings → Secrets:
   - `SIGNING_CERT_BASE64` — base64 of the `.pfx` file
   - `SIGNING_CERT_PASSWORD` — the `.pfx` password
3. In `.github/workflows/release.yml`, uncomment the "Sign EXE (Authenticode)" step.

The signing command used is `signtool.exe` (available on all `windows-latest` runners).

---

## macOS — Apple Developer ID + Notarization

**Effect:** Eliminates Gatekeeper "unidentified developer" prompt on first run.

**Requirement:**
- An Apple Developer account ($99/year).
- A "Developer ID Application" certificate.

**How to enable:**

1. Export the certificate chain as a `.p12` file.
2. Add repository secrets:
   - `APPLE_CERT_BASE64` — base64 of the `.p12`
   - `APPLE_CERT_PASSWORD` — the `.p12` password
   - `APPLE_TEAM_ID` — your 10-character team ID
   - `APPLE_NOTARY_USER` — Apple ID used for notarization
   - `APPLE_NOTARY_PASSWORD` — an app-specific password
3. Uncomment the "Sign and notarize" step in the release workflow.

The signing command is `codesign`; notarization uses `xcrun notarytool`.

---

## Current first-run workarounds (unsigned builds)

**Windows:** When SmartScreen appears, click "More info" → "Run anyway."

**macOS:** Right-click (or Control-click) the app → "Open" → "Open" in the dialog.

These workarounds are needed once per machine. After the first successful launch
macOS and Windows remember the user's choice.
