# Null guards around a single call should use the null-conditional operator

`qa_style_null_guard_to_conditional` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

An `if (x != null)` whose body is a single call on `x` restates a check the language already has. The null-conditional operator `?.` performs the same guarded call in one expression and makes the optional nature of the receiver obvious at the call site.

## Noncompliant code example

```csharp
public void Raise(System.Action handler)
{
    if (handler != null) // Noncompliant
    {
        handler();
    }
}
```

## Compliant solution

```csharp
public void Raise(System.Action handler)
{
    handler?.Invoke();
}
```
