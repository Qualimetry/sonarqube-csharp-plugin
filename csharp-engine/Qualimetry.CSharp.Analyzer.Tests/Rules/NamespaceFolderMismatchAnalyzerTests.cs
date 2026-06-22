using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Naming;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class NamespaceFolderMismatchAnalyzerTests
{
    private static (string, string)[] Project(string projectDir, string rootNamespace) => new[]
    {
        ("build_property.ProjectDir", projectDir),
        ("build_property.RootNamespace", rootNamespace),
    };

    [Fact]
    public Task RootNamespaceDiffersFromFolder_IsClean()
    {
        const string source = """
            namespace Acme.Web.AppConfig
            {
                public sealed class AppSettings
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<NamespaceFolderMismatchAnalyzer>.VerifyFilesWithOptionsAsync(
            Project("/repo/Acme.Web/", "Acme.Web"),
            ("/repo/Acme.Web/AppConfig/AppSettings.cs", source));
    }

    [Fact]
    public Task FileAtProjectRoot_MatchesRootNamespace_IsClean()
    {
        const string source = """
            namespace Acme.Web
            {
                public sealed class Startup
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<NamespaceFolderMismatchAnalyzer>.VerifyFilesWithOptionsAsync(
            Project("/repo/Acme.Web/", "Acme.Web"),
            ("/repo/Acme.Web/Startup.cs", source));
    }

    [Fact]
    public Task NamespaceMismatch_IsReported()
    {
        const string source = """
            namespace Acme.Web.Wrong
            {
                public sealed class {|qa_naming_namespace_folder_mismatch:InvoiceService|}
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<NamespaceFolderMismatchAnalyzer>.VerifyFilesWithOptionsAsync(
            Project("/repo/Acme.Web/", "Acme.Web"),
            ("/repo/Acme.Web/AppConfig/InvoiceService.cs", source));
    }

    [Fact]
    public Task FolderWithNonIdentifierChars_IsSanitizedBeforeCompare_IsClean()
    {
        const string source = """
            namespace Acme.My_Folder
            {
                public sealed class Widget
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<NamespaceFolderMismatchAnalyzer>.VerifyFilesWithOptionsAsync(
            Project("/repo/Acme/", "Acme"),
            ("/repo/Acme/My-Folder/Widget.cs", source));
    }

    [Fact]
    public Task NoProjectContext_IsClean()
    {
        const string source = """
            namespace Whatever.At.All
            {
                public sealed class Thing
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<NamespaceFolderMismatchAnalyzer>.VerifyFilesAsync(
            ("/repo/Acme/Some/Thing.cs", source));
    }

    [Fact]
    public Task FileOutsideProjectDirectory_IsClean()
    {
        const string source = """
            namespace Acme.Shared
            {
                public sealed class Helper
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<NamespaceFolderMismatchAnalyzer>.VerifyFilesWithOptionsAsync(
            Project("/repo/Acme.Web/", "Acme.Web"),
            ("/repo/Shared/Acme.Shared/Helper.cs", source));
    }
}
