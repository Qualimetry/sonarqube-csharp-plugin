# Boolean results should be returned directly instead of through if/else

`qa_style_boolean_return_via_if` &middot; Style &middot; Code Smell &middot; severity INFO &middot; enabled in the recommended profile

An `if` that returns `true` in one branch and `false` in the other is just the condition itself, spelled out the long way. Returning the expression directly removes four lines of ceremony and states the intent plainly.

## Noncompliant code example

```csharp
public bool IsPositive(int value)
{
    if (value > 0) // Noncompliant
    {
        return true;
    }
    else
    {
        return false;
    }
}
```

## Compliant solution

```csharp
public bool IsPositive(int value)
{
    return value > 0;
}
```
