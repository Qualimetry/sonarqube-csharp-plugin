# Properties that only wrap a backing field should be auto-implemented

`qa_style_explicit_backing_field_property` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

When a property's getter only returns a private field and its setter only assigns that same field, the explicit backing field and accessor bodies repeat what an auto-implemented property expresses in one line. The compiler generates the same field behind the scenes.

Replacing the pattern with `{ get; set; }` removes boilerplate and keeps the field private to the property, where it belongs.

## Noncompliant code example

```csharp
public class Person
{
    private string _name;

    public string Name // Noncompliant
    {
        get { return _name; }
        set { _name = value; }
    }
}
```

## Compliant solution

```csharp
public class Person
{
    public string Name { get; set; }
}
```
