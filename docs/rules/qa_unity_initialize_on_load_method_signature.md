# InitializeOnLoadMethod targets must be static and parameterless

`qa_unity_initialize_on_load_method_signature` &middot; Unity &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

Unity calls a method decorated with `InitializeOnLoadMethod` (or `RuntimeInitializeOnLoadMethod`) automatically during load, with no instance and no arguments. The engine therefore only ever resolves a static, parameterless method; an instance method or one that declares parameters cannot be invoked this way.

A non-static or parameterised target is silently skipped, so the load-time setup the attribute promises never runs. Declare the method `static` and remove its parameters so the callback actually fires.

## Noncompliant code example

```csharp
public class Loader
{
    [InitializeOnLoadMethod]
    void Register() // Noncompliant
    {
    }
}
```

## Compliant solution

```csharp
public class Loader
{
    [InitializeOnLoadMethod]
    static void Register()
    {
    }
}
```

## See also

- [InitializeOnLoadMethodAttribute (Unity scripting reference)](https://docs.unity3d.com/ScriptReference/InitializeOnLoadMethodAttribute.html)
