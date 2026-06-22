# Absolute URI literals should be valid

`qa_reliability_invalid_uri_literal` &middot; Reliability &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

Constructing a `Uri` from a constant string defaults to requiring an absolute URI, and an ill-formed literal throws a `UriFormatException` only when that line executes. A missing scheme or an illegal character in a fixed literal can be detected as soon as it is written.

Correct the literal so it is a well-formed absolute URI, or construct it with the relative kind when a relative reference is intended.

## Noncompliant code example

```csharp
using System;

public sealed class Service
{
    public Uri Endpoint() => new Uri("http://:// invalid"); // Noncompliant
}
```

## Compliant solution

```csharp
using System;

public sealed class Service
{
    public Uri Endpoint() => new Uri("https://example.com/api");
}
```

## See also

- [System.Uri (.NET API reference)](https://learn.microsoft.com/dotnet/api/system.uri)
