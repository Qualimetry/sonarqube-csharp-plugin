using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Unity;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class PreferSetPixels32AnalyzerTests
{
    [Fact]
    public Task SetPixelsOnTexture2D_IsReported()
    {
        const string source = """
            using UnityEngine;

            public class Painter
            {
                void Paint(Texture2D texture)
                {
                    texture.{|qa_unity_prefer_set_pixels32:SetPixels|}(null);
                }
            }

            namespace UnityEngine
            {
                public class Texture2D { public void SetPixels(object data) { } }
            }
            """;

        return CSharpAnalyzerVerifier<PreferSetPixels32Analyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task SetPixelsOnLookAlikeType_IsClean()
    {
        const string source = """
            public class Canvas
            {
                public void SetPixels(object data) { }
            }

            public class Painter
            {
                void Paint(Canvas canvas)
                {
                    canvas.SetPixels(null);
                }
            }
            """;

        return CSharpAnalyzerVerifier<PreferSetPixels32Analyzer>.VerifyAsync(source);
    }
}
