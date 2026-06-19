using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class MutableStructShouldBeClassAnalyzerTests
{
    [Fact]
    public Task MutableStruct_IsReported()
    {
        const string source = """
            public struct {|qa_style_mutable_struct_should_be_class:Cursor|}
            {
                public int Line;
                public int Column;
            }
            """;

        return CSharpAnalyzerVerifier<MutableStructShouldBeClassAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ReadonlyStruct_IsClean()
    {
        const string source = """
            public readonly struct Cursor
            {
                public int Line { get; }
                public int Column { get; }
            }
            """;

        return CSharpAnalyzerVerifier<MutableStructShouldBeClassAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ImmutableStruct_IsClean()
    {
        const string source = """
            public struct Cursor
            {
                public int Line { get; init; }
                public int Column { get; init; }
            }
            """;

        return CSharpAnalyzerVerifier<MutableStructShouldBeClassAnalyzer>.VerifyAsync(source);
    }
}
