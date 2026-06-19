# Dangerous thread control methods should not be used

`qa_quality_dangerous_thread_method` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

Methods such as `Thread.Abort`, `Thread.Suspend`, and `Thread.Resume` stop a thread at an arbitrary point. Locks may be left held, partial writes may be left behind, and finalizers may not run, which leaves shared state corrupted and the process prone to deadlock. These methods are unsupported on modern .NET and throw at run time.

Signal a thread to stop cooperatively with a `CancellationToken`, and let the worker reach a safe point before it exits.

## Noncompliant code example

```csharp
public void Stop(Thread worker)
{
    worker.Abort(); // Noncompliant
}
```

## Compliant solution

```csharp
public void Stop(CancellationTokenSource cancellation)
{
    cancellation.Cancel();
}
```

## See also

- [Cancellation in managed threads](https://learn.microsoft.com/dotnet/standard/threading/cancellation-in-managed-threads)
