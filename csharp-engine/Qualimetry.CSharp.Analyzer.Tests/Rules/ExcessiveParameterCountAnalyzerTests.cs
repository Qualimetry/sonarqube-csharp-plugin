using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Metrics;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ExcessiveParameterCountAnalyzerTests
{
    [Fact]
    public Task MethodWithManyParameters_IsReported()
    {
        const string source = """
            public class C
            {
                public void {|qa_metrics_excessive_parameter_count:Send|}(int a, int b, int c)
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<ExcessiveParameterCountAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_excessive_parameter_count.maxparameters", "2"));
    }

    [Fact]
    public Task MethodWithFewParameters_IsClean()
    {
        const string source = """
            public class C
            {
                public void Send(int a, int b)
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<ExcessiveParameterCountAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_excessive_parameter_count.maxparameters", "2"));
    }
}
