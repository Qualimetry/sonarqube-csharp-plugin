using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class SplittableConjunctionIfAnalyzerTests
{
    [Fact]
    public Task ConjunctionWithoutElse_IsReported()
    {
        const string source = """
            public class C
            {
                public void Save(string name, bool dirty)
                {
                    {|qa_style_splittable_conjunction_if:if|} (name != null && dirty)
                    {
                        System.Console.WriteLine(name);
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<SplittableConjunctionIfAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NestedIfs_AreClean()
    {
        const string source = """
            public class C
            {
                public void Save(string name, bool dirty)
                {
                    if (name != null)
                    {
                        if (dirty)
                        {
                            System.Console.WriteLine(name);
                        }
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<SplittableConjunctionIfAnalyzer>.VerifyAsync(source);
    }
}
