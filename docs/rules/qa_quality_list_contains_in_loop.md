# Avoid List.Contains lookups inside loops

`qa_quality_list_contains_in_loop` &middot; CodeQuality &middot; Code Smell &middot; severity MINOR &middot; optional

Membership testing on a `List<T>` is a linear scan. Doing it inside a loop turns the surrounding work into a quadratic cost that is invisible on small inputs and crippling on large ones. When the same collection is searched repeatedly, build a `HashSet<T>` once and test against it, turning each lookup into a constant-time operation.

## Noncompliant code example

```csharp
using System.Collections.Generic;

public sealed class Filter
{
    public int Count(List<int> known, IEnumerable<int> input)
    {
        var hits = 0;
        foreach (var value in input)
        {
            if (known.Contains(value)) // Noncompliant
            {
                hits++;
            }
        }

        return hits;
    }
}
```

## Compliant solution

```csharp
using System.Collections.Generic;

public sealed class Filter
{
    public int Count(List<int> known, IEnumerable<int> input)
    {
        var lookup = new HashSet<int>(known);
        var hits = 0;
        foreach (var value in input)
        {
            if (lookup.Contains(value))
            {
                hits++;
            }
        }

        return hits;
    }
}
```
