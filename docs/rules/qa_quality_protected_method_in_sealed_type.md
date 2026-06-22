# Lower the visibility of protected methods in sealed types

`qa_quality_protected_method_in_sealed_type` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

A `protected` method exists so that derived types can call or override it. In a `sealed` type there are no derived types, so the modifier is meaningless and only widens the apparent contract. Declaring the method `private` matches its real reachability.

## Noncompliant code example

```csharp
public sealed class Report
{
    protected void Render() // Noncompliant
    {
    }
}
```

## Compliant solution

```csharp
public sealed class Report
{
    private void Render()
    {
    }
}
```

## See also

- [Access Modifiers (C# guide)](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/access-modifiers)
