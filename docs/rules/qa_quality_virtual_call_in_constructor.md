# Constructors should not call overridable members

`qa_quality_virtual_call_in_constructor` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

When a constructor calls a virtual or abstract member, the most derived override executes before the derived class constructor has run. The override then observes fields that are still at their default values, which leads to subtle bugs or a `NullReferenceException` that is hard to trace back to the base constructor.

Keep constructors free of overridable calls. Do the work with non-virtual logic, or move it to an initialization method the caller invokes once the object is fully constructed.

## Noncompliant code example

```csharp
public abstract class Document
{
    protected Document()
    {
        Render(); // Noncompliant
    }

    protected abstract void Render();
}
```

## Compliant solution

```csharp
public abstract class Document
{
    protected Document()
    {
    }

    public void Initialize() => Render();

    protected abstract void Render();
}
```

## See also

- [Virtual members (C# reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/virtual)
