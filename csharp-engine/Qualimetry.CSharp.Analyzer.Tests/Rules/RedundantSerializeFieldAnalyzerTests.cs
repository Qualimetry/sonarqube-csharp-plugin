using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Unity;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class RedundantSerializeFieldAnalyzerTests
{
    [Fact]
    public Task SerializeFieldOnPublicUnityField_IsReported()
    {
        const string source = """
            using UnityEngine;

            public class Player : MonoBehaviour
            {
                [{|qa_unity_redundant_serialize_field:SerializeField|}]
                public int health;
            }

            namespace UnityEngine
            {
                public class MonoBehaviour { }
                public class SerializeField : System.Attribute { }
            }
            """;

        return CSharpAnalyzerVerifier<RedundantSerializeFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task SerializeFieldOnLookAlikeType_IsClean()
    {
        const string source = """
            public class SerializeField : System.Attribute { }

            public class Settings
            {
                [SerializeField]
                public int level;
            }
            """;

        return CSharpAnalyzerVerifier<RedundantSerializeFieldAnalyzer>.VerifyAsync(source);
    }
}
