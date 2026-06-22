# Empty object or collection initializers should be removed

`qa_style_empty_initializer` &middot; Style &middot; Code Smell &middot; severity MINOR &middot; optional

An object or collection initializer with no members between its braces does nothing the constructor call did not already do. The empty braces are pure noise and can read as an unfinished edit.

Removing them leaves a plain constructor call that says exactly the same thing with less ceremony.

## Noncompliant code example

```csharp
var list = new List<int>() { }; // Noncompliant
```

## Compliant solution

```csharp
var list = new List<int>();
```

## See also

- [Object and collection initializers](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/object-and-collection-initializers)
