# GC.Collect should be followed by GC.WaitForPendingFinalizers

`qa_quality_garbage_collector_collect_without_wait` &middot; CodeQuality &middot; Bug &middot; severity MAJOR &middot; enabled in the recommended profile

Forcing a collection with `GC.Collect` only schedules unreachable objects for finalization; the finalizers run later, on a separate thread. Code that forces a collection in order to release unmanaged resources or reclaim memory deterministically must wait for those finalizers, then collect again, or the resources are still pending when execution continues. If the method does not pair the call with `GC.WaitForPendingFinalizers`, the forced collection does not give the determinism it appears to promise.

## Noncompliant code example

```csharp
using System;

public sealed class Cleaner
{
    public void Purge()
    {
        GC.Collect(); // Noncompliant
    }
}
```

## Compliant solution

```csharp
using System;

public sealed class Cleaner
{
    public void Purge()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
}
```
