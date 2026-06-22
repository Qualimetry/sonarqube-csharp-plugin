using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class MutableCollectionPropertySetterAnalyzerTests
{
    [Fact]
    public Task CollectionPropertyWithSetter_IsReported()
    {
        const string source = """
            using System.Collections.Generic;

            public class Order
            {
                public List<string> {|qa_quality_mutable_collection_property_setter:Items|} { get; set; } = new();
            }
            """;

        return CSharpAnalyzerVerifier<MutableCollectionPropertySetterAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task GetOnlyCollectionProperty_IsClean()
    {
        const string source = """
            using System.Collections.Generic;

            public class Order
            {
                public List<string> Items { get; } = new();
            }
            """;

        return CSharpAnalyzerVerifier<MutableCollectionPropertySetterAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ScalarPropertyWithSetter_IsClean()
    {
        const string source = """
            public class Order
            {
                public int Total { get; set; }
            }
            """;

        return CSharpAnalyzerVerifier<MutableCollectionPropertySetterAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task UserTypeNamedList_IsClean()
    {
        const string source = """
            namespace Acme
            {
                public class List<T>
                {
                }

                public class Order
                {
                    public List<string> Items { get; set; }
                }
            }
            """;

        return CSharpAnalyzerVerifier<MutableCollectionPropertySetterAnalyzer>.VerifyAsync(source);
    }
}
