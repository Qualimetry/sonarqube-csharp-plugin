# Qualimetry C#

Roslyn analyzers that enforce the Qualimetry C# code-quality rule set: naming, style, design,
reliability, and maintainability conventions, with code fixes where a safe correction exists.

Referencing this package adds the analyzers to your build and IDE. The recommended rule set is enabled
by default through a shipped `.globalconfig`; the full set can be turned on per rule in your own
`.editorconfig` or `.globalconfig`.

Rule ids are `QCS####`. The same ids are used by the Qualimetry SonarQube plugin, so issues line up
across your IDE, your build, and your SonarQube server.
