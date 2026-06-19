using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ProtectedNestedTypeInSealedTypeAnalyzerTests
{
    [Fact]
    public Task ProtectedNestedTypeInsideSealedClass_IsReported()
    {
        const string source = """
            public sealed class Cache
            {
                protected class {|qa_quality_protected_nested_type_in_sealed_type:Slot|}
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<ProtectedNestedTypeInSealedTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PrivateNestedTypeInsideSealedClass_IsClean()
    {
        const string source = """
            public sealed class Cache
            {
                private class Slot
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<ProtectedNestedTypeInSealedTypeAnalyzer>.VerifyAsync(source);
    }
}
