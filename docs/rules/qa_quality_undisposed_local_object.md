# Newly created disposables should not be discarded

`qa_quality_undisposed_local_object` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

Constructing an `IDisposable` as a standalone statement and ignoring the result leaks whatever it holds: a file handle, socket, or native buffer stays open until finalization, if it is reclaimed at all. The allocation had a side effect that is never undone.

Capture the instance in a `using` declaration or statement so it is disposed deterministically when the scope ends.

## Noncompliant code example

```csharp
using System.IO;

public class Loader
{
    public void Touch(string path)
    {
        new StreamReader(path); // Noncompliant
    }
}
```

## Compliant solution

```csharp
using System.IO;

public class Loader
{
    public string Touch(string path)
    {
        using var reader = new StreamReader(path);
        return reader.ReadToEnd();
    }
}
```

## See also

- [using statement (C# reference)](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/using)
