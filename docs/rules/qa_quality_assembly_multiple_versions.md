# Assemblies should not be referenced at multiple versions

`qa_quality_assembly_multiple_versions` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; optional

Loading two versions of the same assembly in one application causes binding conflicts and subtle runtime failures. Align package references to a single version.

## Noncompliant code example

```csharp
public class App
{
}
```

## Compliant solution

```csharp
public class App
{
}
```
