using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class UndisposedDisposableLocalAnalyzerTests
{
    [Fact]
    public Task UndisposedLocal_IsReported()
    {
        const string source = """
            using System.IO;

            public class Writer
            {
                public void Save(string path)
                {
                    var stream = {|qa_quality_undisposed_disposable_local:new FileStream(path, FileMode.Create)|};
                    stream.WriteByte(0);
                }
            }
            """;

        return CSharpAnalyzerVerifier<UndisposedDisposableLocalAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task UsingDeclaration_IsClean()
    {
        const string source = """
            using System.IO;

            public class Writer
            {
                public void Save(string path)
                {
                    using var stream = new FileStream(path, FileMode.Create);
                    stream.WriteByte(0);
                }
            }
            """;

        return CSharpAnalyzerVerifier<UndisposedDisposableLocalAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ReturnedDisposable_IsClean()
    {
        const string source = """
            using System.IO;

            public class Writer
            {
                public Stream Open(string path)
                {
                    var stream = new FileStream(path, FileMode.Open);
                    return stream;
                }
            }
            """;

        return CSharpAnalyzerVerifier<UndisposedDisposableLocalAnalyzer>.VerifyAsync(source);
    }
}
