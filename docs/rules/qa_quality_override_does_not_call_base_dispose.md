# Overrides of Dispose(bool) should call the base implementation

`qa_quality_override_does_not_call_base_dispose` &middot; CodeQuality &middot; Bug &middot; severity MAJOR &middot; optional

The dispose pattern relies on every level of an inheritance chain releasing the resources it owns. When a type overrides `Dispose(bool)` but never invokes the base version, the resources held by base types are silently leaked, and the leak only surfaces under load when handles or memory run out.

An override of `Dispose(bool)` should forward the call to `base.Dispose(disposing)` on every path so the chain stays intact.

## Noncompliant code example

```csharp
public class CachingStream : MemoryStream
{
    protected override void Dispose(bool disposing) // Noncompliant
    {
        if (disposing)
        {
            _cache.Clear();
        }
    }

    private readonly System.Collections.Generic.List<byte[]> _cache = new();
}
```

## Compliant solution

```csharp
public class CachingStream : MemoryStream
{
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cache.Clear();
        }

        base.Dispose(disposing);
    }

    private readonly System.Collections.Generic.List<byte[]> _cache = new();
}
```

## See also

- [Implement a Dispose method](https://learn.microsoft.com/dotnet/standard/garbage-collection/implementing-dispose)
