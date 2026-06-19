# Qualimetry C# - SonarQube plugin

Server-side SonarQube plugin that registers the Qualimetry C# rule set (210 rules, ids `QCS####`)
and two built-in quality profiles, and provisions the Qualimetry Roslyn analyzer into .NET builds.

## Requirement: SonarScanner for .NET

This plugin analyses C# through **SonarScanner for .NET** (the `dotnet-sonarscanner` / MSBuild
scanner). A language-agnostic scanner (`sonar-scanner` CLI) will **not** run the rules - the
Qualimetry rules are Roslyn analyzers that must execute inside the MSBuild compilation.

The plugin **embeds** the analyzer payload (`Qualimetry.CSharp.Analyzer.zip`, containing the
analyzer and code-fix assemblies plus the shipped `.globalconfig`) as a static resource, and
**provisions** it to the scanner. When you run an analysis against a profile from this plugin,
SonarScanner for .NET downloads the analyzer from the server and injects it into the build
automatically (it passes the assemblies to the Roslyn compilation via `/analyzer:`), with **no
project-level `PackageReference`** and **no special scanner build** required.

Provisioning uses the contract SonarScanner for .NET defines for third-party Roslyn analyzers, so a
stock `dotnet sonarscanner` works unchanged. The rules live in a repository keyed
`roslyn.qualimetry-csharp`; the scanner provisions any `roslyn.*` repository by reading the matching
`qualimetry-csharp.*` server settings to locate and download the static payload:
`qualimetry-csharp.pluginKey` = `qualimetrycsharp`, `qualimetry-csharp.pluginVersion` = the plugin
version, `qualimetry-csharp.staticResourceName` = `Qualimetry.CSharp.Analyzer.zip`, plus
`qualimetry-csharp.analyzerId` / `ruleNamespace` / `nuget.packageId` / `nuget.packageVersion`.

These settings stay within the plugin's own namespace and never touch the reserved
`sonaranalyzer-cs.*` settings owned by the platform's bundled C# analyzer.

The scanner downloads the payload from `static/qualimetrycsharp/Qualimetry.CSharp.Analyzer.zip`,
extracts the analyzer assemblies, and runs only the rules active in the selected quality profile.

Typical run:

```
dotnet sonarscanner begin /k:"<project-key>" /d:sonar.host.url="<server>" /d:sonar.token="<token>"
dotnet build
dotnet sonarscanner end /d:sonar.token="<token>"
```

## Quality profiles

Two built-in profiles are registered for language C#. **Neither is set as the default** for the
language (`setDefault(false)`); a project must opt in by selecting one.

| Profile | Contents |
|---|---|
| `Qualimetry C#` | Recommended set - the rules marked for default activation. |
| `Qualimetry Way` | The full set - all 210 rules active. |

## Build

```
mvn -f csharp-plugin/pom.xml clean package
```

The build copies `csharp-rules/rules.json` and the per-rule HTML into the plugin resources and
produces `target/qualimetry-csharp-plugin-<version>.jar`. Drop the jar into
`<sonarqube>/extensions/plugins/` and restart the server.

The embedded `Qualimetry.CSharp.Analyzer.zip` is regenerated from the freshly built analyzer
output by `scripts/build-analyzer-zip.ps1` (run as part of `scripts/package-plugin.ps1`).
