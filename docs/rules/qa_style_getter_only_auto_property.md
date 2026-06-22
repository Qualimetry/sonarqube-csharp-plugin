# Read-only properties over a backing field should be getter-only auto-properties

`qa_style_getter_only_auto_property` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

A property that only returns a private readonly field, where that field is written nowhere but the constructor, is exactly what a getter-only auto-property provides. The explicit field and getter add boilerplate for no behaviour.

A getter-only auto-property keeps the same immutability while removing the hand-written backing field.

## Noncompliant code example

```csharp
public sealed class Point
{
    private readonly int _x;

    public Point(int x) => _x = x;

    public int X => _x; // Noncompliant
}
```

## Compliant solution

```csharp
public sealed class Point
{
    public Point(int x) => X = x;

    public int X { get; }
}
```

## See also

- [Auto-implemented properties](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties)
