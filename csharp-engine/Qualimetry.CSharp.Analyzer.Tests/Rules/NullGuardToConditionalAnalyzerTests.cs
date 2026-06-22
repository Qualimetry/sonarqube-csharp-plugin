using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class NullGuardToConditionalAnalyzerTests
{
    [Fact]
    public Task NullGuardAroundSingleCall_IsReported()
    {
        const string source = """
            using System;

            public class C
            {
                public void Raise(Action handler)
                {
                    {|qa_style_null_guard_to_conditional:if|} (handler != null)
                    {
                        handler();
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<NullGuardToConditionalAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NullConditionalCall_IsClean()
    {
        const string source = """
            using System;

            public class C
            {
                public void Raise(Action handler)
                {
                    handler?.Invoke();
                }
            }
            """;

        return CSharpAnalyzerVerifier<NullGuardToConditionalAnalyzer>.VerifyAsync(source);
    }
}
