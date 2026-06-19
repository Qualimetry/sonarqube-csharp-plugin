# Public collection members should have a plural name

`qa_naming_collection_member_plural_name` &middot; Naming &middot; Code Smell &middot; severity INFO &middot; optional

A public property or field whose type is an array or sequence holds many values, so a singular name misrepresents it to every caller. A plural name communicates the multiplicity at the use site and matches how the rest of the framework names its sequence-valued members.

## Noncompliant code example

```csharp
using System.Collections.Generic;

public class Order
{
    public List<string> Line { get; set; } // Noncompliant
}
```

## Compliant solution

```csharp
using System.Collections.Generic;

public class Order
{
    public List<string> Lines { get; set; }
}
```

## See also

- [Names of type members](https://learn.microsoft.com/dotnet/standard/design-guidelines/names-of-type-members)
