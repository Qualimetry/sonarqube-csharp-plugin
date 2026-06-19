#requires -Version 5.1
<#
.SYNOPSIS
  Produce the analyzer NuGet package into artifacts/nuget. Does not push.
#>
[CmdletBinding()]
param(
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$out = Join-Path $root 'artifacts/nuget'
New-Item -ItemType Directory -Force -Path $out | Out-Null

$proj = Join-Path $root 'csharp-engine/Qualimetry.CSharp.Analyzer/Qualimetry.CSharp.Analyzer.csproj'
dotnet pack $proj -c $Configuration -o $out
if ($LASTEXITCODE -ne 0) { throw "dotnet pack failed ($LASTEXITCODE)" }

Get-ChildItem $out -Filter '*.nupkg' | ForEach-Object { Write-Host "Packed: $($_.FullName)" }
