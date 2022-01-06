#Requires -PSEdition Core

[CmdletBinding()]
param (
    [string]$Configuration = 'Release',
    [switch]$NoPack)

$ErrorActionPreference = 'Stop'

Push-Location $PSScriptRoot

try
{
    $props = New-Object psobject
    dotnet msbuild $PSScriptRoot -t:Inspect -noLogo -v:m |
        ForEach-Object {
            Write-Verbose "Inspect: $_"
            $tokens = $_.Trim() -split '=', 2
            Add-Member -InputObject $props -MemberType NoteProperty -Name $tokens[0] -Value $tokens[1]
        }
    if ($LASTEXITCODE) { throw }
    if (!$props.RestorePackagesPath) {
        throw '"RestorePackagesPath" is not set.'
    }
    Remove-Item -Recurse -Force (Join-Path $props.RestorePackagesPath docopt.net) -ErrorAction SilentlyContinue
    if (!$noPack) {
        dotnet pwsh -NoProfile -File ../../pack.ps1 -Configuration Release
        if ($LASTEXITCODE) { throw }
    }
    Remove-Item bin, obj -Recurse -Force
    dotnet test -c $configuration
    if ($LASTEXITCODE) { throw }
}
finally
{
    Pop-Location
}
