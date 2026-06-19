using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class UnusedPrivateMethodAnalyzerTests
{
    [Fact]
    public Task UncalledPrivateMethod_IsReported()
    {
        const string source = """
            public class ReportBuilder
            {
                public string Build() => Header();

                private string Header() => "Report";

                private string {|qa_quality_unused_private_method:Footer|}() => "End";
            }
            """;

        return CSharpAnalyzerVerifier<UnusedPrivateMethodAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task CalledPrivateMethod_IsClean()
    {
        const string source = """
            public class ReportBuilder
            {
                public string Build() => Header() + Footer();

                private string Header() => "Report";

                private string Footer() => "End";
            }
            """;

        return CSharpAnalyzerVerifier<UnusedPrivateMethodAnalyzer>.VerifyAsync(source);
    }
}
