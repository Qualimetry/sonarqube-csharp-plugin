using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class StaticMutableCollectionFieldAnalyzerTests
{
    [Fact]
    public Task PublicStaticMutableList_IsReported()
    {
        const string source = """
            using System.Collections.Generic;

            public class Registry
            {
                public static readonly List<string> {|qa_quality_static_mutable_collection_field:Names|} = new();
            }
            """;

        return CSharpAnalyzerVerifier<StaticMutableCollectionFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task InternalStaticMutableDictionary_IsReported()
    {
        const string source = """
            using System.Collections.Generic;

            public class Registry
            {
                internal static readonly Dictionary<string, string> {|qa_quality_static_mutable_collection_field:Codes|} = new();
            }
            """;

        return CSharpAnalyzerVerifier<StaticMutableCollectionFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PrivateStaticReadonlyLookup_IsClean()
    {
        const string source = """
            using System.Collections.Generic;

            public static class CountryLookup
            {
                private static readonly Dictionary<string, string> NameToCode =
                    new(System.StringComparer.OrdinalIgnoreCase)
                    {
                        { "Afghanistan", "AF" },
                        { "Albania", "AL" },
                    };
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
                public static readonly IReadOnlyList<string> Names = new List<string>();
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

    [Fact]
    public Task PublicStaticMutableListInInternalType_IsClean()
    {
        const string source = """
            using System.Collections.Generic;

            internal class Registry
            {
                public static readonly List<string> Names = new();
            }
            """;

        return CSharpAnalyzerVerifier<StaticMutableCollectionFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task UserDefinedListType_IsClean()
    {
        const string source = """
            public sealed class List<T>
            {
            }

            public class Registry
            {
                public static readonly List<string> Names = new();
            }
            """;

        return CSharpAnalyzerVerifier<StaticMutableCollectionFieldAnalyzer>.VerifyAsync(source);
    }
}
