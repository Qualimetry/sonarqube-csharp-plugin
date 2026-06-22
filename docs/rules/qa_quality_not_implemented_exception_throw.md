# Methods should not throw NotImplementedException

`qa_quality_not_implemented_exception_throw` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

Throwing `NotImplementedException` marks code as unfinished. When it ships, the member compiles and looks complete but fails at run time the first time it is exercised, often deep inside an unrelated feature where the cause is hard to find.

Implement the member before release. If the operation is genuinely unsupported by design, throw `NotSupportedException` with a message that explains why, which is a permanent contract rather than a placeholder.

## Noncompliant code example

```csharp
public decimal CalculateTax(decimal amount)
{
    throw new NotImplementedException(); // Noncompliant
}
```

## Compliant solution

```csharp
public decimal CalculateTax(decimal amount)
{
    return amount * 0.2m;
}
```

## See also

- [NotImplementedException](https://learn.microsoft.com/dotnet/api/system.notimplementedexception)
