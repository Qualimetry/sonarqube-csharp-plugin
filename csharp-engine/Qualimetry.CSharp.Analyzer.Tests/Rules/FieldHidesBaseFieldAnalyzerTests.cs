using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class FieldHidesBaseFieldAnalyzerTests
{
    [Fact]
    public Task FieldHidingBaseField_IsReported()
    {
        const string source = """
            public class Base
            {
                protected int count;
            }

            public sealed class Derived : Base
            {
                private int {|qa_quality_field_hides_base_field:count|};
            }
            """;

        return CSharpAnalyzerVerifier<FieldHidesBaseFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DistinctFieldName_IsClean()
    {
        const string source = """
            public class Base
            {
                protected int count;
            }

            public sealed class Derived : Base
            {
                private int processedCount;
            }
            """;

        return CSharpAnalyzerVerifier<FieldHidesBaseFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ExplicitNewModifier_IsClean()
    {
        const string source = """
            public class Base
            {
                protected int count;
            }

            public sealed class Derived : Base
            {
                private new int count;
            }
            """;

        return CSharpAnalyzerVerifier<FieldHidesBaseFieldAnalyzer>.VerifyAsync(source);
    }
}
