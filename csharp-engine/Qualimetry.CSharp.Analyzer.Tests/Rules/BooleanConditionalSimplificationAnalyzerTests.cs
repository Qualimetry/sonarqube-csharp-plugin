using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class BooleanConditionalSimplificationAnalyzerTests
{
    [Fact]
    public Task ConditionalReturningBooleanLiterals_IsReported()
    {
        const string source = """
            public class C
            {
                public bool CanProceed(int retries)
                {
                    return {|qa_style_boolean_conditional_simplification:retries < 3 ? true : false|};
                }
            }
            """;

        return CSharpAnalyzerVerifier<BooleanConditionalSimplificationAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DirectCondition_IsClean()
    {
        const string source = """
            public class C
            {
                public bool CanProceed(int retries)
                {
                    return retries < 3;
                }
            }
            """;

        return CSharpAnalyzerVerifier<BooleanConditionalSimplificationAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ConditionalReturningValues_IsClean()
    {
        const string source = """
            public class C
            {
                public int Limit(bool high)
                {
                    return high ? 100 : 10;
                }
            }
            """;

        return CSharpAnalyzerVerifier<BooleanConditionalSimplificationAnalyzer>.VerifyAsync(source);
    }
}
