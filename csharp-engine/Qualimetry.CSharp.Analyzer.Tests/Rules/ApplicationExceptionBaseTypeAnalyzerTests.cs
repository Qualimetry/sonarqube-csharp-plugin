using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ApplicationExceptionBaseTypeAnalyzerTests
{
    [Fact]
    public Task DerivesFromApplicationException_IsReported()
    {
        const string source = """
            using System;

            public class {|qa_quality_application_exception_base_type:OrderFailedException|} : ApplicationException
            {
            }
            """;

        return CSharpAnalyzerVerifier<ApplicationExceptionBaseTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DerivesFromException_IsClean()
    {
        const string source = """
            using System;

            public class OrderFailedException : Exception
            {
            }
            """;

        return CSharpAnalyzerVerifier<ApplicationExceptionBaseTypeAnalyzer>.VerifyAsync(source);
    }
}
