# A negated 'Any' query should be written as 'All'

`qa_style_negated_any_to_all` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

Negating `Any` reads as "there is no element that matches", which is the same as "every element fails the condition". Writing it as `All` with the inverted predicate makes that universal statement explicit.

Both expressions evaluate identically; the change only clarifies what the query is really asserting.

## Noncompliant code example

```csharp
if (!items.Any(i => i.IsExpired)) // Noncompliant
{
    KeepAll();
}
```

## Compliant solution

```csharp
if (items.All(i => !i.IsExpired))
{
    KeepAll();
}
```

## See also

- [Enumerable.All](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.all)
