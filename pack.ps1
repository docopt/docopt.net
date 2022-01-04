#Requires -PSEdition Core

[CmdletBinding()]
param (
    [string]$Configuration = 'Release',
    [string]$VersionSuffix,
    [string]$PackageReleaseNotesFile,
    [switch]$NoBuild)

$ErrorActionPreference = 'Stop'

try
{
    Push-Location $PSScriptRoot
    $packArgs = @()
    if ($noBuild) {
        $packArgs += '--no-build'
    }
    if ($versionSuffix) {
        $packArgs += @('--version-suffix', $versionSuffix)
    }
    if ($packageReleaseNotesFile) {
        $packArgs += "-p:PackageReleaseNotesFile=$packageReleaseNotesFile"
    }
    $tempFile = New-TemporaryFile
    Remove-Item $tempFile
    $tempDir = New-Item $tempFile -ItemType Directory
    dotnet pack -c $configuration -o $tempDir @packArgs
    if ($LASTEXITCODE) { throw }
    $pkgDirs =
        Get-ChildItem -File -Filter *.nupkg $tempDir |
            ForEach-Object {
                $destDir = Join-Path $tempDir (Split-Path $_ -LeafBase)
                Expand-Archive $_ -Destination $destDir
                Get-Item $destDir
            }
    $codeGenPkgDir = $pkgDirs |
        Where-Object { $_.Name -like 'DocoptNet.CodeGeneration.*' }
    $basePkgDir = $pkgDirs |
        Where-Object { $_.Name -like 'docopt.net.*' }
    if (!$codeGenPkgDir) { throw "No code generation package found." }
    if (!$basePkgDir) { throw "No base package found." }
    Get-ChildItem -Recurse (Join-Path $codeGenPkgDir build* * *.targets) |
        Rename-Item -NewName 'docopt.net.targets'
    Copy-Item -Recurse (Join-Path $codeGenPkgDir analyzers) $basePkgDir
    Copy-Item -Recurse (Join-Path $codeGenPkgDir build*) $basePkgDir
    $basePkgPath = "$($basePkgDir.Name).nupkg"
    Get-ChildItem $basePkgDir | Compress-Archive -DestinationPath $basePkgPath -Force
    if (!(Test-Path -PathType Container dist)) {
        New-Item -ItemType Container -Path dist | Out-Null
    }
    Move-Item $basePkgPath dist -Force
}
finally
{
    Remove-Item $tempDir -Recurse -Force
    Pop-Location
}
