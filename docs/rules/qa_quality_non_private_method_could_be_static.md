# Method that does not use instance state should be static

`qa_quality_non_private_method_could_be_static` &middot; CodeQuality &middot; Code Smell &middot; severity MINOR &middot; optional

A method that never touches `this`, an instance field, or an instance member does not depend on the object it is called on. Leaving it as an instance method hides that fact, forces callers to hold an instance they do not need, and adds an avoidable `this` argument to every call.

Mark the method `static` to state that it is independent of instance state. Members that implement an interface or override a base member are excluded.

## Noncompliant code example

```csharp
public class Calculator
{
    public int Square(int value) // Noncompliant
    {
        return value * value;
    }
}
```

## Compliant solution

```csharp
public class Calculator
{
    public static int Square(int value)
    {
        return value * value;
    }
}
```
