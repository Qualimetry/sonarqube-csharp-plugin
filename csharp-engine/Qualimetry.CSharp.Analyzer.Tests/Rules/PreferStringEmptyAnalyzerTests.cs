using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class PreferStringEmptyAnalyzerTests
{
    [Fact]
    public Task EmptyStringLiteral_IsReported()
    {
        const string source = """
            public class C
            {
                public string Prefix = {|qa_style_prefer_string_empty:""|};
            }
            """;

        return CSharpAnalyzerVerifier<PreferStringEmptyAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task StringEmptyMember_IsClean()
    {
        const string source = """
            public class C
            {
                public string Prefix = string.Empty;
            }
            """;

        return CSharpAnalyzerVerifier<PreferStringEmptyAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ConstantContext_IsClean()
    {
        const string source = """
            public class C
            {
                private const string Default = "";
            }
            """;

        return CSharpAnalyzerVerifier<PreferStringEmptyAnalyzer>.VerifyAsync(source);
    }
}
