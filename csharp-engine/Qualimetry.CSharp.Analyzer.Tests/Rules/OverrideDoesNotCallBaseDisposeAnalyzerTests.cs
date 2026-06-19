using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class OverrideDoesNotCallBaseDisposeAnalyzerTests
{
    [Fact]
    public Task DisposeOverrideWithoutBaseCall_IsReported()
    {
        const string source = """
            using System.IO;

            public class CachingStream : MemoryStream
            {
                protected override void {|qa_quality_override_does_not_call_base_dispose:Dispose|}(bool disposing)
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<OverrideDoesNotCallBaseDisposeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DisposeOverrideCallingBase_IsClean()
    {
        const string source = """
            using System.IO;

            public class CachingStream : MemoryStream
            {
                protected override void Dispose(bool disposing)
                {
                    base.Dispose(disposing);
                }
            }
            """;

        return CSharpAnalyzerVerifier<OverrideDoesNotCallBaseDisposeAnalyzer>.VerifyAsync(source);
    }
}
