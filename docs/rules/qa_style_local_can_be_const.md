# Local variables that never change should be declared const

`qa_style_local_can_be_const` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

A local initialised with a compile-time constant and never reassigned is a constant in everything but declaration. Marking it `const` records that fact and lets the compiler reject any later attempt to change it.

The reader then knows the value is fixed for the whole method without scanning for reassignments.

## Noncompliant code example

```csharp
void Render()
{
    int margin = 8; // Noncompliant
    Apply(margin);
}
```

## Compliant solution

```csharp
void Render()
{
    const int margin = 8;
    Apply(margin);
}
```

## See also

- [const (C# reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/const)
