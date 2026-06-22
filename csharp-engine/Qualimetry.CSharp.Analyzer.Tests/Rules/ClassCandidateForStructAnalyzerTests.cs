using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ClassCandidateForStructAnalyzerTests
{
    [Fact]
    public Task ImmutableValueHolderClass_IsReported()
    {
        const string source = """
            public sealed class {|qa_quality_class_candidate_for_struct:Point|}
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

        return CSharpAnalyzerVerifier<ClassCandidateForStructAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ClassWithBehaviour_IsClean()
    {
        const string source = """
            public sealed class Point
            {
                public Point(int x, int y)
                {
                    X = x;
                    Y = y;
                }

                public int X { get; }

                public int Y { get; }

                public int DistanceSquared() => (X * X) + (Y * Y);
            }
            """;

        return CSharpAnalyzerVerifier<ClassCandidateForStructAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ClassWithMutableField_IsClean()
    {
        const string source = """
            public sealed class Point
            {
                public int X;

                public int Y;
            }
            """;

        return CSharpAnalyzerVerifier<ClassCandidateForStructAnalyzer>.VerifyAsync(source);
    }
}
