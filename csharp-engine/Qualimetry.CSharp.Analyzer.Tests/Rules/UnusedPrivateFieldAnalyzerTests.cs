using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class UnusedPrivateFieldAnalyzerTests
{
    [Fact]
    public Task UnreferencedPrivateField_IsReported()
    {
        const string source = """
            public class OrderProcessor
            {
                private int {|qa_quality_unused_private_field:_retryCount|};

                public void Process()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<UnusedPrivateFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ReferencedPrivateField_IsClean()
    {
        const string source = """
            public class OrderProcessor
            {
                private int _retryCount;

                public int Process()
                {
                    return _retryCount;
                }
            }
            """;

        return CSharpAnalyzerVerifier<UnusedPrivateFieldAnalyzer>.VerifyAsync(source);
    }
}
