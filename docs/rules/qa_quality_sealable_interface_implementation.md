# Methods implementing a non-public interface should be sealed

`qa_quality_sealable_interface_implementation` &middot; CodeQuality &middot; Security Hotspot &middot; severity MAJOR &middot; enabled in the recommended profile

When a public, overridable method exists only to satisfy an interface that is not part of the assembly's public surface, leaving it open invites accidental overrides that no caller of the interface ever needed. Sealing the member states that the implementation is final and lets the runtime devirtualise the call.

Mark the implementing member `sealed`, or make the interface itself public if extension was intended.

## Noncompliant code example

```csharp
internal interface IHandler
{
    void Handle();
}

public class Worker : IHandler
{
    public virtual void Handle() // Noncompliant
    {
    }
}
```

## Compliant solution

```csharp
internal interface IHandler
{
    void Handle();
}

public class Worker : IHandler
{
    public void Handle()
    {
    }
}
```
