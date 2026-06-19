# Remove empty static constructors

`qa_quality_empty_static_constructor` &middot; CodeQuality &middot; Code Smell &middot; severity MINOR &middot; optional

An empty static constructor does nothing, but its presence forces the runtime to add an initialization check before the first access of the type and removes the `beforefieldinit` flag the compiler would otherwise emit. Deleting it is behaviour-preserving and slightly faster.

## Noncompliant code example

```csharp
public class Settings
{
    static Settings() // Noncompliant
    {
    }

    public static int Limit = 100;
}
```

## Compliant solution

```csharp
public class Settings
{
    public static int Limit = 100;
}
```

## See also

- [Static Constructors (C# guide)](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/static-constructors)
