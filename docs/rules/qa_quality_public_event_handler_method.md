# Keep event handler methods non-public

`qa_quality_public_event_handler_method` &middot; CodeQuality &middot; Code Smell &middot; severity MINOR &middot; optional

A method shaped like an event handler (returning `void` and taking an `object` sender plus an `EventArgs` argument) is wired up by code that subscribes to an event, not called directly by other types. Exposing it publicly enlarges the API surface and invites callers to invoke it out of context.

## Noncompliant code example

```csharp
public void OnSaved(object sender, EventArgs e) // Noncompliant
{
    Refresh();
}
```

## Compliant solution

```csharp
private void OnSaved(object sender, EventArgs e)
{
    Refresh();
}
```

## See also

- [Handling and raising events](https://learn.microsoft.com/dotnet/standard/events/)
