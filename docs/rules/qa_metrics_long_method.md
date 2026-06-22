# Methods should not be too long

`qa_metrics_long_method` &middot; Metrics &middot; Code Smell &middot; severity CRITICAL &middot; optional

A method that stretches over many statements usually does several things at once, which makes it hard to name, test, and reason about. Counting the statements in a body is a blunt but reliable signal that a method has accreted more than one responsibility.

The statement limit is configurable so generated-style or table-driven bodies can be tuned.

## Noncompliant code example

```csharp
public int Tally(int[] values)
{
    int a = 0;
    int b = 0;
    int c = 0;
    a = values[0];
    b = values[1];
    c = values[2];
    a += b;
    b += c;
    c += a;
    return a + b + c;
}
```

## Compliant solution

```csharp
public int Tally(int[] values)
{
    int sum = 0;
    foreach (var value in values)
    {
        sum += value;
    }

    return sum;
}
```

## Parameters

This rule is configurable. Edit `maxstatements` (default `40`) on the rule in your quality profile; SonarQube applies the value during analysis and synchronises it to connected IDEs.
