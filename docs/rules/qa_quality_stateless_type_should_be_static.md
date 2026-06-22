# Types with only static members should be static

`qa_quality_stateless_type_should_be_static` &middot; CodeQuality &middot; Code Smell &middot; severity MINOR &middot; optional

A type that carries no instance state and exposes only static members is never meant to be instantiated. Leaving it as a normal class invites callers to create useless instances and hides the type's real intent. Marking it `static` makes the compiler enforce that no instance is ever created and signals at a glance that the type is a pure utility holder.

## Noncompliant code example

```csharp
public class MathHelpers // Noncompliant
{
    public static int Square(int value) => value * value;

    public static int Cube(int value) => value * value * value;
}
```

## Compliant solution

```csharp
public static class MathHelpers
{
    public static int Square(int value) => value * value;

    public static int Cube(int value) => value * value * value;
}
```
