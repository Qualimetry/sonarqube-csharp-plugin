using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class AttributeTypeShouldBeSealedAnalyzerTests
{
    [Fact]
    public Task UnsealedAttributeType_IsReported()
    {
        const string source = """
            using System;

            public class {|qa_quality_attribute_type_should_be_sealed:AuditAttribute|} : Attribute
            {
                public string Reason { get; set; }
            }
            """;

        return CSharpAnalyzerVerifier<AttributeTypeShouldBeSealedAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task SealedAttributeType_IsClean()
    {
        const string source = """
            using System;

            public sealed class AuditAttribute : Attribute
            {
                public string Reason { get; set; }
            }
            """;

        return CSharpAnalyzerVerifier<AttributeTypeShouldBeSealedAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task AbstractAttributeBase_IsClean()
    {
        const string source = """
            using System;

            public abstract class PolicyAttribute : Attribute
            {
            }
            """;

        return CSharpAnalyzerVerifier<AttributeTypeShouldBeSealedAnalyzer>.VerifyAsync(source);
    }
}
