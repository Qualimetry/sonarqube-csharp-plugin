# Constructors should not read overridable properties

`qa_quality_virtual_property_access_in_constructor` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

Reading a `virtual` or `abstract` property from a constructor dispatches to the most-derived override, which executes while the derived constructor has not run yet. The override sees fields that are still at their default values and can return a wrong answer or throw.

Read a non-virtual member, pass the value in as a constructor argument, or defer the work to an initialization step that runs after construction completes.

## Noncompliant code example

```csharp
public abstract class View
{
    private readonly string _label;

    protected View()
    {
        _label = Caption; // Noncompliant
    }

    public string Label => _label;

    protected abstract string Caption { get; }
}
```

## Compliant solution

```csharp
public abstract class View
{
    private readonly string _label;

    protected View(string caption)
    {
        _label = caption;
    }

    public string Label => _label;

    protected abstract string Caption { get; }
}
```

## See also

- [Virtual members and construction order](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/virtual)
