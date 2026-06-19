# Nested type should not be more accessible than its container

`qa_quality_overexposed_nested_type` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

A nested type can never be reached more widely than the type that contains it, so declaring it `public` inside an `internal` or otherwise non-public container is misleading. The declared accessibility promises a reach the type does not have, and a reader has to trace the enclosing type to learn its true scope.

Lower the nested type to match what its container actually allows, typically `private` or `internal`.

## Noncompliant code example

```csharp
internal class Pipeline
{
    public class Stage // Noncompliant
    {
        public int Order { get; set; }
    }
}
```

## Compliant solution

```csharp
internal class Pipeline
{
    private sealed class Stage
    {
        public int Order { get; set; }
    }
}
```
