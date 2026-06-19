using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class OverexposedNestedTypeAnalyzerTests
{
    [Fact]
    public Task PublicNestedTypeInsideInternalParent_IsReported()
    {
        const string source = """
            internal class Container
            {
                public class {|qa_quality_overexposed_nested_type:Entry|}
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<OverexposedNestedTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NestedTypeNotMoreVisibleThanParent_IsClean()
    {
        const string source = """
            internal class Container
            {
                internal class Entry
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<OverexposedNestedTypeAnalyzer>.VerifyAsync(source);
    }
}
