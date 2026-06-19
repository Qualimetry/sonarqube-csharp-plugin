# Catch blocks should not be empty

`qa_quality_empty_catch_block` &middot; CodeQuality &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

An empty `catch` block discards an exception with no record that anything went wrong. The program continues in an unknown state, and the original failure becomes invisible to operators and to whoever later debugs the resulting corruption.

Do something deliberate in the handler: recover, translate the exception, log it with context, or rethrow. If catching is genuinely intentional, narrow the clause to the specific exception type and document why it is safe to continue.

## Noncompliant code example

```csharp
try
{
    File.Delete(path);
}
catch (IOException) // Noncompliant
{
}
```

## Compliant solution

```csharp
try
{
    File.Delete(path);
}
catch (IOException ex)
{
    logger.LogWarning(ex, "Could not delete {Path}", path);
}
```

## See also

- [Exception handling (C# programming guide)](https://learn.microsoft.com/dotnet/csharp/fundamentals/exceptions/)
