using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Interop;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class PlatformInvokeInNativeMethodsAnalyzerTests
{
    [Fact]
    public Task PlatformInvokeOnOrdinaryClass_IsReported()
    {
        const string source = """
            using System;
            using System.Runtime.InteropServices;

            public static class FileService
            {
                [DllImport("kernel32.dll", SetLastError = true)]
                public static extern bool {|qa_interop_platform_invoke_in_native_methods:CloseHandle|}(IntPtr handle);
            }
            """;

        return CSharpAnalyzerVerifier<PlatformInvokeInNativeMethodsAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PlatformInvokeInNativeMethodsClass_IsClean()
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

        return CSharpAnalyzerVerifier<PlatformInvokeInNativeMethodsAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ManagedMethodOnOrdinaryClass_IsClean()
    {
        const string source = """
            public static class FileService
            {
                public static bool CloseHandle(int handle) => handle == 0;
            }
            """;

        return CSharpAnalyzerVerifier<PlatformInvokeInNativeMethodsAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task LibraryImportOnOrdinaryClass_IsReported()
    {
        const string source = """
            using System;
            using System.Runtime.InteropServices;

            public static partial class FileService
            {
                [LibraryImport("kernel32.dll", SetLastError = true)]
                public static partial bool {|qa_interop_platform_invoke_in_native_methods:CloseHandle|}(IntPtr handle);
            }
            """;

        return CSharpAnalyzerVerifier<PlatformInvokeInNativeMethodsAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task UserDefinedDllImportAttribute_IsClean()
    {
        const string source = """
            using System;

            public sealed class DllImportAttribute : Attribute
            {
            }

            public static class FileService
            {
                [DllImport]
                public static bool CloseHandle(int handle) => handle == 0;
            }
            """;

        return CSharpAnalyzerVerifier<PlatformInvokeInNativeMethodsAnalyzer>.VerifyAsync(source);
    }
}
