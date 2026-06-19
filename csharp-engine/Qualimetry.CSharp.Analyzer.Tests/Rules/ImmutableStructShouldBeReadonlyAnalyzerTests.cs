using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Contract;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ImmutableStructShouldBeReadonlyAnalyzerTests
{
    [Fact]
    public Task StructWithOnlyImmutableState_IsReported()
    {
        const string source = """
            public struct {|qa_contract_immutable_struct_should_be_readonly:Money|}
            {
                private readonly decimal _amount;

                public Money(decimal amount)
                {
                    _amount = amount;
                }

                public decimal Amount => _amount;
            }
            """;

        return CSharpAnalyzerVerifier<ImmutableStructShouldBeReadonlyAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ReadonlyStruct_IsClean()
    {
        const string source = """
            public readonly struct Money
            {
                private readonly decimal _amount;

                public Money(decimal amount)
                {
                    _amount = amount;
                }

                public decimal Amount => _amount;
            }
            """;

        return CSharpAnalyzerVerifier<ImmutableStructShouldBeReadonlyAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task StructWithMutableField_IsClean()
    {
        const string source = """
            public struct MutablePoint
            {
                public int X;
                public int Y;
            }
            """;

        return CSharpAnalyzerVerifier<ImmutableStructShouldBeReadonlyAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task StructWithSettableProperty_IsClean()
    {
        const string source = """
            public struct Label
            {
                public string Text { get; set; }
            }
            """;

        return CSharpAnalyzerVerifier<ImmutableStructShouldBeReadonlyAnalyzer>.VerifyAsync(source);
    }
}
