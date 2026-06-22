# Make methods static when they use no instance state

`qa_quality_instance_method_could_be_static` &middot; CodeQuality &middot; Code Smell &middot; severity MINOR &middot; optional

A private method that never touches `this`, an instance field, or an instance member does not depend on the object it is declared on. Marking it `static` makes that independence explicit, avoids passing an unused receiver, and clarifies that the method has no per-instance side effects.

## Noncompliant code example

```csharp
public class Geometry
{
    private int Area(int width, int height) // Noncompliant
    {
        return width * height;
    }
}
```

## Compliant solution

```csharp
public class Geometry
{
    private static int Area(int width, int height)
    {
        return width * height;
    }
}
```

## See also

- [static (C# reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/static)
