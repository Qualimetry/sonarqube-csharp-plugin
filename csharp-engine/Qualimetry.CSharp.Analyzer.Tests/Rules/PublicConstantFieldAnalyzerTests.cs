using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class PublicConstantFieldAnalyzerTests
{
    [Fact]
    public Task PublicConstant_IsReported()
    {
        const string source = """
            public class Limits
            {
                public const int {|qa_quality_public_constant_field:MaxRetries|} = 3;
            }
            """;

        return CSharpAnalyzerVerifier<PublicConstantFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PublicStaticReadonly_IsClean()
    {
        const string source = """
            public class Limits
            {
                public static readonly int MaxRetries = 3;
            }
            """;

        return CSharpAnalyzerVerifier<PublicConstantFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PrivateConstant_IsClean()
    {
        const string source = """
            public class Limits
            {
                private const int MaxRetries = 3;
            }
            """;

        return CSharpAnalyzerVerifier<PublicConstantFieldAnalyzer>.VerifyAsync(source);
    }
}
