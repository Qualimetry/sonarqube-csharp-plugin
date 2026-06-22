using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Reliability;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ComSecurityInitializationAnalyzerTests
{
    [Fact]
    public Task CoInitializeSecurityCall_IsReported()
    {
        string source = """
            public sealed class Interop
            {
                static int CoInitializeSecurity(int level) => level;

                void Configure()
                {
                    {|qa_reliability_com_security_initialization:CoInitializeSecurity|}(3);
                }
            }
            """;

        return CSharpAnalyzerVerifier<ComSecurityInitializationAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task UnrelatedCall_IsClean()
    {
        string source = """
            public sealed class Interop
            {
                static int Configure(int level) => level;

                void M()
                {
                    Configure(3);
                }
            }
            """;

        return CSharpAnalyzerVerifier<ComSecurityInitializationAnalyzer>.VerifyAsync(source);
    }
}
