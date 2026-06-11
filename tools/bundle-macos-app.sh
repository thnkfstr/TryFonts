#!/usr/bin/env bash
# bundle-macos-app.sh — Wraps a published .NET binary in a macOS .app bundle.
#
# Usage: tools/bundle-macos-app.sh <binary-path> <rid>
#   binary-path : path to the published TryFonts binary (no extension)
#   rid         : osx-arm64 or osx-x64
#
# Produces: publish/TryFonts.app (overwrites if present)

set -euo pipefail

BINARY="${1:?binary-path required}"
RID="${2:?rid required}"
APP="publish/TryFonts.app"
CONTENTS="$APP/Contents"
MACOS="$CONTENTS/MacOS"
RESOURCES="$CONTENTS/Resources"
VERSION="${VERSION:-0.1.0}"

echo "Bundling $BINARY into $APP ($RID)…"

# Recreate the bundle directory
rm -rf "$APP"
mkdir -p "$MACOS" "$RESOURCES"

# Copy the binary
cp "$BINARY" "$MACOS/TryFonts"
chmod +x "$MACOS/TryFonts"

# Copy the icon (convert .ico → .icns if iconutil is available, otherwise skip)
if [ -f "src/TryFonts.App/Assets/TryFonts.ico" ]; then
  if command -v sips &>/dev/null && command -v iconutil &>/dev/null; then
    # macOS: convert .ico to .icns via intermediary .iconset
    ICONSET=$(mktemp -d)/TryFonts.iconset
    mkdir "$ICONSET"
    sips -z 512 512 src/TryFonts.App/Assets/TryFonts.ico \
         --out "$ICONSET/icon_512x512.png" 2>/dev/null || true
    sips -z 256 256 src/TryFonts.App/Assets/TryFonts.ico \
         --out "$ICONSET/icon_256x256.png" 2>/dev/null || true
    sips -z 128 128 src/TryFonts.App/Assets/TryFonts.ico \
         --out "$ICONSET/icon_128x128.png" 2>/dev/null || true
    sips -z 64  64  src/TryFonts.App/Assets/TryFonts.ico \
         --out "$ICONSET/icon_64x64.png"   2>/dev/null || true
    sips -z 32  32  src/TryFonts.App/Assets/TryFonts.ico \
         --out "$ICONSET/icon_32x32.png"   2>/dev/null || true
    sips -z 16  16  src/TryFonts.App/Assets/TryFonts.ico \
         --out "$ICONSET/icon_16x16.png"   2>/dev/null || true
    iconutil -c icns "$ICONSET" --output "$RESOURCES/TryFonts.icns" 2>/dev/null || true
  fi
fi

# Write Info.plist
cat > "$CONTENTS/Info.plist" <<PLIST
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN"
  "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleDisplayName</key>     <string>Try Fonts</string>
    <key>CFBundleExecutable</key>      <string>TryFonts</string>
    <key>CFBundleIdentifier</key>      <string>com.benadams.tryfonts</string>
    <key>CFBundleName</key>            <string>TryFonts</string>
    <key>CFBundlePackageType</key>     <string>APPL</string>
    <key>CFBundleShortVersionString</key> <string>${VERSION}</string>
    <key>CFBundleVersion</key>         <string>${VERSION}</string>
    <key>CFBundleIconFile</key>        <string>TryFonts</string>
    <key>LSMinimumSystemVersion</key>  <string>11.0</string>
    <key>NSHighResolutionCapable</key> <true/>
    <key>NSHumanReadableCopyright</key>
        <string>Copyright © Ben Adams. GPL-3.0.</string>
    <key>LSApplicationCategoryType</key>
        <string>public.app-category.graphics-design</string>
</dict>
</plist>
PLIST

echo "Bundle created: $APP"
