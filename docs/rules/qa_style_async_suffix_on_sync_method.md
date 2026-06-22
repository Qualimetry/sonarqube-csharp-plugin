# Synchronous methods should not carry an 'Async' name suffix

`qa_style_async_suffix_on_sync_method` &middot; Style &middot; Code Smell &middot; severity MINOR &middot; optional

The `Async` suffix is a promise that the method returns a task to await. A synchronous method that returns a plain value yet ends in `Async` breaks that promise and leads callers to await something that cannot be awaited.

Naming the method without the suffix keeps the convention trustworthy, so the suffix continues to mean "awaitable" wherever it appears.

## Noncompliant code example

```csharp
public int ComputeAsync(int value) // Noncompliant
{
    return value * 2;
}
```

## Compliant solution

```csharp
public int Compute(int value)
{
    return value * 2;
}
```

## See also

- [Task-based asynchronous pattern naming](https://learn.microsoft.com/dotnet/standard/asynchronous-programming-patterns/task-based-asynchronous-pattern-tap)
