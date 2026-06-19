# Types owning unmanaged resources should declare a finalizer

`qa_quality_unmanaged_resource_missing_finalizer` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

A type that stores a raw handle such as an `IntPtr` and implements `IDisposable` owns an unmanaged resource. If a caller forgets to call `Dispose`, nothing reclaims that handle and the process leaks operating-system resources.

Provide a finalizer as a backstop that frees the handle during garbage collection, or, preferably, wrap the handle in a `SafeHandle` so the cleanup is guaranteed without writing a finalizer by hand.

## Noncompliant code example

```csharp
public class FileWrapper : IDisposable // Noncompliant
{
    private IntPtr handle;

    public void Dispose()
    {
        Release(handle);
    }

    private static void Release(IntPtr h)
    {
    }
}
```

## Compliant solution

```csharp
public class FileWrapper : IDisposable
{
    private IntPtr handle;

    ~FileWrapper()
    {
        Release(handle);
    }

    public void Dispose()
    {
        Release(handle);
        GC.SuppressFinalize(this);
    }

    private static void Release(IntPtr h)
    {
    }
}
```

## See also

- [Implement a Dispose method](https://learn.microsoft.com/dotnet/standard/garbage-collection/implementing-dispose)
