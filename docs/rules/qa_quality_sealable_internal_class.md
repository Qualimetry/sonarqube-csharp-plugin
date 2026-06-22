# Non-extensible classes should be sealed

`qa_quality_sealable_internal_class` &middot; CodeQuality &middot; Code Smell &middot; severity MINOR &middot; optional

An `internal` class that nobody derives from is, in practice, final, but leaving it open invites an accidental subclass and forces the runtime to treat its members as potentially overridden. Sealing such a class documents that it is a leaf in the hierarchy, lets the just-in-time compiler devirtualize calls, and prevents inheritance from being added without a deliberate decision.

## Noncompliant code example

```csharp
internal class PriceFormatter // Noncompliant
{
    public string Format(decimal value) => value.ToString("C");
}
```

## Compliant solution

```csharp
internal sealed class PriceFormatter
{
    public string Format(decimal value) => value.ToString("C");
}
```
