using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ListContainsInLoopAnalyzerTests
{
    [Fact]
    public Task ListContainsInsideLoop_IsReported()
    {
        const string source = """
            using System.Collections.Generic;

            public sealed class Filter
            {
                public int Count(List<int> known, IEnumerable<int> input)
                {
                    var hits = 0;
                    foreach (var value in input)
                    {
                        if ({|qa_quality_list_contains_in_loop:known.Contains(value)|})
                        {
                            hits++;
                        }
                    }

                    return hits;
                }
            }
            """;

        return CSharpAnalyzerVerifier<ListContainsInLoopAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task HashSetContainsInsideLoop_IsClean()
    {
        const string source = """
            using System.Collections.Generic;

            public sealed class Filter
            {
                public int Count(HashSet<int> known, IEnumerable<int> input)
                {
                    var hits = 0;
                    foreach (var value in input)
                    {
                        if (known.Contains(value))
                        {
                            hits++;
                        }
                    }

                    return hits;
                }
            }
            """;

        return CSharpAnalyzerVerifier<ListContainsInLoopAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ListContainsOutsideLoop_IsClean()
    {
        const string source = """
            using System.Collections.Generic;

            public sealed class Filter
            {
                public bool Has(List<int> known, int value) => known.Contains(value);
            }
            """;

        return CSharpAnalyzerVerifier<ListContainsInLoopAnalyzer>.VerifyAsync(source);
    }
}
