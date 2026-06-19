using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class IfChainToSwitchAnalyzerTests
{
    [Fact]
    public Task ThreeWayEqualityChain_IsReported()
    {
        const string source = """
            public class C
            {
                public string Name(int code)
                {
                    {|qa_style_if_chain_to_switch:if|} (code == 1)
                    {
                        return "one";
                    }
                    else if (code == 2)
                    {
                        return "two";
                    }
                    else if (code == 3)
                    {
                        return "three";
                    }

                    return "other";
                }
            }
            """;

        return CSharpAnalyzerVerifier<IfChainToSwitchAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ShortChain_IsClean()
    {
        const string source = """
            public class C
            {
                public string Name(int code)
                {
                    if (code == 1)
                    {
                        return "one";
                    }

                    return "other";
                }
            }
            """;

        return CSharpAnalyzerVerifier<IfChainToSwitchAnalyzer>.VerifyAsync(source);
    }
}
