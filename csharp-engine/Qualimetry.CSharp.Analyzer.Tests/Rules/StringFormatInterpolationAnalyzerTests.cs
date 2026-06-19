using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class StringFormatInterpolationAnalyzerTests
{
    [Fact]
    public Task StringFormatCall_IsReported()
    {
        const string source = """
            public class C
            {
                public string Describe(string name, int age)
                {
                    return {|qa_style_string_format_interpolation:string.Format("{0} is {1}", name, age)|};
                }
            }
            """;

        return CSharpAnalyzerVerifier<StringFormatInterpolationAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task InterpolatedString_IsClean()
    {
        const string source = """
            public class C
            {
                public string Describe(string name, int age)
                {
                    return $"{name} is {age}";
                }
            }
            """;

        return CSharpAnalyzerVerifier<StringFormatInterpolationAnalyzer>.VerifyAsync(source);
    }
}
