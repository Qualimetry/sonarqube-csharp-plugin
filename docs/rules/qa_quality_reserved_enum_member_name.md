# Enum members should not be named Reserved

`qa_quality_reserved_enum_member_name` &middot; CodeQuality &middot; Code Smell &middot; severity MINOR &middot; optional

Naming an enum member `Reserved` bakes a placeholder into a public contract. It tells callers nothing about what the value means, yet it occupies a real numeric slot they can serialize, persist, and switch on. If a value is needed later, add it with a descriptive name then; reserving an ordinal in advance only leaves a meaningless option in every consumer's list.

## Noncompliant code example

```csharp
public enum Channel
{
    Email,
    Sms,
    Reserved // Noncompliant
}
```

## Compliant solution

```csharp
public enum Channel
{
    Email,
    Sms,
    Push
}
```
