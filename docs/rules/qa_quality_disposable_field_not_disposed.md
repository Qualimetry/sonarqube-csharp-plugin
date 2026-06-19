# Disposable fields should be disposed

`qa_quality_disposable_field_not_disposed` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

A type that holds a field of a disposable type owns that resource for its lifetime. If the owning type implements `IDisposable` but its `Dispose` method never disposes the field, the wrapped resource leaks every time an instance is discarded.

Release each owned disposable field inside `Dispose`. If ownership is intentionally transferred elsewhere, make that transfer explicit so the field is not treated as owned here.

## Noncompliant code example

```csharp
public class Importer : IDisposable
{
    private readonly Resource resource = new(); // Noncompliant

    public void Dispose()
    {
    }
}
```

## Compliant solution

```csharp
public class Importer : IDisposable
{
    private readonly Resource resource = new();

    public void Dispose()
    {
        resource.Dispose();
    }
}
```

## See also

- [Implement a Dispose method](https://learn.microsoft.com/dotnet/standard/garbage-collection/implementing-dispose)
