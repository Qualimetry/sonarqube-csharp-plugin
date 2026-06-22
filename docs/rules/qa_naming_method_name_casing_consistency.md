# Methods in a type should not differ only by letter case

`qa_naming_method_name_casing_consistency` &middot; Naming &middot; Code Smell &middot; severity MINOR &middot; optional

When two methods on the same type share a name that differs only in capitalization, callers cannot tell them apart at a glance and the pair is invalid for any consumer that treats identifiers case-insensitively. Picking one spelling removes the ambiguity and keeps the public surface predictable.

## Noncompliant code example

```csharp
public class Mailbox
{
    public void Send() { }

    public void send() { } // Noncompliant
}
```

## Compliant solution

```csharp
public class Mailbox
{
    public void Send() { }

    public void Receive() { }
}
```

## See also

- [Capitalization conventions](https://learn.microsoft.com/dotnet/standard/design-guidelines/capitalization-conventions)
