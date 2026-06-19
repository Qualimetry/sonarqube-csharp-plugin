# Event handler methods should not be public

`qa_quality_public_event_handler_exposed` &middot; CodeQuality &middot; Security Hotspot &middot; severity MINOR &middot; optional

A method shaped like an event handler, taking a sender and an `EventArgs` and named for the event it serves, is wiring rather than contract. Exposing it as `public` invites callers to invoke it directly, bypassing the event and coupling them to an internal detail of how the type reacts to notifications.

Give event handlers `private` or `protected` visibility, and expose a separately named public method when the behaviour is part of the contract.

## Noncompliant code example

```csharp
using System;

public class Uploader
{
    public void OnProgressChanged(object sender, EventArgs e) // Noncompliant
    {
    }
}
```

## Compliant solution

```csharp
using System;

public class Uploader
{
    private void OnProgressChanged(object sender, EventArgs e)
    {
    }
}
```
