using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class NonPrivateMethodCouldBeStaticAnalyzerTests
{
    [Fact]
    public Task PublicMethodWithoutInstanceState_IsReported()
    {
        const string source = """
            public class MathHelper
            {
                public int {|qa_quality_non_private_method_could_be_static:Add|}(int a, int b)
                {
                    return a + b;
                }
            }
            """;

        return CSharpAnalyzerVerifier<NonPrivateMethodCouldBeStaticAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PublicMethodUsingInstanceField_IsClean()
    {
        const string source = """
            public class MathHelper
            {
                private int _offset = 1;

                public int Add(int a, int b)
                {
                    return a + b + _offset;
                }
            }
            """;

        return CSharpAnalyzerVerifier<NonPrivateMethodCouldBeStaticAnalyzer>.VerifyAsync(source);
    }
}
