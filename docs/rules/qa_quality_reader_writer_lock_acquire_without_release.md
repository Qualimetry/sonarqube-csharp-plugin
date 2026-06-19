# ReaderWriterLock acquire and release should be paired in one method

`qa_quality_reader_writer_lock_acquire_without_release` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

Acquiring a `ReaderWriterLock` with `AcquireReaderLock` or `AcquireWriterLock` and not releasing it in the same method leaves the lock held when control leaves through an exception or an early return. Subsequent readers and writers then block indefinitely.

Release the lock in a `finally` within the same method so every acquisition is matched no matter how the method exits.

## Noncompliant code example

```csharp
using System.Threading;

public class Store
{
    private readonly ReaderWriterLock _lock = new ReaderWriterLock();

    public void Append()
    {
        _lock.AcquireWriterLock(1000); // Noncompliant
        Write();
    }

    private void Write()
    {
    }
}
```

## Compliant solution

```csharp
using System.Threading;

public class Store
{
    private readonly ReaderWriterLock _lock = new ReaderWriterLock();

    public void Append()
    {
        _lock.AcquireWriterLock(1000);
        try
        {
            Write();
        }
        finally
        {
            _lock.ReleaseWriterLock();
        }
    }

    private void Write()
    {
    }
}
```

## See also

- [ReaderWriterLockSlim (.NET API)](https://learn.microsoft.com/dotnet/api/system.threading.readerwriterlockslim)
