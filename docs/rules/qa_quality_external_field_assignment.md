# Fields should be assigned only within their declaring hierarchy

`qa_quality_external_field_assignment` &middot; CodeQuality &middot; Code Smell &middot; severity CRITICAL &middot; enabled in the recommended profile

When code outside a field's own type hierarchy writes to that field, the owning type loses control of its own state: any invariant the type tries to keep can be broken from the outside and the bug surfaces far from the type that is supposed to be responsible for it. Mutation should travel through a member the owning type exposes on purpose, so the type stays the single place that decides how its state changes.

## Noncompliant code example

```csharp
public sealed class Account
{
    public decimal Balance;
}

public sealed class Teller
{
    public void Reset(Account account)
    {
        account.Balance = 0m; // Noncompliant
    }
}
```

## Compliant solution

```csharp
public sealed class Account
{
    public decimal Balance { get; private set; }

    public void Reset() => Balance = 0m;
}

public sealed class Teller
{
    public void Reset(Account account) => account.Reset();
}
```
