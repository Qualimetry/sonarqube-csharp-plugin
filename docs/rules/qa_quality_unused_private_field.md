# Private field is never used

`qa_quality_unused_private_field` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

A private field that is never referenced anywhere in its declaring type is dead state: it consumes memory on every instance, suggests behaviour that does not exist, and makes the type harder to reason about. Because the field is private, the compiler and reader can be certain no other type depends on it.

Delete the field, or wire it into the logic it was meant to support.

## Noncompliant code example

```csharp
public class OrderProcessor
{
    private int _retryCount; // Noncompliant

    public void Process(Order order)
    {
        order.Submit();
    }
}
```

## Compliant solution

```csharp
public class OrderProcessor
{
    public void Process(Order order)
    {
        order.Submit();
    }
}
```
