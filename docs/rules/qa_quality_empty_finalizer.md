# Empty finalizers should be removed

`qa_quality_empty_finalizer` &middot; CodeQuality &middot; Bug &middot; severity MAJOR &middot; enabled in the recommended profile

Declaring a finalizer, even an empty one, puts every instance of the type on the finalization queue. The garbage collector must then keep those objects alive for an extra generation and run a finalizer that does nothing, paying real cost for no effect.

Delete the empty finalizer. Add one back only when there is genuine unmanaged state to release, ideally through the dispose pattern.

## Noncompliant code example

```csharp
public class Buffer
{
    ~Buffer() // Noncompliant
    {
    }
}
```

## Compliant solution

```csharp
public class Buffer
{
}
```

## See also

- [Finalizers (C# reference)](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/finalizers)
