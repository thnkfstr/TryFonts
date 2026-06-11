#!/usr/bin/env bash
# build-macos.sh — Produces self-contained DMGs for macOS arm64 and x64.
#
# Usage: tools/build-macos.sh [version] [arch]
#   version : version string (default: 0.1.0-local)
#   arch    : arm64 | x64 | both (default: both)
#
# Requires: dotnet 8+, hdiutil (macOS built-in)

set -euo pipefail

VERSION="${1:-0.1.0-local}"
ARCH="${2:-both}"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
PROJECT="$ROOT/src/TryFonts.App/TryFonts.App.csproj"
SOLUTION="$ROOT/TryFonts.sln"

build_arch() {
  local RID="$1"
  local LABEL="${RID#osx-}"  # arm64 or x64
  local OUT="$ROOT/publish/$RID"
  local DMG="$ROOT/publish/TryFonts-macos-$LABEL-$VERSION.dmg"

  echo ""
  echo "==> Publish ($RID, single-file, self-contained)"
  dotnet publish "$PROJECT" \
    --configuration Release \
    --runtime "$RID" \
    --self-contained true \
    -p:PublishSingleFile=true \
    -p:EnableCompressionInSingleFile=true \
    -p:PublishTrimmed=true \
    -p:DebugType=embedded \
    -p:Version="$VERSION" \
    --output "$OUT"

  echo "==> Bundle .app ($RID)"
  # Invoke via bash: the script's executable bit is not set in the git index,
  # so direct execution fails on a fresh checkout.
  VERSION="$VERSION" bash "$ROOT/tools/bundle-macos-app.sh" "$OUT/TryFonts" "$RID"

  echo "==> Create DMG"
  hdiutil create \
    -volname "Try Fonts" \
    -srcfolder "$ROOT/publish/TryFonts.app" \
    -ov -format UDZO \
    "$DMG"

  echo "==> Done: $DMG"
  echo "    Size: $(du -sh "$DMG" | cut -f1)"

  # Clean up the .app so the next arch gets a fresh bundle
  rm -rf "$ROOT/publish/TryFonts.app"
}

echo "==> Restore"
dotnet restore "$SOLUTION"

echo "==> Build"
dotnet build "$SOLUTION" --no-restore --configuration Release

echo "==> Test"
dotnet test "$SOLUTION" --no-build --configuration Release

mkdir -p "$ROOT/publish"

case "$ARCH" in
  arm64) build_arch "osx-arm64" ;;
  x64)   build_arch "osx-x64"   ;;
  both)  build_arch "osx-arm64"; build_arch "osx-x64" ;;
  *)
    echo "Unknown arch: $ARCH (use arm64, x64, or both)"
    exit 1
    ;;
esac
