# Private static fields should use an 's_' prefix

`qa_naming_static_field_prefix` &middot; Naming &middot; Code Smell &middot; severity MINOR &middot; optional

An `s_` prefix on a private static field marks shared state that outlives any single instance, so a reader spots at the assignment site that the write is process-wide rather than per-object. It keeps static state visually distinct from the underscore-prefixed instance fields and from constants.

## Noncompliant code example

```csharp
public class Counter
{
    private static int instances; // Noncompliant
}
```

## Compliant solution

```csharp
public class Counter
{
    private static int s_instances;
}
```

## See also

- [Field naming conventions](https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/identifier-names)
