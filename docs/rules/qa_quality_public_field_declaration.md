# Avoid exposing fields with public or internal visibility

`qa_quality_public_field_declaration` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; optional

A field that is `public` or `internal` lets any caller read and overwrite the value with no validation, no change notification, and no way to evolve the implementation later. Properties give the same syntax to callers while keeping the storage encapsulated.

Constants are exempt because their value can never change.

## Noncompliant code example

```csharp
public class Account
{
    public decimal Balance; // Noncompliant
}
```

## Compliant solution

```csharp
public class Account
{
    public decimal Balance { get; private set; }
}
```

## See also

- [Fields (C# guide)](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/fields)
