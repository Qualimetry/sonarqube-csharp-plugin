using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class RedundantToStringInConcatenationAnalyzerTests
{
    [Fact]
    public Task ToStringInStringConcatenation_IsReported()
    {
        const string source = """
            public class C
            {
                public string Label(int count)
                {
                    return "Items: " + {|qa_style_redundant_to_string_in_concatenation:count.ToString()|};
                }
            }
            """;

        return CSharpAnalyzerVerifier<RedundantToStringInConcatenationAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ConcatenationWithoutToString_IsClean()
    {
        const string source = """
            public class C
            {
                public string Label(int count)
                {
                    return "Items: " + count;
                }
            }
            """;

        return CSharpAnalyzerVerifier<RedundantToStringInConcatenationAnalyzer>.VerifyAsync(source);
    }
}
