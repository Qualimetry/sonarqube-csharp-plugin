# Method names should not be excessively long

`qa_metrics_long_method_name` &middot; Metrics &middot; Code Smell &middot; severity INFO &middot; optional

When a method name grows very long it often tries to describe a sequence of steps rather than a single intent. That is a hint that the method itself is doing too much, or that the name is restating information the type and parameters already give.

The maximum length is configurable.

## Noncompliant code example

```csharp
public sealed class Service
{
    public void LoadValidateTransformAndPersistAllIncomingCustomerRecords()
    {
    }
}
```

## Compliant solution

```csharp
public sealed class Service
{
    public void ImportCustomers()
    {
    }
}
```

## Parameters

This rule is configurable. Edit `maxnamelength` (default `40`) on the rule in your quality profile; SonarQube applies the value during analysis and synchronises it to connected IDEs.
