# Attributes should restrict the targets they apply to

`qa_quality_attribute_usage_all_targets` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; optional

An attribute declared with `AttributeTargets.All` can be placed on assemblies, classes, methods, parameters, return values, and more. Almost no attribute is meaningful in every one of those positions, so the broad target list lets the attribute be applied where it does nothing and hides genuine misuse from the compiler.

List only the program elements the attribute is designed for in its `AttributeUsage`.

## Noncompliant code example

```csharp
using System;

[AttributeUsage(AttributeTargets.All)] // Noncompliant
public sealed class AuditAttribute : Attribute
{
}
```

## Compliant solution

```csharp
using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AuditAttribute : Attribute
{
}
```
