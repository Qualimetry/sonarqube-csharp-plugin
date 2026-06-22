using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ArgumentExceptionParameterNameAnalyzerTests
{
    [Fact]
    public Task MisspelledParameterName_IsReported()
    {
        const string source = """
            using System;

            public class Account
            {
                public void Withdraw(decimal amount)
                {
                    if (amount <= 0)
                    {
                        throw new ArgumentOutOfRangeException({|qa_quality_argument_exception_parameter_name:"ammount"|});
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<ArgumentExceptionParameterNameAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NameofParameter_IsClean()
    {
        const string source = """
            using System;

            public class Account
            {
                public void Withdraw(decimal amount)
                {
                    if (amount <= 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(amount));
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<ArgumentExceptionParameterNameAnalyzer>.VerifyAsync(source);
    }
}
