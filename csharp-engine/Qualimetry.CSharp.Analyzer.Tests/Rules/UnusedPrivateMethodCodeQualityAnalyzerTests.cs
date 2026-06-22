using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class UnusedPrivateMethodCodeQualityAnalyzerTests
{
    [Fact]
    public Task PrivateMethodNeverCalled_IsReported()
    {
        const string source = """
            public class Ledger
            {
                public int Balance() => 0;

                private int {|qa_quality_unused_private_method:Recalculate|}() => 1;
            }
            """;

        return CSharpAnalyzerVerifier<UnusedPrivateMethodAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PrivateMethodCalledElsewhere_IsClean()
    {
        const string source = """
            public class Ledger
            {
                public int Balance() => Recalculate();

                private int Recalculate() => 1;
            }
            """;

        return CSharpAnalyzerVerifier<UnusedPrivateMethodAnalyzer>.VerifyAsync(source);
    }
}
