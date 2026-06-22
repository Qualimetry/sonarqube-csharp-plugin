using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ProtectedFieldInSealedTypeAnalyzerTests
{
    [Fact]
    public Task ProtectedFieldInSealedClass_IsReported()
    {
        const string source = """
            public sealed class Cache
            {
                protected int {|qa_quality_protected_field_in_sealed_type:_size|};
            }
            """;

        return CSharpAnalyzerVerifier<ProtectedFieldInSealedTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PrivateFieldInSealedClass_IsClean()
    {
        const string source = """
            public sealed class Cache
            {
                private int _size;
            }
            """;

        return CSharpAnalyzerVerifier<ProtectedFieldInSealedTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ProtectedFieldInOpenClass_IsClean()
    {
        const string source = """
            public class Cache
            {
                protected int _size;
            }
            """;

        return CSharpAnalyzerVerifier<ProtectedFieldInSealedTypeAnalyzer>.VerifyAsync(source);
    }
}
