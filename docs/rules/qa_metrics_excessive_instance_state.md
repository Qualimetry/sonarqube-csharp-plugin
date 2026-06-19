# Types should not hold too much instance state

`qa_metrics_excessive_instance_state` &middot; Metrics &middot; Code Smell &middot; severity MAJOR &middot; optional

The amount of state an instance carries is the sum of its instance fields and its instance auto-implemented properties. A type that holds a large number of these slots tends to be a grab-bag of loosely related data and is a candidate for splitting into smaller, more focused objects.

The state limit is configurable.

## Noncompliant code example

```csharp
public sealed class Profile
{
    private int _a;
    private int _b;
    public int C { get; set; }
    public int D { get; set; }
    public int E { get; set; }
    public int F { get; set; }
}
```

## Compliant solution

```csharp
public sealed class Profile
{
    public Name Name { get; set; }
    public Address Address { get; set; }
}
```

## Parameters

This rule is configurable. Edit `maxinstancefields` (default `15`) on the rule in your quality profile; SonarQube applies the value during analysis and synchronises it to connected IDEs.
