using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class NotImplementedExceptionThrowAnalyzerTests
{
    [Fact]
    public Task ThrowNotImplemented_IsReported()
    {
        const string source = """
            using System;

            public class Calculator
            {
                public decimal CalculateTax(decimal amount)
                {
                    throw {|qa_quality_not_implemented_exception_throw:new NotImplementedException()|};
                }
            }
            """;

        return CSharpAnalyzerVerifier<NotImplementedExceptionThrowAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task RealImplementation_IsClean()
    {
        const string source = """
            public class Calculator
            {
                public decimal CalculateTax(decimal amount)
                {
                    return amount * 0.2m;
                }
            }
            """;

        return CSharpAnalyzerVerifier<NotImplementedExceptionThrowAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ThrowNotSupported_IsClean()
    {
        const string source = """
            using System;

            public class Calculator
            {
                public decimal CalculateTax(decimal amount)
                {
                    throw new NotSupportedException();
                }
            }
            """;

        return CSharpAnalyzerVerifier<NotImplementedExceptionThrowAnalyzer>.VerifyAsync(source);
    }
}
