using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class TrailingWhitespaceAnalyzerTests
{
    [Fact]
    public Task LineWithTrailingSpaces_IsReported()
    {
        const string source = "public class Account\n{\n    public int Balance { get; set; }{|qa_style_trailing_whitespace:   |}\n}\n";

        return CSharpAnalyzerVerifier<TrailingWhitespaceAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task LinesWithoutTrailingWhitespace_AreClean()
    {
        const string source = "public class Account\n{\n    public int Balance { get; set; }\n}\n";

        return CSharpAnalyzerVerifier<TrailingWhitespaceAnalyzer>.VerifyAsync(source);
    }
}
