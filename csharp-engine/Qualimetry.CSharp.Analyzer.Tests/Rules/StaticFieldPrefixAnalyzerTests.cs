using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Naming;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class StaticFieldPrefixAnalyzerTests
{
    [Fact]
    public Task PrivateStaticFieldWithoutPrefix_IsReported()
    {
        const string source = """
            public class Counter
            {
                private static int {|qa_naming_static_field_prefix:instances|};
            }
            """;

        return CSharpAnalyzerVerifier<StaticFieldPrefixAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PrivateStaticFieldWithPrefix_IsClean()
    {
        const string source = """
            public class Counter
            {
                private static int s_instances;
            }
            """;

        return CSharpAnalyzerVerifier<StaticFieldPrefixAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task InstanceField_IsClean()
    {
        const string source = """
            public class Counter
            {
                private int instances;
            }
            """;

        return CSharpAnalyzerVerifier<StaticFieldPrefixAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ConstField_IsClean()
    {
        const string source = """
            public class Counter
            {
                private const int Limit = 10;
            }
            """;

        return CSharpAnalyzerVerifier<StaticFieldPrefixAnalyzer>.VerifyAsync(source);
    }
}
