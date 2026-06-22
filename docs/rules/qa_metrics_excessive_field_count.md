# Types should not declare too many fields

`qa_metrics_excessive_field_count` &middot; Metrics &middot; Code Smell &middot; severity MAJOR &middot; optional

A type carrying a large number of fields is usually holding several distinct pieces of state that change for different reasons. Counting the declared fields highlights types whose data wants to be grouped into smaller, more cohesive structures.

The field limit is configurable.

## Noncompliant code example

```csharp
public sealed class Widget
{
    private int _a;
    private int _b;
    private int _c;
    private int _d;
    private int _e;
    private int _f;
}
```

## Compliant solution

```csharp
public sealed class Widget
{
    private Position _position;
    private Style _style;
}
```

## Parameters

This rule is configurable. Edit `maxfields` (default `15`) on the rule in your quality profile; SonarQube applies the value during analysis and synchronises it to connected IDEs.
