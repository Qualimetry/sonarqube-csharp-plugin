# A negated 'All' query should be written as 'Any'

`qa_style_negated_all_to_any` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

Negating `All` asks the reader to invert a universal claim in their head: "not every element matches" really means "some element fails". Stating that directly with `Any` and the opposite predicate removes the double negative.

The two forms return the same result, so the rewrite is purely about reading the intent of the query at a glance.

## Noncompliant code example

```csharp
if (!items.All(i => i.IsValid)) // Noncompliant
{
    Reject();
}
```

## Compliant solution

```csharp
if (items.Any(i => !i.IsValid))
{
    Reject();
}
```

## See also

- [Enumerable.Any](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.any)
