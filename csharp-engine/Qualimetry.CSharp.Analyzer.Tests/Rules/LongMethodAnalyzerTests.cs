using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Metrics;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class LongMethodAnalyzerTests
{
    [Fact]
    public Task MethodWithManyStatements_IsReported()
    {
        const string source = """
            public class C
            {
                public int {|qa_metrics_long_method:Tally|}()
                {
                    int a = 1;
                    int b = 2;
                    return a + b;
                }
            }
            """;

        return CSharpAnalyzerVerifier<LongMethodAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_long_method.maxstatements", "2"));
    }

    [Fact]
    public Task ShortMethod_IsClean()
    {
        const string source = """
            public class C
            {
                public int Tally()
                {
                    return 0;
                }
            }
            """;

        return CSharpAnalyzerVerifier<LongMethodAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_long_method.maxstatements", "2"));
    }

    [Fact]
    public Task ShortMethodWithLocalFunction_IsClean()
    {
        const string source = """
            public class C
            {
                public int Run()
                {
                    int Helper()
                    {
                        int x = 1;
                        int y = 2;
                        int z = 3;
                        return x + y + z;
                    }

                    return Helper();
                }
            }
            """;

        return CSharpAnalyzerVerifier<LongMethodAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_long_method.maxstatements", "3"));
    }

    [Fact]
    public Task MethodWithLocalFunctionAndManyStatements_IsReported()
    {
        const string source = """
            public class C
            {
                public int {|qa_metrics_long_method:Run|}()
                {
                    int Helper() => 1;
                    int a = Helper();
                    int b = a + 1;
                    return b;
                }
            }
            """;

        return CSharpAnalyzerVerifier<LongMethodAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_long_method.maxstatements", "2"));
    }
}
