using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Metrics;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class LargeTypeAnalyzerTests
{
    [Fact]
    public Task TypeSpanningManyLines_IsReported()
    {
        const string source = """
            public class {|qa_metrics_large_type:Big|}
            {
                private int _a;
                private int _b;
            }
            """;

        return CSharpAnalyzerVerifier<LargeTypeAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_large_type.maxlines", "3"));
    }

    [Fact]
    public Task SmallType_IsClean()
    {
        const string source = """
            public class Small
            {
            }
            """;

        return CSharpAnalyzerVerifier<LargeTypeAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_large_type.maxlines", "3"));
    }
}
