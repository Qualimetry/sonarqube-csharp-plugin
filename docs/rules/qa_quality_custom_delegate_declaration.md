# Prefer Func and Action over custom delegate types

`qa_quality_custom_delegate_declaration` &middot; CodeQuality &middot; Code Smell &middot; severity MINOR &middot; optional

A bespoke delegate type whose signature maps cleanly onto the built-in `Func<>` or `Action<>` families adds a name a reader has to look up and an extra type that every caller must reference, for no behavioural gain. The framework delegates are universally understood and compose directly with LINQ and the task-based APIs. Keep a named delegate only when it carries semantics the generic forms cannot, such as `ref` or `out` parameters.

## Noncompliant code example

```csharp
public delegate int Transformer(int input); // Noncompliant

public sealed class Pipeline
{
    public Transformer Step { get; set; }
}
```

## Compliant solution

```csharp
using System;

public sealed class Pipeline
{
    public Func<int, int> Step { get; set; }
}
```
