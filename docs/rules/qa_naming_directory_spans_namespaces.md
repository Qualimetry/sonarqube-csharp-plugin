# Types in the same directory should share one namespace

`qa_naming_directory_spans_namespaces` &middot; Naming &middot; Code Smell &middot; severity MINOR &middot; optional

Mixing namespaces inside one folder makes it hard to tell which feature owns the code on disk. Align the namespace of every type in a directory.

## Noncompliant code example

```csharp
namespace Company.Product
{
    public sealed class Alpha
    {
    }
}
```

## Compliant solution

```csharp
namespace Company.Product
{
    public sealed class Alpha
    {
    }

    public sealed class Beta
    {
    }
}
```
