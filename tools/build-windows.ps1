#Requires -Version 5.1
<#
.SYNOPSIS
    Produces a self-contained, single-file Windows x64 EXE for Try Fonts.

.DESCRIPTION
    Restores, tests, then publishes TryFonts.App targeting win-x64.
    The output EXE is placed in publish/ at the repo root.

.PARAMETER Version
    Version string to embed (default: 0.1.0-local).

.PARAMETER SkipTests
    Skip running unit tests.

.EXAMPLE
    .\tools\build-windows.ps1
    .\tools\build-windows.ps1 -Version 1.0.0 -SkipTests
#>

param(
    [string]$Version    = "0.1.0-local",
    [switch]$SkipTests
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$root     = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $root "TryFonts.sln"
$project  = Join-Path $root "src\TryFonts.App\TryFonts.App.csproj"
$outDir   = Join-Path $root "publish\win-x64"
$artifact = Join-Path $root "publish\TryFonts-windows-x64-$Version.exe"

Write-Host "==> Restore" -ForegroundColor Cyan
dotnet restore $solution

Write-Host "==> Build" -ForegroundColor Cyan
dotnet build $solution --no-restore --configuration Release

if (-not $SkipTests) {
    Write-Host "==> Test" -ForegroundColor Cyan
    dotnet test $solution --no-build --configuration Release
}

Write-Host "==> Publish (win-x64, single-file, self-contained)" -ForegroundColor Cyan
dotnet publish $project `
    --configuration Release `
    --runtime win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -p:DebugType=embedded `
    -p:Version=$Version `
    --output $outDir

$exe = Join-Path $outDir "TryFonts.exe"
if (-not (Test-Path $exe)) {
    Write-Error "Expected output not found: $exe"
    exit 1
}

Copy-Item $exe $artifact -Force
Write-Host ""
Write-Host "==> Done: $artifact" -ForegroundColor Green
Write-Host "    Size: $([Math]::Round((Get-Item $artifact).Length / 1MB, 1)) MB"
