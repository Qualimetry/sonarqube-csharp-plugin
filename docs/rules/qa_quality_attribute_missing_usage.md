# Annotate attribute types with AttributeUsage

`qa_quality_attribute_missing_usage` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; optional

Without an `[AttributeUsage]` declaration an attribute defaults to being valid on every program element and non-repeatable. Stating the intended targets (and whether it may be applied more than once or is inherited) prevents misuse and documents the design.

## Noncompliant code example

```csharp
public sealed class AuditAttribute : Attribute // Noncompliant
{
}
```

## Compliant solution

```csharp
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class AuditAttribute : Attribute
{
}
```

## See also

- [AttributeUsageAttribute Class](https://learn.microsoft.com/dotnet/api/system.attributeusageattribute)
