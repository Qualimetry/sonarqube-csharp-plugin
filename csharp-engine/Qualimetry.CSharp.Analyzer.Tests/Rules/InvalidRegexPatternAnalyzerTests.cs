using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Reliability;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class InvalidRegexPatternAnalyzerTests
{
    [Fact]
    public Task InvalidPattern_IsReported()
    {
        string source = """
            using System.Text.RegularExpressions;

            public sealed class Matcher
            {
                public Regex Build() => new Regex({|qa_reliability_invalid_regex_pattern:"(unclosed"|});
            }
            """;

        return CSharpAnalyzerVerifier<InvalidRegexPatternAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ValidPattern_IsClean()
    {
        string source = """
            using System.Text.RegularExpressions;

            public sealed class Matcher
            {
                public Regex Build() => new Regex("(closed)");
            }
            """;

        return CSharpAnalyzerVerifier<InvalidRegexPatternAnalyzer>.VerifyAsync(source);
    }
}
