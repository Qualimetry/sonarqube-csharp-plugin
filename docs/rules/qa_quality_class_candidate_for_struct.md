# Small immutable value holders are candidates to become structs

`qa_quality_class_candidate_for_struct` &middot; CodeQuality &middot; Code Smell &middot; severity MINOR &middot; optional

A class with no base type, a handful of read-only members of value type, and no behaviour beyond construction is really a value, not an entity. Modelling it as a `struct` gives it value semantics, avoids a heap allocation and an extra reference indirection on every use, and removes the null state that a reference type carries for free. Keep it a class only if identity, inheritance, or reference sharing matters.

## Noncompliant code example

```csharp
public sealed class Point // Noncompliant
{
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }

    public int Y { get; }
}
```

## Compliant solution

```csharp
public readonly struct Point
{
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }

    public int Y { get; }
}
```
