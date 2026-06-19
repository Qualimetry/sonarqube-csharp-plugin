using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Contract;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class PureMethodShouldBeMarkedPureAnalyzerTests
{
    [Fact]
    public Task SideEffectFreeComputation_IsReported()
    {
        const string source = """
            public class Geometry
            {
                public int {|qa_contract_pure_method_should_be_marked_pure:Square|}(int n) => n * n;
            }
            """;

        return CSharpAnalyzerVerifier<PureMethodShouldBeMarkedPureAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task MethodAlreadyMarkedPure_IsClean()
    {
        const string source = """
            using System.Diagnostics.Contracts;

            public class Geometry
            {
                [Pure]
                public int Square(int n) => n * n;
            }
            """;

        return CSharpAnalyzerVerifier<PureMethodShouldBeMarkedPureAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task MethodWithInvocation_IsClean()
    {
        const string source = """
            public class Service
            {
                public int Read(int n) => System.Math.Abs(n);
            }
            """;

        return CSharpAnalyzerVerifier<PureMethodShouldBeMarkedPureAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task VoidExpressionMethod_IsClean()
    {
        const string source = """
            public class Service
            {
                private int _seed;

                public void Reset() => _seed = 0;
            }
            """;

        return CSharpAnalyzerVerifier<PureMethodShouldBeMarkedPureAnalyzer>.VerifyAsync(source);
    }
}
