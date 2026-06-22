# Property getters should not modify object state

`qa_quality_property_getter_mutates_state` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

Callers expect reading a property to be free of side effects: they read it in the debugger, in logging, and in conditions without a second thought. A getter that increments a counter or reassigns a field changes behaviour depending on how often the property is observed, which produces defects that are extremely hard to reproduce.

Keep getters side-effect free. Move state changes into an explicitly named method.

## Noncompliant code example

```csharp
public class Sequence
{
    private int _value;

    public int Next
    {
        get
        {
            _value += 1; // Noncompliant
            return _value;
        }
    }
}
```

## Compliant solution

```csharp
public class Sequence
{
    private int _value;

    public int Current => _value;

    public int MoveNext()
    {
        _value += 1;
        return _value;
    }
}
```
