# Constructor parameters should be used

`qa_style_unused_constructor_parameter` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

A constructor parameter that is never read or stored is dead weight: it suggests the caller is supplying state that the type then ignores, which is usually either a forgotten assignment or a leftover from an earlier design.

Either capture the value in a field or property, or drop the parameter so the constructor's contract matches what it actually does.

## Noncompliant code example

```csharp
public sealed class Report
{
    private readonly string _title;

    public Report(string title, string author) // Noncompliant: 'author' is unused
    {
        _title = title;
    }
}
```

## Compliant solution

```csharp
public sealed class Report
{
    private readonly string _title;
    private readonly string _author;

    public Report(string title, string author)
    {
        _title = title;
        _author = author;
    }
}
```

## See also

- [Constructors (C# programming guide)](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/constructors)
