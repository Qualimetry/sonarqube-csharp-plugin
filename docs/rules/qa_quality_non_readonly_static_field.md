# Static fields should be read-only

`qa_quality_non_readonly_static_field` &middot; CodeQuality &middot; Code Smell &middot; severity CRITICAL &middot; enabled in the recommended profile

A static field that is neither `readonly` nor `const` is shared, writable state for the whole process. Any code in any thread can reassign it, so its value at a given moment is hard to reason about and races are easy to introduce.

Marking the field `readonly` pins the reference after construction and documents that the slot is set once. When the value genuinely needs to change, hide it behind a property with controlled access so the mutation point is explicit.

## Noncompliant code example

```csharp
public class Settings
{
    public static string Environment = "Production"; // Noncompliant
}
```

## Compliant solution

```csharp
public class Settings
{
    public static readonly string Environment = "Production";
}
```

## See also

- [readonly (C# reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/readonly)
