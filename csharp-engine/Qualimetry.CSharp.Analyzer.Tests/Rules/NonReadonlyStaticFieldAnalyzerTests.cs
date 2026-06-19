using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class NonReadonlyStaticFieldAnalyzerTests
{
    [Fact]
    public Task MutableStaticField_IsReported()
    {
        const string source = """
            public class Settings
            {
                public static string {|qa_quality_non_readonly_static_field:Environment|} = "Production";
            }
            """;

        return CSharpAnalyzerVerifier<NonReadonlyStaticFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ReadonlyStaticField_IsClean()
    {
        const string source = """
            public class Settings
            {
                public static readonly string Environment = "Production";
            }
            """;

        return CSharpAnalyzerVerifier<NonReadonlyStaticFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task InstanceField_IsClean()
    {
        const string source = """
            public class Settings
            {
                public string Environment = "Production";
            }
            """;

        return CSharpAnalyzerVerifier<NonReadonlyStaticFieldAnalyzer>.VerifyAsync(source);
    }
}
