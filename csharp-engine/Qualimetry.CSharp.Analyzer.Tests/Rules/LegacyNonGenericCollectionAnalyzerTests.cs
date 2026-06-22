using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class LegacyNonGenericCollectionAnalyzerTests
{
    [Fact]
    public Task NewArrayList_IsReported()
    {
        const string source = """
            using System.Collections;

            public class Store
            {
                public void Fill()
                {
                    var items = {|qa_quality_legacy_non_generic_collection:new ArrayList()|};
                    items.Add("first");
                }
            }
            """;

        return CSharpAnalyzerVerifier<LegacyNonGenericCollectionAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task GenericList_IsClean()
    {
        const string source = """
            using System.Collections.Generic;

            public class Store
            {
                public void Fill()
                {
                    var items = new List<string>();
                    items.Add("first");
                }
            }
            """;

        return CSharpAnalyzerVerifier<LegacyNonGenericCollectionAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task UserTypeNamedArrayList_IsClean()
    {
        const string source = """
            namespace Acme
            {
                public class ArrayList
                {
                }

                public class Store
                {
                    public void Fill()
                    {
                        var items = new ArrayList();
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<LegacyNonGenericCollectionAnalyzer>.VerifyAsync(source);
    }
}
