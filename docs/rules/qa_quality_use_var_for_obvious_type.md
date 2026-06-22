# Use var when the type is evident from a constructor call

`qa_quality_use_var_for_obvious_type` &middot; CodeQuality &middot; Code Smell &middot; severity INFO &middot; enabled in the recommended profile

When a local variable is initialised with a constructor call whose type is spelled out on the right-hand side, repeating that type on the left adds nothing. The declaration reads the type name twice and any later change to the constructed type has to be made in two places.

Use `var` so the type is stated once, by the initializer.

## Noncompliant code example

```csharp
using System.Collections.Generic;

public class Loader
{
    public void Run()
    {
        List<string> names = new List<string>(); // Noncompliant
        names.Add("a");
    }
}
```

## Compliant solution

```csharp
using System.Collections.Generic;

public class Loader
{
    public void Run()
    {
        var names = new List<string>();
        names.Add("a");
    }
}
```
