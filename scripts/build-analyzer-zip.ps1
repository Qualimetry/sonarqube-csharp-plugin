#requires -Version 5.1
<#
.SYNOPSIS
  Rebuild the analyzer payload that the SonarQube plugin embeds and provisions to
  SonarScanner for .NET. Produces csharp-plugin/src/main/resources/static/Qualimetry.CSharp.Analyzer.zip
  with the NuGet analyzer layout (analyzers/dotnet/cs/*.dll + config/analyzers.globalconfig).
#>
[CmdletBinding()]
param(
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$engine = Join-Path $root 'csharp-engine'

dotnet build (Join-Path $engine 'Qualimetry.CSharp.Analyzer/Qualimetry.CSharp.Analyzer.csproj') -c $Configuration
dotnet build (Join-Path $engine 'Qualimetry.CSharp.Analyzer.CodeFixes/Qualimetry.CSharp.Analyzer.CodeFixes.csproj') -c $Configuration

$analyzerDll = Join-Path $engine "Qualimetry.CSharp.Analyzer/bin/$Configuration/netstandard2.0/Qualimetry.CSharp.Analyzer.dll"
$codeFixDll  = Join-Path $engine "Qualimetry.CSharp.Analyzer.CodeFixes/bin/$Configuration/netstandard2.0/Qualimetry.CSharp.Analyzer.CodeFixes.dll"
$globalConfig = Join-Path $engine 'Qualimetry.CSharp.Analyzer/config/analyzers.globalconfig'

# Forward-slash entry names are mandatory: the Java plugin and SonarScanner for .NET
# read the zip with java.util.zip, which does not normalise backslashes. Build entries
# explicitly rather than via ZipFile.CreateFromDirectory, which emits OS-native separators
# (backslashes on Windows PowerShell 5.1) and breaks extraction.
$entries = @(
    @{ Path = $analyzerDll;  Name = 'analyzers/dotnet/cs/Qualimetry.CSharp.Analyzer.dll' },
    @{ Path = $codeFixDll;   Name = 'analyzers/dotnet/cs/Qualimetry.CSharp.Analyzer.CodeFixes.dll' },
    @{ Path = $globalConfig; Name = 'config/analyzers.globalconfig' }
)

$target = Join-Path $root 'csharp-plugin/src/main/resources/static/Qualimetry.CSharp.Analyzer.zip'
New-Item -ItemType Directory -Force -Path (Split-Path -Parent $target) | Out-Null
if (Test-Path $target) { Remove-Item $target -Force }

Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem
$archive = [System.IO.Compression.ZipFile]::Open($target, [System.IO.Compression.ZipArchiveMode]::Create)
try {
    foreach ($entry in $entries) {
        [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($archive, $entry.Path, $entry.Name) | Out-Null
    }
} finally {
    $archive.Dispose()
}

Write-Host "Wrote $target"
