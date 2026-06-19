using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Metrics;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class HighCyclomaticComplexityAnalyzerTests
{
    [Fact]
    public Task ComplexMethod_IsReported()
    {
        const string source = """
            public class C
            {
                public int {|qa_metrics_high_cyclomatic_complexity:Classify|}(int n)
                {
                    if (n > 0)
                    {
                        return 1;
                    }

                    if (n < 0)
                    {
                        return -1;
                    }

                    return 0;
                }
            }
            """;

        return CSharpAnalyzerVerifier<HighCyclomaticComplexityAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_high_cyclomatic_complexity.maxcomplexity", "2"));
    }

    [Fact]
    public Task SimpleMethod_IsClean()
    {
        const string source = """
            public class C
            {
                public int Classify(int n)
                {
                    return n;
                }
            }
            """;

        return CSharpAnalyzerVerifier<HighCyclomaticComplexityAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_high_cyclomatic_complexity.maxcomplexity", "2"));
    }
}
