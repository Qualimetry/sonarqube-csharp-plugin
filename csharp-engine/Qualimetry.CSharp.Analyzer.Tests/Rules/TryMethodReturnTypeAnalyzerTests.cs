using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class TryMethodReturnTypeAnalyzerTests
{
    [Fact]
    public Task TryMethodReturningInt_IsReported()
    {
        const string source = """
            public class Parser
            {
                public int {|qa_quality_try_method_return_type:TryParseCount|}(string text)
                {
                    return int.Parse(text);
                }
            }
            """;

        return CSharpAnalyzerVerifier<TryMethodReturnTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task TryMethodReturningBool_IsClean()
    {
        const string source = """
            public class Parser
            {
                public bool TryParseCount(string text, out int count)
                {
                    return int.TryParse(text, out count);
                }
            }
            """;

        return CSharpAnalyzerVerifier<TryMethodReturnTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NonTryMethod_IsClean()
    {
        const string source = """
            public class Parser
            {
                public int Tristate(string text)
                {
                    return int.Parse(text);
                }
            }
            """;

        return CSharpAnalyzerVerifier<TryMethodReturnTypeAnalyzer>.VerifyAsync(source);
    }
}
