# Structs with only immutable state should be declared readonly

`qa_contract_immutable_struct_should_be_readonly` &middot; Contract &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

When every instance field of a `struct` is `readonly` and none of its properties expose a setter, the value can never change after construction. Marking the type itself `readonly` states that contract explicitly and lets the compiler enforce it, so a later edit that introduces a mutating member is rejected at compile time rather than slipping through.

A `readonly struct` also lets the compiler skip defensive copies when the value is accessed through a `readonly` reference, so the annotation documents intent and removes a class of hidden allocations at the same time.

## Noncompliant code example

```csharp
public struct Money // Noncompliant
{
    private readonly decimal _amount;

    public Money(decimal amount)
    {
        _amount = amount;
    }

    public decimal Amount => _amount;
}
```

## Compliant solution

```csharp
public readonly struct Money
{
    private readonly decimal _amount;

    public Money(decimal amount)
    {
        _amount = amount;
    }

    public decimal Amount => _amount;
}
```

## See also

- [readonly struct (C# language reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/struct#readonly-struct)
