# Single-statement members should use an expression body

`qa_style_expression_body_opportunity` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

A method or accessor whose block contains a single return or expression statement carries three lines of braces around one line of work. An expression body states the same logic with `=>`, reducing the visual weight and making the one-liner read as the simple mapping it is.

## Noncompliant code example

```csharp
public int Square(int value)
{
    return value * value; // Noncompliant
}
```

## Compliant solution

```csharp
public int Square(int value) => value * value;
```

## See also

- [Expression-bodied members (C# language reference)](https://learn.microsoft.com/dotnet/csharp/programming-guide/statements-expressions-operators/expression-bodied-members)
