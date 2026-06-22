# 'string.Format' calls should prefer string interpolation

`qa_style_string_format_interpolation` &middot; Style &middot; Code Smell &middot; severity INFO &middot; optional

`string.Format` separates the layout text from the values it interleaves, so each `{0}` has to be cross-checked against the argument list and an index mistake surfaces only at run time. An interpolated string places every value inline where it is read.

Interpolation keeps the format and the data together and lets the compiler verify each embedded expression.

## Noncompliant code example

```csharp
public string Describe(string name, int age)
{
    return string.Format("{0} is {1}", name, age); // Noncompliant
}
```

## Compliant solution

```csharp
public string Describe(string name, int age)
{
    return $"{name} is {age}";
}
```
