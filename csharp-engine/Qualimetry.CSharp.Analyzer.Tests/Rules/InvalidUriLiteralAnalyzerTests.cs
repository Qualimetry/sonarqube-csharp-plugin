using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Reliability;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class InvalidUriLiteralAnalyzerTests
{
    [Fact]
    public Task InvalidAbsoluteUri_IsReported()
    {
        string source = """
            using System;

            public sealed class Service
            {
                public Uri Endpoint() => new Uri({|qa_reliability_invalid_uri_literal:"http://:// invalid"|});
            }
            """;

        return CSharpAnalyzerVerifier<InvalidUriLiteralAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ValidAbsoluteUri_IsClean()
    {
        string source = """
            using System;

            public sealed class Service
            {
                public Uri Endpoint() => new Uri("https://example.com/api");
            }
            """;

        return CSharpAnalyzerVerifier<InvalidUriLiteralAnalyzer>.VerifyAsync(source);
    }
}
