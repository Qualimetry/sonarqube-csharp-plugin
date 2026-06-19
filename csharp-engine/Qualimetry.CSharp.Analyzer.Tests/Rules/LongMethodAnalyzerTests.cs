using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Metrics;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class LongMethodAnalyzerTests
{
    [Fact]
    public Task MethodWithManyStatements_IsReported()
    {
        const string source = """
            public class C
            {
                public int {|qa_metrics_long_method:Tally|}()
                {
                    int a = 1;
                    int b = 2;
                    return a + b;
                }
            }
            """;

        return CSharpAnalyzerVerifier<LongMethodAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_long_method.maxstatements", "2"));
    }

    [Fact]
    public Task ShortMethod_IsClean()
    {
        const string source = """
            public class C
            {
                public int Tally()
                {
                    return 0;
                }
            }
            """;

        return CSharpAnalyzerVerifier<LongMethodAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_long_method.maxstatements", "2"));
    }
}
