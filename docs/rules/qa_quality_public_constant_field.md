# Public constants should be replaced with static read-only fields

`qa_quality_public_constant_field` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

The value of a `public const` is copied into every assembly that references it at compile time. If the constant ever changes, consumers keep the old value until they are recompiled, which produces silent version skew across a solution.

A `public static readonly` field is resolved at run time, so updating the declaring assembly is enough for every caller to observe the new value. Reserve `public const` for values that are part of the language or protocol and can never change.

## Noncompliant code example

```csharp
public class Limits
{
    public const int MaxRetries = 3; // Noncompliant
}
```

## Compliant solution

```csharp
public class Limits
{
    public static readonly int MaxRetries = 3;
}
```

## See also

- [const (C# reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/const)
