# XML doc should not describe a nonexistent parameter

`qa_quality_xml_doc_nonexistent_parameter` &middot; CodeQuality &middot; Code Smell &middot; severity MINOR &middot; optional

A `<param>` tag whose name does not match any parameter of the documented member is stale documentation. It usually means a parameter was renamed or removed and the comment was left behind, so readers and documentation generators are told about an argument that the method does not accept.

Update the tag to the actual parameter name, or delete it.

## Noncompliant code example

```csharp
public class Converter
{
    /// <summary>Converts a value.</summary>
    /// <param name="input">The value.</param>
    /// <param name="culture">Unused.</param>
    public string Convert(string input) // Noncompliant
    {
        return input;
    }
}
```

## Compliant solution

```csharp
public class Converter
{
    /// <summary>Converts a value.</summary>
    /// <param name="input">The value.</param>
    public string Convert(string input)
    {
        return input;
    }
}
```
