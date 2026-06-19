using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class GeneralExceptionTypeThrownAnalyzerTests
{
    [Fact]
    public Task ThrowGeneralException_IsReported()
    {
        const string source = """
            using System;

            public class Client
            {
                public void Connect(string host)
                {
                    if (string.IsNullOrEmpty(host))
                    {
                        throw {|qa_quality_general_exception_type_thrown:new Exception("Host is required.")|};
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<GeneralExceptionTypeThrownAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ThrowSpecificException_IsClean()
    {
        const string source = """
            using System;

            public class Client
            {
                public void Connect(string host)
                {
                    if (string.IsNullOrEmpty(host))
                    {
                        throw new ArgumentException("Host is required.", nameof(host));
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<GeneralExceptionTypeThrownAnalyzer>.VerifyAsync(source);
    }
}
