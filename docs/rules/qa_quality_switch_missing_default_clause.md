# Switch statements should declare a default clause

`qa_quality_switch_missing_default_clause` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; optional

When a `switch` statement lists explicit cases but omits a `default` clause, any value that matches none of them falls straight through with no handling, which is rarely the author's intent and silently hides unexpected input.

Declare a `default` clause that performs a deliberate action, even if that action is only to record or throw on the unexpected value.

## Noncompliant code example

```csharp
public sealed class Router
{
    public string Resolve(int code)
    {
        switch (code) // Noncompliant
        {
            case 1:
                return "one";
            case 2:
                return "two";
        }

        return "unknown";
    }
}
```

## Compliant solution

```csharp
public sealed class Router
{
    public string Resolve(int code)
    {
        switch (code)
        {
            case 1:
                return "one";
            case 2:
                return "two";
            default:
                return "unknown";
        }
    }
}
```

## See also

- [C# switch statement reference](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/selection-statements#the-switch-statement)
