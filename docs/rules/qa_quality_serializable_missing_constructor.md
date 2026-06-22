# Serializable types must provide a deserialization constructor

`qa_quality_serializable_missing_constructor` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

A type that implements `ISerializable` promises to rebuild itself from a `SerializationInfo`, but that contract is only complete when the type also declares a constructor accepting `SerializationInfo` and `StreamingContext`. Without it the runtime cannot reconstruct an instance and throws at deserialization time.

Declare the matching constructor (protected on an unsealed type, private on a sealed one) so the serialization round trip works for both the writing and the reading side.

## Noncompliant code example

```csharp
[Serializable]
public class Money : ISerializable // Noncompliant
{
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
    }
}
```

## Compliant solution

```csharp
[Serializable]
public class Money : ISerializable
{
    public Money()
    {
    }

    protected Money(SerializationInfo info, StreamingContext context)
    {
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
    }
}
```

## See also

- [ISerializable interface](https://learn.microsoft.com/dotnet/api/system.runtime.serialization.iserializable)
