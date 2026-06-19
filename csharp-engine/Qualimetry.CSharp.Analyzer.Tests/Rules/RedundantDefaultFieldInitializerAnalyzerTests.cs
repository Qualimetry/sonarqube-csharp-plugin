using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class RedundantDefaultFieldInitializerAnalyzerTests
{
    [Fact]
    public Task ZeroInitializer_IsReported()
    {
        const string source = """
            public class C
            {
                private int _retries = {|qa_style_redundant_default_field_initializer:0|};
            }
            """;

        return CSharpAnalyzerVerifier<RedundantDefaultFieldInitializerAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NoInitializer_IsClean()
    {
        const string source = """
            public class C
            {
                private int _retries;
            }
            """;

        return CSharpAnalyzerVerifier<RedundantDefaultFieldInitializerAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NonDefaultInitializer_IsClean()
    {
        const string source = """
            public class C
            {
                private int _retries = 3;
            }
            """;

        return CSharpAnalyzerVerifier<RedundantDefaultFieldInitializerAnalyzer>.VerifyAsync(source);
    }
}
