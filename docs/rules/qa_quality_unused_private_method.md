# Private method is never used

`qa_quality_unused_private_method` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

A private method that is never called within its declaring type is unreachable code. It cannot be invoked from outside, so the reader is left guessing whether it is a forgotten helper, an unfinished feature, or a hazard waiting to be wired up incorrectly.

Delete the method, or call it from the code path it was written for.

## Noncompliant code example

```csharp
public class ReportBuilder
{
    public string Build() => Header();

    private string Header() => "Report";

    private string Footer() => "End"; // Noncompliant
}
```

## Compliant solution

```csharp
public class ReportBuilder
{
    public string Build() => Header() + Footer();

    private string Header() => "Report";

    private string Footer() => "End";
}
```
