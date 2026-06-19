using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class BooleanLiteralComparisonAnalyzerTests
{
    [Fact]
    public Task ComparisonToTrue_IsReported()
    {
        const string source = """
            public class C
            {
                public void M(bool ready)
                {
                    if ({|qa_style_boolean_literal_comparison:ready == true|})
                    {
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<BooleanLiteralComparisonAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DirectCondition_IsClean()
    {
        const string source = """
            public class C
            {
                public void M(bool ready)
                {
                    if (ready)
                    {
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<BooleanLiteralComparisonAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NullableBoolComparison_IsClean()
    {
        const string source = """
            public class C
            {
                public void M(bool? ready)
                {
                    if (ready == true)
                    {
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<BooleanLiteralComparisonAnalyzer>.VerifyAsync(source);
    }
}
