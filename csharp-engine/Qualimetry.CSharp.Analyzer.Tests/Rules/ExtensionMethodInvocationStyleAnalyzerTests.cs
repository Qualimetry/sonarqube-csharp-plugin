using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ExtensionMethodInvocationStyleAnalyzerTests
{
    [Fact]
    public Task StaticCallOfExtensionMethod_IsReported()
    {
        const string source = """
            public static class Helpers
            {
                public static string Shorten(this string value) => value;
            }

            public class C
            {
                public void M(string name)
                {
                    _ = {|qa_style_extension_method_invocation_style:Helpers.Shorten(name)|};
                }
            }
            """;

        return CSharpAnalyzerVerifier<ExtensionMethodInvocationStyleAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task MemberAccessCall_IsClean()
    {
        const string source = """
            public static class Helpers
            {
                public static string Shorten(this string value) => value;
            }

            public class C
            {
                public void M(string name)
                {
                    _ = name.Shorten();
                }
            }
            """;

        return CSharpAnalyzerVerifier<ExtensionMethodInvocationStyleAnalyzer>.VerifyAsync(source);
    }
}
