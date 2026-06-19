# Obsolete APIs should not be referenced

`qa_quality_obsolete_member_usage` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

An API marked with `[Obsolete]` has been retired by its author: it may behave differently than expected, carry known defects, or disappear entirely in a future version. Continuing to call it ties new code to a contract that is already on its way out.

Move to the replacement the attribute points to, or drop the dependency on the deprecated member, so the code stays aligned with the surface its libraries actually support.

## Noncompliant code example

```csharp
public class Report
{
    [System.Obsolete("Use BuildV2 instead.")]
    public string Build() => "v1";

    public string Render() => Build(); // Noncompliant
}
```

## Compliant solution

```csharp
public class Report
{
    [System.Obsolete("Use BuildV2 instead.")]
    public string Build() => "v1";

    public string BuildV2() => "v2";

    public string Render() => BuildV2();
}
```

## See also

- [ObsoleteAttribute (.NET API)](https://learn.microsoft.com/dotnet/api/system.obsoleteattribute)
