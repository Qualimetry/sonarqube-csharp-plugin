# Private fields assigned only during construction should be readonly

`qa_style_field_can_be_readonly` &middot; Style &middot; Code Smell &middot; severity MINOR &middot; optional

A private field that is set in its declaration or a constructor and never written again is conceptually immutable. Declaring it `readonly` turns that intent into a guarantee the compiler enforces.

The modifier documents that later code must not reassign the field and blocks an accidental write from slipping in during maintenance.

## Noncompliant code example

```csharp
public sealed class Cache
{
    private int _capacity; // Noncompliant

    public Cache(int capacity)
    {
        _capacity = capacity;
    }
}
```

## Compliant solution

```csharp
public sealed class Cache
{
    private readonly int _capacity;

    public Cache(int capacity)
    {
        _capacity = capacity;
    }
}
```

## See also

- [readonly (C# reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/readonly)
