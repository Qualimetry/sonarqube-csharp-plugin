# Dispose should suppress finalization

`qa_quality_dispose_missing_suppress_finalize` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

When a type has both a finalizer and a `Dispose` method, calling `Dispose` already releases everything. If `Dispose` does not tell the runtime to skip finalization, the object is still placed on the finalizer queue, which delays its reclamation by a full garbage-collection cycle and runs cleanup code a second time.

Call `GC.SuppressFinalize(this)` at the end of `Dispose` so the now-cleaned object bypasses the finalizer.

## Noncompliant code example

```csharp
public class Connection : IDisposable
{
    ~Connection()
    {
        Close();
    }

    public void Dispose() // Noncompliant
    {
        Close();
    }

    private void Close()
    {
    }
}
```

## Compliant solution

```csharp
public class Connection : IDisposable
{
    ~Connection()
    {
        Close();
    }

    public void Dispose()
    {
        Close();
        GC.SuppressFinalize(this);
    }

    private void Close()
    {
    }
}
```

## See also

- [GC.SuppressFinalize](https://learn.microsoft.com/dotnet/api/system.gc.suppressfinalize)
