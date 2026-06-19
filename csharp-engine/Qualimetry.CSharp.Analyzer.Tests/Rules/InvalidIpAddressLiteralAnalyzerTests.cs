using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Reliability;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class InvalidIpAddressLiteralAnalyzerTests
{
    [Fact]
    public Task InvalidLiteral_IsReported()
    {
        string source = """
            using System.Net;

            public sealed class Endpoint
            {
                public IPAddress Address() => IPAddress.Parse({|qa_reliability_invalid_ip_address_literal:"10.0.0.300"|});
            }
            """;

        return CSharpAnalyzerVerifier<InvalidIpAddressLiteralAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ValidLiteral_IsClean()
    {
        string source = """
            using System.Net;

            public sealed class Endpoint
            {
                public IPAddress Address() => IPAddress.Parse("10.0.0.30");
            }
            """;

        return CSharpAnalyzerVerifier<InvalidIpAddressLiteralAnalyzer>.VerifyAsync(source);
    }
}
