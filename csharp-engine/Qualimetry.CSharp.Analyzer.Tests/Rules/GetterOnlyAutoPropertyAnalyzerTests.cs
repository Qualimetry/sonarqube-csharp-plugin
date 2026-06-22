using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class GetterOnlyAutoPropertyAnalyzerTests
{
    [Fact]
    public Task ReadOnlyPropertyOverBackingField_IsReported()
    {
        const string source = """
            public sealed class Point
            {
                private readonly int _x;

                public Point(int x)
                {
                    _x = x;
                }

                public int {|qa_style_getter_only_auto_property:X|} => _x;
            }
            """;

        return CSharpAnalyzerVerifier<GetterOnlyAutoPropertyAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task GetterOnlyAutoProperty_IsClean()
    {
        const string source = """
            public sealed class Point
            {
                public Point(int x)
                {
                    X = x;
                }

                public int X { get; }
            }
            """;

        return CSharpAnalyzerVerifier<GetterOnlyAutoPropertyAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task BackingFieldUsedElsewhere_IsClean()
    {
        const string source = """
            public sealed class Point
            {
                private readonly int _x;

                public Point(int x)
                {
                    _x = x;
                }

                public int X => _x;

                public int Double() => _x * 2;
            }
            """;

        return CSharpAnalyzerVerifier<GetterOnlyAutoPropertyAnalyzer>.VerifyAsync(source);
    }
}
