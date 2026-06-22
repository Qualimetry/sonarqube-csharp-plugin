# Fields should not be assigned from too many methods

`qa_metrics_field_assigned_from_many_methods` &middot; Metrics &middot; Code Smell &middot; severity MAJOR &middot; optional

When a single field is written from many methods, the rules that govern its value are scattered across the type and the order in which those methods run starts to matter in subtle ways. Counting the distinct methods that assign to a field highlights mutable state whose lifecycle has become hard to follow and which often wants to be wrapped behind a single setter or moved into its own object.

The limit on assigning methods is configurable.

## Noncompliant code example

```csharp
public sealed class Session
{
    private string _state;

    public void Open() { _state = "open"; }
    public void Pause() { _state = "paused"; }
    public void Resume() { _state = "open"; }
    public void Close() { _state = "closed"; }
}
```

## Compliant solution

```csharp
public sealed class Session
{
    private string _state;

    private void Transition(string next) { _state = next; }

    public void Open() => Transition("open");
    public void Close() => Transition("closed");
}
```

## Parameters

This rule is configurable. Edit `maxassigningmethods` (default `3`) on the rule in your quality profile; SonarQube applies the value during analysis and synchronises it to connected IDEs.
