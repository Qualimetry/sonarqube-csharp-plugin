using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Reliability;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class TypeInitializationCycleAnalyzerTests
{
    [Fact]
    public Task StaticFieldCycle_IsReported()
    {
        const string source = """
            public sealed class Cycle
            {
                private static readonly int A = {|qa_reliability_type_initialization_cycle:Cycle.B|} + 1;
                private static readonly int B = Cycle.A + 1;
            }
            """;

        return CSharpAnalyzerVerifier<TypeInitializationCycleAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task LinearStaticInitialization_IsClean()
    {
        const string source = """
            public sealed class NoCycle
            {
                private static readonly int A = 1;
                private static readonly int B = A + 1;
            }
            """;

        return CSharpAnalyzerVerifier<TypeInitializationCycleAnalyzer>.VerifyAsync(source);
    }
}
