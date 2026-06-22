using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class UseNameOfForParameterNameAnalyzerTests
{
    [Fact]
    public Task ArgumentExceptionWithLiteralParameterName_IsReported()
    {
        const string source = """
            using System;

            public class Validator
            {
                public void Check(string account)
                {
                    if (account is null)
                    {
                        throw new ArgumentNullException({|qa_quality_use_name_of_for_parameter_name:"account"|});
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<UseNameOfForParameterNameAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ArgumentExceptionWithNameOf_IsClean()
    {
        const string source = """
            using System;

            public class Validator
            {
                public void Check(string account)
                {
                    if (account is null)
                    {
                        throw new ArgumentNullException(nameof(account));
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<UseNameOfForParameterNameAnalyzer>.VerifyAsync(source);
    }
}
