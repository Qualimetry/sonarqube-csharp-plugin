# Side-effect-free computations should be marked pure

`qa_contract_pure_method_should_be_marked_pure` &middot; Contract &middot; Code Smell &middot; severity MINOR &middot; optional

An expression-bodied method that returns a value built only from its inputs, without calling other methods, constructing objects, or assigning to any state, is observably pure. Annotating such a method with `[Pure]` documents that contract for callers and lets analysers reason about safe caching and reordering.

The annotation is a no-op at runtime, so adding it never changes behaviour; it only records an invariant that the method already satisfies.

## Noncompliant code example

```csharp
public class Geometry
{
    public int Square(int n) => n * n; // Noncompliant
}
```

## Compliant solution

```csharp
using System.Diagnostics.Contracts;

public class Geometry
{
    [Pure]
    public int Square(int n) => n * n;
}
```

## See also

- [PureAttribute (System.Diagnostics.Contracts)](https://learn.microsoft.com/dotnet/api/system.diagnostics.contracts.pureattribute)
