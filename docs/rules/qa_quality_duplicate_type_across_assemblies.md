# Types should not be defined in more than one assembly

`qa_quality_duplicate_type_across_assemblies` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; optional

Duplicating a full type definition across assemblies creates ambiguous references and divergent fixes. Share one canonical type through a single class library instead.

## Noncompliant code example

```csharp
namespace Shared.Models
{
    public sealed class Customer
    {
        public string Name { get; set; }
    }
}
```

## Compliant solution

```csharp
namespace Shared.Models
{
    public sealed class Customer
    {
        public string Name { get; set; }
    }
}
```
