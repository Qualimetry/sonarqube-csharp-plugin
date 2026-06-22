using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Reliability;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class InvalidJsonLiteralAnalyzerTests
{
    private const string NewtonsoftStub = """
        namespace Newtonsoft.Json.Linq
        {
            public class JObject
            {
                public static JObject Parse(string json) => new JObject();
            }
        }
        """;

    private const string LookAlikeStub = """
        public static class Json
        {
            public static object Parse(string text) => text;
        }
        """;

    [Fact]
    public Task MalformedJson_OnNewtonsoftReceiver_IsReported()
    {
        string source = NewtonsoftStub + """

            public sealed class Config
            {
                public object Load() => Newtonsoft.Json.Linq.JObject.Parse({|qa_reliability_invalid_json_literal:"{ \"enabled\": }"|});
            }
            """;

        return CSharpAnalyzerVerifier<InvalidJsonLiteralAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task TrailingComma_OnNewtonsoftReceiver_IsClean()
    {
        string source = NewtonsoftStub + """

            public sealed class Config
            {
                public object Load() => Newtonsoft.Json.Linq.JObject.Parse("{ \"enabled\": true, }");
            }
            """;

        return CSharpAnalyzerVerifier<InvalidJsonLiteralAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task Comment_OnNewtonsoftReceiver_IsClean()
    {
        string source = NewtonsoftStub + """

            public sealed class Config
            {
                public object Load() => Newtonsoft.Json.Linq.JObject.Parse("{ \"enabled\": true /* note */ }");
            }
            """;

        return CSharpAnalyzerVerifier<InvalidJsonLiteralAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task LookAlikeJsonType_IsClean()
    {
        string source = LookAlikeStub + """

            public sealed class Config
            {
                public object Load() => Json.Parse("{ \"enabled\": true, }");
            }
            """;

        return CSharpAnalyzerVerifier<InvalidJsonLiteralAnalyzer>.VerifyAsync(source);
    }
}
