using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ReaderWriterLockAcquireWithoutReleaseAnalyzerTests
{
    [Fact]
    public Task AcquireWithoutRelease_IsReported()
    {
        const string source = """
            using System.Threading;

            public class Store
            {
                private readonly ReaderWriterLock _lock = new ReaderWriterLock();

                public void Append()
                {
                    {|qa_quality_reader_writer_lock_acquire_without_release:_lock.AcquireWriterLock(1000)|};
                    Write();
                }

                private void Write()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<ReaderWriterLockAcquireWithoutReleaseAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task AcquireWithRelease_IsClean()
    {
        const string source = """
            using System.Threading;

            public class Store
            {
                private readonly ReaderWriterLock _lock = new ReaderWriterLock();

                public void Append()
                {
                    _lock.AcquireWriterLock(1000);
                    try
                    {
                        Write();
                    }
                    finally
                    {
                        _lock.ReleaseWriterLock();
                    }
                }

                private void Write()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<ReaderWriterLockAcquireWithoutReleaseAnalyzer>.VerifyAsync(source);
    }
}
