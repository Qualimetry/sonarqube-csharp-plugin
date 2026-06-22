# Unnecessary parentheses should be removed

`qa_style_unnecessary_return_parentheses` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

Parentheses wrapping a whole returned value that is already a single atomic expression, such as an identifier, literal or member access, add nothing to precedence or meaning. They make the reader pause to confirm there is no hidden grouping. Removing them leaves the intent unchanged and the line cleaner.

## Noncompliant code example

```csharp
public int Current(int value)
{
    return (value); // Noncompliant
}
```

## Compliant solution

```csharp
public int Current(int value)
{
    return value;
}
```
