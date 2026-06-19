# Monitor acquisition and release should be paired in one method

`qa_quality_monitor_enter_without_exit` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

A `Monitor.Enter` or `Monitor.TryEnter` that is not balanced by a `Monitor.Exit` in the same method leaks the lock: any exception, early return, or simple oversight leaves the monitor held forever and the next thread deadlocks.

Keep the acquire and release together, with the release in a `finally`, or prefer a `lock` statement which guarantees the pairing.

## Noncompliant code example

```csharp
using System.Threading;

public class Cache
{
    private readonly object _gate = new object();

    public void Add()
    {
        Monitor.Enter(_gate); // Noncompliant
        Mutate();
    }

    private void Mutate()
    {
    }
}
```

## Compliant solution

```csharp
using System.Threading;

public class Cache
{
    private readonly object _gate = new object();

    public void Add()
    {
        Monitor.Enter(_gate);
        try
        {
            Mutate();
        }
        finally
        {
            Monitor.Exit(_gate);
        }
    }

    private void Mutate()
    {
    }
}
```

## See also

- [Monitor class (.NET API)](https://learn.microsoft.com/dotnet/api/system.threading.monitor)
