# Prefer generic collections over Hashtable and ArrayList

`qa_quality_legacy_non_generic_collection` &middot; CodeQuality &middot; Code Smell &middot; severity MINOR &middot; optional

`ArrayList` and `Hashtable` store everything as `object`, so every value type is boxed and every read needs a cast that the compiler cannot check. The generic collections introduced with .NET 2.0 keep the element type, avoid boxing, and catch type mistakes at compile time.

## Noncompliant code example

```csharp
var items = new ArrayList(); // Noncompliant
items.Add("first");
```

## Compliant solution

```csharp
var items = new List<string>();
items.Add("first");
```

## See also

- [Generic collections in .NET](https://learn.microsoft.com/dotnet/standard/generics/collections)
