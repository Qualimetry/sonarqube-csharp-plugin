using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class GarbageCollectorCollectCallAnalyzerTests
{
    [Fact]
    public Task GarbageCollectorCollect_IsReported()
    {
        const string source = """
            using System;

            public class Importer
            {
                public void Finish()
                {
                    {|qa_quality_garbage_collector_collect_call:GC.Collect()|};
                }
            }
            """;

        return CSharpAnalyzerVerifier<GarbageCollectorCollectCallAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NoForcedCollection_IsClean()
    {
        const string source = """
            public class Importer
            {
                public void Finish()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<GarbageCollectorCollectCallAnalyzer>.VerifyAsync(source);
    }
}
