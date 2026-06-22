using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class PublicEventHandlerMethodAnalyzerTests
{
    [Fact]
    public Task PublicEventHandler_IsReported()
    {
        const string source = """
            using System;

            public class View
            {
                public void {|qa_quality_public_event_handler_method:OnSaved|}(object sender, EventArgs e)
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<PublicEventHandlerMethodAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PrivateEventHandler_IsClean()
    {
        const string source = """
            using System;

            public class View
            {
                private void OnSaved(object sender, EventArgs e)
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<PublicEventHandlerMethodAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PublicNonHandlerMethod_IsClean()
    {
        const string source = """
            public class View
            {
                public void Refresh(object source, int count)
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<PublicEventHandlerMethodAnalyzer>.VerifyAsync(source);
    }
}
