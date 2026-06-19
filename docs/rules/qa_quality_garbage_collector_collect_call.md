# Code should not force garbage collection

`qa_quality_garbage_collector_collect_call` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

An explicit `GC.Collect` forces a full collection on the runtime's schedule rather than its own. It almost always hurts: it promotes short-lived objects, stalls threads, and masks the real allocation problem instead of fixing it.

Remove the call and address the underlying allocation pattern; the garbage collector is tuned to decide when collection is worthwhile.

## Noncompliant code example

```csharp
using System;

public class Importer
{
    public void Finish()
    {
        GC.Collect(); // Noncompliant
    }
}
```

## Compliant solution

```csharp
public class Importer
{
    public void Finish()
    {
    }
}
```

## See also

- [GC.Collect (.NET API)](https://learn.microsoft.com/dotnet/api/system.gc.collect)
