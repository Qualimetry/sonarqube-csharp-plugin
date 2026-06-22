using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Metrics;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ExcessiveMethodOverloadsAnalyzerTests
{
    [Fact]
    public Task ManyOverloads_IsReported()
    {
        const string source = """
            public class {|qa_metrics_excessive_method_overloads:Logger|}
            {
                public void Log() { }
                public void Log(int level) { }
                public void Log(int level, string category) { }
            }
            """;

        return CSharpAnalyzerVerifier<ExcessiveMethodOverloadsAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_excessive_method_overloads.maxoverloads", "2"));
    }

    [Fact]
    public Task FewOverloads_IsClean()
    {
        const string source = """
            public class Logger
            {
                public void Log() { }
                public void Log(int level) { }
            }
            """;

        return CSharpAnalyzerVerifier<ExcessiveMethodOverloadsAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_excessive_method_overloads.maxoverloads", "2"));
    }
}
