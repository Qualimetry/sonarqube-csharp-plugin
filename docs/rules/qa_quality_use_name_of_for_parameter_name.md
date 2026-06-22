# Use nameof for parameter names in argument exceptions

`qa_quality_use_name_of_for_parameter_name` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; optional

Passing a parameter name to an argument exception as a string literal silently rots when the parameter is renamed: the refactoring tool updates the signature but leaves the literal pointing at a name that no longer exists. The `nameof` operator binds the text to the symbol, so a rename keeps them in step and a typo becomes a compile error.

Replace the literal with `nameof(parameter)`.

## Noncompliant code example

```csharp
using System;

public class Account
{
    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException("amount"); // Noncompliant
        }
    }
}
```

## Compliant solution

```csharp
using System;

public class Account
{
    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }
    }
}
```
