using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class XmlDocNonexistentParameterAnalyzerTests
{
    [Fact]
    public Task ParamDocForMissingParameter_IsReported()
    {
        const string source = """
            public class Service
            {
                /// <summary>Runs the task.</summary>
                /// <param name="{|qa_quality_xml_doc_nonexistent_parameter:timeout|}">Unused.</param>
                public void Run(int retries)
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<XmlDocNonexistentParameterAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ParamDocMatchingParameter_IsClean()
    {
        const string source = """
            public class Service
            {
                /// <summary>Runs the task.</summary>
                /// <param name="retries">How many attempts.</param>
                public void Run(int retries)
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<XmlDocNonexistentParameterAnalyzer>.VerifyAsync(source);
    }
}
