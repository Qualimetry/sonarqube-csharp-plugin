using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Unity;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class InitializeOnLoadStaticConstructorAnalyzerTests
{
    private const string Stubs = """
        using System;

        public sealed class InitializeOnLoadAttribute : Attribute { }
        """;

    [Fact]
    public Task InitializeOnLoadWithoutStaticConstructor_IsReported()
    {
        const string source = Stubs + """

            [InitializeOnLoad]
            public class {|qa_unity_initialize_on_load_static_constructor:EditorBootstrap|}
            {
                public static bool Ready;
            }
            """;

        return CSharpAnalyzerVerifier<InitializeOnLoadStaticConstructorAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task InitializeOnLoadWithStaticConstructor_IsClean()
    {
        const string source = Stubs + """

            [InitializeOnLoad]
            public class EditorBootstrap
            {
                public static bool Ready;

                static EditorBootstrap()
                {
                    Ready = true;
                }
            }
            """;

        return CSharpAnalyzerVerifier<InitializeOnLoadStaticConstructorAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PlainClassWithoutStaticConstructor_IsClean()
    {
        const string source = """
            public class PlainType
            {
                public static bool Ready;
            }
            """;

        return CSharpAnalyzerVerifier<InitializeOnLoadStaticConstructorAnalyzer>.VerifyAsync(source);
    }
}
