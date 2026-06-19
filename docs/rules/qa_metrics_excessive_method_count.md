# Types should not declare too many methods

`qa_metrics_excessive_method_count` &middot; Metrics &middot; Code Smell &middot; severity MAJOR &middot; optional

A type with a very long list of methods is often a hub that has accumulated behaviour belonging to several collaborators. Counting the methods it declares is a quick way to spot types that have drifted past a single responsibility.

The method limit is configurable.

## Noncompliant code example

```csharp
public sealed class Manager
{
    public void One() { }
    public void Two() { }
    public void Three() { }
    public void Four() { }
    public void Five() { }
}
```

## Compliant solution

```csharp
public sealed class Coordinator
{
    public void Start() { }
    public void Stop() { }
}
```

## Parameters

This rule is configurable. Edit `maxmethods` (default `20`) on the rule in your quality profile; SonarQube applies the value during analysis and synchronises it to connected IDEs.
