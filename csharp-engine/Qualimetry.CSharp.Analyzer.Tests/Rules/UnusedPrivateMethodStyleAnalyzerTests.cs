using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class UnusedPrivateMethodStyleAnalyzerTests
{
    [Fact]
    public Task PrivateMethodNeverReferenced_IsReported()
    {
        const string source = """
            public class C
            {
                public void Run()
                {
                    Used();
                }

                private void Used()
                {
                }

                private void {|qa_style_unused_private_method:Unused|}()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<UnusedPrivateMethodAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PrivateMethodThatIsCalled_IsClean()
    {
        const string source = """
            public class C
            {
                public void Run()
                {
                    Helper();
                }

                private void Helper()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<UnusedPrivateMethodAnalyzer>.VerifyAsync(source);
    }
}
