using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class SealableInternalClassAnalyzerTests
{
    [Fact]
    public Task InternalClassWithoutDescendants_IsReported()
    {
        const string source = """
            internal class {|qa_quality_sealable_internal_class:PriceFormatter|}
            {
                public string Format(decimal value) => value.ToString("C");
            }
            """;

        return CSharpAnalyzerVerifier<SealableInternalClassAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task SealedInternalClass_IsClean()
    {
        const string source = """
            internal sealed class PriceFormatter
            {
                public string Format(decimal value) => value.ToString("C");
            }
            """;

        return CSharpAnalyzerVerifier<SealableInternalClassAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task InternalBaseClassWithDescendant_IsClean()
    {
        const string source = """
            internal class Shape
            {
            }

            internal sealed class Circle : Shape
            {
            }
            """;

        return CSharpAnalyzerVerifier<SealableInternalClassAnalyzer>.VerifyAsync(source);
    }
}
