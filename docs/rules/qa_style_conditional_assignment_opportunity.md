# if/else assigning one variable should use the conditional operator

`qa_style_conditional_assignment_opportunity` &middot; Style &middot; Code Smell &middot; severity INFO &middot; enabled in the recommended profile

An `if`/`else` whose only job is to assign the same variable in each branch spreads a single decision over several lines and repeats the assignment target twice. The conditional operator expresses the same choice as one assignment, making it obvious that exactly one value is produced.

## Noncompliant code example

```csharp
public int Clamp(bool high)
{
    int limit;
    if (high) // Noncompliant
    {
        limit = 100;
    }
    else
    {
        limit = 10;
    }

    return limit;
}
```

## Compliant solution

```csharp
public int Clamp(bool high)
{
    int limit = high ? 100 : 10;
    return limit;
}
```
