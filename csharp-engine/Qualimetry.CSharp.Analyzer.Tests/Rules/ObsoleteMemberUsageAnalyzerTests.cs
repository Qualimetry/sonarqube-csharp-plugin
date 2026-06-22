using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ObsoleteMemberUsageAnalyzerTests
{
    [Fact]
    public Task ObsoleteMethodCall_IsReported()
    {
        const string source = """
            public class Report
            {
                [System.Obsolete("Use BuildV2 instead.")]
                public string Build() => "v1";

                public string Render() => {|qa_quality_obsolete_member_usage:Build|}();
            }
            """;

        return CSharpAnalyzerVerifier<ObsoleteMemberUsageAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task SupportedMethodCall_IsClean()
    {
        const string source = """
            public class Report
            {
                [System.Obsolete("Use BuildV2 instead.")]
                public string Build() => "v1";

                public string BuildV2() => "v2";

                public string Render() => BuildV2();
            }
            """;

        return CSharpAnalyzerVerifier<ObsoleteMemberUsageAnalyzer>.VerifyAsync(source);
    }
}
