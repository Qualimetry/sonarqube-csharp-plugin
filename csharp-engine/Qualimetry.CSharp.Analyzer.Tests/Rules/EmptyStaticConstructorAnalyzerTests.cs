using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class EmptyStaticConstructorAnalyzerTests
{
    [Fact]
    public Task EmptyStaticConstructor_IsReported()
    {
        const string source = """
            public class Settings
            {
                static {|qa_quality_empty_static_constructor:Settings|}()
                {
                }

                public static int Limit = 100;
            }
            """;

        return CSharpAnalyzerVerifier<EmptyStaticConstructorAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NoStaticConstructor_IsClean()
    {
        const string source = """
            public class Settings
            {
                public static int Limit = 100;
            }
            """;

        return CSharpAnalyzerVerifier<EmptyStaticConstructorAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task StaticConstructorWithBody_IsClean()
    {
        const string source = """
            public class Settings
            {
                public static int Limit;

                static Settings()
                {
                    Limit = 100;
                }
            }
            """;

        return CSharpAnalyzerVerifier<EmptyStaticConstructorAnalyzer>.VerifyAsync(source);
    }
}
