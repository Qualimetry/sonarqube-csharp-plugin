# Methods prefixed with Try should return bool

`qa_quality_try_method_return_type` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

By convention a method whose name starts with `Try` signals success or failure through a `bool` result and returns any produced value through an `out` parameter. A `Try` method that returns something other than `bool` contradicts that expectation and confuses callers.

## Noncompliant code example

```csharp
public int TryParseCount(string text) // Noncompliant
{
    return int.Parse(text);
}
```

## Compliant solution

```csharp
public bool TryParseCount(string text, out int count)
{
    return int.TryParse(text, out count);
}
```

## See also

- [Exceptions and the Try-Parse pattern](https://learn.microsoft.com/dotnet/standard/design-guidelines/exceptions-and-performance)
