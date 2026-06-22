using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Naming;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class TypeNameUpperCaseAnalyzerTests
{
    [Fact]
    public Task LowercaseClassName_IsReported()
    {
        const string source = """
            public class {|qa_naming_type_name_upper_case:widget|}
            {
            }
            """;

        return CSharpAnalyzerVerifier<TypeNameUpperCaseAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task LowercaseEnumName_IsReported()
    {
        const string source = """
            public enum {|qa_naming_type_name_upper_case:colour|}
            {
                Red,
            }
            """;

        return CSharpAnalyzerVerifier<TypeNameUpperCaseAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task UppercaseTypeName_IsClean()
    {
        const string source = """
            public class Widget
            {
            }
            """;

        return CSharpAnalyzerVerifier<TypeNameUpperCaseAnalyzer>.VerifyAsync(source);
    }
}
