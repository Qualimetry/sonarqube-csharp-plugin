# Avoid exposing concrete XmlNode return types

`qa_quality_concrete_xml_node_return_type` &middot; CodeQuality &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

Returning a concrete `XmlNode`, `XmlElement`, or `XmlDocument` ties every caller to the legacy DOM implementation and to the mutable object graph behind it. The API can no longer switch to a different XML representation without breaking consumers.

Expose a reader, a projected model, or an interface that conveys only what the caller needs.

## Noncompliant code example

```csharp
public XmlElement LoadConfig() // Noncompliant
{
    var document = new XmlDocument();
    document.LoadXml("<config/>");
    return document.DocumentElement;
}
```

## Compliant solution

```csharp
public XmlReader LoadConfig()
{
    return XmlReader.Create(new StringReader("<config/>"));
}
```

## See also

- [XML processing in .NET](https://learn.microsoft.com/dotnet/standard/data/xml/)
