# Abstract base classes should carry a 'Base' suffix

`qa_naming_abstract_base_class_suffix` &middot; Naming &middot; Code Smell &middot; severity MINOR &middot; optional

A consistent `Base` suffix on an `abstract` class signals at the point of use that the type is meant to be inherited rather than instantiated. The suffix can be turned off through configuration where a project follows a different convention.

## Noncompliant code example

```csharp
public abstract class Repository // Noncompliant
{
    public abstract void Save();
}
```

## Compliant solution

```csharp
public abstract class RepositoryBase
{
    public abstract void Save();
}
```

## Parameters

This rule is configurable. Edit `suffix` (default `Base`) on the rule in your quality profile; SonarQube applies the value during analysis and synchronises it to connected IDEs.

## See also

- [Names of classes, structs, and interfaces](https://learn.microsoft.com/dotnet/standard/design-guidelines/names-of-classes-structs-and-interfaces)
