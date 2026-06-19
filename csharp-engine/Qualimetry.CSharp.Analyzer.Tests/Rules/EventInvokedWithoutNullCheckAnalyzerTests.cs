using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class EventInvokedWithoutNullCheckAnalyzerTests
{
    [Fact]
    public Task DirectEventRaise_IsReported()
    {
        const string source = """
            using System;

            public class Button
            {
                public event EventHandler Changed;

                protected void OnChanged()
                {
                    {|qa_quality_event_invoked_without_null_check:Changed|}(this, EventArgs.Empty);
                }
            }
            """;

        return CSharpAnalyzerVerifier<EventInvokedWithoutNullCheckAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NullConditionalRaise_IsClean()
    {
        const string source = """
            using System;

            public class Button
            {
                public event EventHandler Changed;

                protected void OnChanged()
                {
                    Changed?.Invoke(this, EventArgs.Empty);
                }
            }
            """;

        return CSharpAnalyzerVerifier<EventInvokedWithoutNullCheckAnalyzer>.VerifyAsync(source);
    }
}
