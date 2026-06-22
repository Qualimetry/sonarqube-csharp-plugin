# Readonly fields of mutable reference types give a false sense of immutability

`qa_quality_readonly_mutable_reference_field` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

The `readonly` modifier freezes only the reference, not the object it points to. A readonly field typed as an array or a mutable collection still lets any holder add, remove, or overwrite elements, so the field reads as immutable while its contents are wide open. Expose a read-only view such as `IReadOnlyList<T>`, or use an immutable collection type, when the intent is that the contents cannot change.

## Noncompliant code example

```csharp
using System.Collections.Generic;

public sealed class Registry
{
    private readonly List<string> _names = new List<string>(); // Noncompliant
}
```

## Compliant solution

```csharp
using System.Collections.Generic;

public sealed class Registry
{
    private readonly IReadOnlyList<string> _names = new List<string>();
}
```
