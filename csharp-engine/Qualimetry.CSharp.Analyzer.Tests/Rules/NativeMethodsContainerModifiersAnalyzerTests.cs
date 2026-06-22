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
            using System;
            using System.Runtime.InteropServices;

            public class {|qa_interop_native_methods_container_modifiers:NativeMethods|}
            {
                [DllImport("kernel32.dll", SetLastError = true)]
                public static extern bool CloseHandle(IntPtr handle);
            }
            """;

        return CSharpAnalyzerVerifier<NativeMethodsContainerModifiersAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NonStaticNativeMethodsClass_IsReported()
    {
        const string source = """
            using System;
            using System.Runtime.InteropServices;

            internal class {|qa_interop_native_methods_container_modifiers:SafeNativeMethods|}
            {
                [DllImport("kernel32.dll", SetLastError = true)]
                internal static extern bool CloseHandle(IntPtr handle);
            }
            """;

        return CSharpAnalyzerVerifier<NativeMethodsContainerModifiersAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task InternalStaticNativeMethodsClass_IsClean()
    {
        const string source = """
            using System;
            using System.Runtime.InteropServices;

            internal static class NativeMethods
            {
                [DllImport("kernel32.dll", SetLastError = true)]
                internal static extern bool CloseHandle(IntPtr handle);
            }
            """;

        return CSharpAnalyzerVerifier<NativeMethodsContainerModifiersAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DomainTypeNamedNativeMethodsWithoutPlatformInvoke_IsClean()
    {
        const string source = """
            public class OrderNativeMethods
            {
                public int Total { get; set; }

                public int Calculate(int quantity) => Total * quantity;
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
