# Private instance fields should start with an underscore

`qa_naming_instance_field_underscore` &middot; Naming &middot; Code Smell &middot; severity MINOR &middot; optional

A leading underscore on a private instance field distinguishes backing state from parameters and local variables of the same word, so an assignment inside a constructor or method reads unambiguously without a `this.` qualifier. The convention applies only to private instance fields; public members follow PascalCase instead.

## Noncompliant code example

```csharp
public class Cache
{
    private int count; // Noncompliant
}
```

## Compliant solution

```csharp
public class Cache
{
    private int _count;
}
```

## See also

- [Field naming conventions](https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/identifier-names)
