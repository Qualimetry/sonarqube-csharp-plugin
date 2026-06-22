# Types should be declared inside a namespace

`qa_quality_type_without_namespace` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

Types declared in the global namespace pollute the root scope and collide easily with other libraries. Place every type in a namespace that reflects its ownership and feature area.

## Noncompliant code example

```csharp
public sealed class GlobalWidget
{
}
```

## Compliant solution

```csharp
namespace Company.Product
{
    public sealed class Widget
    {
    }
}
```
