using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class UnnecessaryReturnParenthesesAnalyzerTests
{
    [Fact]
    public Task ParenthesizedReturnedIdentifier_IsReported()
    {
        const string source = """
            public class C
            {
                public int Current(int value)
                {
                    return {|qa_style_unnecessary_return_parentheses:(value)|};
                }
            }
            """;

        return CSharpAnalyzerVerifier<UnnecessaryReturnParenthesesAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task BareReturn_IsClean()
    {
        const string source = """
            public class C
            {
                public int Current(int value)
                {
                    return value;
                }
            }
            """;

        return CSharpAnalyzerVerifier<UnnecessaryReturnParenthesesAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task GroupingParenthesesInExpression_AreClean()
    {
        const string source = """
            public class C
            {
                public int Current(int a, int b, int c)
                {
                    return (a + b) * c;
                }
            }
            """;

        return CSharpAnalyzerVerifier<UnnecessaryReturnParenthesesAnalyzer>.VerifyAsync(source);
    }
}
