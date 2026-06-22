using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class StatelessTypeShouldBeStaticAnalyzerTests
{
    [Fact]
    public Task ClassWithOnlyStaticMembers_IsReported()
    {
        const string source = """
            public class {|qa_quality_stateless_type_should_be_static:MathHelpers|}
            {
                public static int Square(int value) => value * value;

                public static int Cube(int value) => value * value * value;
            }
            """;

        return CSharpAnalyzerVerifier<StatelessTypeShouldBeStaticAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task StaticClass_IsClean()
    {
        const string source = """
            public static class MathHelpers
            {
                public static int Square(int value) => value * value;
            }
            """;

        return CSharpAnalyzerVerifier<StatelessTypeShouldBeStaticAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ClassWithInstanceState_IsClean()
    {
        const string source = """
            public class Counter
            {
                private int _value;

                public static int Square(int value) => value * value;
            }
            """;

        return CSharpAnalyzerVerifier<StatelessTypeShouldBeStaticAnalyzer>.VerifyAsync(source);
    }
}
