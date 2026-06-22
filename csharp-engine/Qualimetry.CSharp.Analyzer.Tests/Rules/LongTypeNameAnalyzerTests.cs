using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Metrics;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class LongTypeNameAnalyzerTests
{
    [Fact]
    public Task LongTypeName_IsReported()
    {
        const string source = """
            public class {|qa_metrics_long_type_name:Widget|}
            {
            }
            """;

        return CSharpAnalyzerVerifier<LongTypeNameAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_long_type_name.maxnamelength", "5"));
    }

    [Fact]
    public Task ShortTypeName_IsClean()
    {
        const string source = """
            public class Box
            {
            }
            """;

        return CSharpAnalyzerVerifier<LongTypeNameAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_long_type_name.maxnamelength", "5"));
    }
}
