using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Metrics;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class LongFieldNameAnalyzerTests
{
    [Fact]
    public Task LongFieldName_IsReported()
    {
        const string source = """
            public class C
            {
                private int {|qa_metrics_long_field_name:_value|};
            }
            """;

        return CSharpAnalyzerVerifier<LongFieldNameAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_long_field_name.maxnamelength", "5"));
    }

    [Fact]
    public Task ShortFieldName_IsClean()
    {
        const string source = """
            public class C
            {
                private int _n;
            }
            """;

        return CSharpAnalyzerVerifier<LongFieldNameAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_long_field_name.maxnamelength", "5"));
    }
}
