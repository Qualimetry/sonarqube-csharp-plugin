# ThreadStatic should only be applied to static fields

`qa_quality_thread_static_on_instance_field` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

The `ThreadStatic` attribute gives each thread its own copy of a field, but the runtime only honors it on `static` fields. Placing it on an instance field compiles without complaint yet does nothing, so code that relies on per-thread isolation silently shares one value across threads.

Make the field `static` if per-thread storage is the intent, or remove the attribute if it was applied by mistake. For per-thread instance state, use `ThreadLocal<T>` instead.

## Noncompliant code example

```csharp
public class Context
{
    [ThreadStatic]
    private int depth; // Noncompliant
}
```

## Compliant solution

```csharp
public class Context
{
    [ThreadStatic]
    private static int depth;
}
```

## See also

- [ThreadStaticAttribute](https://learn.microsoft.com/dotnet/api/system.threadstaticattribute)
