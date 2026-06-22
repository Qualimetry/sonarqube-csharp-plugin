# Collection properties should be read-only

`qa_quality_mutable_collection_property_setter` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

A collection property with a public setter lets callers swap the whole container for a different instance, including `null`. Code that captured the previous reference keeps mutating an object the owner no longer sees, and every access has to defend against the property being reassigned.

Expose the collection through a getter only and initialize it once. Callers add and remove items on the owned instance, and the owner keeps control of the container's lifetime.

## Noncompliant code example

```csharp
public class Order
{
    public List<string> Items { get; set; } = new(); // Noncompliant
}
```

## Compliant solution

```csharp
public class Order
{
    public List<string> Items { get; } = new();
}
```

## See also

- [Collection design guidelines](https://learn.microsoft.com/dotnet/standard/design-guidelines/guidelines-for-collections)
