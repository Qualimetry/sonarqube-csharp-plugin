# Avoid reflection calls inside loops

`qa_quality_reflection_inside_loop` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

Reflection lookups and invocations such as `MethodInfo.Invoke`, `PropertyInfo.GetValue`, or `Activator.CreateInstance` are far slower than direct calls. Performing them on every iteration of a loop multiplies that cost and often dominates the loop body.

Resolve the member once before the loop and reuse it, cache a compiled delegate, or replace the reflection with a direct call where the type is known.

## Noncompliant code example

```csharp
foreach (var item in items)
{
    var value = property.GetValue(item); // Noncompliant
    Process(value);
}
```

## Compliant solution

```csharp
foreach (var item in items)
{
    var value = item.Value;
    Process(value);
}
```

## See also

- [Reflection in .NET](https://learn.microsoft.com/dotnet/fundamentals/reflection/reflection)
