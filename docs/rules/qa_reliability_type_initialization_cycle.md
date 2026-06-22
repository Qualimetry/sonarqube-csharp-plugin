# Static initialization cycles should be avoided

`qa_reliability_type_initialization_cycle` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; optional

Static field initializers and static constructors run in dependency order. A cycle leaves one member uninitialized at first access and can throw `TypeInitializationException`.

## Noncompliant code example

```csharp
public sealed class Cycle
{
    private static readonly int A = B + 1;
    private static readonly int B = A + 1;
}
```

## Compliant solution

```csharp
public sealed class NoCycle
{
    private static readonly int A = 1;
    private static readonly int B = A + 1;
}
```
