using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ExternalFieldAssignmentAnalyzerTests
{
    [Fact]
    public Task FieldWrittenFromUnrelatedType_IsReported()
    {
        const string source = """
            public sealed class Account
            {
                public decimal Balance;
            }

            public sealed class Teller
            {
                public void Reset(Account account)
                {
                    {|qa_quality_external_field_assignment:account.Balance|} = 0m;
                }
            }
            """;

        return CSharpAnalyzerVerifier<ExternalFieldAssignmentAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task FieldWrittenThroughOwnMember_IsClean()
    {
        const string source = """
            public sealed class Account
            {
                public decimal Balance { get; private set; }

                public void Reset() => Balance = 0m;
            }

            public sealed class Teller
            {
                public void Reset(Account account) => account.Reset();
            }
            """;

        return CSharpAnalyzerVerifier<ExternalFieldAssignmentAnalyzer>.VerifyAsync(source);
    }
}
