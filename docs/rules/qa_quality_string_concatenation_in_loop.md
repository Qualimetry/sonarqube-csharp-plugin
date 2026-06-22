# Avoid string concatenation inside loops

`qa_quality_string_concatenation_in_loop` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; optional

Strings are immutable, so a `+=` against a `string` allocates a brand new object on every iteration and copies all the characters seen so far. Inside a loop this turns linear work into quadratic work and creates a large amount of short-lived garbage.

Use a `StringBuilder` (or a single `string.Join`) so the characters are appended into one growable buffer and materialised once.

## Noncompliant code example

```csharp
string result = "";
foreach (var item in items)
{
    result += item + ", "; // Noncompliant
}
```

## Compliant solution

```csharp
var builder = new StringBuilder();
foreach (var item in items)
{
    builder.Append(item).Append(", ");
}
string result = builder.ToString();
```

## See also

- [StringBuilder Class](https://learn.microsoft.com/dotnet/api/system.text.stringbuilder)
