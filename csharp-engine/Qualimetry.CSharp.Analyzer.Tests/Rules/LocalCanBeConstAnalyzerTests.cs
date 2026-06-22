using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class LocalCanBeConstAnalyzerTests
{
    [Fact]
    public Task ConstantLocalNeverReassigned_IsReported()
    {
        const string source = """
            public class C
            {
                public int M()
                {
                    int {|qa_style_local_can_be_const:margin|} = 8;
                    return margin;
                }
            }
            """;

        return CSharpAnalyzerVerifier<LocalCanBeConstAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task AlreadyConst_IsClean()
    {
        const string source = """
            public class C
            {
                public int M()
                {
                    const int margin = 8;
                    return margin;
                }
            }
            """;

        return CSharpAnalyzerVerifier<LocalCanBeConstAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ReassignedLocal_IsClean()
    {
        const string source = """
            public class C
            {
                public int M()
                {
                    int margin = 8;
                    margin = 9;
                    return margin;
                }
            }
            """;

        return CSharpAnalyzerVerifier<LocalCanBeConstAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NonConstantInitializer_IsClean()
    {
        const string source = """
            public class C
            {
                public int M(int seed)
                {
                    int margin = seed;
                    return margin;
                }
            }
            """;

        return CSharpAnalyzerVerifier<LocalCanBeConstAnalyzer>.VerifyAsync(source);
    }
}
