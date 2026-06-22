# Static fields should not be assigned from instance methods

`qa_quality_static_field_assigned_from_instance_method` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; optional

Writing to a static field from an instance method means every object of the type mutates state that all instances share. The intent is almost always to change per-object state, so this is usually a typo where an instance field was meant. Even when deliberate, concurrent callers race on the shared field with no synchronisation. Assign instance fields from instance methods, and confine static field writes to a static initializer or a clearly synchronised static method.

## Noncompliant code example

```csharp
public sealed class Counter
{
    private static int _total;

    public void Add(int amount)
    {
        _total = _total + amount; // Noncompliant
    }
}
```

## Compliant solution

```csharp
public sealed class Counter
{
    private int _total;

    public void Add(int amount)
    {
        _total = _total + amount;
    }
}
```
