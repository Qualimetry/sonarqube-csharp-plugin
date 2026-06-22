# Static fields should not hold mutable collection types

`qa_quality_static_mutable_collection_field` &middot; CodeQuality &middot; Code Smell &middot; severity CRITICAL &middot; enabled in the recommended profile

Declaring a static field with a mutable collection type such as `List<T>`, `Dictionary<TKey, TValue>`, or an array shares a single growable container across the whole process. Marking the field `readonly` only freezes the reference, not the contents, so any caller can still add, remove, or replace elements and corrupt state observed elsewhere.

Expose the data through a read-only or immutable collection type so the shared state cannot be mutated after initialization.

## Noncompliant code example

```csharp
public class Registry
{
    public static readonly List<string> Names = new(); // Noncompliant
}
```

## Compliant solution

```csharp
public class Registry
{
    public static readonly IReadOnlyList<string> Names = new List<string>().AsReadOnly();
}
```

## See also

- [Immutable collections](https://learn.microsoft.com/dotnet/api/system.collections.immutable)
