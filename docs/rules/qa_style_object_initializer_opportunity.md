# Object creation followed by property assignments should use an object initializer

`qa_style_object_initializer_opportunity` &middot; Style &middot; Code Smell &middot; severity INFO &middot; enabled in the recommended profile

Creating an object and then setting its members one statement at a time scatters a single logical construction across several lines and leaves the instance in a half-built state in between. An object initializer keeps the whole shape of the new value in one expression.

This rule flags a local that is assigned a freshly created object and immediately followed by member assignments on that same local.

## Noncompliant code example

```csharp
public Widget Build()
{
    var widget = new Widget(); // Noncompliant
    widget.Width = 10;
    return widget;
}
```

## Compliant solution

```csharp
public Widget Build()
{
    var widget = new Widget
    {
        Width = 10,
    };
    return widget;
}
```
