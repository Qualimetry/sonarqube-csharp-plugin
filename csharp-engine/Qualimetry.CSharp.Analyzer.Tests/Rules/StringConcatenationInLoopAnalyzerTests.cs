using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class StringConcatenationInLoopAnalyzerTests
{
    [Fact]
    public Task ConcatenationInForeach_IsReported()
    {
        const string source = """
            using System.Collections.Generic;

            public class Builder
            {
                public string Join(List<string> items)
                {
                    string result = "";
                    foreach (var item in items)
                    {
                        {|qa_quality_string_concatenation_in_loop:result += item|};
                    }

                    return result;
                }
            }
            """;

        return CSharpAnalyzerVerifier<StringConcatenationInLoopAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task StringBuilderInLoop_IsClean()
    {
        const string source = """
            using System.Collections.Generic;
            using System.Text;

            public class Builder
            {
                public string Join(List<string> items)
                {
                    var builder = new StringBuilder();
                    foreach (var item in items)
                    {
                        builder.Append(item);
                    }

                    return builder.ToString();
                }
            }
            """;

        return CSharpAnalyzerVerifier<StringConcatenationInLoopAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ConcatenationOutsideLoop_IsClean()
    {
        const string source = """
            public class Builder
            {
                public string Join(string a, string b)
                {
                    string result = a;
                    result += b;
                    return result;
                }
            }
            """;

        return CSharpAnalyzerVerifier<StringConcatenationInLoopAnalyzer>.VerifyAsync(source);
    }
}
