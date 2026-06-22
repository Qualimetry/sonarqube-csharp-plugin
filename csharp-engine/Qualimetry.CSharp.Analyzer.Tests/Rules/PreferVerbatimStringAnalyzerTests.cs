using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class PreferVerbatimStringAnalyzerTests
{
    [Fact]
    public Task StringWithDoubledBackslashes_IsReported()
    {
        const string source = """
            public class C
            {
                public string LogPath()
                {
                    return {|qa_style_prefer_verbatim_string:"C:\\logs\\app.txt"|};
                }
            }
            """;

        return CSharpAnalyzerVerifier<PreferVerbatimStringAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task VerbatimString_IsClean()
    {
        const string source = """
            public class C
            {
                public string LogPath()
                {
                    return @"C:\logs\app.txt";
                }
            }
            """;

        return CSharpAnalyzerVerifier<PreferVerbatimStringAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task StringWithOtherEscapes_IsClean()
    {
        const string source = """
            public class C
            {
                public string Line()
                {
                    return "first\nsecond";
                }
            }
            """;

        return CSharpAnalyzerVerifier<PreferVerbatimStringAnalyzer>.VerifyAsync(source);
    }
}
