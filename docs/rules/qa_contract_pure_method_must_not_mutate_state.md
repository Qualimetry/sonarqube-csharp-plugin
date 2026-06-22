# Methods marked pure must not mutate observable state

`qa_contract_pure_method_must_not_mutate_state` &middot; Contract &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

Applying `[Pure]` tells callers and tooling that a method computes a result without changing any observable state, which makes it safe to elide, reorder, or cache. A pure method that assigns to a field or property of its type, or to static state, breaks that promise: callers that rely on the annotation will silently drop the side effect and behave incorrectly.

Either remove the mutation so the method honours its contract, or remove the `[Pure]` attribute so the side effect is visible to everyone reading the signature.

## Noncompliant code example

```csharp
using System.Diagnostics.Contracts;

public class Counter
{
    private int _count;

    [Pure]
    public int Next() // Noncompliant
    {
        _count++;
        return _count;
    }
}
```

## Compliant solution

```csharp
using System.Diagnostics.Contracts;

public class Calculator
{
    [Pure]
    public int Add(int a, int b)
    {
        return a + b;
    }
}
```

## See also

- [PureAttribute (System.Diagnostics.Contracts)](https://learn.microsoft.com/dotnet/api/system.diagnostics.contracts.pureattribute)
