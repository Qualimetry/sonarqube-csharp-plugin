# Types should not expose too many mutable reference-typed fields

`qa_metrics_mutable_complex_field` &middot; Metrics &middot; Code Smell &middot; severity MAJOR &middot; optional

A field that holds a reference or user-defined type and is left writable invites callers and later code to swap the whole object out, which makes the owning instance's state harder to reason about. Marking such fields `readonly` pins the reference at construction and keeps mutation inside the object itself. Counting the writable complex-typed fields gives a tunable budget for how much of this looseness a type may keep.

The limit is configurable; set it to zero to require every complex field to be readonly.

## Noncompliant code example

```csharp
public sealed class Cache
{
    private Store _store = new Store(); // Noncompliant
}
```

## Compliant solution

```csharp
public sealed class Cache
{
    private readonly Store _store = new Store();
}
```

## Parameters

This rule is configurable. Edit `maxmutablecomplexfields` (default `0`) on the rule in your quality profile; SonarQube applies the value during analysis and synchronises it to connected IDEs.
