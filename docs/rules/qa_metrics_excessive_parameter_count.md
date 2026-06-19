# Methods should not declare too many parameters

`qa_metrics_excessive_parameter_count` &middot; Metrics &middot; Code Smell &middot; severity MAJOR &middot; optional

A long parameter list is hard to read at the call site, easy to misorder, and usually a sign that several related values want to travel together inside a dedicated type. Counting the parameters on a method or constructor flags signatures that have outgrown a comfortable shape.

The parameter limit is configurable.

## Noncompliant code example

```csharp
public sealed class Mailer
{
    public void Send(string to, string from, string subject, string body, bool html, int priority, string replyTo)
    {
    }
}
```

## Compliant solution

```csharp
public sealed class Mailer
{
    public void Send(MailMessage message)
    {
    }
}
```

## Parameters

This rule is configurable. Edit `maxparameters` (default `7`) on the rule in your quality profile; SonarQube applies the value during analysis and synchronises it to connected IDEs.
