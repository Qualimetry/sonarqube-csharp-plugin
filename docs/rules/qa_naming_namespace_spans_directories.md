# Types in the same namespace should live in the same directory

`qa_naming_namespace_spans_directories` &middot; Naming &middot; Code Smell &middot; severity MINOR &middot; optional

When one namespace spans several folders, navigation and ownership become unclear. Keep all types for a namespace under one directory.

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
