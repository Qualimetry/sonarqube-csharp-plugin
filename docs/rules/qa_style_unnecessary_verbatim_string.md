# Verbatim strings without escapes should be regular strings

`qa_style_unnecessary_verbatim_string` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

The `@` prefix exists for strings that contain backslashes, embedded quotes, or line breaks. A verbatim literal whose content has none of these gains nothing from the prefix and just looks like it might.

Dropping the prefix on such literals keeps the verbatim form meaningful: where it appears, it signals that the text really does need it.

## Noncompliant code example

```csharp
var label = @"Pending"; // Noncompliant
```

## Compliant solution

```csharp
var label = "Pending";
```

## See also

- [String literals (C# reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/reference-types#string-literals)
