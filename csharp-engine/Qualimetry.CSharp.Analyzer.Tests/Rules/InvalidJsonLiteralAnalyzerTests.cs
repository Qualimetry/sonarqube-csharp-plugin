using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Reliability;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class InvalidJsonLiteralAnalyzerTests
{
    private const string Stub = """
        public static class Json
        {
            public static object Parse(string text) => text;
        }
        """;

    [Fact]
    public Task MalformedJson_IsReported()
    {
        string source = Stub + """

            public sealed class Config
            {
                public object Load() => Json.Parse({|qa_reliability_invalid_json_literal:"{ \"enabled\": true, }"|});
            }
            """;

        return CSharpAnalyzerVerifier<InvalidJsonLiteralAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task WellFormedJson_IsClean()
    {
        string source = Stub + """

            public sealed class Config
            {
                public object Load() => Json.Parse("{ \"enabled\": true, \"count\": 3 }");
            }
            """;

        return CSharpAnalyzerVerifier<InvalidJsonLiteralAnalyzer>.VerifyAsync(source);
    }
}
