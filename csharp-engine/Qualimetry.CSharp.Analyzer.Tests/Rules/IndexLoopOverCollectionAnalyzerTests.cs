using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class IndexLoopOverCollectionAnalyzerTests
{
    [Fact]
    public Task IndexLoopOnlyIndexingCollection_IsReported()
    {
        const string source = """
            public class C
            {
                public int Sum(int[] items)
                {
                    var total = 0;
                    {|qa_style_index_loop_over_collection:for|} (int i = 0; i < items.Length; i++)
                    {
                        total += items[i];
                    }

                    return total;
                }
            }
            """;

        return CSharpAnalyzerVerifier<IndexLoopOverCollectionAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task Foreach_IsClean()
    {
        const string source = """
            public class C
            {
                public int Sum(int[] items)
                {
                    var total = 0;
                    foreach (var item in items)
                    {
                        total += item;
                    }

                    return total;
                }
            }
            """;

        return CSharpAnalyzerVerifier<IndexLoopOverCollectionAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task CounterUsedBeyondIndexing_IsClean()
    {
        const string source = """
            using System;

            public class C
            {
                public void Print(int[] items)
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        Console.WriteLine(i + ": " + items[i]);
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<IndexLoopOverCollectionAnalyzer>.VerifyAsync(source);
    }
}
