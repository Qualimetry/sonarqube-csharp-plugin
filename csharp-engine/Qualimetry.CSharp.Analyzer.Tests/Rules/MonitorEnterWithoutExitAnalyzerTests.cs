using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class MonitorEnterWithoutExitAnalyzerTests
{
    [Fact]
    public Task EnterWithoutExit_IsReported()
    {
        const string source = """
            using System.Threading;

            public class Cache
            {
                private readonly object _gate = new object();

                public void Add()
                {
                    {|qa_quality_monitor_enter_without_exit:Monitor.Enter(_gate)|};
                    Mutate();
                }

                private void Mutate()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<MonitorEnterWithoutExitAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task EnterWithExit_IsClean()
    {
        const string source = """
            using System.Threading;

            public class Cache
            {
                private readonly object _gate = new object();

                public void Add()
                {
                    Monitor.Enter(_gate);
                    try
                    {
                        Mutate();
                    }
                    finally
                    {
                        Monitor.Exit(_gate);
                    }
                }

                private void Mutate()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<MonitorEnterWithoutExitAnalyzer>.VerifyAsync(source);
    }
}
