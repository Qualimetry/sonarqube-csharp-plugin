# Emptiness checks should not compare a string to an empty literal

`qa_style_empty_string_comparison` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

Comparing a string to `""` tests only for an empty value and silently treats `null` as different, which is rarely what the author meant. The intent of "is there any text here" is clearer through a dedicated check.

Using `string.IsNullOrEmpty` or comparing the length states the question directly and handles the null case explicitly.

## Noncompliant code example

```csharp
if (name == "") // Noncompliant
{
    UseDefault();
}
```

## Compliant solution

```csharp
if (string.IsNullOrEmpty(name))
{
    UseDefault();
}
```

## See also

- [String.IsNullOrEmpty](https://learn.microsoft.com/dotnet/api/system.string.isnullorempty)
