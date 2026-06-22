using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class NegatedAllToAnyAnalyzerTests
{
    [Fact]
    public Task NegatedAll_IsReported()
    {
        const string source = """
            using System.Collections.Generic;
            using System.Linq;

            public class C
            {
                public bool M(IEnumerable<int> items)
                {
                    return !{|qa_style_negated_all_to_any:items.All(i => i > 0)|};
                }
            }
            """;

        return CSharpAnalyzerVerifier<NegatedAllToAnyAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task AnyWithNegatedPredicate_IsClean()
    {
        const string source = """
            using System.Collections.Generic;
            using System.Linq;

            public class C
            {
                public bool M(IEnumerable<int> items)
                {
                    return items.Any(i => i <= 0);
                }
            }
            """;

        return CSharpAnalyzerVerifier<NegatedAllToAnyAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NegatedAllWithImpurePredicate_IsClean()
    {
        const string source = """
            using System.Collections.Generic;
            using System.Linq;

            public class C
            {
                public bool M(IEnumerable<int> items)
                {
                    return !items.All(i => Check(i));
                }

                private bool Check(int value) => value > 0;
            }
            """;

        return CSharpAnalyzerVerifier<NegatedAllToAnyAnalyzer>.VerifyAsync(source);
    }
}
