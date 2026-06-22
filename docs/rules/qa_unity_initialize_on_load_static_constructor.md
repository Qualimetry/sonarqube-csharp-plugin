# Types marked InitializeOnLoad require a static constructor

`qa_unity_initialize_on_load_static_constructor` &middot; Unity &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

The `InitializeOnLoad` attribute tells the editor to run a type's static initialisation as soon as the project loads. Unity triggers that initialisation by invoking the type's static constructor, so a class carrying the attribute with no static constructor declares an intent it cannot honour.

Either add the static constructor that holds the load-time setup, or drop the attribute if no editor-load behaviour is needed. Leaving the attribute without a static constructor is dead intent that misleads the next reader.

## Noncompliant code example

```csharp
[InitializeOnLoad]
public class EditorBootstrap // Noncompliant
{
    public static bool Ready;
}
```

## Compliant solution

```csharp
[InitializeOnLoad]
public class EditorBootstrap
{
    public static bool Ready;

    static EditorBootstrap()
    {
        Ready = true;
    }
}
```

## See also

- [InitializeOnLoadAttribute (Unity scripting reference)](https://docs.unity3d.com/ScriptReference/InitializeOnLoadAttribute.html)
