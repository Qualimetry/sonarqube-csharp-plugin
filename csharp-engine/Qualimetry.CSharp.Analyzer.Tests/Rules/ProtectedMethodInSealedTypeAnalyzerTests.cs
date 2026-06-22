using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ProtectedMethodInSealedTypeAnalyzerTests
{
    [Fact]
    public Task ProtectedMethodInSealedClass_IsReported()
    {
        const string source = """
            public sealed class Report
            {
                protected void {|qa_quality_protected_method_in_sealed_type:Render|}()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<ProtectedMethodInSealedTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PrivateMethodInSealedClass_IsClean()
    {
        const string source = """
            public sealed class Report
            {
                private void Render()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<ProtectedMethodInSealedTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ProtectedMethodInOpenClass_IsClean()
    {
        const string source = """
            public class Report
            {
                protected void Render()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<ProtectedMethodInSealedTypeAnalyzer>.VerifyAsync(source);
    }
}
