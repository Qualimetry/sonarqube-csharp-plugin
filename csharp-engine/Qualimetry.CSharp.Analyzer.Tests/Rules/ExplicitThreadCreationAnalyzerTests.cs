using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ExplicitThreadCreationAnalyzerTests
{
    [Fact]
    public Task NewThread_IsReported()
    {
        const string source = """
            using System.Threading;

            public class Worker
            {
                public void Run()
                {
                    var worker = {|qa_quality_explicit_thread_creation:new Thread(Work)|};
                    worker.Start();
                }

                private void Work()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<ExplicitThreadCreationAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task TaskRun_IsClean()
    {
        const string source = """
            using System.Threading.Tasks;

            public class Worker
            {
                public void Run()
                {
                    Task.Run(Work);
                }

                private void Work()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<ExplicitThreadCreationAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task UserTypeNamedThread_IsClean()
    {
        const string source = """
            namespace Acme
            {
                public class Thread
                {
                    public Thread(string name)
                    {
                    }
                }

                public class Worker
                {
                    public void Run()
                    {
                        var worker = new Thread("background");
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<ExplicitThreadCreationAnalyzer>.VerifyAsync(source);
    }
}
