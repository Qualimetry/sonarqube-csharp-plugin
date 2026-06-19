using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class DisposeMethodWithoutIDisposableAnalyzerTests
{
    [Fact]
    public Task DisposeOnNonDisposableType_IsReported()
    {
        const string source = """
            public sealed class Buffer
            {
                public void {|qa_quality_dispose_method_without_i_disposable:Dispose|}()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<DisposeMethodWithoutIDisposableAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DisposeOnDisposableType_IsClean()
    {
        const string source = """
            using System;

            public sealed class Buffer : IDisposable
            {
                public void Dispose()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<DisposeMethodWithoutIDisposableAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DifferentlyNamedMethod_IsClean()
    {
        const string source = """
            public sealed class Buffer
            {
                public void Clear()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<DisposeMethodWithoutIDisposableAnalyzer>.VerifyAsync(source);
    }
}
