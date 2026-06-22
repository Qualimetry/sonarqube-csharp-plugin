# Attribute types should be sealed

`qa_quality_attribute_type_should_be_sealed` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; optional

Attributes are looked up by reflection, and the runtime walks the inheritance chain when it resolves them. An attribute type that can be derived from makes that lookup ambiguous and slower, and it is almost never the intent. Unless the type exists purely as a base for a family of attributes, sealing it states that it is a concrete, final attribute and removes the surprise of an accidental subclass.

## Noncompliant code example

```csharp
using System;

public class AuditAttribute : Attribute // Noncompliant
{
    public string Reason { get; set; }
}
```

## Compliant solution

```csharp
using System;

public sealed class AuditAttribute : Attribute
{
    public string Reason { get; set; }
}
```
