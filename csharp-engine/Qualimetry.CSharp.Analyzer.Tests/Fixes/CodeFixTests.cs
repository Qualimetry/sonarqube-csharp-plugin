using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.CodeFixes;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Fixes;

public class CodeFixTests
{
    [Fact]
    public Task PreferStringEmpty_ReplacesEmptyLiteral()
    {
        const string source = """
            public class C
            {
                public string M() => {|qa_style_prefer_string_empty:""|};
            }
            """;
        const string @fixed = """
            public class C
            {
                public string M() => string.Empty;
            }
            """;
        return CSharpCodeFixVerifier<PreferStringEmptyAnalyzer, PreferStringEmptyCodeFix>.VerifyAsync(source, @fixed);
    }

    [Fact]
    public Task RedundantDefaultInitializer_IsRemoved()
    {
        const string source = """
            public class C
            {
                private int _count = {|qa_style_redundant_default_field_initializer:0|};
            }
            """;
        const string @fixed = """
            public class C
            {
                private int _count;
            }
            """;
        return CSharpCodeFixVerifier<RedundantDefaultFieldInitializerAnalyzer, RedundantDefaultFieldInitializerCodeFix>.VerifyAsync(source, @fixed);
    }

    [Fact]
    public Task BooleanLiteralComparison_EqualsTrue_IsSimplified()
    {
        const string source = """
            public class C
            {
                public bool M(bool flag) => {|qa_style_boolean_literal_comparison:flag == true|};
            }
            """;
        const string @fixed = """
            public class C
            {
                public bool M(bool flag) => flag;
            }
            """;
        return CSharpCodeFixVerifier<BooleanLiteralComparisonAnalyzer, BooleanLiteralComparisonCodeFix>.VerifyAsync(source, @fixed);
    }

    [Fact]
    public Task BooleanLiteralComparison_EqualsFalse_IsNegated()
    {
        const string source = """
            public class C
            {
                public bool M(bool flag) => {|qa_style_boolean_literal_comparison:flag == false|};
            }
            """;
        const string @fixed = """
            public class C
            {
                public bool M(bool flag) => !flag;
            }
            """;
        return CSharpCodeFixVerifier<BooleanLiteralComparisonAnalyzer, BooleanLiteralComparisonCodeFix>.VerifyAsync(source, @fixed);
    }

    [Fact]
    public Task EmptyInitializer_IsRemoved()
    {
        const string source = """
            using System.Collections.Generic;

            public class C
            {
                public List<int> M() => new List<int> {|qa_style_empty_initializer:{ }|};
            }
            """;
        const string @fixed = """
            using System.Collections.Generic;

            public class C
            {
                public List<int> M() => new List<int>();
            }
            """;
        return CSharpCodeFixVerifier<EmptyInitializerAnalyzer, EmptyInitializerCodeFix>.VerifyAsync(source, @fixed);
    }
}
