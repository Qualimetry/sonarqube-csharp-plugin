using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class CustomDelegateDeclarationAnalyzerTests
{
    [Fact]
    public Task CustomDelegateExpressibleAsFunc_IsReported()
    {
        const string source = """
            public delegate int {|qa_quality_custom_delegate_declaration:Transformer|}(int input);
            """;

        return CSharpAnalyzerVerifier<CustomDelegateDeclarationAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task FuncBackedMember_IsClean()
    {
        const string source = """
            using System;

            public sealed class Pipeline
            {
                public Func<int, int> Step { get; set; }
            }
            """;

        return CSharpAnalyzerVerifier<CustomDelegateDeclarationAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DelegateWithRefParameter_IsClean()
    {
        const string source = """
            public delegate bool TryParser(string text, ref int result);
            """;

        return CSharpAnalyzerVerifier<CustomDelegateDeclarationAnalyzer>.VerifyAsync(source);
    }
}
