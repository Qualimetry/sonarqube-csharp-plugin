using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class NonPublicExceptionTypeAnalyzerTests
{
    [Fact]
    public Task InternalException_IsReported()
    {
        const string source = """
            using System;

            internal sealed class {|qa_quality_non_public_exception_type:ImportFailedException|} : Exception
            {
                public ImportFailedException(string message) : base(message)
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<NonPublicExceptionTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PublicException_IsClean()
    {
        const string source = """
            using System;

            public sealed class ImportFailedException : Exception
            {
                public ImportFailedException(string message) : base(message)
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<NonPublicExceptionTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task InternalNonException_IsClean()
    {
        const string source = """
            internal sealed class ImportResult
            {
            }
            """;

        return CSharpAnalyzerVerifier<NonPublicExceptionTypeAnalyzer>.VerifyAsync(source);
    }
}
