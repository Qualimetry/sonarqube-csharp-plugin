# Native methods container should be internal and static

`qa_interop_native_methods_container_modifiers` &middot; Interop &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

A class that gathers platform invoke declarations is a private implementation detail of one assembly. Exposing it as `public` leaks the unmanaged surface to callers who should never reach native code directly, and leaving it instantiable invites pointless objects that carry no state.

Declaring the container `internal static` keeps the native entry points hidden behind the assembly boundary and signals that the type is a stateless gateway, not something to construct.

## Noncompliant code example

```csharp
public class NativeMethods // Noncompliant
{
}
```

## Compliant solution

```csharp
internal static class NativeMethods
{
}
```

## See also

- [Native interoperability best practices](https://learn.microsoft.com/dotnet/standard/native-interop/best-practices)
