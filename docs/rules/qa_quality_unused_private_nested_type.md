# Private nested type is never used

`qa_quality_unused_private_nested_type` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

A private nested type is only visible inside the type that declares it. When nothing in that enclosing type refers to it, the nested type is dead code: it adds surface area and maintenance cost without contributing to any behaviour.

Remove the nested type, or use it from the enclosing type that was meant to depend on it.

## Noncompliant code example

```csharp
public class Scheduler
{
    public void Run() { }

    private sealed class PendingJob // Noncompliant
    {
        public int Id { get; set; }
    }
}
```

## Compliant solution

```csharp
public class Scheduler
{
    private readonly PendingJob _next = new();

    public void Run() => _next.Id++;

    private sealed class PendingJob
    {
        public int Id { get; set; }
    }
}
```
