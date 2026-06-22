# Remove or make static classes that can never be instantiated

`qa_quality_uninstantiable_class` &middot; CodeQuality &middot; Code Smell &middot; severity MINOR &middot; optional

A non-static class whose only constructors are private, and which exposes no static members, cannot be instantiated from anywhere and offers no usable surface. It is either dead code or was meant to be a `static` utility class.

Mark it `static`, give it an accessible constructor, or delete it.

## Noncompliant code example

```csharp
public class StringTools // Noncompliant
{
    private StringTools()
    {
    }
}
```

## Compliant solution

```csharp
public static class StringTools
{
    public static string Reverse(string value) => value;
}
```

## See also

- [Static Classes (C# guide)](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/static-classes-and-static-class-members)
