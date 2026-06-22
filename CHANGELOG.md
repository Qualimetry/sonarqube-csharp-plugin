# Changelog

## [Unreleased]

- (None.)

## [1.0.11] - 2026-06-23

- Renamed the rules repository to "Qualimetry" so SonarQube no longer shows the language label twice.

## [1.0.10] - 2026-06-23

- General improvements to analyzers.

## [1.0.8] - 2026-06-19

- Maintenance release to keep the C# plugin family aligned on a single version. No functional changes to the SonarQube plugin.

## [1.0.7] - 2026-06-18

- Added five Roslyn rules for namespace dependency cycles, static initialization cycles, duplicate types across assemblies, multiple assembly versions, and poor type cohesion. Extended readonly-array coverage to all accessibility levels (210 rules total).

## [1.0.6] - 2026-06-18

- Added six Roslyn rules for namespace/folder layout, type-in-namespace, boxing, and type/namespace name clash (205 rules total).

## [1.0.5] - 2026-06-18

- Configurable rules now expose native SonarQube rule parameters (editable in the UI and synced to the IDE in connected mode) instead of static documentation.

## [1.0.4] - 2026-06-17

- Fixed the plugin failing to load alongside the platform C# analyzer; analyzer rules are now imported under a dedicated rule namespace, and the plugin key is aligned to `qualimetry-csharp`.

## [1.0.3] - 2026-06-17

- Rule content (rules, descriptors, and rule documentation) is now generated from a single canonical source, with a build-time parity check that fails if any representation drifts.

## [1.0.2] - 2026-06-17

- Fix the SonarQube plugin failing to load on current SonarQube versions; quality profiles now use the supported quality-profile API.

## [1.0.1] - 2026-06-17

- Maintenance release to keep the C# plugin family aligned on a single version. No functional changes to the SonarQube plugin.

## [1.0.0] - 2026-06-17

First general release.

- 210 C# rules across Code Quality, Style, Metrics, Naming, Reliability and security, Contract, Interop, and Unity.
- Two built-in quality profiles, both non-default: **Qualimetry C#** (90 recommended rules) and **Qualimetry Way** (all 210).
- Analyzer embedded in the plugin and provisioned to SonarScanner for .NET; no per-project package reference required.
- Per-rule HTML documentation with original noncompliant and compliant examples.
- Rule keys, Roslyn diagnostic ids, and `.globalconfig` entries share the same `qa_<category>_<slug>` form across SonarQube, VS Code, and Rider.

## [0.1.0] - 2026-06-17

First release.

- 210 C# rules across Code Quality, Style, Metrics, Naming, Reliability and security, Contract, Interop, and Unity.
- Two built-in quality profiles, both non-default: **Qualimetry C#** (90 recommended rules) and **Qualimetry Way** (all 210).
- Analyzer embedded in the plugin and provisioned to SonarScanner for .NET; no per-project package reference required.
- Per-rule HTML documentation with original noncompliant and compliant examples.
- Rule keys, Roslyn diagnostic ids, and `.globalconfig` entries share the same `qa_<category>_<slug>` form across SonarQube, VS Code, and Rider.
