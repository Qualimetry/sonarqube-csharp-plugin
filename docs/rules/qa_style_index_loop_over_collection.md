# Index-based 'for' loops over a collection should prefer 'foreach'

`qa_style_index_loop_over_collection` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

A `for` loop that starts at zero, runs while the counter is below a collection's `Length` or `Count`, increments by one, and only uses the counter to index that collection is a manual `foreach`. The index adds an off-by-one risk and obscures the simple intent of visiting each element.

When the index itself is not needed, `foreach` states the loop's purpose directly and cannot run past the end of the sequence.

## Noncompliant code example

```csharp
public int Sum(int[] items)
{
    var total = 0;
    for (int i = 0; i < items.Length; i++) // Noncompliant
    {
        total += items[i];
    }

    return total;
}
```

## Compliant solution

```csharp
public int Sum(int[] items)
{
    var total = 0;
    foreach (var item in items)
    {
        total += item;
    }

    return total;
}
```
