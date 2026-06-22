using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class PrivateSetCandidateAnalyzerTests
{
    [Fact]
    public Task SetterAssignedOnlyInConstructor_IsReported()
    {
        const string source = """
            public class Account
            {
                public decimal Balance { get; {|qa_style_private_set_candidate:set|}; }

                public Account(decimal opening)
                {
                    Balance = opening;
                }
            }
            """;

        return CSharpAnalyzerVerifier<PrivateSetCandidateAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task AlreadyPrivateSetter_IsClean()
    {
        const string source = """
            public class Account
            {
                public decimal Balance { get; private set; }

                public Account(decimal opening)
                {
                    Balance = opening;
                }
            }
            """;

        return CSharpAnalyzerVerifier<PrivateSetCandidateAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task SetterAssignedOutsideConstructor_IsClean()
    {
        const string source = """
            public class Account
            {
                public decimal Balance { get; set; }

                public Account(decimal opening)
                {
                    Balance = opening;
                }

                public void Deposit(decimal amount)
                {
                    Balance += amount;
                }
            }
            """;

        return CSharpAnalyzerVerifier<PrivateSetCandidateAnalyzer>.VerifyAsync(source);
    }
}
