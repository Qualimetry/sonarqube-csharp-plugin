using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Naming;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class MethodNameUpperCaseAnalyzerTests
{
    [Fact]
    public Task LowercaseMethodName_IsReported()
    {
        const string source = """
            public class Report
            {
                public void {|qa_naming_method_name_upper_case:generate|}()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<MethodNameUpperCaseAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task OverrideMethodWithLowercaseName_IsClean()
    {
        const string source = """
            public class Widget
            {
                public virtual void {|qa_naming_method_name_upper_case:render|}()
                {
                }
            }

            public sealed class Button : Widget
            {
                public override void render()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<MethodNameUpperCaseAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task UppercaseMethodName_IsClean()
    {
        const string source = """
            public class Report
            {
                public void Generate()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<MethodNameUpperCaseAnalyzer>.VerifyAsync(source);
    }
}
