# Nested if statements without an else should be combined

`qa_style_mergeable_nested_if` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

When an `if` with no `else` contains nothing but another `if` that also has no `else`, both conditions must hold for the body to run. Two levels of nesting express a single combined condition.

Joining the tests with `&&` flattens the structure and puts the full entry condition in one place.

## Noncompliant code example

```csharp
if (user != null) // Noncompliant
{
    if (user.IsActive)
    {
        Grant();
    }
}
```

## Compliant solution

```csharp
if (user != null && user.IsActive)
{
    Grant();
}
```

## See also

- [The if statement (C# reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/selection-statements#the-if-statement)
