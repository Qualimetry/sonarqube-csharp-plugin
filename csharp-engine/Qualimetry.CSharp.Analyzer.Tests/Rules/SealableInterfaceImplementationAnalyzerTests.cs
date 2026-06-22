using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class SealableInterfaceImplementationAnalyzerTests
{
    [Fact]
    public Task VirtualImplementationOfInternalInterface_IsReported()
    {
        const string source = """
            internal interface IHandler
            {
                void Handle();
            }

            public class Worker : IHandler
            {
                public virtual void {|qa_quality_sealable_interface_implementation:Handle|}()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<SealableInterfaceImplementationAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NonVirtualImplementation_IsClean()
    {
        const string source = """
            internal interface IHandler
            {
                void Handle();
            }

            public class Worker : IHandler
            {
                public void Handle()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<SealableInterfaceImplementationAnalyzer>.VerifyAsync(source);
    }
}
