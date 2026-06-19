using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class PublicPlatformInvokeAnalyzerTests
{
    [Fact]
    public Task PublicPlatformInvoke_IsReported()
    {
        const string source = """
            using System.Runtime.InteropServices;

            public class Native
            {
                [DllImport("kernel32.dll")]
                public static extern int {|qa_quality_public_platform_invoke:GetCurrentProcessId|}();
            }
            """;

        return CSharpAnalyzerVerifier<PublicPlatformInvokeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task InternalPlatformInvoke_IsClean()
    {
        const string source = """
            using System.Runtime.InteropServices;

            internal static class Native
            {
                [DllImport("kernel32.dll")]
                internal static extern int GetCurrentProcessId();
            }
            """;

        return CSharpAnalyzerVerifier<PublicPlatformInvokeAnalyzer>.VerifyAsync(source);
    }
}
