using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class SwitchSectionBracesAnalyzerTests
{
    [Fact]
    public Task UnbracedSectionWithStatements_IsReported()
    {
        const string source = """
            public class C
            {
                void M(int x)
                {
                    switch (x)
                    {
                        {|qa_style_switch_section_braces:case 1:|}
                            D();
                            D();
                            break;
                    }
                }

                void D()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<SwitchSectionBracesAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task BracedSection_IsClean()
    {
        const string source = """
            public class C
            {
                void M(int x)
                {
                    switch (x)
                    {
                        case 1:
                        {
                            D();
                            D();
                            break;
                        }
                    }
                }

                void D()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<SwitchSectionBracesAnalyzer>.VerifyAsync(source);
    }
}
