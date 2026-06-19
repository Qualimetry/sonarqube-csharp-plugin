# Repeated 'Regex.IsMatch' with an inline pattern should use a cached Regex

`qa_style_static_regex_is_match` &middot; Style &middot; Code Smell &middot; severity MINOR &middot; optional

The static `Regex.IsMatch` overloads parse and build the pattern on every call. In a hot path or a loop that repeated work is wasted: the pattern is constant but the engine recompiles it each time.

Promoting the pattern to a shared `Regex` instance (optionally `RegexOptions.Compiled`) parses it once and reuses the automaton for every match.

## Noncompliant code example

```csharp
using System.Text.RegularExpressions;

public class Validator
{
    public bool IsCode(string input)
    {
        return Regex.IsMatch(input, "^[A-Z]{3}$"); // Noncompliant
    }
}
```

## Compliant solution

```csharp
using System.Text.RegularExpressions;

public class Validator
{
    private static readonly Regex CodePattern = new Regex("^[A-Z]{3}$");

    public bool IsCode(string input)
    {
        return CodePattern.IsMatch(input);
    }
}
```
