# Public members should return a collection abstraction

`qa_quality_concrete_collection_return_type` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

Returning a concrete collection such as `List<T>` or `Dictionary<TKey, TValue>` from a public API leaks an implementation choice to every caller. Consumers may start to depend on members that the contract never promised, and the type can no longer be swapped for a different implementation without a breaking change.

Return an interface such as `IReadOnlyList<T>`, `IList<T>`, or `IEnumerable<T>` that expresses only what the contract guarantees.

## Noncompliant code example

```csharp
using System.Collections.Generic;

public class Catalog
{
    public List<string> GetNames() // Noncompliant
    {
        return new List<string> { "a", "b" };
    }
}
```

## Compliant solution

```csharp
using System.Collections.Generic;

public class Catalog
{
    public IReadOnlyList<string> GetNames()
    {
        return new List<string> { "a", "b" };
    }
}
```
