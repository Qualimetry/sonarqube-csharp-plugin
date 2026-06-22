# Nested type members should not mask an outer static member

`qa_quality_nested_type_masks_outer_static_member` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

When a nested type declares a member with the same name as a static member of an enclosing type, an unqualified reference inside the nested type now binds to the nested member instead of the outer one. A reader who expects the enclosing static value is silently wrong, and the bug only surfaces at run time.

Give the nested member a distinct name so the two never compete for the same unqualified reference.

## Noncompliant code example

```csharp
public class Registry
{
    public static int Version;

    public class Entry
    {
        public int Version; // Noncompliant
    }
}
```

## Compliant solution

```csharp
public class Registry
{
    public static int Version;

    public class Entry
    {
        public int EntryVersion;
    }
}
```

## See also

- [Nested types (C# reference)](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/nested-types)
