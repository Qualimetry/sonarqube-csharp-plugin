# JSON string literals should be well-formed

`qa_reliability_invalid_json_literal` &middot; Reliability &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

When a constant string is handed to a JSON parser, malformed content is only discovered when the line runs and throws. A missing brace, a trailing comma, or an unquoted key turns into a runtime parse failure that a quick check of the literal would have caught.

Fix the literal so it is valid JSON before it reaches the parser.

## Noncompliant code example

```csharp
public sealed class Config
{
    public object Load() => Json.Parse("{ \"enabled\": true, }"); // Noncompliant
}
```

## Compliant solution

```csharp
public sealed class Config
{
    public object Load() => Json.Parse("{ \"enabled\": true }");
}
```

## See also

- [RFC 8259: The JavaScript Object Notation (JSON) Data Interchange Format](https://www.rfc-editor.org/rfc/rfc8259)
