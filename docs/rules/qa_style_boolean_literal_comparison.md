# Boolean expressions should not be compared to a boolean literal

`qa_style_boolean_literal_comparison` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

Comparing a boolean value to `true` or `false` adds a layer that says nothing: `x == true` is just `x`, and `x == false` is just `!x`. The literal comparison only lengthens the expression.

Dropping the comparison reads closer to the condition's intent and removes a classic spot where a stray single `=` turns a test into an assignment.

## Noncompliant code example

```csharp
if (isReady == true) // Noncompliant
{
    Start();
}
```

## Compliant solution

```csharp
if (isReady)
{
    Start();
}
```

## See also

- [Boolean logical operators (C# reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/boolean-logical-operators)
