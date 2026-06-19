# Constructed exceptions should be thrown, not discarded

`qa_quality_discarded_exception_instance` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

Creating an exception object as a stand-alone statement does nothing useful: the instance is constructed and then immediately discarded, so the error it was meant to report never surfaces. This is almost always a forgotten `throw`, which means a failure path silently continues as if nothing went wrong.

Either `throw` the exception so the caller is notified, or delete the dead construction if it was left behind by mistake.

## Noncompliant code example

```csharp
using System;

public class Validator
{
    public void Check(int value)
    {
        if (value < 0)
        {
            new ArgumentOutOfRangeException(nameof(value)); // Noncompliant
        }
    }
}
```

## Compliant solution

```csharp
using System;

public class Validator
{
    public void Check(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
    }
}
```

## See also

- [throw statement (C# language reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/exception-handling-statements)
