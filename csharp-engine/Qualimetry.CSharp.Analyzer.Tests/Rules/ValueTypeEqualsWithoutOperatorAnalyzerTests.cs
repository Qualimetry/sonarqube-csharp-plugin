using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ValueTypeEqualsWithoutOperatorAnalyzerTests
{
    [Fact]
    public Task EqualsWithoutOperator_IsReported()
    {
        const string source = """
            public struct {|qa_quality_value_type_equals_without_operator:Temperature|}
            {
                public int Degrees;

                public override bool Equals(object obj) => obj is Temperature t && t.Degrees == Degrees;

                public override int GetHashCode() => Degrees;
            }
            """;

        return CSharpAnalyzerVerifier<ValueTypeEqualsWithoutOperatorAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task EqualsWithOperator_IsClean()
    {
        const string source = """
            public struct Temperature
            {
                public int Degrees;

                public override bool Equals(object obj) => obj is Temperature t && t.Degrees == Degrees;

                public override int GetHashCode() => Degrees;

                public static bool operator ==(Temperature left, Temperature right) => left.Degrees == right.Degrees;

                public static bool operator !=(Temperature left, Temperature right) => !(left == right);
            }
            """;

        return CSharpAnalyzerVerifier<ValueTypeEqualsWithoutOperatorAnalyzer>.VerifyAsync(source);
    }
}
