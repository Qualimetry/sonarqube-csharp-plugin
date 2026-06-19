using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Interop;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class NativeMethodsContainerModifiersAnalyzerTests
{
    [Fact]
    public Task PublicNativeMethodsClass_IsReported()
    {
        const string source = """
            public class {|qa_interop_native_methods_container_modifiers:NativeMethods|}
            {
            }
            """;

        return CSharpAnalyzerVerifier<NativeMethodsContainerModifiersAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NonStaticNativeMethodsClass_IsReported()
    {
        const string source = """
            internal class {|qa_interop_native_methods_container_modifiers:SafeNativeMethods|}
            {
            }
            """;

        return CSharpAnalyzerVerifier<NativeMethodsContainerModifiersAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task InternalStaticNativeMethodsClass_IsClean()
    {
        const string source = """
            internal static class NativeMethods
            {
            }
            """;

        return CSharpAnalyzerVerifier<NativeMethodsContainerModifiersAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task UnrelatedClass_IsClean()
    {
        const string source = """
            public class FileService
            {
            }
            """;

        return CSharpAnalyzerVerifier<NativeMethodsContainerModifiersAnalyzer>.VerifyAsync(source);
    }
}
