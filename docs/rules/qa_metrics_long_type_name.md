# Type names should not be excessively long

`qa_metrics_long_type_name` &middot; Metrics &middot; Code Smell &middot; severity INFO &middot; optional

A type name that runs very long usually folds its namespace, its role, and several qualifiers into one identifier. Shorter names that lean on the namespace for context read better at every use site.

The maximum length is configurable.

## Noncompliant code example

```csharp
public sealed class GenericRepositoryFactoryConfigurationProviderBuilder
{
}
```

## Compliant solution

```csharp
public sealed class RepositoryBuilder
{
}
```

## Parameters

This rule is configurable. Edit `maxnamelength` (default `40`) on the rule in your quality profile; SonarQube applies the value during analysis and synchronises it to connected IDEs.
