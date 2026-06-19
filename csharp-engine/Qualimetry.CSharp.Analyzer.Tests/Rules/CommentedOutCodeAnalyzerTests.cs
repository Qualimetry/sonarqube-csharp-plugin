using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class CommentedOutCodeAnalyzerTests
{
    [Fact]
    public Task CommentEndingWithStatementTerminator_IsReported()
    {
        const string source = """
            public class C
            {
                public int Total(int price)
                {
                    {|qa_style_commented_out_code:// price = price * 2;|}
                    return price;
                }
            }
            """;

        return CSharpAnalyzerVerifier<CommentedOutCodeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ExplanatoryComment_IsClean()
    {
        const string source = """
            public class C
            {
                public int Total(int price)
                {
                    // Price is doubled by the caller.
                    return price;
                }
            }
            """;

        return CSharpAnalyzerVerifier<CommentedOutCodeAnalyzer>.VerifyAsync(source);
    }
}
