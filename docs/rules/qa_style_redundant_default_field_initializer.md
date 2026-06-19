# Fields should not be explicitly initialized to their default value

`qa_style_redundant_default_field_initializer` &middot; Style &middot; Bug &middot; severity MINOR &middot; optional

The runtime already zero-initialises every field before a constructor runs, so assigning `0`, `false`, or `null` in the declaration repeats work that has already happened.

Dropping the redundant initialiser leaves only the genuinely non-default values visible, so a reader can tell which fields are deliberately seeded.

## Noncompliant code example

```csharp
private int _retries = 0; // Noncompliant
private bool _enabled = false; // Noncompliant
```

## Compliant solution

```csharp
private int _retries;
private bool _enabled;
```

## See also

- [Fields (C# programming guide)](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/fields)
