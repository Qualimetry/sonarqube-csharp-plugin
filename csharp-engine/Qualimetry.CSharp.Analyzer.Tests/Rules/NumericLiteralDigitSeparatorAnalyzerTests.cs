using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class NumericLiteralDigitSeparatorAnalyzerTests
{
    [Fact]
    public Task UngroupedLongLiteral_IsReported()
    {
        const string source = """
            public class C
            {
                public const int Timeout = {|qa_style_numeric_literal_digit_separator:60000|};
            }
            """;

        return CSharpAnalyzerVerifier<NumericLiteralDigitSeparatorAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task SeparatedLiteral_IsClean()
    {
        const string source = """
            public class C
            {
                public const int Timeout = 60_000;
            }
            """;

        return CSharpAnalyzerVerifier<NumericLiteralDigitSeparatorAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ShortLiteral_IsClean()
    {
        const string source = """
            public class C
            {
                public const int Limit = 1000;
            }
            """;

        return CSharpAnalyzerVerifier<NumericLiteralDigitSeparatorAnalyzer>.VerifyAsync(source);
    }
}
