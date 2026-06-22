using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class MissingConfigureAwaitAnalyzerTests
{
    [Fact]
    public Task AwaitWithoutConfigureAwait_IsReported()
    {
        const string source = """
            using System.Threading.Tasks;

            public class Loader
            {
                public async Task RunAsync()
                {
                    await {|qa_style_missing_configure_await:FetchAsync()|};
                }

                private Task FetchAsync() => Task.CompletedTask;
            }
            """;

        return CSharpAnalyzerVerifier<MissingConfigureAwaitAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task AwaitWithConfigureAwait_IsClean()
    {
        const string source = """
            using System.Threading.Tasks;

            public class Loader
            {
                public async Task RunAsync()
                {
                    await FetchAsync().ConfigureAwait(false);
                }

                private Task FetchAsync() => Task.CompletedTask;
            }
            """;

        return CSharpAnalyzerVerifier<MissingConfigureAwaitAnalyzer>.VerifyAsync(source);
    }
}
