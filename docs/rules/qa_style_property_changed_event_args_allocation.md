# PropertyChangedEventArgs should be cached rather than allocated per notification

`qa_style_property_changed_event_args_allocation` &middot; Style &middot; Code Smell &middot; severity MINOR &middot; optional

Change notification fires often, and creating a fresh `PropertyChangedEventArgs` on every raise allocates a short-lived object whose only content is a property name that never varies for a given property.

Holding one instance per property in a static readonly field reuses the same arguments on each notification and keeps the hot path allocation-free.

## Noncompliant code example

```csharp
protected void OnNameChanged()
{
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name")); // Noncompliant
}
```

## Compliant solution

```csharp
private static readonly PropertyChangedEventArgs NameChangedArgs = new("Name");

protected void OnNameChanged()
{
    PropertyChanged?.Invoke(this, NameChangedArgs);
}
```

## See also

- [PropertyChangedEventArgs](https://learn.microsoft.com/dotnet/api/system.componentmodel.propertychangedeventargs)
