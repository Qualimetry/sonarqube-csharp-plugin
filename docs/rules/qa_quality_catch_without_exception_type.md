# Catch clauses should name an exception type

`qa_quality_catch_without_exception_type` &middot; CodeQuality &middot; Bug &middot; severity MINOR &middot; enabled in the recommended profile

A general `catch` with no exception type swallows everything, including the cancellation, out-of-memory, and stack-overflow conditions that the code is in no position to handle. The reader cannot tell which failure the author actually expected here.

Name the specific exception type the clause handles. When a broad catch is genuinely intended, write `catch (Exception)` so the intent is explicit.

## Noncompliant code example

```csharp
using System;

public class Worker
{
    public void Run()
    {
        try
        {
            Execute();
        }
        catch // Noncompliant
        {
        }
    }

    private void Execute()
    {
    }
}
```

## Compliant solution

```csharp
using System;

public class Worker
{
    public void Run()
    {
        try
        {
            Execute();
        }
        catch (InvalidOperationException)
        {
        }
    }

    private void Execute()
    {
    }
}
```

## See also

- [Exception handling (C# reference)](https://learn.microsoft.com/dotnet/csharp/fundamentals/exceptions/exception-handling)
