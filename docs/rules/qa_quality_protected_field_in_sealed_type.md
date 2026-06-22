# Lower the visibility of protected fields in sealed types

`qa_quality_protected_field_in_sealed_type` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

A `sealed` type can never be inherited, so a `protected` member on it is reachable from nowhere that `private` would not also serve. The wider modifier is misleading and the compiler even warns about it. Declaring the field `private` states the real scope.

## Noncompliant code example

```csharp
public sealed class Cache
{
    protected int _size; // Noncompliant
}
```

## Compliant solution

```csharp
public sealed class Cache
{
    private int _size;
}
```

## See also

- [sealed (C# reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/sealed)
