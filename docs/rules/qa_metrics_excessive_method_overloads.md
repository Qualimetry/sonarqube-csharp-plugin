# Methods should not have too many overloads

`qa_metrics_excessive_method_overloads` &middot; Metrics &middot; Code Smell &middot; severity MAJOR &middot; optional

A large family of overloads for one method name pushes the burden of choosing the right entry point onto every caller and tends to hide subtle behavioural differences between near-identical signatures. Counting the overloads of each name surfaces APIs that might be better served by optional parameters, a builder, or a single options object.

The overload limit is configurable.

## Noncompliant code example

```csharp
public sealed class Logger
{
    public void Log(string message) { }
    public void Log(string message, int level) { }
    public void Log(string message, int level, string category) { }
    public void Log(string message, int level, string category, bool toFile) { }
    public void Log(string message, int level, string category, bool toFile, bool toConsole) { }
}
```

## Compliant solution

```csharp
public sealed class Logger
{
    public void Log(LogEntry entry)
    {
    }
}
```

## Parameters

This rule is configurable. Edit `maxoverloads` (default `6`) on the rule in your quality profile; SonarQube applies the value during analysis and synchronises it to connected IDEs.
