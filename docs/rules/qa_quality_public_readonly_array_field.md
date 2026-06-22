# Readonly array fields can still be mutated

`qa_quality_public_readonly_array_field` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

The `readonly` modifier on a field only freezes the reference, not what it points at. A `readonly` array therefore advertises immutability it does not have: callers can replace, clear, or rewrite its elements while the field reference itself never changes.

Return a copy, or expose the data through `IReadOnlyList<T>` or `ReadOnlyCollection<T>`, so the contents are genuinely protected.

## Noncompliant code example

```csharp
public class Palette
{
    private readonly string[] _colors = { "red", "green", "blue" };
}
```

## Compliant solution

```csharp
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class Palette
{
    private readonly string[] _colors = { "red", "green", "blue" };

    public IReadOnlyList<string> Colors => new ReadOnlyCollection<string>(_colors);
}
```

## See also

- [CA2105: Array fields should not be read only](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2105)
