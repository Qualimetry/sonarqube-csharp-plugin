using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Unity;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class FixedDeltaTimeInUpdateAnalyzerTests
{
    private const string Stubs = """
        public static class Time
        {
            public static float deltaTime;
            public static float fixedDeltaTime;
        }

        public class MonoBehaviour { }
        """;

    [Fact]
    public Task FixedDeltaTimeInUpdate_IsReported()
    {
        const string source = Stubs + """

            public class Mover : MonoBehaviour
            {
                public float position;
                public float speed;

                void Update()
                {
                    position += speed * {|qa_unity_fixed_delta_time_in_update:Time.fixedDeltaTime|};
                }
            }
            """;

        return CSharpAnalyzerVerifier<FixedDeltaTimeInUpdateAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DeltaTimeInUpdateAndFixedDeltaTimeInFixedUpdate_IsClean()
    {
        const string source = Stubs + """

            public class Mover : MonoBehaviour
            {
                public float position;
                public float speed;

                void Update()
                {
                    position += speed * Time.deltaTime;
                }

                void FixedUpdate()
                {
                    position += speed * Time.fixedDeltaTime;
                }
            }
            """;

        return CSharpAnalyzerVerifier<FixedDeltaTimeInUpdateAnalyzer>.VerifyAsync(source);
    }
}
