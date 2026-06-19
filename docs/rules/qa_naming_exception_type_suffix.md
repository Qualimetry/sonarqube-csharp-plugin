# Exception types should carry an 'Exception' suffix

`qa_naming_exception_type_suffix` &middot; Naming &middot; Code Smell &middot; severity MINOR &middot; optional

A type that ultimately derives from `System.Exception` shows up in `catch` clauses, stack traces, and logs. Ending its name with `Exception` signals at every one of those sites that the type represents a failure, and keeps it consistent with the framework's own exception types.

## Noncompliant code example

```csharp
using System;

public class PaymentFailed : Exception // Noncompliant
{
}
```

## Compliant solution

```csharp
using System;

public class PaymentFailedException : Exception
{
}
```

## See also

- [Names of exceptions](https://learn.microsoft.com/dotnet/standard/design-guidelines/names-of-classes-structs-and-interfaces)
