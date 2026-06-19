using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Naming;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class NamespaceSpansDirectoriesAnalyzerTests
{
    [Fact]
    public Task NamespaceSplitAcrossDirectories_IsReported()
    {
        const string alpha = """
            namespace Company.Product
            {
                public sealed class {|qa_naming_namespace_spans_directories:Alpha|}
                {
                }
            }
            """;

        const string beta = """
            namespace Company.Product
            {
                public sealed class {|qa_naming_namespace_spans_directories:Beta|}
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<NamespaceSpansDirectoriesAnalyzer>.VerifyFilesAsync(
            ("/Project/Company/Product/Alpha.cs", alpha),
            ("/Project/Other/Beta.cs", beta));
    }

    [Fact]
    public Task NamespaceInOneDirectory_IsClean()
    {
        const string alpha = """
            namespace Company.Product
            {
                public sealed class Alpha
                {
                }
            }
            """;

        const string beta = """
            namespace Company.Product
            {
                public sealed class Beta
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<NamespaceSpansDirectoriesAnalyzer>.VerifyFilesAsync(
            ("/Project/Company/Product/Alpha.cs", alpha),
            ("/Project/Company/Product/Beta.cs", beta));
    }
}
