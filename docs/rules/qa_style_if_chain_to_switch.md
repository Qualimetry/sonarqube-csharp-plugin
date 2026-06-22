# Long if/else-if chains testing one value should use a 'switch'

`qa_style_if_chain_to_switch` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

A chain of `if`/`else if` branches that each compare the same variable to a constant is a `switch` written the verbose way. The repeated variable name on every line hides the fact that one value is being dispatched, and it is easy to test the wrong variable in one of the branches.

A `switch` names the dispatched value once and lines the cases up so the full set of handled values is visible at a glance.

## Noncompliant code example

```csharp
public string Name(int code)
{
    if (code == 1) // Noncompliant
    {
        return "one";
    }
    else if (code == 2)
    {
        return "two";
    }
    else if (code == 3)
    {
        return "three";
    }

    return "other";
}
```

## Compliant solution

```csharp
public string Name(int code)
{
    switch (code)
    {
        case 1:
            return "one";
        case 2:
            return "two";
        case 3:
            return "three";
        default:
            return "other";
    }
}
```
