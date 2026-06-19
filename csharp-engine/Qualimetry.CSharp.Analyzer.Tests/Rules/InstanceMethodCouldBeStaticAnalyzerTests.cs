using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class InstanceMethodCouldBeStaticAnalyzerTests
{
    [Fact]
    public Task PrivateMethodWithoutInstanceState_IsReported()
    {
        const string source = """
            public class Geometry
            {
                private int {|qa_quality_instance_method_could_be_static:Area|}(int width, int height)
                {
                    return width * height;
                }
            }
            """;

        return CSharpAnalyzerVerifier<InstanceMethodCouldBeStaticAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task StaticMethod_IsClean()
    {
        const string source = """
            public class Geometry
            {
                private static int Area(int width, int height)
                {
                    return width * height;
                }
            }
            """;

        return CSharpAnalyzerVerifier<InstanceMethodCouldBeStaticAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task MethodUsingInstanceField_IsClean()
    {
        const string source = """
            public class Geometry
            {
                private int _scale = 1;

                private int Area(int width, int height)
                {
                    return width * height * _scale;
                }
            }
            """;

        return CSharpAnalyzerVerifier<InstanceMethodCouldBeStaticAnalyzer>.VerifyAsync(source);
    }
}
