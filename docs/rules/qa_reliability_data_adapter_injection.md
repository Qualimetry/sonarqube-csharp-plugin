# Data adapter queries should not be built by string concatenation

`qa_reliability_data_adapter_injection` &middot; Reliability &middot; Security Hotspot &middot; severity CRITICAL &middot; enabled in the recommended profile

A data adapter constructed with a SQL string that is assembled from concatenation, interpolation, or `string.Format` carries the same injection risk as a raw command: the runtime values become part of the statement the adapter runs to fill or update a dataset.

Build the adapter from a command whose text is constant and whose runtime values are supplied as parameters.

## Noncompliant code example

```csharp
public Adapter Build(string status)
{
    return new Adapter("SELECT * FROM Orders WHERE Status = '" + status + "'", _connection); // Noncompliant
}
```

## Compliant solution

```csharp
public Adapter Build(string status)
{
    var command = new Command("SELECT * FROM Orders WHERE Status = @status", _connection);
    command.AddParameter("@status", status);
    return new Adapter(command);
}
```

## See also

- [CWE-89: Improper Neutralization of Special Elements used in an SQL Command](https://cwe.mitre.org/data/definitions/89.html)
