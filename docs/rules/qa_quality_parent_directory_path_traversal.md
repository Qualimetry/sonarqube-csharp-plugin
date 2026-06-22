# Hard-coded paths should not climb above the working directory

`qa_quality_parent_directory_path_traversal` &middot; CodeQuality &middot; Code Smell &middot; severity CRITICAL &middot; enabled in the recommended profile

A literal file path that walks up the tree with `..` segments assumes a fixed layout outside the location the program controls. It breaks the moment the code runs from a different working directory, is deployed, or is packaged, and it can let input steer access to unintended locations. Resolve the base location from configuration or a well-defined root and build paths beneath it rather than reaching outside with parent-directory traversal.

## Noncompliant code example

```csharp
using System.IO;

public sealed class Loader
{
    public string Read() => File.ReadAllText("..\\..\\config\\settings.json"); // Noncompliant
}
```

## Compliant solution

```csharp
using System.IO;

public sealed class Loader
{
    private readonly string _root;

    public Loader(string root) => _root = root;

    public string Read() => File.ReadAllText(Path.Combine(_root, "settings.json"));
}
```
