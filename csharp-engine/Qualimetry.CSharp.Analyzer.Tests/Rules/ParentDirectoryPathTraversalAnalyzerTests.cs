using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ParentDirectoryPathTraversalAnalyzerTests
{
    [Fact]
    public Task PathClimbingOutOfDirectory_IsReported()
    {
        const string source = """
            public sealed class Loader
            {
                public string Path() => {|qa_quality_parent_directory_path_traversal:"../config/app.json"|};
            }
            """;

        return CSharpAnalyzerVerifier<ParentDirectoryPathTraversalAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task RelativePathWithinDirectory_IsClean()
    {
        const string source = """
            public sealed class Loader
            {
                public string Path() => "config/app.json";
            }
            """;

        return CSharpAnalyzerVerifier<ParentDirectoryPathTraversalAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DottedFileName_IsClean()
    {
        const string source = """
            public sealed class Loader
            {
                public string Path() => "archive.v2/app.json";
            }
            """;

        return CSharpAnalyzerVerifier<ParentDirectoryPathTraversalAnalyzer>.VerifyAsync(source);
    }
}
