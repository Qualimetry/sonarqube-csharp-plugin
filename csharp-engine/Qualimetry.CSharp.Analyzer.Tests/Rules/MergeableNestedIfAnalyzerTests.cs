using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class MergeableNestedIfAnalyzerTests
{
    [Fact]
    public Task NestedIfWithoutElse_IsReported()
    {
        const string source = """
            public class C
            {
                public void M(bool a, bool b)
                {
                    {|qa_style_mergeable_nested_if:if|} (a)
                    {
                        if (b)
                        {
                            Do();
                        }
                    }
                }

                private void Do()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<MergeableNestedIfAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task CombinedCondition_IsClean()
    {
        const string source = """
            public class C
            {
                public void M(bool a, bool b)
                {
                    if (a && b)
                    {
                        Do();
                    }
                }

                private void Do()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<MergeableNestedIfAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NestedIfWithElse_IsClean()
    {
        const string source = """
            public class C
            {
                public void M(bool a, bool b)
                {
                    if (a)
                    {
                        if (b)
                        {
                            Do();
                        }
                        else
                        {
                            Do();
                        }
                    }
                }

                private void Do()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<MergeableNestedIfAnalyzer>.VerifyAsync(source);
    }
}
