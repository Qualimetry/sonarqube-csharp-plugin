using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class EmptyFinalizerAnalyzerTests
{
    [Fact]
    public Task EmptyFinalizer_IsReported()
    {
        const string source = """
            public class Buffer
            {
                ~{|qa_quality_empty_finalizer:Buffer|}()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<EmptyFinalizerAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NoFinalizer_IsClean()
    {
        const string source = """
            public class Buffer
            {
            }
            """;

        return CSharpAnalyzerVerifier<EmptyFinalizerAnalyzer>.VerifyAsync(source);
    }
}
