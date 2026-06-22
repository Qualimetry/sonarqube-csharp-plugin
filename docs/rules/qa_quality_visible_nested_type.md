# Avoid publicly visible nested types

`qa_quality_visible_nested_type` &middot; CodeQuality &middot; Code Smell &middot; severity MINOR &middot; optional

A `public` or `protected` nested type is awkward for consumers: it must be referenced through its containing type, complicates discovery, and usually signals that an independent concept was placed inside another type by accident. Promote it to namespace scope, or keep it nested but make it private when it is an implementation detail.

## Noncompliant code example

```csharp
public class Parser
{
    public class Options // Noncompliant
    {
        public bool Strict { get; set; }
    }
}
```

## Compliant solution

```csharp
public class ParserOptions
{
    public bool Strict { get; set; }
}

public class Parser
{
}
```

## See also

- [Nested Types (C# guide)](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/nested-types)
