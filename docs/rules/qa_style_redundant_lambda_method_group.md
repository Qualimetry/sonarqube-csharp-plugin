# Lambdas that only forward their arguments should be method groups

`qa_style_redundant_lambda_method_group` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

A lambda whose body does nothing but call another method with the same parameters, in the same order, adds a layer of indirection that the reader has to unwind. Passing the method group directly states the intent more plainly and removes the throwaway parameter names.

## Noncompliant code example

```csharp
public IEnumerable<string> Describe(IEnumerable<int> numbers)
{
    return numbers.Select(n => Format(n)); // Noncompliant
}

private static string Format(int value) => value.ToString();
```

## Compliant solution

```csharp
public IEnumerable<string> Describe(IEnumerable<int> numbers)
{
    return numbers.Select(Format);
}

private static string Format(int value) => value.ToString();
```

## See also

- [Method group conversions (C# language reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/delegate-operator)
