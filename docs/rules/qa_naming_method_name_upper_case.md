# Method names should start with an uppercase letter

`qa_naming_method_name_upper_case` &middot; Naming &middot; Code Smell &middot; severity MINOR &middot; optional

Methods in .NET are written in PascalCase, so the first letter is uppercase. A method whose name opens with a lowercase letter reads like a field or a local, and breaks the casing pattern that lets a reader tell members apart by sight.

## Noncompliant code example

```csharp
public class Report
{
    public void generate() // Noncompliant
    {
    }
}
```

## Compliant solution

```csharp
public class Report
{
    public void Generate()
    {
    }
}
```

## See also

- [Capitalization conventions](https://learn.microsoft.com/dotnet/standard/design-guidelines/capitalization-conventions)
