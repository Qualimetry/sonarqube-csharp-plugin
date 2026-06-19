# Types holding disposable fields should be disposable

`qa_quality_disposable_field_in_non_disposable_type` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

When a type keeps an `IDisposable` in an instance field, it owns that resource for the life of the instance. If the type itself is not disposable, there is no point at which the held resource can be released, so it survives until finalization or leaks outright.

Implement `IDisposable` on the owning type and dispose the field in `Dispose`.

## Noncompliant code example

```csharp
using System.IO;

public class Recorder // Noncompliant
{
    private readonly MemoryStream _stream = new MemoryStream();

    public long Size => _stream.Length;
}
```

## Compliant solution

```csharp
using System;
using System.IO;

public class Recorder : IDisposable
{
    private readonly MemoryStream _stream = new MemoryStream();

    public long Size => _stream.Length;

    public void Dispose() => _stream.Dispose();
}
```

## See also

- [Implement a Dispose method](https://learn.microsoft.com/dotnet/standard/garbage-collection/implementing-dispose)
