# Prefer 'string.Empty' over an empty string literal

`qa_style_prefer_string_empty` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

An empty string literal `""` is easy to confuse with a single space or to overlook entirely in a dense expression. The named member `string.Empty` states the intent unambiguously.

Reserving `string.Empty` for the deliberate empty value also makes a genuine literal stand out as carrying real text.

## Noncompliant code example

```csharp
public string Prefix { get; set; } = ""; // Noncompliant
```

## Compliant solution

```csharp
public string Prefix { get; set; } = string.Empty;
```

## See also

- [String.Empty field](https://learn.microsoft.com/dotnet/api/system.string.empty)
