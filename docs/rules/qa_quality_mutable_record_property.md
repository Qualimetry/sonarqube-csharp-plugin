# Record properties should not have public setters

`qa_quality_mutable_record_property` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

Records exist to model immutable values: equality, `with` expressions, and safe sharing all assume the value cannot change after construction. A property with a public `set` accessor breaks that assumption, so two records that were equal can silently diverge and hashed lookups can lose track of them.

Declare the property with `init` or make it get-only and set it through the constructor.

## Noncompliant code example

```csharp
public record Money
{
    public decimal Amount { get; set; } // Noncompliant
    public string Currency { get; init; } = "USD";
}
```

## Compliant solution

```csharp
public record Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "USD";
}
```
