# Types should not share a name with their containing namespace

`qa_naming_type_conflicts_with_namespace` &middot; Naming &middot; Code Smell &middot; severity MAJOR &middot; optional

When a type and its containing namespace share the same identifier, unqualified references and generated documentation become ambiguous. Choose a distinct type name or rename the namespace segment.

## Noncompliant code example

```csharp
namespace Billing
{
    public sealed class Billing
    {
    }
}
```

## Compliant solution

```csharp
namespace Billing
{
    public sealed class BillingService
    {
    }
}
```
