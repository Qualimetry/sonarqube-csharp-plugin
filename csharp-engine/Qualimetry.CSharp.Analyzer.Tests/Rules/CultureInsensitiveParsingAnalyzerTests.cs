using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class CultureInsensitiveParsingAnalyzerTests
{
    [Fact]
    public Task ParseWithoutCulture_IsReported()
    {
        const string source = """
            public class Money
            {
                public decimal Read(string text) => {|qa_quality_culture_insensitive_parsing:decimal.Parse(text)|};
            }
            """;

        return CSharpAnalyzerVerifier<CultureInsensitiveParsingAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ParseWithInvariantCulture_IsClean()
    {
        const string source = """
            using System.Globalization;

            public class Money
            {
                public decimal Read(string text) => decimal.Parse(text, CultureInfo.InvariantCulture);
            }
            """;

        return CSharpAnalyzerVerifier<CultureInsensitiveParsingAnalyzer>.VerifyAsync(source);
    }
}
