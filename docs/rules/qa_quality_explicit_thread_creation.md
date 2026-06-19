# Prefer task-based concurrency over manual thread creation

`qa_quality_explicit_thread_creation` &middot; CodeQuality &middot; Code Smell &middot; severity CRITICAL &middot; enabled in the recommended profile

Constructing a `Thread` by hand allocates a dedicated OS thread, bypasses the managed thread pool, and leaves you to coordinate lifetime, exceptions, and results yourself. Most workloads are better expressed with `Task.Run` or the higher-level task abstractions, which pool threads, propagate exceptions, and compose with `async`/`await`.

## Noncompliant code example

```csharp
var worker = new Thread(DoWork); // Noncompliant
worker.Start();
```

## Compliant solution

```csharp
Task.Run(DoWork);
```

## See also

- [Task-based asynchronous programming](https://learn.microsoft.com/dotnet/standard/parallel-programming/task-based-asynchronous-programming)
