# Declare enums with the default Int32 underlying type

`qa_quality_non_standard_enum_underlying_type` &middot; CodeQuality &middot; Code Smell &middot; severity MINOR &middot; optional

Unless an enum is constrained by interop, persistence, or a wire format, it should keep the default `int` underlying type. Smaller or larger integer types save little in practice, surprise readers, and can break code that assumes the conventional 32-bit storage.

If a non-default size is genuinely required (for example to match a native struct), keep the explicit type and document why.

## Noncompliant code example

```csharp
public enum Priority : long // Noncompliant
{
    Low,
    Normal,
    High
}
```

## Compliant solution

```csharp
public enum Priority
{
    Low,
    Normal,
    High
}
```

## See also

- [Enumeration types (C# reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/enum)
