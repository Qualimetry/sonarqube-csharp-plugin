using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ReservedExceptionTypeThrownAnalyzerTests
{
    [Fact]
    public Task ThrowReservedException_IsReported()
    {
        const string source = """
            using System;

            public class Validator
            {
                public void Validate(string value)
                {
                    if (value == null)
                    {
                        throw {|qa_quality_reserved_exception_type_thrown:new NullReferenceException()|};
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<ReservedExceptionTypeThrownAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ThrowSpecificException_IsClean()
    {
        const string source = """
            using System;

            public class Validator
            {
                public void Validate(string value)
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException(nameof(value));
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<ReservedExceptionTypeThrownAnalyzer>.VerifyAsync(source);
    }
}
