# Private method has an unused parameter

`qa_quality_unused_parameter` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; optional

A parameter that is never read inside a private method is dead weight on the signature. Every caller is forced to compute and pass a value that has no effect, and the unused parameter often hides a bug where the value was meant to be used but never was.

Remove the parameter, or use it in the method body as intended.

## Noncompliant code example

```csharp
public class Mailer
{
    public void Send(string to) => Format(to, true);

    private string Format(string to, bool html) // Noncompliant
    {
        return to.Trim();
    }
}
```

## Compliant solution

```csharp
public class Mailer
{
    public void Send(string to) => Format(to);

    private string Format(string to)
    {
        return to.Trim();
    }
}
```
