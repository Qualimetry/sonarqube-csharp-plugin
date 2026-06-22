using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Naming;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class AttributeTypeSuffixAnalyzerTests
{
    [Fact]
    public Task AttributeWithoutSuffix_IsReported()
    {
        const string source = """
            using System;

            public sealed class {|qa_naming_attribute_type_suffix:Audited|} : Attribute
            {
            }
            """;

        return CSharpAnalyzerVerifier<AttributeTypeSuffixAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task AttributeWithSuffix_IsClean()
    {
        const string source = """
            using System;

            public sealed class AuditedAttribute : Attribute
            {
            }
            """;

        return CSharpAnalyzerVerifier<AttributeTypeSuffixAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NonAttributeClass_IsClean()
    {
        const string source = """
            public class Audited
            {
            }
            """;

        return CSharpAnalyzerVerifier<AttributeTypeSuffixAnalyzer>.VerifyAsync(source);
    }
}
