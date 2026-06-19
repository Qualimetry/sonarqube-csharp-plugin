using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Metrics;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ExcessiveFieldCountAnalyzerTests
{
    [Fact]
    public Task TypeWithManyFields_IsReported()
    {
        const string source = """
            public class {|qa_metrics_excessive_field_count:Widget|}
            {
                private int _a;
                private int _b;
                private int _c;
            }
            """;

        return CSharpAnalyzerVerifier<ExcessiveFieldCountAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_excessive_field_count.maxfields", "2"));
    }

    [Fact]
    public Task TypeWithFewFields_IsClean()
    {
        const string source = """
            public class Widget
            {
                private int _a;
                private int _b;
            }
            """;

        return CSharpAnalyzerVerifier<ExcessiveFieldCountAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_excessive_field_count.maxfields", "2"));
    }
}
