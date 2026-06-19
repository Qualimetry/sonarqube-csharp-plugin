# Field names should not be excessively long

`qa_metrics_long_field_name` &middot; Metrics &middot; Code Smell &middot; severity INFO &middot; optional

An identifier that runs on for dozens of characters is usually a sign that the name is encoding context that the surrounding type or namespace already supplies. Long field names slow reading and push lines past comfortable widths.

The maximum length is configurable so teams can match their own house style.

## Noncompliant code example

```csharp
public sealed class Report
{
    private int theTotalNumberOfProcessedRecordsAcrossEveryBatch;
}
```

## Compliant solution

```csharp
public sealed class Report
{
    private int processedRecordCount;
}
```

## Parameters

This rule is configurable. Edit `maxnamelength` (default `40`) on the rule in your quality profile; SonarQube applies the value during analysis and synchronises it to connected IDEs.
