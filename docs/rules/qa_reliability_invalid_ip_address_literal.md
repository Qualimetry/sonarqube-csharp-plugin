# IP address literals should be valid

`qa_reliability_invalid_ip_address_literal` &middot; Reliability &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

A constant string passed to `IPAddress.Parse` is parsed when the line runs, not when it is written, so a typo in the literal stays hidden until that code path executes and then throws a `FormatException`. Because the value is fixed in source, the mistake can be caught immediately.

Correct the literal so it is a well-formed IPv4 or IPv6 address.

## Noncompliant code example

```csharp
using System.Net;

public sealed class Endpoint
{
    public IPAddress Address() => IPAddress.Parse("10.0.0.300"); // Noncompliant
}
```

## Compliant solution

```csharp
using System.Net;

public sealed class Endpoint
{
    public IPAddress Address() => IPAddress.Parse("10.0.0.30");
}
```

## See also

- [CWE-20: Improper Input Validation](https://cwe.mitre.org/data/definitions/20.html)
