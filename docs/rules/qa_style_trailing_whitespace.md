# Lines should not end with trailing whitespace

`qa_style_trailing_whitespace` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

Spaces or tabs sitting at the end of a line are invisible in most editors yet they pollute diffs, trigger noisy version-control changes, and can break here-strings or shell snippets embedded in source. They carry no meaning and should not survive review.

## Noncompliant code example

```csharp
public class Account
{
    public int Balance { get; set; }   
}
```

## Compliant solution

```csharp
public class Account
{
    public int Balance { get; set; }
}
```
