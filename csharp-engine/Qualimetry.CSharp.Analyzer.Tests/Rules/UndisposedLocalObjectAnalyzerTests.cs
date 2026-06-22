using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class UndisposedLocalObjectAnalyzerTests
{
    [Fact]
    public Task DiscardedDisposable_IsReported()
    {
        const string source = """
            using System.IO;

            public class Loader
            {
                public void Touch(string path)
                {
                    {|qa_quality_undisposed_local_object:new StreamReader(path)|};
                }
            }
            """;

        return CSharpAnalyzerVerifier<UndisposedLocalObjectAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task UsingDeclaration_IsClean()
    {
        const string source = """
            using System.IO;

            public class Loader
            {
                public string Touch(string path)
                {
                    using var reader = new StreamReader(path);
                    return reader.ReadToEnd();
                }
            }
            """;

        return CSharpAnalyzerVerifier<UndisposedLocalObjectAnalyzer>.VerifyAsync(source);
    }
}
