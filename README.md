# Qualimetry C# - SonarQube Plugin

[![CI](https://github.com/Qualimetry/sonarqube-csharp-plugin/actions/workflows/ci.yml/badge.svg)](https://github.com/Qualimetry/sonarqube-csharp-plugin/actions/workflows/ci.yml)

A SonarQube plugin that adds Roslyn-based C# rules and two quality profiles to your SonarQube server, and provisions the analysis engine to SonarScanner for .NET as part of your CI quality gate.

Powered by the same analysis engine as the [Qualimetry C# extension for VS Code](https://github.com/Qualimetry/vscode-csharp-plugin) and the [Qualimetry C# plugin for Rider](https://github.com/Qualimetry/rider-csharp-plugin).

## Requirements

This plugin analyses C# through **SonarScanner for .NET** (the MSBuild begin/end scanner). It does not analyse plain-text C# files on its own. A scan must run as:

```bash
dotnet sonarscanner begin /k:"<project-key>" /d:sonar.host.url="<server>" /d:sonar.token="<token>"
dotnet build
dotnet sonarscanner end /d:sonar.token="<token>"
```

The plugin ships the analyzer inside the JAR and serves it to the scanner automatically, so no per-project `PackageReference` is needed.

## Features

- **210 C# rules** across eight categories - see the breakdown below.
- **Two quality profiles**, both non-default so they never silently replace your active profile.
- **Consistent rule keys** - the SonarQube rule key, the Roslyn diagnostic id, and the `.globalconfig` entry are the same `qa_<category>_<slug>` string, so server findings match editor findings exactly.
- **Per-rule HTML** with original, side-by-side noncompliant and compliant examples.

## Rule set

| Category | Rules |
| --- | ---: |
| Code Quality | 109 |
| Style | 45 |
| Metrics | 17 |
| Naming | 16 |
| Reliability | 10 |
| Unity | 8 |
| Contract | 3 |
| Interop | 2 |
| **Total** | **210** |

## Quality profiles

- **Qualimetry C#** - the recommended set.
- **Qualimetry Way** - all rules enabled, for teams that want full coverage.

Customise rules (enable/disable, severity, parameters) in SonarQube under **Quality Profiles**. Rules use the repository key `qualimetry-csharp`.

## Compatibility

- **SonarQube 10.x or later.**
- **SonarScanner for .NET** for the analysed build.

## Installation

1. Download the latest `qualimetry-csharp-plugin-<version>.jar` from [Releases](https://github.com/Qualimetry/sonarqube-csharp-plugin/releases).
2. Place the JAR in `SONARQUBE_HOME/extensions/plugins/`.
3. Restart SonarQube.
4. In **Quality Profiles**, select **Qualimetry C#** (or **Qualimetry Way**) for the C# language.

## Also available

- **[VS Code extension](https://github.com/Qualimetry/vscode-csharp-plugin)** - real-time analysis as you type.
- **[Rider plugin](https://github.com/Qualimetry/rider-csharp-plugin)** - the same inside JetBrains Rider.

Rule keys and severities align across all three tools so findings are directly comparable.

## Building from source

Requires JDK 17+, Maven 3.6+, and the .NET 10 SDK (to build and embed the analyzer).

```bash
mvn clean verify
```

The packaged plugin JAR is at `csharp-plugin/target/qualimetry-csharp-plugin-<version>.jar`.

## Contributing

Issues and feature requests are welcome. This project does not accept pull requests, commits, or other code contributions from third parties; the repository is maintained by the Qualimetry team only.

## License

Licensed under the [Apache License, Version 2.0](https://www.apache.org/licenses/LICENSE-2.0).

Copyright 2026 SHAZAM Analytics Ltd
