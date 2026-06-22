# Dispose objects a method creates and owns

`qa_quality_undisposed_disposable_local` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; optional

A local that holds a freshly created `IDisposable` owns that resource. If the method never disposes it, never returns it, and never hands ownership to another component, the underlying handle (file, socket, connection) leaks until the finalizer eventually runs, if ever.

Wrap the local in a `using` declaration or call `Dispose` on every exit path.

## Noncompliant code example

```csharp
public void Save(string path)
{
    var stream = new FileStream(path, FileMode.Create); // Noncompliant
    stream.WriteByte(0);
}
```

## Compliant solution

```csharp
public void Save(string path)
{
    using var stream = new FileStream(path, FileMode.Create);
    stream.WriteByte(0);
}
```

## See also

- [Using objects that implement IDisposable](https://learn.microsoft.com/dotnet/standard/garbage-collection/using-objects)
