# Redundant 'ToString()' in string concatenation should be removed

`qa_style_redundant_to_string_in_concatenation` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

When an expression takes part in a string concatenation, the compiler already calls `ToString()` on each non-string operand. An explicit `ToString()` in that position is pure noise and, with a null operand, can even turn a safe empty result into a `NullReferenceException`.

## Noncompliant code example

```csharp
public string Label(int count)
{
    return "Items: " + count.ToString(); // Noncompliant
}
```

## Compliant solution

```csharp
public string Label(int count)
{
    return "Items: " + count;
}
```
