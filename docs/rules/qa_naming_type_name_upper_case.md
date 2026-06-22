# Type names should start with an uppercase letter

`qa_naming_type_name_upper_case` &middot; Naming &middot; Code Smell &middot; severity MINOR &middot; optional

Classes, structs, records, enums, and interfaces are all written in PascalCase, beginning with an uppercase letter. A type whose name opens with a lowercase letter looks like a variable or a keyword and breaks the casing pattern readers rely on to recognise types.

## Noncompliant code example

```csharp
public class widget // Noncompliant
{
}
```

## Compliant solution

```csharp
public class Widget
{
}
```

## See also

- [Capitalization conventions](https://learn.microsoft.com/dotnet/standard/design-guidelines/capitalization-conventions)
