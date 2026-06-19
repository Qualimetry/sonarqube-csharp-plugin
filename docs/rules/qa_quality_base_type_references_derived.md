# Base classes should not reference their derived types

`qa_quality_base_type_references_derived` &middot; CodeQuality &middot; Code Smell &middot; severity CRITICAL &middot; enabled in the recommended profile

When a base class names one of its own subclasses, the inheritance hierarchy depends on itself in both directions. The base type can no longer be understood, tested, or extended without dragging in the very types that are supposed to depend on it, and adding a new subclass risks breaking the parent.

Keep the base type unaware of its descendants. Express variation through virtual members, interfaces, or a factory supplied from the outside rather than constructing or casting to a concrete subclass inside the base.

## Noncompliant code example

```csharp
public class Widget
{
    public Widget CreateDefault()
    {
        return new Button(); // Noncompliant
    }
}

public class Button : Widget
{
}
```

## Compliant solution

```csharp
public class Widget
{
    public virtual Widget CreateDefault()
    {
        return new Widget();
    }
}

public class Button : Widget
{
}
```

## See also

- [Inheritance (C# programming guide)](https://learn.microsoft.com/dotnet/csharp/fundamentals/object-oriented/inheritance)
