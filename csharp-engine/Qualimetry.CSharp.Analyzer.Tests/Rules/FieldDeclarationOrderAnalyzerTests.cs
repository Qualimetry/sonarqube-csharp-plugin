using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class FieldDeclarationOrderAnalyzerTests
{
    [Fact]
    public Task FieldAfterMethod_IsReported()
    {
        const string source = """
            public sealed class Counter
            {
                public void Increment()
                {
                    _value++;
                }

                private int {|qa_style_field_declaration_order:_value|};
            }
            """;

        return CSharpAnalyzerVerifier<FieldDeclarationOrderAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task FieldsFirst_IsClean()
    {
        const string source = """
            public sealed class Counter
            {
                private int _value;

                public void Increment()
                {
                    _value++;
                }
            }
            """;

        return CSharpAnalyzerVerifier<FieldDeclarationOrderAnalyzer>.VerifyAsync(source);
    }
}
