using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Contract;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class PureMethodMustNotMutateStateAnalyzerTests
{
    [Fact]
    public Task PureMethodMutatingField_IsReported()
    {
        const string source = """
            using System.Diagnostics.Contracts;

            public class Counter
            {
                private int _count;

                [Pure]
                public int {|qa_contract_pure_method_must_not_mutate_state:Next|}()
                {
                    _count++;
                    return _count;
                }
            }
            """;

        return CSharpAnalyzerVerifier<PureMethodMustNotMutateStateAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PureMethodWithoutSideEffects_IsClean()
    {
        const string source = """
            using System.Diagnostics.Contracts;

            public class Calculator
            {
                [Pure]
                public int Add(int a, int b)
                {
                    return a + b;
                }
            }
            """;

        return CSharpAnalyzerVerifier<PureMethodMustNotMutateStateAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PureMethodAssigningLocal_IsClean()
    {
        const string source = """
            using System.Diagnostics.Contracts;

            public class Calculator
            {
                [Pure]
                public int Scale(int value)
                {
                    var doubled = value * 2;
                    return doubled;
                }
            }
            """;

        return CSharpAnalyzerVerifier<PureMethodMustNotMutateStateAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task MethodWithoutPureAttribute_IsClean()
    {
        const string source = """
            public class Counter
            {
                private int _count;

                public int Next()
                {
                    _count++;
                    return _count;
                }
            }
            """;

        return CSharpAnalyzerVerifier<PureMethodMustNotMutateStateAnalyzer>.VerifyAsync(source);
    }
}
