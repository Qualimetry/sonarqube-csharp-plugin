using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class RedundantLambdaMethodGroupAnalyzerTests
{
    [Fact]
    public Task ForwardingLambda_IsReported()
    {
        const string source = """
            using System.Collections.Generic;
            using System.Linq;

            public class C
            {
                public IEnumerable<string> M(IEnumerable<int> numbers)
                {
                    return numbers.Select({|qa_style_redundant_lambda_method_group:n => Format(n)|});
                }

                private static string Format(int value) => value.ToString();
            }
            """;

        return CSharpAnalyzerVerifier<RedundantLambdaMethodGroupAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task MethodGroup_IsClean()
    {
        const string source = """
            using System.Collections.Generic;
            using System.Linq;

            public class C
            {
                public IEnumerable<string> M(IEnumerable<int> numbers)
                {
                    return numbers.Select(Format);
                }

                private static string Format(int value) => value.ToString();
            }
            """;

        return CSharpAnalyzerVerifier<RedundantLambdaMethodGroupAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ExpressionTreeLambda_IsClean()
    {
        const string source = """
            using System;
            using System.Linq.Expressions;

            public class C
            {
                public Expression<Func<int, int>> M()
                {
                    return x => Identity(x);
                }

                private static int Identity(int value) => value;
            }
            """;

        return CSharpAnalyzerVerifier<RedundantLambdaMethodGroupAnalyzer>.VerifyAsync(source);
    }
}
