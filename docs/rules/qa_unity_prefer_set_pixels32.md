# Prefer the 32-bit pixel API over SetPixels

`qa_unity_prefer_set_pixels32` &middot; Unity &middot; Code Smell &middot; severity MAJOR &middot; enabled in the recommended profile

`SetPixels` accepts an array of floating-point `Color` values, which the engine has to convert down to the eight-bit channels textures actually store. For large textures that conversion and the wider element size make each upload measurably slower and allocate more managed memory than necessary.

`SetPixels32` takes `Color32` values that already match the stored format, so the write skips the conversion step. Prefer it whenever the pixel data does not need full floating-point precision.

## Noncompliant code example

```csharp
public class TextureWriter
{
    public void Fill(Texture2D texture, Color[] pixels)
    {
        texture.SetPixels(pixels); // Noncompliant
    }
}
```

## Compliant solution

```csharp
public class TextureWriter
{
    public void Fill(Texture2D texture, Color32[] pixels)
    {
        texture.SetPixels32(pixels);
    }
}
```

## See also

- [Texture2D.SetPixels32 (Unity scripting reference)](https://docs.unity3d.com/ScriptReference/Texture2D.SetPixels32.html)
