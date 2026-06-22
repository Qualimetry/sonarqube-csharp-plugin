using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class DangerousThreadMethodAnalyzerTests
{
    [Fact]
    public Task ThreadAbort_IsReported()
    {
        const string source = """
            using System.Threading;

            #pragma warning disable SYSLIB0006
            public class Worker
            {
                public void Stop(Thread worker)
                {
                    {|qa_quality_dangerous_thread_method:worker.Abort()|};
                }
            }
            """;

        return CSharpAnalyzerVerifier<DangerousThreadMethodAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task CooperativeCancellation_IsClean()
    {
        const string source = """
            using System.Threading;

            public class Worker
            {
                public void Stop(CancellationTokenSource cancellation)
                {
                    cancellation.Cancel();
                }
            }
            """;

        return CSharpAnalyzerVerifier<DangerousThreadMethodAnalyzer>.VerifyAsync(source);
    }
}
