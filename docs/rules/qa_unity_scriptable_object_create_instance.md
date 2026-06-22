# Instantiate ScriptableObject types with CreateInstance

`qa_unity_scriptable_object_create_instance` &middot; Unity &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

A `ScriptableObject` is a Unity-managed object whose lifetime, serialisation and asset bindings are wired up by the engine, not by the C# constructor. Calling `new` on a type derived from `ScriptableObject` produces an instance the engine never registered, so its serialised fields are not restored and Unity may discard or mishandle it.

`ScriptableObject.CreateInstance` performs the engine-side construction that gives back a fully initialised, properly tracked instance. Use it for every ScriptableObject-derived type.

## Noncompliant code example

```csharp
public class SettingsFactory
{
    public GameSettings Build()
    {
        return new GameSettings(); // Noncompliant
    }
}
```

## Compliant solution

```csharp
public class SettingsFactory
{
    public GameSettings Build()
    {
        return ScriptableObject.CreateInstance<GameSettings>();
    }
}
```

## See also

- [ScriptableObject.CreateInstance (Unity scripting reference)](https://docs.unity3d.com/ScriptReference/ScriptableObject.CreateInstance.html)
