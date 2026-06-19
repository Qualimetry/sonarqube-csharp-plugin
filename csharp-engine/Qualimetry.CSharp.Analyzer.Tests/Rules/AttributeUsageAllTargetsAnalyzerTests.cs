using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class AttributeUsageAllTargetsAnalyzerTests
{
    [Fact]
    public Task AttributeUsageAllowingEveryTarget_IsReported()
    {
        const string source = """
            using System;

            [{|qa_quality_attribute_usage_all_targets:AttributeUsage(AttributeTargets.All)|}]
            public sealed class TraceAttribute : Attribute
            {
            }
            """;

        return CSharpAnalyzerVerifier<AttributeUsageAllTargetsAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task AttributeUsageWithSpecificTarget_IsClean()
    {
        const string source = """
            using System;

            [AttributeUsage(AttributeTargets.Method)]
            public sealed class TraceAttribute : Attribute
            {
            }
            """;

        return CSharpAnalyzerVerifier<AttributeUsageAllTargetsAnalyzer>.VerifyAsync(source);
    }
}
