# Frame updates should use Time.deltaTime not Time.fixedDeltaTime

`qa_unity_fixed_delta_time_in_update` &middot; Unity &middot; Bug &middot; severity CRITICAL &middot; enabled in the recommended profile

`Update` and `LateUpdate` run once per rendered frame at a variable cadence, so motion and timing in them must be scaled by `Time.deltaTime`, the elapsed time since the previous frame. Reading `Time.fixedDeltaTime` there instead pulls in the fixed physics step, which is unrelated to how long the current frame actually took.

The mismatch produces movement that speeds up or slows down with the frame rate instead of staying constant in real time. Use `Time.deltaTime` in frame callbacks and reserve `Time.fixedDeltaTime` for `FixedUpdate`.

## Noncompliant code example

```csharp
public class Mover : MonoBehaviour
{
    void Update()
    {
        position += speed * Time.fixedDeltaTime; // Noncompliant
    }
}
```

## Compliant solution

```csharp
public class Mover : MonoBehaviour
{
    void Update()
    {
        position += speed * Time.deltaTime;
    }
}
```

## See also

- [Time.deltaTime (Unity scripting reference)](https://docs.unity3d.com/ScriptReference/Time-deltaTime.html)
