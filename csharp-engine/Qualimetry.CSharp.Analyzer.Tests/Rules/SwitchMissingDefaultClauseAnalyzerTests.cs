using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class SwitchMissingDefaultClauseAnalyzerTests
{
    [Fact]
    public Task SwitchWithoutDefault_IsReported()
    {
        const string source = """
            public sealed class Router
            {
                public string Resolve(int code)
                {
                    {|qa_quality_switch_missing_default_clause:switch|} (code)
                    {
                        case 1:
                            return "one";
                        case 2:
                            return "two";
                    }

                    return "unknown";
                }
            }
            """;

        return CSharpAnalyzerVerifier<SwitchMissingDefaultClauseAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task SwitchWithDefault_IsClean()
    {
        const string source = """
            public sealed class Router
            {
                public string Resolve(int code)
                {
                    switch (code)
                    {
                        case 1:
                            return "one";
                        case 2:
                            return "two";
                        default:
                            return "unknown";
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<SwitchMissingDefaultClauseAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task EmptySwitch_IsClean()
    {
        const string source = """
            public sealed class Router
            {
                public void Resolve(int code)
                {
                    switch (code)
                    {
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<SwitchMissingDefaultClauseAnalyzer>.VerifyAsync(source);
    }
}
