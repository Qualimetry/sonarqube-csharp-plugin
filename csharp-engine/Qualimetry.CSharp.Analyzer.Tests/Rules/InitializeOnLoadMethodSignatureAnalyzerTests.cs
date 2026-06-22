using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Unity;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class InitializeOnLoadMethodSignatureAnalyzerTests
{
    private const string Stubs = """
        using System;

        public sealed class InitializeOnLoadMethodAttribute : Attribute { }
        """;

    [Fact]
    public Task InstanceLoadMethod_IsReported()
    {
        const string source = Stubs + """

            public class Loader
            {
                [InitializeOnLoadMethod]
                void {|qa_unity_initialize_on_load_method_signature:Register|}()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<InitializeOnLoadMethodSignatureAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task LoadMethodWithParameter_IsReported()
    {
        const string source = Stubs + """

            public class Loader
            {
                [InitializeOnLoadMethod]
                static void {|qa_unity_initialize_on_load_method_signature:Register|}(int value)
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<InitializeOnLoadMethodSignatureAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task StaticParameterlessLoadMethod_IsClean()
    {
        const string source = Stubs + """

            public class Loader
            {
                [InitializeOnLoadMethod]
                static void Register()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<InitializeOnLoadMethodSignatureAnalyzer>.VerifyAsync(source);
    }
}
