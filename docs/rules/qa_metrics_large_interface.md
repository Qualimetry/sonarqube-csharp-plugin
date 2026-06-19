# Interfaces should not declare too many members

`qa_metrics_large_interface` &middot; Metrics &middot; Code Smell &middot; severity MAJOR &middot; optional

A wide interface forces every implementer to satisfy the whole surface, even when a caller needs only a slice of it. Counting the members an interface declares highlights contracts that have grown into catch-alls and are candidates for splitting along the lines their callers actually use.

The member limit is configurable.

## Noncompliant code example

```csharp
public interface IDevice
{
    void Open();
    void Close();
    void Read();
    void Write();
    void Reset();
    void Calibrate();
    void Diagnose();
}
```

## Compliant solution

```csharp
public interface IReadableDevice
{
    void Open();
    void Read();
    void Close();
}

public interface IWritableDevice
{
    void Open();
    void Write();
    void Close();
}
```

## Parameters

This rule is configurable. Edit `maxmembers` (default `20`) on the rule in your quality profile; SonarQube applies the value during analysis and synchronises it to connected IDEs.
