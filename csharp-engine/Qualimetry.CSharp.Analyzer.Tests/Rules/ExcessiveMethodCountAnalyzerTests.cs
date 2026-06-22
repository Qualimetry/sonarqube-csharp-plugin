using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Metrics;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ExcessiveMethodCountAnalyzerTests
{
    [Fact]
    public Task TypeWithManyMethods_IsReported()
    {
        const string source = """
            public class {|qa_metrics_excessive_method_count:Manager|}
            {
                public void One() { }
                public void Two() { }
                public void Three() { }
            }
            """;

        return CSharpAnalyzerVerifier<ExcessiveMethodCountAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_excessive_method_count.maxmethods", "2"));
    }

    [Fact]
    public Task TypeWithFewMethods_IsClean()
    {
        const string source = """
            public class Manager
            {
                public void Start() { }
                public void Stop() { }
            }
            """;

        return CSharpAnalyzerVerifier<ExcessiveMethodCountAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_excessive_method_count.maxmethods", "2"));
    }
}
