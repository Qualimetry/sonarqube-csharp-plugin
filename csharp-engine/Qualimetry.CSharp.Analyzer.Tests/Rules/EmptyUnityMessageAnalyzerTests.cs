using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Unity;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class EmptyUnityMessageAnalyzerTests
{
    [Fact]
    public Task EmptyLifecycleMethod_IsReported()
    {
        const string source = """
            public class MonoBehaviour { }

            public class Spinner : MonoBehaviour
            {
                void {|qa_unity_empty_unity_message:Update|}()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<EmptyUnityMessageAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task LifecycleMethodWithBody_IsClean()
    {
        const string source = """
            public class MonoBehaviour { }

            public class Spinner : MonoBehaviour
            {
                int frames;

                void Update()
                {
                    frames++;
                }
            }
            """;

        return CSharpAnalyzerVerifier<EmptyUnityMessageAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task EmptyMethodOutsideBehaviour_IsClean()
    {
        const string source = """
            public class PlainService
            {
                void Update()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<EmptyUnityMessageAnalyzer>.VerifyAsync(source);
    }
}
