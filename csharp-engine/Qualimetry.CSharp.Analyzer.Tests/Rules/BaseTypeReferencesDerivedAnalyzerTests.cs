using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class BaseTypeReferencesDerivedAnalyzerTests
{
    [Fact]
    public Task BaseConstructsDerived_IsReported()
    {
        const string source = """
            public class Widget
            {
                public Widget CreateDefault()
                {
                    return new {|qa_quality_base_type_references_derived:Button|}();
                }
            }

            public class Button : Widget
            {
            }
            """;

        return CSharpAnalyzerVerifier<BaseTypeReferencesDerivedAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task BaseIndependentOfDerived_IsClean()
    {
        const string source = """
            public class Widget
            {
                public virtual Widget CreateDefault()
                {
                    return new Widget();
                }
            }

            public class Button : Widget
            {
            }
            """;

        return CSharpAnalyzerVerifier<BaseTypeReferencesDerivedAnalyzer>.VerifyAsync(source);
    }
}
