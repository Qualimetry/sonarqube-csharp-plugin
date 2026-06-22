using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class RedundantElseAfterJumpAnalyzerTests
{
    [Fact]
    public Task ElseAfterReturn_IsReported()
    {
        const string source = """
            public class C
            {
                public string Describe(int value)
                {
                    if (value < 0)
                    {
                        return "negative";
                    }
                    {|qa_style_redundant_else_after_jump:else|}
                    {
                        return "non-negative";
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<RedundantElseAfterJumpAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ElseWithoutTerminatingThen_IsClean()
    {
        const string source = """
            public class C
            {
                public int Describe(int value)
                {
                    int result;
                    if (value < 0)
                    {
                        result = -1;
                    }
                    else
                    {
                        result = 1;
                    }

                    return result;
                }
            }
            """;

        return CSharpAnalyzerVerifier<RedundantElseAfterJumpAnalyzer>.VerifyAsync(source);
    }
}
