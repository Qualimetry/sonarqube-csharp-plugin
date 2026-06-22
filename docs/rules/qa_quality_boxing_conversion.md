# Boxing and unboxing conversions should be avoided

`qa_quality_boxing_conversion` &middot; CodeQuality &middot; Code Smell &middot; severity MINOR &middot; optional

Boxing value types onto the heap and unboxing them back adds allocation and runtime cost on hot paths. Prefer generic APIs, typed overloads, or ref-based patterns that keep values on the stack.

## Noncompliant code example

```csharp
public sealed class Counter
{
    public void Report(object value)
    {
        System.Console.WriteLine(value);
    }

    public void Emit(int count)
    {
        Report(count);
    }
}
```

## Compliant solution

```csharp
public sealed class Counter
{
    public void Report(int value)
    {
        System.Console.WriteLine(value);
    }

    public void Emit(int count)
    {
        Report(count);
    }
}
```
