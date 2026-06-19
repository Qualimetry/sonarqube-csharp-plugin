# Marker interfaces with no members should be avoided

`qa_quality_empty_interface` &middot; CodeQuality &middot; Code Smell &middot; severity MINOR &middot; optional

An interface with no members and no base interfaces declares no behaviour. It is used only as a tag the runtime can test for, which couples consumers to a type-check rather than to a capability and cannot be applied to types that are already sealed by someone else. Where the goal is metadata, an attribute expresses intent more directly; where the goal is a contract, the interface should declare the members that contract requires.

## Noncompliant code example

```csharp
public interface IEntity // Noncompliant
{
}
```

## Compliant solution

```csharp
public interface IEntity
{
    int Id { get; }
}
```
