using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ExpressionBodyOpportunityAnalyzerTests
{
    [Fact]
    public Task SingleStatementMethod_IsReported()
    {
        const string source = """
            public class C
            {
                public int {|qa_style_expression_body_opportunity:Square|}(int value)
                {
                    return value * value;
                }
            }
            """;

        return CSharpAnalyzerVerifier<ExpressionBodyOpportunityAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ExpressionBodiedMethod_IsClean()
    {
        const string source = """
            public class C
            {
                public int Square(int value) => value * value;
            }
            """;

        return CSharpAnalyzerVerifier<ExpressionBodyOpportunityAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task MultiStatementMethod_IsClean()
    {
        const string source = """
            public class C
            {
                public int Sum(int a, int b)
                {
                    int total = a + b;
                    return total;
                }
            }
            """;

        return CSharpAnalyzerVerifier<ExpressionBodyOpportunityAnalyzer>.VerifyAsync(source);
    }
}
