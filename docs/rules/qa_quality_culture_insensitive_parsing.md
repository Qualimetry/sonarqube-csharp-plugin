# Parsing of numbers and dates should specify a culture

`qa_quality_culture_insensitive_parsing` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

The culture-free overloads of `Parse` and `TryParse` on `double`, `float`, `decimal`, `DateTime` and `DateTimeOffset` read the thread's current culture. The same string then parses differently on two machines: a decimal comma, a date order, or a grouping separator silently changes the result or fails outright.

State the intent explicitly. Use `CultureInfo.InvariantCulture` for machine-readable data, or the specific user culture when the value really is locale dependent.

## Noncompliant code example

```csharp
using System;

public class Money
{
    public decimal Read(string text) => decimal.Parse(text); // Noncompliant
}
```

## Compliant solution

```csharp
using System;
using System.Globalization;

public class Money
{
    public decimal Read(string text) => decimal.Parse(text, CultureInfo.InvariantCulture);
}
```

## See also

- [CA1305: Specify IFormatProvider](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1305)
