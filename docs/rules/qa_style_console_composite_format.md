# Composite formatting in console output should use string interpolation

`qa_style_console_composite_format` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

Passing a format string with numbered placeholders and a separate argument list to `Console.Write` or `Console.WriteLine` forces the reader to match each `{0}` to its argument by position. A misnumbered or missing placeholder is only caught at run time.

An interpolated string puts each value where it appears in the text, so the output reads in order and the compiler checks every expression.

## Noncompliant code example

```csharp
using System;

public class Greeter
{
    public void Hello(string name)
    {
        Console.WriteLine("Hello {0}", name); // Noncompliant
    }
}
```

## Compliant solution

```csharp
using System;

public class Greeter
{
    public void Hello(string name)
    {
        Console.WriteLine($"Hello {name}");
    }
}
```
