# Argument exceptions should name a real parameter

`qa_quality_argument_exception_parameter_name` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

The `paramName` argument of `ArgumentException` and its relatives is what tooling, logs and callers use to find the offending input. When the literal passed there is misspelled or stale, the diagnostic points at a parameter that does not exist and the real culprit stays hidden.

Use `nameof(parameter)` so the compiler keeps the name correct through every rename.

## Noncompliant code example

```csharp
using System;

public class Account
{
    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException("ammount"); // Noncompliant
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

## See also

- [nameof expression (C# reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/nameof)
