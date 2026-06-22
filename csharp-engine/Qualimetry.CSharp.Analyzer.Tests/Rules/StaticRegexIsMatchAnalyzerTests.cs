using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class StaticRegexIsMatchAnalyzerTests
{
    [Fact]
    public Task StaticRegexIsMatch_IsReported()
    {
        const string source = """
            using System.Text.RegularExpressions;

            public class Validator
            {
                public bool IsCode(string input)
                {
                    return {|qa_style_static_regex_is_match:Regex.IsMatch(input, "^[A-Z]$")|};
                }
            }
            """;

        return CSharpAnalyzerVerifier<StaticRegexIsMatchAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task CachedRegexInstance_IsClean()
    {
        const string source = """
            using System.Text.RegularExpressions;

            public class Validator
            {
                private static readonly Regex CodePattern = new Regex("^[A-Z]$");

                public bool IsCode(string input)
                {
                    return CodePattern.IsMatch(input);
                }
            }
            """;

        return CSharpAnalyzerVerifier<StaticRegexIsMatchAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DynamicPattern_IsClean()
    {
        const string source = """
            using System.Text.RegularExpressions;

            public class Validator
            {
                public bool IsMatch(string input, string pattern)
                {
                    return Regex.IsMatch(input, pattern);
                }
            }
            """;

        return CSharpAnalyzerVerifier<StaticRegexIsMatchAnalyzer>.VerifyAsync(source);
    }
}
