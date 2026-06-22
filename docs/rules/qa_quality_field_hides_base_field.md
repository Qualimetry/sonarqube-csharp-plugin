# Fields should not hide a field of the same name in a base type

`qa_quality_field_hides_base_field` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; optional

Declaring a field whose name matches a field in a base type silently shadows the inherited one. Two distinct storage slots then share a name, and which one a piece of code reads or writes depends on the static type at the access site. That is a frequent source of bugs where a value is set on one slot and read from the other. Choose a distinct name, or if shadowing is genuinely intended, make it explicit with the `new` modifier.

## Noncompliant code example

```csharp
public class Base
{
    protected int count;
}

public sealed class Derived : Base
{
    private int count; // Noncompliant
}
```

## Compliant solution

```csharp
public class Base
{
    protected int count;
}

public sealed class Derived : Base
{
    private int processedCount;
}
```
