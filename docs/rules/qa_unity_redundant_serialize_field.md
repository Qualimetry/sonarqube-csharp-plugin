# SerializeField is redundant on a public field

`qa_unity_redundant_serialize_field` &middot; Unity &middot; Code Smell &middot; severity MINOR &middot; optional

Unity serialises public fields of a serialisable type automatically. The `SerializeField` attribute exists to opt a non-public field into that same serialisation, so applying it to a field that is already `public` adds nothing the engine was not going to do anyway.

The redundant attribute clutters the declaration and hints that the author was unsure of the serialisation rules. Drop the attribute from public fields, and keep it only where it changes behaviour, on private or internal fields you want serialised.

## Noncompliant code example

```csharp
public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    public int health; // Noncompliant
}
```

## Compliant solution

```csharp
public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private int health;
}
```

## See also

- [SerializeField (Unity scripting reference)](https://docs.unity3d.com/ScriptReference/SerializeField.html)
