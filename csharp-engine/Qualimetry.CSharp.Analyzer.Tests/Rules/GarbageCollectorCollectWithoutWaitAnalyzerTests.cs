using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class GarbageCollectorCollectWithoutWaitAnalyzerTests
{
    [Fact]
    public Task CollectWithoutWait_IsReported()
    {
        const string source = """
            using System;

            public sealed class Cleaner
            {
                public void Purge()
                {
                    {|qa_quality_garbage_collector_collect_without_wait:GC.Collect()|};
                }
            }
            """;

        return CSharpAnalyzerVerifier<GarbageCollectorCollectWithoutWaitAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task CollectFollowedByWait_IsClean()
    {
        const string source = """
            using System;

            public sealed class Cleaner
            {
                public void Purge()
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
            }
            """;

        return CSharpAnalyzerVerifier<GarbageCollectorCollectWithoutWaitAnalyzer>.VerifyAsync(source);
    }
}
