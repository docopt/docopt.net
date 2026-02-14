#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Build, test, and pack DocoptNet with multi-Roslyn variant support.

.DESCRIPTION
    This script orchestrates builds across Roslyn 3.10 (baseline) and Roslyn 4.4 variants,
    runs tests for both variants, and produces a NuGet package containing both analyzer DLLs.

.PARAMETER Build
    Build the solution (default parameter set).

.PARAMETER Test
    Run tests for both Roslyn variants.

.PARAMETER Pack
    Create a NuGet package containing both analyzer variants.

.PARAMETER Configuration
    The build configuration (default: Release).

.PARAMETER NoBuild
    Skip the build step when running tests (only applies to baseline tests).

.PARAMETER VersionSuffix
    Optional version suffix for the NuGet package (e.g., "beta1").

.PARAMETER PackageReleaseNotesFile
    Optional path to a file containing release notes for the NuGet package.

.EXAMPLE
    ./build.ps1
    Build both Roslyn variants (baseline + 4.4).

.EXAMPLE
    ./build.ps1 -Test
    Build and test both Roslyn variants.

.EXAMPLE
    ./build.ps1 -Test -NoBuild
    Run tests without rebuilding baseline (Roslyn 4.4 tests always build as needed).

.EXAMPLE
    ./build.ps1 -Pack -VersionSuffix "beta1"
    Build and pack with version suffix.

.EXAMPLE
    ./build.ps1 -Pack -PackageReleaseNotesFile "/path/to/notes.txt"
    Build and pack with release notes from a file.
#>

[CmdletBinding(DefaultParameterSetName = 'Build')]
param(
    [Parameter(ParameterSetName = 'Build')]
    [switch] $Build,

    [Parameter(ParameterSetName = 'Test', Mandatory)]
    [switch] $Test,

    [Parameter(ParameterSetName = 'Pack', Mandatory)]
    [switch] $Pack,

    [Parameter(ParameterSetName = 'Build')]
    [Parameter(ParameterSetName = 'Test')]
    [Parameter(ParameterSetName = 'Pack')]
    [string] $Configuration = 'Release',

    [Parameter(ParameterSetName = 'Test')]
    [switch] $NoBuild,

    [Parameter(ParameterSetName = 'Pack')]
    [string] $VersionSuffix,

    [Parameter(ParameterSetName = 'Pack')]
    [string] $PackageReleaseNotesFile
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

# Make the script directory-independent
Push-Location $PSScriptRoot
try {
    function Invoke-DotNet {
        param(
            [string[]] $Arguments
        )

        Write-Host "dotnet $($Arguments -join ' ')" -ForegroundColor Cyan
        & dotnet @Arguments
        if ($LASTEXITCODE -ne 0) {
            throw "dotnet command failed with exit code $LASTEXITCODE"
        }
    }

    function Invoke-BuildFlow {
        Write-Host "`n=== Building Baseline (Roslyn 3.10) ===" -ForegroundColor Green
        Invoke-DotNet 'build', '--configuration', $Configuration

        Write-Host "`n=== Building Roslyn 4.4 Variant ===" -ForegroundColor Green
        Invoke-DotNet 'build', 'src/DocoptNet/DocoptNet.csproj', '-f', 'netstandard2.0', '-p:RoslynVersion=4.4', '--configuration', $Configuration
    }

    function Invoke-TestFlow {
        if (-not $NoBuild) {
            Invoke-BuildFlow
        }

        Write-Host "`n=== Running Tests ===" -ForegroundColor Green
        Invoke-DotNet 'test', '--no-build', '--configuration', $Configuration

        # Note: Roslyn 4.4 analyzer is validated through integration tests
        # that use the packed NuGet package containing both analyzer variants
    }

    function Invoke-PackFlow {
        Invoke-BuildFlow

        Write-Host "`n=== Packing NuGet Package ===" -ForegroundColor Green
        $packArgs = @('pack', 'src/DocoptNet/DocoptNet.csproj', '--no-build', '--configuration', $Configuration)
        if ($VersionSuffix) {
            $packArgs += @('--version-suffix', $VersionSuffix)
        }
        if ($PackageReleaseNotesFile) {
            $packArgs += "-p:PackageReleaseNotesFile=$PackageReleaseNotesFile"
        }
        Invoke-DotNet $packArgs
    }

    # Execute the appropriate flow based on parameter set
    switch ($PSCmdlet.ParameterSetName) {
        'Build' {
            Invoke-BuildFlow
        }
        'Test' {
            Invoke-TestFlow
        }
        'Pack' {
            Invoke-PackFlow
        }
    }

    Write-Host "`n=== Success ===" -ForegroundColor Green
}
finally {
    Pop-Location
}
