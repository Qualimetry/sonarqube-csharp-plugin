using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class UnnecessaryVerbatimStringAnalyzerTests
{
    [Fact]
    public Task VerbatimWithoutEscapes_IsReported()
    {
        const string source = """
            public class C
            {
                public string Status() => {|qa_style_unnecessary_verbatim_string:@"Pending"|};
            }
            """;

        return CSharpAnalyzerVerifier<UnnecessaryVerbatimStringAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task RegularString_IsClean()
    {
        const string source = """
            public class C
            {
                public string Status() => "Pending";
            }
            """;

        return CSharpAnalyzerVerifier<UnnecessaryVerbatimStringAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task VerbatimWithBackslash_IsClean()
    {
        const string source = """
            public class C
            {
                public string Path() => @"C:\temp";
            }
            """;

        return CSharpAnalyzerVerifier<UnnecessaryVerbatimStringAnalyzer>.VerifyAsync(source);
    }
}
