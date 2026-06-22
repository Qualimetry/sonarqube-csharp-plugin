# ISerializable implementers should carry SerializableAttribute

`qa_quality_serializable_interface_missing_attribute` &middot; CodeQuality &middot; Code Smell &middot; severity CRITICAL &middot; enabled in the recommended profile

Implementing `ISerializable` states an intent to participate in serialization, but the runtime only treats a type as serializable when it also carries `[Serializable]`. Without the attribute the custom `GetObjectData` is never reached and serialization fails at run time.

Mark the type `[Serializable]` so the declared contract and the runtime behaviour agree.

## Noncompliant code example

```csharp
using System.Runtime.Serialization;

#pragma warning disable SYSLIB0051
public class Ticket : ISerializable // Noncompliant
{
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
    }
}
```

## Compliant solution

```csharp
using System;
using System.Runtime.Serialization;

#pragma warning disable SYSLIB0051
[Serializable]
public class Ticket : ISerializable
{
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
    }
}
```

## See also

- [SerializableAttribute (.NET API)](https://learn.microsoft.com/dotnet/api/system.serializableattribute)
