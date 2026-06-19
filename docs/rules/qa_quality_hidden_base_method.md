# Members should not hide inherited members

`qa_quality_hidden_base_method` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

Declaring a member with the `new` modifier hides the inherited one rather than overriding it. The member that runs then depends on the compile-time type of the reference, so the same object behaves differently when seen through the base type versus the derived type. That split is a frequent source of bugs that only appear through polymorphism.

If the intent is to specialize behavior, mark the base member `virtual` and `override` it so dispatch is consistent. If the members are genuinely unrelated, give the new one a distinct name.

## Noncompliant code example

```csharp
public class Animal
{
    public virtual string Describe() => "animal";
}

public class Dog : Animal
{
    public new string Describe() => "dog"; // Noncompliant
}
```

## Compliant solution

```csharp
public class Animal
{
    public virtual string Describe() => "animal";
}

public class Dog : Animal
{
    public override string Describe() => "dog";
}
```

## See also

- [new modifier (C# reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/new-modifier)
