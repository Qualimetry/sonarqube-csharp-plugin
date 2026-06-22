# Avoid implementing ICloneable

`qa_quality_cloneable_implementation` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

`ICloneable` has a single `Clone()` member that returns `object` and never states whether the copy is deep or shallow. Callers cannot tell what they get back and must cast the result, so the contract carries no useful guarantee.

Provide an explicit, strongly typed copy operation (for example a copy constructor or a `Copy()` method that returns the concrete type) and document its depth.

## Noncompliant code example

```csharp
public sealed class Box : ICloneable // Noncompliant
{
    public int Value { get; set; }

    public object Clone() => new Box { Value = Value };
}
```

## Compliant solution

```csharp
public sealed class Box
{
    public int Value { get; set; }

    public Box Copy() => new Box { Value = Value };
}
```

## See also

- [ICloneable Interface](https://learn.microsoft.com/dotnet/api/system.icloneable)
