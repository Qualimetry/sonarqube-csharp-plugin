using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class AsyncMethodNameSuffixAnalyzerTests
{
    [Fact]
    public Task AsyncMethodWithoutSuffix_IsReported()
    {
        const string source = """
            using System.Threading.Tasks;

            public class C
            {
                public async Task {|qa_style_async_method_name_suffix:Save|}()
                {
                    await Task.Delay(1);
                }
            }
            """;

        return CSharpAnalyzerVerifier<AsyncMethodNameSuffixAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task AsyncMethodWithSuffix_IsClean()
    {
        const string source = """
            using System.Threading.Tasks;

            public class C
            {
                public async Task SaveAsync()
                {
                    await Task.Delay(1);
                }
            }
            """;

        return CSharpAnalyzerVerifier<AsyncMethodNameSuffixAnalyzer>.VerifyAsync(source);
    }
}
