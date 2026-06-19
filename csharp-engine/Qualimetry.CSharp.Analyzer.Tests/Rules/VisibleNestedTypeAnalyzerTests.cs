using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class VisibleNestedTypeAnalyzerTests
{
    [Fact]
    public Task PublicNestedType_IsReported()
    {
        const string source = """
            public class Parser
            {
                public class {|qa_quality_visible_nested_type:Options|}
                {
                    public bool Strict { get; set; }
                }
            }
            """;

        return CSharpAnalyzerVerifier<VisibleNestedTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PrivateNestedType_IsClean()
    {
        const string source = """
            public class Parser
            {
                private class Options
                {
                    public bool Strict { get; set; }
                }
            }
            """;

        return CSharpAnalyzerVerifier<VisibleNestedTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task TopLevelType_IsClean()
    {
        const string source = """
            public class ParserOptions
            {
                public bool Strict { get; set; }
            }
            """;

        return CSharpAnalyzerVerifier<VisibleNestedTypeAnalyzer>.VerifyAsync(source);
    }
}
