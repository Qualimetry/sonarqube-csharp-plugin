using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ConcreteXmlNodeReturnTypeAnalyzerTests
{
    [Fact]
    public Task MethodReturningXmlElement_IsReported()
    {
        const string source = """
            using System.Xml;

            public class Loader
            {
                public {|qa_quality_concrete_xml_node_return_type:XmlElement|} Load()
                {
                    var document = new XmlDocument();
                    document.LoadXml("<config/>");
                    return document.DocumentElement;
                }
            }
            """;

        return CSharpAnalyzerVerifier<ConcreteXmlNodeReturnTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task MethodReturningReader_IsClean()
    {
        const string source = """
            using System.IO;
            using System.Xml;

            public class Loader
            {
                public XmlReader Load()
                {
                    return XmlReader.Create(new StringReader("<config/>"));
                }
            }
            """;

        return CSharpAnalyzerVerifier<ConcreteXmlNodeReturnTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PublicMethodInInternalType_IsClean()
    {
        const string source = """
            using System.Xml;

            internal class Loader
            {
                public XmlElement Load()
                {
                    var document = new XmlDocument();
                    document.LoadXml("<config/>");
                    return document.DocumentElement;
                }
            }
            """;

        return CSharpAnalyzerVerifier<ConcreteXmlNodeReturnTypeAnalyzer>.VerifyAsync(source);
    }
}
