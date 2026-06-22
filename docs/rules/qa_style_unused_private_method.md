# Unused private methods should be removed

`qa_style_unused_private_method` &middot; Style &middot; Code Smell &middot; severity MAJOR &middot; optional

A `private` method can only be invoked from inside the type that declares it. When no reference to it exists anywhere in that type, it is dead code: it cannot be reached, yet it still has to be read, maintained and compiled.

This rule reports a private method whose name appears nowhere else in the declaring type. If the method is genuinely an entry point reached by reflection, give it a usage or document it so the intent is explicit.

## Noncompliant code example

```csharp
public class Report
{
    public int Total() => 1;

    private int Unused() => 0; // Noncompliant
}
```

## Compliant solution

```csharp
public class Report
{
    public int Total() => Helper();

    private int Helper() => 0;
}
```
