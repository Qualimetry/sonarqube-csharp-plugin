using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class BoxingConversionAnalyzerTests
{
    [Fact]
    public Task ImplicitBoxing_IsReported()
    {
        const string source = """
            public sealed class Counter
            {
                public void Report(object value)
                {
                }

                public void Emit(int count)
                {
                    Report({|qa_quality_boxing_conversion:count|});
                }
            }
            """;

        return CSharpAnalyzerVerifier<BoxingConversionAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task TypedCall_IsClean()
    {
        const string source = """
            public sealed class Counter
            {
                public void Report(int value)
                {
                }

                public void Emit(int count)
                {
                    Report(count);
                }
            }
            """;

        return CSharpAnalyzerVerifier<BoxingConversionAnalyzer>.VerifyAsync(source);
    }
}
