using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class AttributeMissingUsageAnalyzerTests
{
    [Fact]
    public Task AttributeWithoutUsage_IsReported()
    {
        const string source = """
            using System;

            public sealed class {|qa_quality_attribute_missing_usage:AuditAttribute|} : Attribute
            {
            }
            """;

        return CSharpAnalyzerVerifier<AttributeMissingUsageAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task AttributeWithUsage_IsClean()
    {
        const string source = """
            using System;

            [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
            public sealed class AuditAttribute : Attribute
            {
            }
            """;

        return CSharpAnalyzerVerifier<AttributeMissingUsageAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NonAttributeType_IsClean()
    {
        const string source = """
            public sealed class Audit
            {
            }
            """;

        return CSharpAnalyzerVerifier<AttributeMissingUsageAnalyzer>.VerifyAsync(source);
    }
}
