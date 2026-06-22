# Methods named Dispose should belong to a disposable type

`qa_quality_dispose_method_without_i_disposable` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; optional

The name `Dispose` is a strong signal that a type participates in the `IDisposable` pattern and that callers, including `using` statements, can release it deterministically. Putting a `Dispose` method on a type that does not implement `IDisposable` breaks that expectation: the method will not be called by `using` or by disposal-aware containers. Either implement the interface so the method is the real disposal entry point, or give the method a name that describes what it actually does.

## Noncompliant code example

```csharp
public sealed class Buffer
{
    public void Dispose() // Noncompliant
    {
    }
}
```

## Compliant solution

```csharp
using System;

public sealed class Buffer : IDisposable
{
    public void Dispose()
    {
    }
}
```
