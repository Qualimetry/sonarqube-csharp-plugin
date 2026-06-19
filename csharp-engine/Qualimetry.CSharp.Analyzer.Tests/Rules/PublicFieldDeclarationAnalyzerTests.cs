using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class PublicFieldDeclarationAnalyzerTests
{
    [Fact]
    public Task PublicField_IsReported()
    {
        const string source = """
            public class Account
            {
                public decimal {|qa_quality_public_field_declaration:Balance|};
            }
            """;

        return CSharpAnalyzerVerifier<PublicFieldDeclarationAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PrivateFieldWithProperty_IsClean()
    {
        const string source = """
            public class Account
            {
                private decimal _balance;

                public decimal Balance => _balance;
            }
            """;

        return CSharpAnalyzerVerifier<PublicFieldDeclarationAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PublicConstant_IsClean()
    {
        const string source = """
            public class Account
            {
                public const decimal MinBalance = 0m;
            }
            """;

        return CSharpAnalyzerVerifier<PublicFieldDeclarationAnalyzer>.VerifyAsync(source);
    }
}
