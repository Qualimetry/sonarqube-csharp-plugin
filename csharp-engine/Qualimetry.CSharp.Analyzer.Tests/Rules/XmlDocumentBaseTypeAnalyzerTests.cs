using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class XmlDocumentBaseTypeAnalyzerTests
{
    [Fact]
    public Task DerivesFromXmlDocument_IsReported()
    {
        const string source = """
            using System.Xml;

            public class {|qa_quality_xml_document_base_type:ConfigDocument|} : XmlDocument
            {
            }
            """;

        return CSharpAnalyzerVerifier<XmlDocumentBaseTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ComposesXmlDocument_IsClean()
    {
        const string source = """
            using System.Xml;

            public class ConfigDocument
            {
                private readonly XmlDocument _document = new XmlDocument();

                public void Load(string path) => _document.Load(path);
            }
            """;

        return CSharpAnalyzerVerifier<XmlDocumentBaseTypeAnalyzer>.VerifyAsync(source);
    }
}
