# Asynchronous methods should carry an 'Async' name suffix

`qa_style_async_method_name_suffix` &middot; Style &middot; Code Smell &middot; severity MINOR &middot; optional

When a method is declared `async`, callers usually need to `await` its result. Naming the method with an `Async` suffix advertises that obligation at the call site, so a reader spots the awaitable without inspecting the signature.

An asynchronous method whose name gives no hint of its nature invites callers to ignore the returned task, which silently drops exceptions and continuations. The suffix is the convention the framework itself follows for its task-returning members.

## Noncompliant code example

```csharp
public async Task Save(Order order) // Noncompliant
{
    await _store.WriteAsync(order);
}
```

## Compliant solution

```csharp
public async Task SaveAsync(Order order)
{
    await _store.WriteAsync(order);
}
```

## See also

- [Task-based asynchronous pattern naming](https://learn.microsoft.com/dotnet/standard/asynchronous-programming-patterns/task-based-asynchronous-pattern-tap)
