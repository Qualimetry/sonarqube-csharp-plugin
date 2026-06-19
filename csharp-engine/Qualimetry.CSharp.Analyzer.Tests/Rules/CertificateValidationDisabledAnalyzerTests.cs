using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Reliability;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class CertificateValidationDisabledAnalyzerTests
{
    private const string Handler = """
        using System;

        public sealed class Handler
        {
            public Func<object, object, object, object, bool>? ServerCertificateCustomValidationCallback;
        }
        """;

    [Fact]
    public Task CallbackAlwaysReturningTrue_IsReported()
    {
        string source = Handler + """

            public class C
            {
                void M()
                {
                    var handler = new Handler();
                    handler.ServerCertificateCustomValidationCallback = {|qa_reliability_certificate_validation_disabled:(request, cert, chain, errors) => true|};
                }
            }
            """;

        return CSharpAnalyzerVerifier<CertificateValidationDisabledAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task CallbackThatValidates_IsClean()
    {
        string source = Handler + """

            public class C
            {
                void M()
                {
                    var handler = new Handler();
                    handler.ServerCertificateCustomValidationCallback = (request, cert, chain, errors) => errors == null;
                }
            }
            """;

        return CSharpAnalyzerVerifier<CertificateValidationDisabledAnalyzer>.VerifyAsync(source);
    }
}
