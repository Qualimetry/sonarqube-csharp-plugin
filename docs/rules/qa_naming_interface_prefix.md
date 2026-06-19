# Interface names should begin with the letter 'I'

`qa_naming_interface_prefix` &middot; Naming &middot; Code Smell &middot; severity MINOR &middot; optional

The leading `I` is the one signal at a use site that distinguishes an interface from a class or struct, since both appear in the same positions. An interface without it forces the reader to look up the declaration to know whether they are programming against a contract or a concrete type.

## Noncompliant code example

```csharp
public interface Repository // Noncompliant
{
    void Save();
}
```

## Compliant solution

```csharp
public interface IRepository
{
    void Save();
}
```

## See also

- [Names of classes, structs, and interfaces](https://learn.microsoft.com/dotnet/standard/design-guidelines/names-of-classes-structs-and-interfaces)
