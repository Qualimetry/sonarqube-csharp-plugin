using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class DuplicateTypeNameAnalyzerTests
{
    [Fact]
    public Task TwoTypesSharingSimpleName_AreReported()
    {
        const string source = """
            namespace Billing
            {
                public sealed class {|qa_quality_duplicate_type_name:Account|}
                {
                }
            }

            namespace Identity
            {
                public sealed class {|qa_quality_duplicate_type_name:Account|}
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<DuplicateTypeNameAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DistinctTypeNames_AreClean()
    {
        const string source = """
            namespace Billing
            {
                public sealed class BillingAccount
                {
                }
            }

            namespace Identity
            {
                public sealed class UserAccount
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<DuplicateTypeNameAnalyzer>.VerifyAsync(source);
    }
}
