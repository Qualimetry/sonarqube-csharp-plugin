# Redundant 'else' after a terminating branch should be removed

`qa_style_redundant_else_after_jump` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

When the body of an `if` ends with a statement that leaves the enclosing method or loop, such as `return`, `throw`, `break` or `continue`, the matching `else` can never be reached as a fall-through. Keeping it only adds a level of nesting.

Dropping the `else` and letting the following code run unconditionally keeps the happy path flat and easier to read without changing behaviour.

## Noncompliant code example

```csharp
public string Describe(int value)
{
    if (value < 0)
    {
        return "negative";
    }
    else // Noncompliant
    {
        return "non-negative";
    }
}
```

## Compliant solution

```csharp
public string Describe(int value)
{
    if (value < 0)
    {
        return "negative";
    }

    return "non-negative";
}
```
