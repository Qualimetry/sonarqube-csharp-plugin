# Empty Unity lifecycle methods should be removed

`qa_unity_empty_unity_message` &middot; Unity &middot; Code Smell &middot; severity MINOR &middot; optional

The Unity runtime detects lifecycle callbacks such as `Update`, `FixedUpdate`, `LateUpdate` and `Awake` by name on a `MonoBehaviour` and adds every one it finds to its internal invocation lists. A callback with an empty body still pays that per-object dispatch cost on each frame or physics step while doing nothing.

Deleting an empty lifecycle method removes it from the engine schedule entirely, which keeps frame and physics loops free of dead calls and makes it clear which callbacks the component actually relies on.

## Noncompliant code example

```csharp
public class Spinner : MonoBehaviour
{
    void Update() // Noncompliant
    {
    }
}
```

## Compliant solution

```csharp
public class Spinner : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0f, 1f, 0f);
    }
}
```

## See also

- [MonoBehaviour lifecycle (Unity scripting reference)](https://docs.unity3d.com/ScriptReference/MonoBehaviour.html)
