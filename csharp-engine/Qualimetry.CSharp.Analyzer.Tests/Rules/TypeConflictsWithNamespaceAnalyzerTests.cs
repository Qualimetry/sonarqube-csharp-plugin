using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Naming;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class TypeConflictsWithNamespaceAnalyzerTests
{
    [Fact]
    public Task TypeNameMatchesNamespace_IsReported()
    {
        const string source = """
            namespace Billing
            {
                public sealed class {|qa_naming_type_conflicts_with_namespace:Billing|}
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<TypeConflictsWithNamespaceAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DistinctTypeName_IsClean()
    {
        const string source = """
            namespace Billing
            {
                public sealed class BillingService
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<TypeConflictsWithNamespaceAnalyzer>.VerifyAsync(source);
    }
}
