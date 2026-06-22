using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class NegatedAnyToAllAnalyzerTests
{
    [Fact]
    public Task NegatedAny_IsReported()
    {
        const string source = """
            using System.Collections.Generic;
            using System.Linq;

            public class C
            {
                public bool M(IEnumerable<int> items)
                {
                    return !{|qa_style_negated_any_to_all:items.Any(i => i > 0)|};
                }
            }
            """;

        return CSharpAnalyzerVerifier<NegatedAnyToAllAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task AllWithNegatedPredicate_IsClean()
    {
        const string source = """
            using System.Collections.Generic;
            using System.Linq;

            public class C
            {
                public bool M(IEnumerable<int> items)
                {
                    return items.All(i => i <= 0);
                }
            }
            """;

        return CSharpAnalyzerVerifier<NegatedAnyToAllAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NegatedAnyWithImpurePredicate_IsClean()
    {
        const string source = """
            using System.Collections.Generic;
            using System.Linq;

            public class C
            {
                public bool M(IEnumerable<int> items)
                {
                    return !items.Any(i => Check(i));
                }

                private bool Check(int value) => value > 0;
            }
            """;

        return CSharpAnalyzerVerifier<NegatedAnyToAllAnalyzer>.VerifyAsync(source);
    }
}
