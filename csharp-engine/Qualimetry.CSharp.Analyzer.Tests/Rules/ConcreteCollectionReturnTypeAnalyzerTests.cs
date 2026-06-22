using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ConcreteCollectionReturnTypeAnalyzerTests
{
    [Fact]
    public Task PublicMethodReturningList_IsReported()
    {
        const string source = """
            using System.Collections.Generic;

            public class Catalog
            {
                public {|qa_quality_concrete_collection_return_type:List<string>|} GetNames()
                {
                    return new List<string>();
                }
            }
            """;

        return CSharpAnalyzerVerifier<ConcreteCollectionReturnTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PublicMethodReturningInterface_IsClean()
    {
        const string source = """
            using System.Collections.Generic;

            public class Catalog
            {
                public IReadOnlyList<string> GetNames()
                {
                    return new List<string>();
                }
            }
            """;

        return CSharpAnalyzerVerifier<ConcreteCollectionReturnTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PublicMethodInInternalType_IsClean()
    {
        const string source = """
            using System.Collections.Generic;

            internal static class Helpers
            {
                public static List<string> GetNames()
                {
                    return new List<string>();
                }
            }
            """;

        return CSharpAnalyzerVerifier<ConcreteCollectionReturnTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PublicMethodInPublicTypeNestedInInternalType_IsClean()
    {
        const string source = """
            using System.Collections.Generic;

            internal class Outer
            {
                public class Inner
                {
                    public List<string> GetNames()
                    {
                        return new List<string>();
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<ConcreteCollectionReturnTypeAnalyzer>.VerifyAsync(source);
    }
}
