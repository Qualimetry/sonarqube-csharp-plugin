using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Unity;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class FixedDeltaTimeInUpdateAnalyzerTests
{
    [Fact]
    public Task FixedDeltaTimeInUpdate_IsReported()
    {
        const string source = """
            using UnityEngine;

            public class Spinner : MonoBehaviour
            {
                void Update()
                {
                    var step = {|qa_unity_fixed_delta_time_in_update:Time.fixedDeltaTime|};
                }
            }

            namespace UnityEngine
            {
                public class MonoBehaviour { }
                public static class Time { public static float fixedDeltaTime; }
            }
            """;

        return CSharpAnalyzerVerifier<FixedDeltaTimeInUpdateAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task FixedDeltaTimeOnLookAlikeType_IsClean()
    {
        const string source = """
            public static class Time { public static float fixedDeltaTime; }

            public class Stopwatch
            {
                void Update()
                {
                    var step = Time.fixedDeltaTime;
                }
            }
            """;

        return CSharpAnalyzerVerifier<FixedDeltaTimeInUpdateAnalyzer>.VerifyAsync(source);
    }
}
