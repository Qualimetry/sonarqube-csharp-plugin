# Constructors of abstract types should not be public

`qa_quality_abstract_type_public_constructor` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

An `abstract` type cannot be instantiated directly, so a `public` constructor on it is misleading: callers see a constructor they can never invoke. Declaring such constructors `protected` states the real contract, that only derived types call them.

## Noncompliant code example

```csharp
public abstract class Shape
{
    public Shape(string name) // Noncompliant
    {
        Name = name;
    }

    public string Name { get; }
}
```

## Compliant solution

```csharp
public abstract class Shape
{
    protected Shape(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
```

## See also

- [Abstract classes (C# language reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/abstract)
