# Abstract type constructors should be protected or private

`qa_quality_abstract_type_constructor_accessibility` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

An abstract type can only be instantiated through a derived class, so a constructor that is `public` or `internal` advertises an entry point that no caller can ever use. It misleads readers about how the type is meant to be created.

Constructors of abstract types exist solely for derived classes to chain into. Declare them `protected` (or `private` when only the type's own code chains to them) to state that contract precisely.

## Noncompliant code example

```csharp
public abstract class Repository
{
    internal Repository(string name) // Noncompliant
    {
        Name = name;
    }

    public string Name { get; }
}
```

## Compliant solution

```csharp
public abstract class Repository
{
    protected Repository(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
```

## See also

- [Abstract classes (C# language reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/abstract)
