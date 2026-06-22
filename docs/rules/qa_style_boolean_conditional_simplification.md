# Conditional expressions yielding boolean literals should be simplified

`qa_style_boolean_conditional_simplification` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

A ternary of the shape `condition ? true : false` evaluates to exactly the condition, and `condition ? false : true` is just its negation. Writing the boolean expression directly is shorter and removes a needless branch for the reader to trace.

## Noncompliant code example

```csharp
public bool CanProceed(int retries)
{
    return retries < 3 ? true : false; // Noncompliant
}
```

## Compliant solution

```csharp
public bool CanProceed(int retries)
{
    return retries < 3;
}
```
