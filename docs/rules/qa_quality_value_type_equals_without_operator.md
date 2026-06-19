# Structs that override Equals should define the equality operator

`qa_quality_value_type_equals_without_operator` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

Overriding `Equals` on a value type signals that equality has been given real meaning. But `==` on a struct that defines no equality operator falls back to a different mechanism, so `a.Equals(b)` and `a == b` can return opposite answers for the same pair.

Provide a matching `operator ==` (and `operator !=`) so every form of equality on the type agrees.

## Noncompliant code example

```csharp
public struct Temperature
{
    public int Degrees;

    public override bool Equals(object obj) => obj is Temperature t && t.Degrees == Degrees; // Noncompliant

    public override int GetHashCode() => Degrees;
}
```

## Compliant solution

```csharp
public struct Temperature
{
    public int Degrees;

    public override bool Equals(object obj) => obj is Temperature t && t.Degrees == Degrees;

    public override int GetHashCode() => Degrees;

    public static bool operator ==(Temperature left, Temperature right) => left.Degrees == right.Degrees;

    public static bool operator !=(Temperature left, Temperature right) => !(left == right);
}
```

## See also

- [How to define value equality for a type](https://learn.microsoft.com/dotnet/csharp/programming-guide/statements-expressions-operators/how-to-define-value-equality-for-a-type)
