# Fields should be declared before other members

`qa_style_field_declaration_order` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

Keeping every field at the top of a type gives the reader the data a class holds before the behaviour that uses it. A field that appears after methods or properties is easy to miss and breaks the expected layout.

A consistent member order lets anyone scanning the file find the state without hunting through the body.

## Noncompliant code example

```csharp
public sealed class Counter
{
    public void Increment() => _value++;

    private int _value; // Noncompliant
}
```

## Compliant solution

```csharp
public sealed class Counter
{
    private int _value;

    public void Increment() => _value++;
}
```

## See also

- [C# coding conventions](https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions)
