# Long numeric literals should use digit separators

`qa_style_numeric_literal_digit_separator` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

A run of five or more digits with no grouping forces the reader to count places to judge the magnitude of a constant. The underscore digit separator lets the value be scanned the way numbers are normally written.

The separator is ignored by the compiler, so it improves legibility without affecting the literal's value.

## Noncompliant code example

```csharp
const int Timeout = 60000; // Noncompliant
```

## Compliant solution

```csharp
const int Timeout = 60_000;
```

## See also

- [Integral numeric types (C# reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/integral-numeric-types)
