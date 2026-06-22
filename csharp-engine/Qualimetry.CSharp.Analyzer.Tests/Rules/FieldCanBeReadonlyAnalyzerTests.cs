using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class FieldCanBeReadonlyAnalyzerTests
{
    [Fact]
    public Task FieldAssignedOnlyInConstructor_IsReported()
    {
        const string source = """
            public sealed class Cache
            {
                private int {|qa_style_field_can_be_readonly:_capacity|};

                public Cache(int capacity)
                {
                    _capacity = capacity;
                }
            }
            """;

        return CSharpAnalyzerVerifier<FieldCanBeReadonlyAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ReadonlyField_IsClean()
    {
        const string source = """
            public sealed class Cache
            {
                private readonly int _capacity;

                public Cache(int capacity)
                {
                    _capacity = capacity;
                }
            }
            """;

        return CSharpAnalyzerVerifier<FieldCanBeReadonlyAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task FieldAssignedInMethod_IsClean()
    {
        const string source = """
            public sealed class Cache
            {
                private int _capacity;

                public Cache(int capacity)
                {
                    _capacity = capacity;
                }

                public void Grow()
                {
                    _capacity++;
                }
            }
            """;

        return CSharpAnalyzerVerifier<FieldCanBeReadonlyAnalyzer>.VerifyAsync(source);
    }
}
