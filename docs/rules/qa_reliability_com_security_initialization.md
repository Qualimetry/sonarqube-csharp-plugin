# COM security initialization calls should be reviewed

`qa_reliability_com_security_initialization` &middot; Reliability &middot; Security Hotspot &middot; severity CRITICAL &middot; enabled in the recommended profile

Calls such as `CoInitializeSecurity` and `CoSetProxyBlanket` set the authentication, impersonation, and authorization levels for COM communication across the whole process. A weak level set here applies to every subsequent proxy, so a single mistake silently lowers the trust boundary for all later interop.

These functions should be confined to an audited interop layer where the chosen security levels are explicit and reviewed, rather than called ad hoc from ordinary code.

## Noncompliant code example

```csharp
internal static class Interop
{
    public static void Configure()
    {
        CoInitializeSecurity(IntPtr.Zero, -1, IntPtr.Zero, IntPtr.Zero, 0, 3, IntPtr.Zero, 0, IntPtr.Zero); // Noncompliant
    }
}
```

## Compliant solution

```csharp
internal static class Interop
{
    public static void Configure()
    {
        ConfigureManagedSecurity();
    }
}
```

## See also

- [CWE-749: Exposed Dangerous Method or Function](https://cwe.mitre.org/data/definitions/749.html)
