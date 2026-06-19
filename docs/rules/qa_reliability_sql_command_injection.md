# Database command text should not be built by string concatenation

`qa_reliability_sql_command_injection` &middot; Reliability &middot; Security Hotspot &middot; severity CRITICAL &middot; enabled in the recommended profile

When the `CommandText` of a database command, or the SQL passed to a command constructor, is assembled from string concatenation, interpolation, or `string.Format`, any value that reaches it becomes part of the executed statement. An attacker who controls that value can change the meaning of the query, read data they should not see, or destroy it.

Keep the SQL text constant and pass every runtime value as a command parameter so the database treats it strictly as data.

## Noncompliant code example

```csharp
public void Find(System.Data.IDbCommand command, string name)
{
    command.CommandText = "SELECT * FROM Users WHERE Name = '" + name + "'"; // Noncompliant
}
```

## Compliant solution

```csharp
public void Find(System.Data.IDbCommand command, string name)
{
    command.CommandText = "SELECT * FROM Users WHERE Name = @name";
    var parameter = command.CreateParameter();
    parameter.ParameterName = "@name";
    parameter.Value = name;
    command.Parameters.Add(parameter);
}
```

## See also

- [CWE-89: Improper Neutralization of Special Elements used in an SQL Command](https://cwe.mitre.org/data/definitions/89.html)
