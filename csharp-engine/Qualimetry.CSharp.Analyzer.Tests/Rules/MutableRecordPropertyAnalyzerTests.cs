using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class MutableRecordPropertyAnalyzerTests
{
    [Fact]
    public Task RecordPropertyWithSetter_IsReported()
    {
        const string source = """
            public record Money
            {
                public decimal Amount { get; {|qa_quality_mutable_record_property:set|}; }
                public string Currency { get; init; }
            }
            """;

        return CSharpAnalyzerVerifier<MutableRecordPropertyAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task RecordWithInitOnlyProperties_IsClean()
    {
        const string source = """
            public record Money
            {
                public decimal Amount { get; init; }
                public string Currency { get; init; }
            }
            """;

        return CSharpAnalyzerVerifier<MutableRecordPropertyAnalyzer>.VerifyAsync(source);
    }
}
