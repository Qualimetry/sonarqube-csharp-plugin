# Platform invoke declarations should live in a dedicated native methods class

`qa_interop_platform_invoke_in_native_methods` &middot; Interop &middot; Code Smell &middot; severity MINOR &middot; optional

Each `extern` method carrying a `DllImport` attribute crosses the managed boundary and runs unmanaged code with no verification. Scattering these declarations across ordinary business classes hides the unsafe surface and makes it hard to audit which calls reach native libraries.

Grouping every platform invoke into a class whose name ends with `NativeMethods` keeps the native surface in one reviewable place, so security and lifetime concerns can be reasoned about together rather than chased through unrelated types.

## Noncompliant code example

```csharp
using System;
using System.Runtime.InteropServices;

public static class FileService
{
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool CloseHandle(IntPtr handle); // Noncompliant
}
```

## Compliant solution

```csharp
using System;
using System.Runtime.InteropServices;

internal static class NativeMethods
{
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool CloseHandle(IntPtr handle);
}
```

## See also

- [Native interoperability best practices](https://learn.microsoft.com/dotnet/standard/native-interop/best-practices)
