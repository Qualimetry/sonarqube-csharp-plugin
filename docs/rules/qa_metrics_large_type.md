# Types should not be too long

`qa_metrics_large_type` &middot; Metrics &middot; Code Smell &middot; severity MAJOR &middot; optional

The number of source lines a type occupies is a rough but useful proxy for how much responsibility it carries. A type that runs to many hundreds of lines is hard to hold in the head and is often hiding several smaller types that want to be extracted.

The line limit is configurable.

## Noncompliant code example

```csharp
public sealed class Everything
{
    // ... hundreds of lines of unrelated members ...
    public void A() { }
    public void B() { }
}
```

## Compliant solution

```csharp
public sealed class Orders
{
    public void Place() { }
}

public sealed class Invoices
{
    public void Issue() { }
}
```

## Parameters

This rule is configurable. Edit `maxlines` (default `400`) on the rule in your quality profile; SonarQube applies the value during analysis and synchronises it to connected IDEs.
