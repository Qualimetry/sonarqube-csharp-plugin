# Documentation comments should not have an empty summary

`qa_quality_empty_method_summary` &middot; CodeQuality &middot; Code Smell &middot; severity MINOR &middot; optional

A documentation comment with a missing or blank `<summary>` is worse than no comment at all: tooling renders an empty tooltip, and a reader is led to believe the member is documented when it is not. Either write a summary that explains what the member does and why a caller would use it, or remove the stub so the member is honestly undocumented.

## Noncompliant code example

```csharp
public sealed class Report
{
    /// <summary></summary>
    public void Render() // Noncompliant
    {
    }
}
```

## Compliant solution

```csharp
public sealed class Report
{
    /// <summary>Writes the formatted report to the configured output.</summary>
    public void Render()
    {
    }
}
```
