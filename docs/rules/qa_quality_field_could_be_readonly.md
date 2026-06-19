# Mark fields readonly when they are never reassigned

`qa_quality_field_could_be_readonly` &middot; CodeQuality &middot; Code Smell &middot; severity MINOR &middot; optional

A private field that is only set in its declaration or in a constructor and is never written again can be declared `readonly`. The modifier documents the intent, lets the compiler reject accidental later assignments, and enables small optimizations.

## Noncompliant code example

```csharp
public class Connection
{
    private string _endpoint; // Noncompliant

    public Connection(string endpoint)
    {
        _endpoint = endpoint;
    }

    public string Endpoint => _endpoint;
}
```

## Compliant solution

```csharp
public class Connection
{
    private readonly string _endpoint;

    public Connection(string endpoint)
    {
        _endpoint = endpoint;
    }

    public string Endpoint => _endpoint;
}
```

## See also

- [readonly (C# reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/readonly)
