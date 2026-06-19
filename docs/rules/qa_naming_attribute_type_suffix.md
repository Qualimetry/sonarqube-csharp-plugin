# Attribute types should carry an 'Attribute' suffix

`qa_naming_attribute_type_suffix` &middot; Naming &middot; Code Smell &middot; severity MINOR &middot; optional

A type that derives from `System.Attribute` is consumed through the attribute usage syntax, where the compiler lets callers drop the trailing `Attribute` word. Keeping that word in the declared name is what makes the shorthand readable and keeps the declaration consistent with how the framework's own attributes are spelled.

## Noncompliant code example

```csharp
using System;

public sealed class Audited : Attribute // Noncompliant
{
}
```

## Compliant solution

```csharp
using System;

public sealed class AuditedAttribute : Attribute
{
}
```

## See also

- [Attribute class naming guidance](https://learn.microsoft.com/dotnet/standard/design-guidelines/names-of-classes-structs-and-interfaces)
