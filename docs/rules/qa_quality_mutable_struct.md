# Structures should be immutable

`qa_quality_mutable_struct` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

A value type is copied on assignment, on every method call, and whenever it is boxed. A mutable struct therefore behaves unpredictably: writing to a copy leaves the original untouched, and mutating an element retrieved from a collection updates a throwaway copy. The result is bugs that look like the assignment simply did not happen.

Make structs immutable by declaring fields `readonly` and exposing get-only or `init` properties, setting all state through the constructor.

## Noncompliant code example

```csharp
public struct Point // Noncompliant
{
    public int X { get; set; }
    public int Y { get; set; }
}
```

## Compliant solution

```csharp
public struct Point
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
