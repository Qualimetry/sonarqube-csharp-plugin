# Compound 'if' conditions joined by '&&' may read better as nested ifs

`qa_style_splittable_conjunction_if` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

An `if` whose condition chains several requirements with `&&` packs every precondition onto one line. When a branch has no `else`, splitting the conjunction into nested `if` statements lets each guard stand on its own and gives a natural place to attach an early exit or comment per step.

This is a readability option, not a correctness fix; teams that prefer flat conditions can leave it disabled.

## Noncompliant code example

```csharp
public void Save(string name, bool dirty)
{
    if (name != null && dirty) // Noncompliant
    {
        Persist(name);
    }
}
```

## Compliant solution

```csharp
public void Save(string name, bool dirty)
{
    if (name != null)
    {
        if (dirty)
        {
            Persist(name);
        }
    }
}
```
