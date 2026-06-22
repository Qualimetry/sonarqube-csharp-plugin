# Fold Where into the following LINQ operator

`qa_quality_redundant_where_clause` &middot; CodeQuality &middot; Code Smell &middot; severity INFO &middot; enabled in the recommended profile

Operators such as `Any`, `Count`, `First`, and `Single` accept a predicate directly. Calling `Where` immediately before a parameterless one of them creates an extra iterator for no benefit and reads as two steps where one would do.

Pass the predicate to the terminal operator and drop the `Where` call.

## Noncompliant code example

```csharp
using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    public bool HasExpensive(IEnumerable<int> prices)
    {
        return prices.Where(p => p > 100).Any(); // Noncompliant
    }
}
```

## Compliant solution

```csharp
using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    public bool HasExpensive(IEnumerable<int> prices)
    {
        return prices.Any(p => p > 100);
    }
}
```
