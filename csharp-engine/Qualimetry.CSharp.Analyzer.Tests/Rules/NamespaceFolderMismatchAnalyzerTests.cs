using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Naming;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class NamespaceFolderMismatchAnalyzerTests
{
    [Fact]
    public Task NamespaceMismatch_IsReported()
    {
        const string source = """
            namespace Company.Product.Billing
            {
                public sealed class {|qa_naming_namespace_folder_mismatch:InvoiceService|}
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<NamespaceFolderMismatchAnalyzer>.VerifyFilesAsync(
            ("/Project/Wrong/InvoiceService.cs", source));
    }

    [Fact]
    public Task NamespaceMatchesFolder_IsClean()
    {
        const string source = """
            namespace Company.Product.Billing
            {
                public sealed class InvoiceService
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<NamespaceFolderMismatchAnalyzer>.VerifyFilesAsync(
            ("/Project/Company/Product/Billing/InvoiceService.cs", source));
    }
}
