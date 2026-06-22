# Mutable value types should be reference types

`qa_style_mutable_struct_should_be_class` &middot; Style &middot; Code Smell &middot; severity MINOR &middot; optional

A struct that publishes mutable fields or settable properties copies its state on every assignment, argument pass, and collection access, so a mutation often lands on a hidden copy and is silently lost. A type whose contract is to hold changeable state is almost always clearer and safer as a class, where the reference semantics match the intent.

Value types are best reserved for small, immutable bundles of data. When mutability is required, a class avoids the copy traps that mutable structs are notorious for.

## Noncompliant code example

```csharp
public struct Cursor // Noncompliant
{
    public int Line;
    public int Column;
}
```

## Compliant solution

```csharp
public sealed class Cursor
{
    public int Line { get; set; }
    public int Column { get; set; }
}
```

## See also

- [Choosing between class and struct](https://learn.microsoft.com/dotnet/standard/design-guidelines/choosing-between-class-and-struct)
