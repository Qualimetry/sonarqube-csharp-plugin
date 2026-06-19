# Distinct types should not share the same simple name

`qa_quality_duplicate_type_name` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

When two unrelated types share a simple name and differ only by namespace, every file that needs both must alias one of them, error messages become ambiguous, and a reader cannot tell from an unqualified reference which type is meant. Reflection and serialization that key on the short name also become unreliable. Give each type a name that is unique within the codebase so references stay unambiguous without aliasing.

## Noncompliant code example

```csharp
namespace Billing
{
    public sealed class Account // Noncompliant
    {
    }
}

namespace Identity
{
    public sealed class Account
    {
    }
}
```

## Compliant solution

```csharp
namespace Billing
{
    public sealed class BillingAccount
    {
    }
}

namespace Identity
{
    public sealed class UserAccount
    {
    }
}
```
