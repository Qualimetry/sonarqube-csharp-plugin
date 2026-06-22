using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class UnusedPrivateNestedTypeAnalyzerTests
{
    [Fact]
    public Task UnreferencedPrivateNestedType_IsReported()
    {
        const string source = """
            public class Scheduler
            {
                public void Run()
                {
                }

                private sealed class {|qa_quality_unused_private_nested_type:PendingJob|}
                {
                    public int Id { get; set; }
                }
            }
            """;

        return CSharpAnalyzerVerifier<UnusedPrivateNestedTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ReferencedPrivateNestedType_IsClean()
    {
        const string source = """
            public class Scheduler
            {
                private readonly PendingJob _next = new();

                public int Run() => _next.Id;

                private sealed class PendingJob
                {
                    public int Id { get; set; }
                }
            }
            """;

        return CSharpAnalyzerVerifier<UnusedPrivateNestedTypeAnalyzer>.VerifyAsync(source);
    }
}
