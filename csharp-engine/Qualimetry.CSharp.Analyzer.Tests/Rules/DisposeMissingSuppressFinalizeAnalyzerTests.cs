using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class DisposeMissingSuppressFinalizeAnalyzerTests
{
    [Fact]
    public Task DisposeWithoutSuppressFinalize_IsReported()
    {
        const string source = """
            using System;

            public class Connection : IDisposable
            {
                ~Connection()
                {
                    Close();
                }

                public void {|qa_quality_dispose_missing_suppress_finalize:Dispose|}()
                {
                    Close();
                }

                private void Close()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<DisposeMissingSuppressFinalizeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DisposeWithSuppressFinalize_IsClean()
    {
        const string source = """
            using System;

            public class Connection : IDisposable
            {
                ~Connection()
                {
                    Close();
                }

                public void Dispose()
                {
                    Close();
                    GC.SuppressFinalize(this);
                }

                private void Close()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<DisposeMissingSuppressFinalizeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DisposeWithoutFinalizer_IsClean()
    {
        const string source = """
            using System;

            public class Connection : IDisposable
            {
                public void Dispose()
                {
                    Close();
                }

                private void Close()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<DisposeMissingSuppressFinalizeAnalyzer>.VerifyAsync(source);
    }
}
