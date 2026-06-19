using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class BooleanReturnViaIfAnalyzerTests
{
    [Fact]
    public Task IfElseReturningBooleans_IsReported()
    {
        const string source = """
            public class C
            {
                public bool IsPositive(int value)
                {
                    {|qa_style_boolean_return_via_if:if|} (value > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<BooleanReturnViaIfAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DirectBooleanReturn_IsClean()
    {
        const string source = """
            public class C
            {
                public bool IsPositive(int value)
                {
                    return value > 0;
                }
            }
            """;

        return CSharpAnalyzerVerifier<BooleanReturnViaIfAnalyzer>.VerifyAsync(source);
    }
}
