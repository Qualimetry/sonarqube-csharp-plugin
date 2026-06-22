# Each source file should declare a single top-level type

`qa_quality_multiple_types_per_file` &middot; CodeQuality &middot; Code Smell &middot; severity MINOR &middot; optional

Packing several top-level types into one file makes a type hard to find by name, hides it from a quick directory scan, and produces noisy diffs when unrelated types change together. Keeping one top-level type per file, named after the type, keeps navigation predictable and lets version control track each type's history independently. Nested types stay with their containing type and are not affected.

## Noncompliant code example

```csharp
public sealed class Customer
{
}

public sealed class Order // Noncompliant
{
}
```

## Compliant solution

```csharp
public sealed class Customer
{
}
```
