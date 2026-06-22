# Events should be raised through a null-conditional invocation

`qa_quality_event_invoked_without_null_check` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

An event with no subscribers is `null`. Invoking it directly throws a `NullReferenceException` the moment a handler is absent. A separate `if` null check before the call is also unsafe, because the last subscriber can unsubscribe on another thread between the check and the invocation.

Raise the event with the null-conditional operator, `Handler?.Invoke(...)`. The compiler captures the delegate into a temporary and calls it only when it is non-null, which is both concise and thread-safe.

## Noncompliant code example

```csharp
public event EventHandler? Changed;

protected void OnChanged()
{
    Changed(this, EventArgs.Empty); // Noncompliant
}
```

## Compliant solution

```csharp
public event EventHandler? Changed;

protected void OnChanged()
{
    Changed?.Invoke(this, EventArgs.Empty);
}
```

## See also

- [How to raise base class events](https://learn.microsoft.com/dotnet/csharp/programming-guide/events/how-to-raise-base-class-events-in-derived-classes)
