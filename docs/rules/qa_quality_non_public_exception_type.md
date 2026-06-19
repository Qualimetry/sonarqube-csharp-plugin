# Declare exception types as public

`qa_quality_non_public_exception_type` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; optional

An exception that escapes an assembly but is declared `internal` (or otherwise non-public) cannot be named in a `catch` clause by the calling code. Consumers are forced to catch a broader base type, losing the ability to react to the specific failure.

Make any exception that can reach external callers `public`, or keep it private only if it is always caught within the same assembly.

## Noncompliant code example

```csharp
internal sealed class ImportFailedException : Exception // Noncompliant
{
    public ImportFailedException(string message) : base(message) { }
}
```

## Compliant solution

```csharp
public sealed class ImportFailedException : Exception
{
    public ImportFailedException(string message) : base(message) { }
}
```

## See also

- [Creating and throwing exceptions](https://learn.microsoft.com/dotnet/standard/exceptions/how-to-create-user-defined-exceptions)
