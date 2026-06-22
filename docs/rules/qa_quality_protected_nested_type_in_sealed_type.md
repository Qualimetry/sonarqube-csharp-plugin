# Protected nested type in a sealed class should be private

`qa_quality_protected_nested_type_in_sealed_type` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

A sealed class can never be inherited, so `protected` accessibility on anything it declares is meaningless: there is no derived type that could ever see the member. A `protected` nested type in a sealed class therefore signals an intent that the language cannot honour and only confuses the reader.

Declare the nested type `private`, which is its real effective accessibility.

## Noncompliant code example

```csharp
public sealed class Cache
{
    protected class Entry // Noncompliant
    {
        public int Value { get; set; }
    }
}
```

## Compliant solution

```csharp
public sealed class Cache
{
    private sealed class Entry
    {
        public int Value { get; set; }
    }
}
```
