param([string]$InnoCompiler)

$ErrorActionPreference = "Stop"
$solutionRoot = Split-Path -Parent $PSScriptRoot
$publishDirectory = Join-Path $solutionRoot ".build/publish"
$releaseDirectory = Join-Path $solutionRoot "Releases"
$scriptPath = Join-Path $PSScriptRoot "MarkItDownDesktop.iss"

if (-not (Test-Path (Join-Path $publishDirectory "MarkItDownDesktop.exe"))) {
    throw "Release output is missing. Run build-release.ps1 first."
}
if (-not (Test-Path (Join-Path $publishDirectory "Tools/markitdown.exe"))) {
    throw "Bundled MarkItDown is missing. Run build-release.ps1 first."
}
if (-not $InnoCompiler) {
    $candidatePaths = @(
        (Join-Path $solutionRoot ".build/InnoSetup7/ISCC.exe"),
        (Join-Path $env:ProgramFiles "Inno Setup 7/ISCC.exe"),
        (Join-Path $env:ProgramFiles "Inno Setup 6/ISCC.exe")
    )
    $InnoCompiler = $candidatePaths | Where-Object { Test-Path $_ } | Select-Object -First 1
}
if (-not $InnoCompiler -or -not (Test-Path $InnoCompiler)) {
    throw "Inno Setup compiler not found. Pass its ISCC.exe path with -InnoCompiler."
}

New-Item -ItemType Directory -Force $releaseDirectory | Out-Null
& $InnoCompiler $scriptPath
if ($LASTEXITCODE -ne 0) { throw "Installer compilation failed." }
$installer = Join-Path $releaseDirectory "MarkItDownDesktop-Setup-win-x64.exe"
if (-not (Test-Path $installer)) { throw "Installer output is missing." }
Write-Output $installer
