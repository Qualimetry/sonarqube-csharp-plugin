# Members should not be more accessible than their containing type

`qa_quality_public_method_in_non_public_type` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

Marking a member `public` inside a type that is itself `internal` or otherwise non-public is misleading: the member can never be reached from outside the assembly because the type that holds it cannot. The wider modifier suggests an external contract that does not exist and hides the member's real reach. Declare the member with accessibility that matches how far it can actually be used.

## Noncompliant code example

```csharp
internal sealed class Cache
{
    public void Evict() // Noncompliant
    {
    }
}
```

## Compliant solution

```csharp
internal sealed class Cache
{
    internal void Evict()
    {
    }
}
```
