# Extension methods should be called through member access

`qa_style_extension_method_invocation_style` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

An extension method exists so that it reads as if it belonged to the receiver type. Calling it through the declaring class as a plain static method discards that benefit and makes a fluent chain harder to follow.

Rewriting the call as member access on the first argument keeps related operations on one subject reading left to right, which matches how the rest of the API is used.

## Noncompliant code example

```csharp
var trimmed = StringExtensions.Shorten(name, 10); // Noncompliant
```

## Compliant solution

```csharp
var trimmed = name.Shorten(10);
```

## See also

- [Extension methods (C# programming guide)](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/extension-methods)
