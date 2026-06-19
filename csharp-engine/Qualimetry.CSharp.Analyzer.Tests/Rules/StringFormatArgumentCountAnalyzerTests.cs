using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class StringFormatArgumentCountAnalyzerTests
{
    [Fact]
    public Task PlaceholderWithoutArgument_IsReported()
    {
        const string source = """
            public class Greeter
            {
                public string Build(string user) =>
                    {|qa_quality_string_format_argument_count:string.Format("{0} signed in at {1}", user)|};
            }
            """;

        return CSharpAnalyzerVerifier<StringFormatArgumentCountAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task MatchingArguments_IsClean()
    {
        const string source = """
            public class Greeter
            {
                public string Build(string user, string at) =>
                    string.Format("{0} signed in at {1}", user, at);
            }
            """;

        return CSharpAnalyzerVerifier<StringFormatArgumentCountAnalyzer>.VerifyAsync(source);
    }
}
