using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class StaticMutableCollectionFieldAnalyzerTests
{
    [Fact]
    public Task StaticMutableList_IsReported()
    {
        const string source = """
            using System.Collections.Generic;

            public class Registry
            {
                private static readonly List<string> {|qa_quality_static_mutable_collection_field:Names|} = new();
            }
            """;

        return CSharpAnalyzerVerifier<StaticMutableCollectionFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task StaticReadOnlyInterface_IsClean()
    {
        const string source = """
            using System.Collections.Generic;

            public class Registry
            {
                private static readonly IReadOnlyList<string> Names = new List<string>();
            }
            """;

        return CSharpAnalyzerVerifier<StaticMutableCollectionFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task InstanceList_IsClean()
    {
        const string source = """
            using System.Collections.Generic;

            public class Registry
            {
                private readonly List<string> names = new();
            }
            """;

        return CSharpAnalyzerVerifier<StaticMutableCollectionFieldAnalyzer>.VerifyAsync(source);
    }
}
