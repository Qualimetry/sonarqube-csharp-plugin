using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class LoopCounterEqualityComparisonAnalyzerTests
{
    [Fact]
    public Task InequalityBoundedLoop_IsReported()
    {
        const string source = """
            public class C
            {
                public void M(int count)
                {
                    for (int i = 0; {|qa_style_loop_counter_equality_comparison:i != count|}; i += 2)
                    {
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<LoopCounterEqualityComparisonAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task RelationalBoundedLoop_IsClean()
    {
        const string source = """
            public class C
            {
                public void M(int count)
                {
                    for (int i = 0; i < count; i += 2)
                    {
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<LoopCounterEqualityComparisonAnalyzer>.VerifyAsync(source);
    }
}
