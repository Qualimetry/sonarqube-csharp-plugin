# Namespace names should match the source file folder path

`qa_naming_namespace_folder_mismatch` &middot; Naming &middot; Code Smell &middot; severity MINOR &middot; optional

Keeping namespace segments aligned with folders makes it easy to locate types and prevents drift between physical layout and logical ownership.

## Noncompliant code example

```csharp
namespace Company.Product.Billing
{
    public sealed class InvoiceService
    {
    }
}
```

## Compliant solution

```csharp
namespace Company.Product.Billing
{
    public sealed class InvoiceService
    {
    }
}
```
