using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class MutableStructAnalyzerTests
{
    [Fact]
    public Task StructWithSettableProperties_IsReported()
    {
        const string source = """
            public struct {|qa_quality_mutable_struct:Point|}
            {
                public int X { get; set; }
                public int Y { get; set; }
            }
            """;

        return CSharpAnalyzerVerifier<MutableStructAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ImmutableStruct_IsClean()
    {
        const string source = """
            public struct Point
            {
                public Point(int x, int y)
                {
                    X = x;
                    Y = y;
                }

                public int X { get; }
                public int Y { get; }
            }
            """;

        return CSharpAnalyzerVerifier<MutableStructAnalyzer>.VerifyAsync(source);
    }
}
