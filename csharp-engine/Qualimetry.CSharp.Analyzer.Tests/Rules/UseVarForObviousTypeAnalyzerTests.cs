using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class UseVarForObviousTypeAnalyzerTests
{
    [Fact]
    public Task ExplicitTypeWithMatchingConstructor_IsReported()
    {
        const string source = """
            public class Factory
            {
                public Widget Build()
                {
                    {|qa_quality_use_var_for_obvious_type:Widget|} widget = new Widget();
                    return widget;
                }
            }

            public class Widget
            {
            }
            """;

        return CSharpAnalyzerVerifier<UseVarForObviousTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task VarDeclaration_IsClean()
    {
        const string source = """
            public class Factory
            {
                public Widget Build()
                {
                    var widget = new Widget();
                    return widget;
                }
            }

            public class Widget
            {
            }
            """;

        return CSharpAnalyzerVerifier<UseVarForObviousTypeAnalyzer>.VerifyAsync(source);
    }
}
