using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class PublicMethodInNonPublicTypeAnalyzerTests
{
    [Fact]
    public Task PublicMethodInInternalType_IsReported()
    {
        const string source = """
            internal sealed class Cache
            {
                {|qa_quality_public_method_in_non_public_type:public|} void Evict()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<PublicMethodInNonPublicTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task InternalMethodInInternalType_IsClean()
    {
        const string source = """
            internal sealed class Cache
            {
                internal void Evict()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<PublicMethodInNonPublicTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PublicMethodInPublicType_IsClean()
    {
        const string source = """
            public sealed class Cache
            {
                public void Evict()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<PublicMethodInNonPublicTypeAnalyzer>.VerifyAsync(source);
    }
}
