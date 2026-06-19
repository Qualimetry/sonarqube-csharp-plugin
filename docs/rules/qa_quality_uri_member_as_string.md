# URI and URL members should be typed as System.Uri

`qa_quality_uri_member_as_string` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; optional

A member named for a URI or URL but typed as `string` pushes parsing, validation, and escaping onto every caller. Strings happily hold malformed or relative addresses, and comparisons that should be structural become brittle text matches.

Declare the member as `System.Uri` so the type guarantees a well-formed address and exposes its components directly.

## Noncompliant code example

```csharp
public class Endpoint
{
    public string ServiceUrl { get; set; } = "https://example.test"; // Noncompliant
}
```

## Compliant solution

```csharp
using System;

public class Endpoint
{
    public Uri ServiceUrl { get; set; } = new Uri("https://example.test");
}
```
