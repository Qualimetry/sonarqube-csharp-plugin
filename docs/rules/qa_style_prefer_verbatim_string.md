# String literals with only escaped backslashes should be verbatim

`qa_style_prefer_verbatim_string` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

A regular string literal whose backslashes are all doubled, such as a file path or a regular-expression pattern, is hard to read because each real backslash appears twice. A verbatim string prefixed with `@` takes backslashes literally, so the text matches what the value actually is.

This rule only flags literals whose sole escape sequences are doubled backslashes, where switching to a verbatim string preserves the value exactly.

## Noncompliant code example

```csharp
public string LogPath()
{
    return "C:\\logs\\app.txt"; // Noncompliant
}
```

## Compliant solution

```csharp
public string LogPath()
{
    return @"C:\logs\app.txt";
}
```
