using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class DisposableFieldInNonDisposableTypeAnalyzerTests
{
    [Fact]
    public Task OwnedByInitializer_IsReported()
    {
        const string source = """
            using System.IO;

            public class {|qa_quality_disposable_field_in_non_disposable_type:Recorder|}
            {
                private readonly MemoryStream _stream = new MemoryStream();

                public long Size => _stream.Length;
            }
            """;

        return CSharpAnalyzerVerifier<DisposableFieldInNonDisposableTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task OwnedByConstructor_IsReported()
    {
        const string source = """
            using System.IO;

            public class {|qa_quality_disposable_field_in_non_disposable_type:Recorder|}
            {
                private readonly MemoryStream _stream;

                public Recorder()
                {
                    _stream = new MemoryStream();
                }
            }
            """;

        return CSharpAnalyzerVerifier<DisposableFieldInNonDisposableTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task InjectedDisposable_IsClean()
    {
        const string source = """
            using System.Net.Http;

            public class JwtTokenClient
            {
                private readonly HttpClient _httpClient;

                public JwtTokenClient(HttpClient httpClient)
                {
                    _httpClient = httpClient;
                }
            }
            """;

        return CSharpAnalyzerVerifier<DisposableFieldInNonDisposableTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NonDisposableField_IsClean()
    {
        const string source = """
            public class Settings
            {
            }

            public class JwtTokenClient
            {
                private readonly Settings _settings;

                public JwtTokenClient(Settings settings)
                {
                    _settings = settings;
                }
            }
            """;

        return CSharpAnalyzerVerifier<DisposableFieldInNonDisposableTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DisposableFieldInDisposableType_IsClean()
    {
        const string source = """
            using System;
            using System.IO;

            public class Recorder : IDisposable
            {
                private readonly MemoryStream _stream = new MemoryStream();

                public long Size => _stream.Length;

                public void Dispose() => _stream.Dispose();
            }
            """;

        return CSharpAnalyzerVerifier<DisposableFieldInNonDisposableTypeAnalyzer>.VerifyAsync(source);
    }
}
