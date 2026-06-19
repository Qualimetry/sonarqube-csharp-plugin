# Types should not derive from XmlDocument

`qa_quality_xml_document_base_type` &middot; CodeQuality &middot; Bug &middot; severity MAJOR &middot; enabled in the recommended profile

`XmlDocument` is a large, non-virtual-friendly class that was never designed as a base type. A subclass inherits dozens of members it cannot meaningfully override and becomes tightly bound to an implementation that is expensive and hard to evolve.

Hold an `XmlDocument` as a private field and expose only the operations the type actually needs.

## Noncompliant code example

```csharp
using System.Xml;

public class ConfigDocument : XmlDocument // Noncompliant
{
}
```

## Compliant solution

```csharp
using System.Xml;

public class ConfigDocument
{
    private readonly XmlDocument _document = new XmlDocument();

    public void Load(string path) => _document.Load(path);
}
```

## See also

- [Composition over inheritance](https://learn.microsoft.com/dotnet/standard/design-guidelines/choosing-between-class-and-struct)
