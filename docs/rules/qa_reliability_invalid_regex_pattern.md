# Regular expression literals should compile

`qa_reliability_invalid_regex_pattern` &middot; Reliability &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

A constant regular expression pattern is only validated when the `Regex` is constructed at runtime, so an unbalanced group, a dangling quantifier, or an unterminated character class surfaces as an `ArgumentException` rather than at the point the pattern was written.

Correct the pattern so it is a syntactically valid regular expression.

## Noncompliant code example

```csharp
using System.Text.RegularExpressions;

public sealed class Matcher
{
    public Regex Build() => new Regex("(unclosed"); // Noncompliant
}
```

## Compliant solution

```csharp
using System.Text.RegularExpressions;

public sealed class Matcher
{
    public Regex Build() => new Regex("(closed)");
}
```

## See also

- [.NET regular expression language reference](https://learn.microsoft.com/dotnet/standard/base-types/regular-expression-language-quick-reference)
