# Exceptions should not derive from ApplicationException

`qa_quality_application_exception_base_type` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

`ApplicationException` was meant to separate application errors from system errors, a distinction the framework itself never adopted. Deriving from it adds a layer that carries no behaviour and no meaning, so the guidance is to skip it entirely.

Derive custom exceptions directly from `Exception`, or from a more specific exception type when one fits.

## Noncompliant code example

```csharp
using System;

public class OrderFailedException : ApplicationException // Noncompliant
{
}
```

## Compliant solution

```csharp
using System;

public class OrderFailedException : Exception
{
}
```

## See also

- [CA1058 and exception base types](https://learn.microsoft.com/dotnet/api/system.applicationexception)
