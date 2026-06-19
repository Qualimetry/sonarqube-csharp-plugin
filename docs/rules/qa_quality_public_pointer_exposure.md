# Native handles should not be exposed as public fields

`qa_quality_public_pointer_exposure` &middot; CodeQuality &middot; Security Hotspot &middot; severity CRITICAL &middot; enabled in the recommended profile

A publicly visible field of type `IntPtr` or `UIntPtr` hands a raw native pointer to every caller. They can overwrite it, copy it past the lifetime of the resource it points to, or pass it to unmanaged code, defeating any ownership or disposal guarantees the type tries to make.

Keep the handle private and offer a typed, validated member (a `SafeHandle`, a method, or a read-only property) instead.

## Noncompliant code example

```csharp
using System;

public class DeviceContext
{
    public IntPtr Handle; // Noncompliant
}
```

## Compliant solution

```csharp
using System;

public class DeviceContext
{
    private IntPtr _handle;

    public bool IsValid => _handle != IntPtr.Zero;
}
```

## See also

- [SafeHandle (.NET API)](https://learn.microsoft.com/dotnet/api/system.runtime.interopservices.safehandle)
