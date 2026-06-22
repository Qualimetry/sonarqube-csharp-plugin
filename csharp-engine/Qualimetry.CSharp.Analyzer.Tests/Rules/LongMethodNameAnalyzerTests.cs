using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Metrics;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class LongMethodNameAnalyzerTests
{
    [Fact]
    public Task LongMethodName_IsReported()
    {
        const string source = """
            public class C
            {
                public void {|qa_metrics_long_method_name:Import|}()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<LongMethodNameAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_long_method_name.maxnamelength", "5"));
    }

    [Fact]
    public Task ShortMethodName_IsClean()
    {
        const string source = """
            public class C
            {
                public void Run()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<LongMethodNameAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_long_method_name.maxnamelength", "5"));
    }
}
