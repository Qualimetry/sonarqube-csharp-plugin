using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class TypeWithoutNamespaceAnalyzerTests
{
    [Fact]
    public Task GlobalType_IsReported()
    {
        const string source = """
            public sealed class {|qa_quality_type_without_namespace:GlobalWidget|}
            {
            }
            """;

        return CSharpAnalyzerVerifier<TypeWithoutNamespaceAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NamespacedType_IsClean()
    {
        const string source = """
            namespace Company.Product
            {
                public sealed class Widget
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<TypeWithoutNamespaceAnalyzer>.VerifyAsync(source);
    }
}
