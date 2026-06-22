# Counting loops should bound the counter with a relational comparison

`qa_style_loop_counter_equality_comparison` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

Driving a counting `for` loop with an equality test such as `i != limit` assumes the counter lands exactly on the boundary. If the step ever skips past it, or the limit is recomputed, the loop runs far longer than intended.

A relational comparison stops the loop as soon as the counter reaches the bound, which is the safe and conventional way to express the range.

## Noncompliant code example

```csharp
for (int i = 0; i != count; i += 2) // Noncompliant
{
    Process(i);
}
```

## Compliant solution

```csharp
for (int i = 0; i < count; i += 2)
{
    Process(i);
}
```

## See also

- [The for statement (C# reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement)
