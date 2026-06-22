# Switch sections should be enclosed in braces

`qa_style_switch_section_braces` &middot; Style &middot; Code Smell &middot; severity MINOR &middot; optional

A `switch` section that runs several statements without a block shares one scope with its neighbours, so a local declared in one section is visible in the others and it is easy to misread where a section ends. Wrapping each section in braces gives it its own scope and clear boundaries.

## Noncompliant code example

```csharp
switch (state)
{
    case State.Open:
        var message = Load(); // Noncompliant
        Send(message);
        break;
    case State.Closed:
        break;
}
```

## Compliant solution

```csharp
switch (state)
{
    case State.Open:
    {
        var message = Load();
        Send(message);
        break;
    }
    case State.Closed:
    {
        break;
    }
}
```

## See also

- [The switch statement (C# language reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/selection-statements#the-switch-statement)
