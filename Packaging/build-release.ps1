param([string]$PythonExe = "python")

$ErrorActionPreference = "Stop"
$solutionRoot = Split-Path -Parent $PSScriptRoot
$projectRoot = Join-Path $solutionRoot "MarkItDownDesktop"
$buildRoot = Join-Path $solutionRoot ".build"
$venvPython = Join-Path $buildRoot "markitdown-venv/Scripts/python.exe"
$cliDist = Join-Path $buildRoot "dist/markitdown"
$toolsDirectory = Join-Path $projectRoot "Tools"
$publishDirectory = Join-Path $buildRoot "publish"
$releaseDirectory = Join-Path $solutionRoot "Releases"

New-Item -ItemType Directory -Force $buildRoot, $releaseDirectory | Out-Null
if (-not (Test-Path $venvPython)) {
    & $PythonExe -m venv (Join-Path $buildRoot "markitdown-venv")
    if ($LASTEXITCODE -ne 0) { throw "Python virtual environment creation failed." }
}

& $venvPython -m pip install "markitdown[pdf,docx,pptx,xlsx,xls]==0.1.8" "pyinstaller==6.22.3"
if ($LASTEXITCODE -ne 0) { throw "Python dependencies could not be installed." }

$pyInstallerArgs = @(
    "-m", "PyInstaller", "--noconfirm", "--clean", "--onedir", "--console",
    "--name", "markitdown", "--collect-all", "markitdown", "--collect-all", "magika",
    "--collect-submodules", "onnxruntime", "--collect-all", "pypdfium2",
    "--distpath", (Join-Path $buildRoot "dist"),
    "--workpath", (Join-Path $buildRoot "pyinstaller"),
    "--specpath", $buildRoot,
    (Join-Path $PSScriptRoot "markitdown_runner.py")
)
& $venvPython @pyInstallerArgs
if ($LASTEXITCODE -ne 0) { throw "Bundled MarkItDown build failed." }
if (-not (Test-Path (Join-Path $cliDist "markitdown.exe"))) { throw "Bundled CLI executable is missing." }

if (Test-Path $toolsDirectory) {
    $resolvedTools = (Resolve-Path $toolsDirectory).Path
    if (-not $resolvedTools.StartsWith($projectRoot + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Tools target is outside the project."
    }
    Remove-Item -LiteralPath $toolsDirectory -Recurse -Force
}
Copy-Item -LiteralPath $cliDist -Destination $toolsDirectory -Recurse

$publishArgs = @(
    "publish", (Join-Path $projectRoot "MarkItDownDesktop.csproj"), "-c", "Release",
    "-p:Platform=x64", "-p:WindowsPackageType=None", "-p:WindowsAppSDKSelfContained=true",
    "-p:SelfContained=true", "-p:PublishTrimmed=false", "-p:PublishReadyToRun=false",
    "-r", "win-x64", "-o", $publishDirectory
)
dotnet @publishArgs
if ($LASTEXITCODE -ne 0) { throw "WinUI publish failed." }
Copy-Item -LiteralPath (Join-Path $PSScriptRoot "README-发行.txt") -Destination (Join-Path $publishDirectory "README-发行.txt")
Copy-Item -LiteralPath (Join-Path $PSScriptRoot "MARKITDOWN-LICENSE.txt") -Destination (Join-Path $publishDirectory "MARKITDOWN-LICENSE.txt")

$archive = Join-Path $releaseDirectory "MarkItDownDesktop-win-x64.zip"
if (Test-Path $archive) { Remove-Item -LiteralPath $archive }
Compress-Archive -Path (Join-Path $publishDirectory "*") -DestinationPath $archive
Write-Output $archive
