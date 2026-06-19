using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class UnusedConstructorParameterAnalyzerTests
{
    [Fact]
    public Task UnusedParameter_IsReported()
    {
        const string source = """
            public sealed class Report
            {
                private readonly string _title;

                public Report(string title, string {|qa_style_unused_constructor_parameter:author|})
                {
                    _title = title;
                }
            }
            """;

        return CSharpAnalyzerVerifier<UnusedConstructorParameterAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task AllParametersUsed_IsClean()
    {
        const string source = """
            public sealed class Report
            {
                private readonly string _title;
                private readonly string _author;

                public Report(string title, string author)
                {
                    _title = title;
                    _author = author;
                }
            }
            """;

        return CSharpAnalyzerVerifier<UnusedConstructorParameterAnalyzer>.VerifyAsync(source);
    }
}
