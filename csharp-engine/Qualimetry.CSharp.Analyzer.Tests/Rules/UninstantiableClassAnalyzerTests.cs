using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class UninstantiableClassAnalyzerTests
{
    [Fact]
    public Task PrivateConstructorNoStaticMembers_IsReported()
    {
        const string source = """
            public class {|qa_quality_uninstantiable_class:StringTools|}
            {
                private StringTools()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<UninstantiableClassAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task StaticClass_IsClean()
    {
        const string source = """
            public static class StringTools
            {
                public static string Reverse(string value) => value;
            }
            """;

        return CSharpAnalyzerVerifier<UninstantiableClassAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PrivateConstructorWithStaticMember_IsClean()
    {
        const string source = """
            public class Singleton
            {
                private Singleton()
                {
                }

                public static Singleton Instance { get; } = new Singleton();
            }
            """;

        return CSharpAnalyzerVerifier<UninstantiableClassAnalyzer>.VerifyAsync(source);
    }
}
