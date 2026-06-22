# Awaited tasks in reusable code should specify ConfigureAwait

`qa_style_missing_configure_await` &middot; Style &middot; Bug &middot; severity MAJOR &middot; optional

By default `await` captures the current synchronization context and resumes on it. In library and other reusable code that capture is rarely needed and, on contexts with a single UI or request thread, resuming on it can deadlock when a caller blocks on the task.

Stating `ConfigureAwait(false)` (or `true` where the context really is required) makes the intent explicit at every await and avoids the accidental capture.

## Noncompliant code example

```csharp
using System.Threading.Tasks;

public class Loader
{
    public async Task RunAsync()
    {
        await FetchAsync(); // Noncompliant
    }

    private Task FetchAsync() => Task.CompletedTask;
}
```

## Compliant solution

```csharp
using System.Threading.Tasks;

public class Loader
{
    public async Task RunAsync()
    {
        await FetchAsync().ConfigureAwait(false);
    }

    private Task FetchAsync() => Task.CompletedTask;
}
```
