using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class DisposableFieldNotDisposedAnalyzerTests
{
    [Fact]
    public Task DisposableFieldNotDisposed_IsReported()
    {
        const string source = """
            using System;

            public sealed class Resource : IDisposable
            {
                public void Dispose()
                {
                }
            }

            public class Importer : IDisposable
            {
                private readonly Resource {|qa_quality_disposable_field_not_disposed:resource|} = new();

                public void Dispose()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<DisposableFieldNotDisposedAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DisposableFieldDisposed_IsClean()
    {
        const string source = """
            using System;

            public sealed class Resource : IDisposable
            {
                public void Dispose()
                {
                }
            }

            public class Importer : IDisposable
            {
                private readonly Resource resource = new();

                public void Dispose()
                {
                    resource.Dispose();
                }
            }
            """;

        return CSharpAnalyzerVerifier<DisposableFieldNotDisposedAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NonDisposableField_IsClean()
    {
        const string source = """
            using System;

            public class Importer : IDisposable
            {
                private readonly int count;

                public void Dispose()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<DisposableFieldNotDisposedAnalyzer>.VerifyAsync(source);
    }
}
