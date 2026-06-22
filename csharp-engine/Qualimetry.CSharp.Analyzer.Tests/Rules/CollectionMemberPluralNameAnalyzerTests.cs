using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Naming;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class CollectionMemberPluralNameAnalyzerTests
{
    [Fact]
    public Task SingularCollectionProperty_IsReported()
    {
        const string source = """
            using System.Collections.Generic;

            public class Order
            {
                public List<string> {|qa_naming_collection_member_plural_name:Line|} { get; set; }
            }
            """;

        return CSharpAnalyzerVerifier<CollectionMemberPluralNameAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PluralCollectionProperty_IsClean()
    {
        const string source = """
            using System.Collections.Generic;

            public class Order
            {
                public List<string> Lines { get; set; }
            }
            """;

        return CSharpAnalyzerVerifier<CollectionMemberPluralNameAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task IrregularPluralCollectionProperty_IsClean()
    {
        const string source = """
            using System.Collections.Generic;

            public class Family
            {
                public List<string> Children { get; set; }
            }
            """;

        return CSharpAnalyzerVerifier<CollectionMemberPluralNameAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ScalarProperty_IsClean()
    {
        const string source = """
            public class Order
            {
                public int Total { get; set; }
            }
            """;

        return CSharpAnalyzerVerifier<CollectionMemberPluralNameAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task StringProperty_IsClean()
    {
        const string source = """
            public class Order
            {
                public string Reference { get; set; }
            }
            """;

        return CSharpAnalyzerVerifier<CollectionMemberPluralNameAnalyzer>.VerifyAsync(source);
    }
}
