using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Naming;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ExceptionTypeSuffixAnalyzerTests
{
    [Fact]
    public Task ExceptionWithoutSuffix_IsReported()
    {
        const string source = """
            using System;

            public class {|qa_naming_exception_type_suffix:PaymentFailed|} : Exception
            {
            }
            """;

        return CSharpAnalyzerVerifier<ExceptionTypeSuffixAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ExceptionWithSuffix_IsClean()
    {
        const string source = """
            using System;

            public class PaymentFailedException : Exception
            {
            }
            """;

        return CSharpAnalyzerVerifier<ExceptionTypeSuffixAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NonExceptionClass_IsClean()
    {
        const string source = """
            public class PaymentFailed
            {
            }
            """;

        return CSharpAnalyzerVerifier<ExceptionTypeSuffixAnalyzer>.VerifyAsync(source);
    }
}
