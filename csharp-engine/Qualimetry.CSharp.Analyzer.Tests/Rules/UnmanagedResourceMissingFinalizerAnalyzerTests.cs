using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class UnmanagedResourceMissingFinalizerAnalyzerTests
{
    [Fact]
    public Task HandleFieldWithoutFinalizer_IsReported()
    {
        const string source = """
            using System;

            public class {|qa_quality_unmanaged_resource_missing_finalizer:FileWrapper|} : IDisposable
            {
                private IntPtr handle;

                public void Dispose()
                {
                    Release(handle);
                }

                private static void Release(IntPtr h)
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<UnmanagedResourceMissingFinalizerAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task HandleFieldWithFinalizer_IsClean()
    {
        const string source = """
            using System;

            public class FileWrapper : IDisposable
            {
                private IntPtr handle;

                ~FileWrapper()
                {
                    Release(handle);
                }

                public void Dispose()
                {
                    Release(handle);
                    GC.SuppressFinalize(this);
                }

                private static void Release(IntPtr h)
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<UnmanagedResourceMissingFinalizerAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ManagedDisposableWithoutHandle_IsClean()
    {
        const string source = """
            using System;

            public class Wrapper : IDisposable
            {
                public void Dispose()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<UnmanagedResourceMissingFinalizerAnalyzer>.VerifyAsync(source);
    }
}
