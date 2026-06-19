# Composite format strings should match their argument list

`qa_quality_string_format_argument_count` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

A call to `string.Format` with a literal format string whose highest placeholder index is greater than or equal to the number of arguments supplied will throw `FormatException` the moment it runs. The mismatch is invisible at compile time but fatal in production.

Align the placeholders with the arguments: every `{n}` must have a matching argument at position `n`.

## Noncompliant code example

```csharp
public class Greeter
{
    public string Build(string user) =>
        string.Format("{0} signed in at {1}", user); // Noncompliant
}
```

## Compliant solution

```csharp
using System;

public class Greeter
{
    public string Build(string user) =>
        string.Format("{0} signed in at {1}", user, DateTime.UtcNow);
}
```

## See also

- [Composite formatting (.NET)](https://learn.microsoft.com/dotnet/standard/base-types/composite-formatting)
