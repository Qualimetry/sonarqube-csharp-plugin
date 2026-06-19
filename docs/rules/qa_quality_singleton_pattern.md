# The singleton pattern should be avoided

`qa_quality_singleton_pattern` &middot; CodeQuality &middot; Code Smell &middot; severity MINOR &middot; optional

A type that hides a private constructor behind a static `Instance` accessor builds global mutable state into the type itself. Every consumer reaches for it implicitly, which makes dependencies invisible, couples code to a single concrete instance, and makes the type almost impossible to substitute in a test. Register the type with a container and inject it, so its lifetime is explicit and a fake can take its place where needed.

## Noncompliant code example

```csharp
public sealed class ConfigStore // Noncompliant
{
    private static readonly ConfigStore _instance = new ConfigStore();

    private ConfigStore()
    {
    }

    public static ConfigStore Instance => _instance;
}
```

## Compliant solution

```csharp
public sealed class ConfigStore
{
    public ConfigStore()
    {
    }
}
```
