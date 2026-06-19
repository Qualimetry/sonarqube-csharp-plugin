using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Unity;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class PreferSetPixels32AnalyzerTests
{
    private const string Stubs = """
        public class Color { }
        public class Color32 { }

        public class Texture2D
        {
            public void SetPixels(Color[] pixels) { }
            public void SetPixels32(Color32[] pixels) { }
        }
        """;

    [Fact]
    public Task SetPixelsCall_IsReported()
    {
        const string source = Stubs + """

            public class TextureWriter
            {
                public void Fill(Texture2D texture, Color[] pixels)
                {
                    texture.{|qa_unity_prefer_set_pixels32:SetPixels|}(pixels);
                }
            }
            """;

        return CSharpAnalyzerVerifier<PreferSetPixels32Analyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task SetPixels32Call_IsClean()
    {
        const string source = Stubs + """

            public class TextureWriter
            {
                public void Fill(Texture2D texture, Color32[] pixels)
                {
                    texture.SetPixels32(pixels);
                }
            }
            """;

        return CSharpAnalyzerVerifier<PreferSetPixels32Analyzer>.VerifyAsync(source);
    }
}
