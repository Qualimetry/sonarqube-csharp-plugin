using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Naming;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class DirectorySpansNamespacesAnalyzerTests
{
    [Fact]
    public Task DirectorySplitAcrossNamespaces_IsReported()
    {
        const string alpha = """
            namespace Company.Product
            {
                public sealed class {|qa_naming_directory_spans_namespaces:Alpha|}
                {
                }
            }
            """;

        const string beta = """
            namespace Company.Other
            {
                public sealed class {|qa_naming_directory_spans_namespaces:Beta|}
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<DirectorySpansNamespacesAnalyzer>.VerifyFilesAsync(
            ("/Project/Shared/Alpha.cs", alpha),
            ("/Project/Shared/Beta.cs", beta));
    }

    [Fact]
    public Task DirectoryWithAncestorAndDescendantNamespaces_IsClean()
    {
        const string parent = """
            namespace Company.Product
            {
                public sealed class Parent
                {
                }
            }
            """;

        const string child = """
            namespace Company.Product.Internal
            {
                public sealed class Child
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<DirectorySpansNamespacesAnalyzer>.VerifyFilesAsync(
            ("/Project/Shared/Parent.cs", parent),
            ("/Project/Shared/Child.cs", child));
    }

    [Fact]
    public Task DirectorySplitAcrossNamespaces_WithProjectDir_IsReported()
    {
        const string alpha = """
            namespace Company.Product
            {
                public sealed class {|qa_naming_directory_spans_namespaces:Alpha|}
                {
                }
            }
            """;

        const string beta = """
            namespace Company.Other
            {
                public sealed class {|qa_naming_directory_spans_namespaces:Beta|}
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<DirectorySpansNamespacesAnalyzer>.VerifyFilesWithOptionsAsync(
            new[] { ("build_property.ProjectDir", "/repo/Proj/") },
            ("/repo/Proj/Shared/Alpha.cs", alpha),
            ("/repo/Proj/Shared/Beta.cs", beta));
    }

    [Fact]
    public Task DirectoryWithOneNamespace_IsClean()
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

        return CSharpAnalyzerVerifier<DirectorySpansNamespacesAnalyzer>.VerifyFilesAsync(
            ("/Project/Shared/Alpha.cs", alpha),
            ("/Project/Shared/Beta.cs", beta));
    }
}
