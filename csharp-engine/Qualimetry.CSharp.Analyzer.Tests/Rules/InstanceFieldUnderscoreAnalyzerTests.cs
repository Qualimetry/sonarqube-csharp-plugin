using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Naming;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class InstanceFieldUnderscoreAnalyzerTests
{
    [Fact]
    public Task PrivateInstanceFieldWithoutUnderscore_IsReported()
    {
        const string source = """
            public class Cache
            {
                private int {|qa_naming_instance_field_underscore:count|};
            }
            """;

        return CSharpAnalyzerVerifier<InstanceFieldUnderscoreAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PrivateInstanceFieldWithUnderscore_IsClean()
    {
        const string source = """
            public class Cache
            {
                private int _count;
            }
            """;

        return CSharpAnalyzerVerifier<InstanceFieldUnderscoreAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PublicField_IsClean()
    {
        const string source = """
            public class Cache
            {
                public int Count;
            }
            """;

        return CSharpAnalyzerVerifier<InstanceFieldUnderscoreAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task StaticField_IsClean()
    {
        const string source = """
            public class Cache
            {
                private static int count;
            }
            """;

        return CSharpAnalyzerVerifier<InstanceFieldUnderscoreAnalyzer>.VerifyAsync(source);
    }
}
