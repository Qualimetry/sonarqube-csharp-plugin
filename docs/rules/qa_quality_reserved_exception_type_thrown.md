# Reserved exception types should not be thrown

`qa_quality_reserved_exception_type_thrown` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

Some exception types are reserved for the runtime to signal conditions it detects itself, such as `NullReferenceException`, `IndexOutOfRangeException`, or `OutOfMemoryException`. Throwing them from application code impersonates the runtime, so a handler can no longer tell a real fault from a deliberate one.

Throw an exception that describes the actual problem, for example `ArgumentNullException` for a missing argument or `ArgumentOutOfRangeException` for an out-of-bounds value, so callers can react to the real cause.

## Noncompliant code example

```csharp
public void Validate(string value)
{
    if (value == null)
    {
        throw new NullReferenceException(); // Noncompliant
    }
}
```

## Compliant solution

```csharp
public void Validate(string value)
{
    if (value == null)
    {
        throw new ArgumentNullException(nameof(value));
    }
}
```

## See also

- [Exception throwing guidelines](https://learn.microsoft.com/dotnet/standard/design-guidelines/exception-throwing)
