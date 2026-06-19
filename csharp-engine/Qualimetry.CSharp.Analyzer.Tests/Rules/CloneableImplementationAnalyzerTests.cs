using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class CloneableImplementationAnalyzerTests
{
    [Fact]
    public Task TypeImplementingICloneable_IsReported()
    {
        const string source = """
            using System;

            public sealed class {|qa_quality_cloneable_implementation:Box|} : ICloneable
            {
                public int Value { get; set; }

                public object Clone() => new Box { Value = Value };
            }
            """;

        return CSharpAnalyzerVerifier<CloneableImplementationAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task TypedCopyMethod_IsClean()
    {
        const string source = """
            public sealed class Box
            {
                public int Value { get; set; }

                public Box Copy() => new Box { Value = Value };
            }
            """;

        return CSharpAnalyzerVerifier<CloneableImplementationAnalyzer>.VerifyAsync(source);
    }
}
