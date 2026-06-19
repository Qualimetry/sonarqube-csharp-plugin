# Static constructors should not throw

`qa_quality_throw_in_static_constructor` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

If a static constructor throws, the runtime wraps the exception in a `TypeInitializationException` and marks the type as unusable for the rest of the process. Every later attempt to touch the type, even an unrelated static member, fails with the same error, and the failure is detached from the line that actually went wrong.

Keep static constructors free of code that can fail. Do lazy, fallible initialization on first use where it can be retried and where the exception reaches the caller in context.

## Noncompliant code example

```csharp
public class Config
{
    static Config()
    {
        throw new InvalidOperationException("missing settings"); // Noncompliant
    }
}
```

## Compliant solution

```csharp
public class Config
{
    private static readonly Lazy<Config> instance = new(Load);

    public static Config Instance => instance.Value;

    private static Config Load() => new();
}
```

## See also

- [Static constructors (C# programming guide)](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/static-constructors)
