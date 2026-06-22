using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class EmptyStringComparisonAnalyzerTests
{
    [Fact]
    public Task ComparisonToEmptyString_IsReported()
    {
        const string source = """
            public class C
            {
                public void M(string name)
                {
                    if ({|qa_style_empty_string_comparison:name == ""|})
                    {
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<EmptyStringComparisonAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task IsNullOrEmptyCheck_IsClean()
    {
        const string source = """
            public class C
            {
                public void M(string name)
                {
                    if (string.IsNullOrEmpty(name))
                    {
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<EmptyStringComparisonAnalyzer>.VerifyAsync(source);
    }
}
