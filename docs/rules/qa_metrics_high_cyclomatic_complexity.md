# Methods should not be hard to maintain

`qa_metrics_high_cyclomatic_complexity` &middot; Metrics &middot; Code Smell &middot; severity CRITICAL &middot; enabled in the recommended profile

Cyclomatic complexity counts the independent paths through a method by adding one for each branch point such as an `if`, loop, `case`, `catch`, conditional operator, or short-circuit boolean. A high count means more paths to understand and to cover with tests, and is a strong predictor of defects.

The complexity limit is configurable.

## Noncompliant code example

```csharp
public string Classify(int n)
{
    if (n < 0) return "neg";
    if (n == 0) return "zero";
    if (n < 10 && n > 0) return "small";
    if (n < 100 || n == 500) return "medium";
    return n > 1000 ? "huge" : "large";
}
```

## Compliant solution

```csharp
public string Classify(int n)
{
    return _ranges.First(range => range.Contains(n)).Label;
}
```

## Parameters

This rule is configurable. Edit `maxcomplexity` (default `15`) on the rule in your quality profile; SonarQube applies the value during analysis and synchronises it to connected IDEs.
