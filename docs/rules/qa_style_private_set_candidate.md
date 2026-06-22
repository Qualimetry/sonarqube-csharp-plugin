# Auto-property setters assigned only in a constructor should be private

`qa_style_private_set_candidate` &middot; Style &middot; Code Smell &middot; severity MINOR &middot; optional

When an auto-property exposes a writable setter but the value is only ever assigned inside a constructor, the broad setter advertises a mutation point that the type never uses. Narrowing the accessor to `private set` keeps the constructor assignment working while signalling to callers that the value is fixed once the instance is built.

A tighter setter also protects the invariant: future code outside the constructor cannot quietly reassign the property, so the intended write-once shape stays enforced by the compiler.

## Noncompliant code example

```csharp
public class Account
{
    public decimal Balance { get; set; } // Noncompliant

    public Account(decimal opening)
    {
        Balance = opening;
    }
}
```

## Compliant solution

```csharp
public class Account
{
    public decimal Balance { get; private set; }

    public Account(decimal opening)
    {
        Balance = opening;
    }
}
```

## See also

- [Properties (C# language reference)](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/properties)
