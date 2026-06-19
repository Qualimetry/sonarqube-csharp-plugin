using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class DiscardedExceptionInstanceAnalyzerTests
{
    [Fact]
    public Task ConstructedExceptionDiscarded_IsReported()
    {
        const string source = """
            using System;

            public class Validator
            {
                public void Check(int value)
                {
                    if (value < 0)
                    {
                        {|qa_quality_discarded_exception_instance:new ArgumentOutOfRangeException(nameof(value))|};
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<DiscardedExceptionInstanceAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ThrownException_IsClean()
    {
        const string source = """
            using System;

            public class Validator
            {
                public void Check(int value)
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value));
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<DiscardedExceptionInstanceAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ConstructedNonException_IsClean()
    {
        const string source = """
            public class Box
            {
                public Box(int size)
                {
                    Size = size;
                }

                public int Size { get; }
            }

            public class Factory
            {
                public void Build()
                {
                    new Box(4);
                }
            }
            """;

        return CSharpAnalyzerVerifier<DiscardedExceptionInstanceAnalyzer>.VerifyAsync(source);
    }
}
