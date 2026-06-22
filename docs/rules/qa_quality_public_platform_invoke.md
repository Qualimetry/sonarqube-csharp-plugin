# Platform invoke declarations should not be publicly visible

`qa_quality_public_platform_invoke` &middot; CodeQuality &middot; Security Hotspot &middot; severity CRITICAL &middot; enabled in the recommended profile

A `[DllImport]` method declared `public` or `protected` exposes a raw native entry point to every consumer of the assembly. Callers bypass any managed validation around it and invoke unmanaged code directly, which is both a security risk and a brittle contract.

Keep interop declarations internal or private inside a dedicated native-methods class, and surface a safe managed wrapper instead.

## Noncompliant code example

```csharp
using System.Runtime.InteropServices;

public class Native
{
    [DllImport("kernel32.dll")]
    public static extern int GetCurrentProcessId(); // Noncompliant
}
```

## Compliant solution

```csharp
using System.Runtime.InteropServices;

internal static class Native
{
    [DllImport("kernel32.dll")]
    internal static extern int GetCurrentProcessId();
}
```

## See also

- [CA1401: P/Invokes should not be visible](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1401)
