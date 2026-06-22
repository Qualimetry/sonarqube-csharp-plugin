# An interface's 'I' prefix should be followed by an uppercase letter

`qa_naming_interface_prefix_casing` &middot; Naming &middot; Code Smell &middot; severity MINOR &middot; optional

The interface convention treats the leading `I` as a one-letter prefix in front of a PascalCase name, so the second character should be uppercase. A name where the `I` runs straight into a lowercase letter reads as an ordinary word that merely starts with `I`, which defeats the prefix and misleads the reader.

## Noncompliant code example

```csharp
public interface Itemizer // Noncompliant
{
    void Run();
}
```

## Compliant solution

```csharp
public interface IItemizer
{
    void Run();
}
```

## See also

- [Names of classes, structs, and interfaces](https://learn.microsoft.com/dotnet/standard/design-guidelines/names-of-classes-structs-and-interfaces)
