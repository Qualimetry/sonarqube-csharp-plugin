using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class EmptyMethodSummaryAnalyzerTests
{
    [Fact]
    public Task EmptySummary_IsReported()
    {
        const string source = """
            public sealed class Report
            {
                /// <summary></summary>
                public void {|qa_quality_empty_method_summary:Render|}()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<EmptyMethodSummaryAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PopulatedSummary_IsClean()
    {
        const string source = """
            public sealed class Report
            {
                /// <summary>Writes the formatted report to the configured output.</summary>
                public void Render()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<EmptyMethodSummaryAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NoDocumentation_IsClean()
    {
        const string source = """
            public sealed class Report
            {
                public void Render()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<EmptyMethodSummaryAnalyzer>.VerifyAsync(source);
    }
}
