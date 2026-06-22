# General exception types should not be thrown

`qa_quality_general_exception_type_thrown` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

Throwing `Exception`, `SystemException`, or `ApplicationException` forces every caller that wants to react to this failure to catch the broadest base type. That handler then also swallows unrelated errors, so the program either over-reacts or hides bugs it should have surfaced.

Throw the most specific type that fits the failure, or define a purpose-built exception for the domain, so callers can catch exactly the condition they know how to handle.

## Noncompliant code example

```csharp
public void Connect(string host)
{
    if (string.IsNullOrEmpty(host))
    {
        throw new Exception("Host is required."); // Noncompliant
    }
}
```

## Compliant solution

```csharp
public void Connect(string host)
{
    if (string.IsNullOrEmpty(host))
    {
        throw new ArgumentException("Host is required.", nameof(host));
    }
}
```

## See also

- [Exception throwing guidelines](https://learn.microsoft.com/dotnet/standard/design-guidelines/exception-throwing)
