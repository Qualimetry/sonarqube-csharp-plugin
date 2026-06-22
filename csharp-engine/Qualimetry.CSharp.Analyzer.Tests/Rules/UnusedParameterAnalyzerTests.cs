using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class UnusedParameterAnalyzerTests
{
    [Fact]
    public Task PrivateMethodWithUnusedParameter_IsReported()
    {
        const string source = """
            public class Calculator
            {
                private int Compute(int value, int {|qa_quality_unused_parameter:scale|})
                {
                    return value;
                }
            }
            """;

        return CSharpAnalyzerVerifier<UnusedParameterAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PrivateMethodUsingAllParameters_IsClean()
    {
        const string source = """
            public class Calculator
            {
                private int Compute(int value, int scale)
                {
                    return value * scale;
                }
            }
            """;

        return CSharpAnalyzerVerifier<UnusedParameterAnalyzer>.VerifyAsync(source);
    }
}
