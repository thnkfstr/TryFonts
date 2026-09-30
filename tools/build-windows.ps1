#Requires -Version 5.1
<#
.SYNOPSIS
    Produces a self-contained, single-file Windows x64 EXE for Try Fonts.

.DESCRIPTION
    Restores, tests, then publishes TryFonts.App targeting win-x64.
    The output EXE is placed at TryFonts.exe in the repository root and as a
    versioned artifact in publish/.

.PARAMETER Version
    Version string to embed (default: the version in TryFonts.App.csproj).

.PARAMETER SkipTests
    Skip running unit tests.

.EXAMPLE
    .\tools\build-windows.ps1
    .\tools\build-windows.ps1 -Version 1.0.0 -SkipTests
#>

param(
    [string]$Version    = "",
    [switch]$SkipTests
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$root     = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $root "TryFonts.sln"
$project  = Join-Path $root "src\TryFonts.App\TryFonts.App.csproj"
$outDir   = Join-Path $root "publish\win-x64"
$rootExe  = Join-Path $root "TryFonts.exe"

if ([string]::IsNullOrWhiteSpace($Version)) {
    $Version = (dotnet msbuild $project -getProperty:Version).Trim()
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($Version)) {
        throw "Could not read the application version."
    }
}
$artifact = Join-Path $root "publish\TryFonts-windows-x64-$Version.exe"

Write-Host "==> Restore" -ForegroundColor Cyan
dotnet restore $solution
if ($LASTEXITCODE -ne 0) { throw "Restore failed ($LASTEXITCODE)." }

Write-Host "==> Build" -ForegroundColor Cyan
dotnet build $solution --no-restore --configuration Release
if ($LASTEXITCODE -ne 0) { throw "Build failed ($LASTEXITCODE)." }

if (-not $SkipTests) {
    Write-Host "==> Test" -ForegroundColor Cyan
    dotnet test $solution --no-build --configuration Release
    if ($LASTEXITCODE -ne 0) { throw "Tests failed ($LASTEXITCODE)." }
}

Write-Host "==> Publish (win-x64, single-file, self-contained, trimmed)" -ForegroundColor Cyan
dotnet publish $project `
    --configuration Release `
    --runtime win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -p:PublishTrimmed=true `
    -p:DebugType=embedded `
    -p:Version=$Version `
    --output $outDir
if ($LASTEXITCODE -ne 0) { throw "Publish failed ($LASTEXITCODE)." }

$exe = Join-Path $outDir "TryFonts.exe"
if (-not (Test-Path $exe)) {
    Write-Error "Expected output not found: $exe"
    exit 1
}

Copy-Item $exe $artifact -Force
Copy-Item $exe $rootExe -Force
Write-Host ""
Write-Host "==> Done: $artifact" -ForegroundColor Green
Write-Host "    Size: $([Math]::Round((Get-Item $artifact).Length / 1MB, 1)) MB"
Write-Host "    Shortcut target: $rootExe"
