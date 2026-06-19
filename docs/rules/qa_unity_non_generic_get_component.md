# Prefer the generic GetComponent overload

`qa_unity_non_generic_get_component` &middot; Unity &middot; Code Smell &middot; severity MINOR &middot; optional

Resolving a component through a `Type` or string argument forces a runtime cast at the call site and gives back a loosely typed result that the caller has to convert before use. It also defers any naming mistake to runtime, where it surfaces as a failed lookup rather than a compile error.

The generic `GetComponent<T>()` overload returns the requested component already typed, so the cast disappears, the intent is explicit, and a wrong type name fails to compile rather than silently returning null.

## Noncompliant code example

```csharp
public class Mover : MonoBehaviour
{
    void Awake()
    {
        var body = GetComponent(typeof(Rigidbody)); // Noncompliant
    }
}
```

## Compliant solution

```csharp
public class Mover : MonoBehaviour
{
    void Awake()
    {
        var body = GetComponent<Rigidbody>();
    }
}
```

## See also

- [Component.GetComponent (Unity scripting reference)](https://docs.unity3d.com/ScriptReference/Component.GetComponent.html)
