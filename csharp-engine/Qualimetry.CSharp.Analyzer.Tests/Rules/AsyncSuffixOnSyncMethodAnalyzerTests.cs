using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class AsyncSuffixOnSyncMethodAnalyzerTests
{
    [Fact]
    public Task SyncMethodWithAsyncSuffix_IsReported()
    {
        const string source = """
            public class C
            {
                public int {|qa_style_async_suffix_on_sync_method:ComputeAsync|}(int value)
                {
                    return value * 2;
                }
            }
            """;

        return CSharpAnalyzerVerifier<AsyncSuffixOnSyncMethodAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task SyncMethodWithoutSuffix_IsClean()
    {
        const string source = """
            public class C
            {
                public int Compute(int value)
                {
                    return value * 2;
                }
            }
            """;

        return CSharpAnalyzerVerifier<AsyncSuffixOnSyncMethodAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task TaskReturningMethodWithSuffix_IsClean()
    {
        const string source = """
            using System.Threading.Tasks;

            public class C
            {
                public Task<int> ComputeAsync(int value)
                {
                    return Task.FromResult(value * 2);
                }
            }
            """;

        return CSharpAnalyzerVerifier<AsyncSuffixOnSyncMethodAnalyzer>.VerifyAsync(source);
    }
}
