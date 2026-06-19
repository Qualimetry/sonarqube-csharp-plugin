using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class PublicEventHandlerExposedAnalyzerTests
{
    [Fact]
    public Task PublicEventHandler_IsReported()
    {
        const string source = """
            using System;

            public class Uploader
            {
                public void {|qa_quality_public_event_handler_exposed:OnProgressChanged|}(object sender, EventArgs e)
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<PublicEventHandlerExposedAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PrivateEventHandler_IsClean()
    {
        const string source = """
            using System;

            public class Uploader
            {
                private void OnProgressChanged(object sender, EventArgs e)
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<PublicEventHandlerExposedAnalyzer>.VerifyAsync(source);
    }
}
