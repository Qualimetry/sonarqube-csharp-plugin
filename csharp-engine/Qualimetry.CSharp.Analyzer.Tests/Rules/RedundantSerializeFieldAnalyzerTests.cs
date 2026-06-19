using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Unity;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class RedundantSerializeFieldAnalyzerTests
{
    private const string Stubs = """
        using System;

        public sealed class SerializeFieldAttribute : Attribute { }

        public class MonoBehaviour { }
        """;

    [Fact]
    public Task SerializeFieldOnPublicField_IsReported()
    {
        const string source = Stubs + """

            public class PlayerStats : MonoBehaviour
            {
                [{|qa_unity_redundant_serialize_field:SerializeField|}]
                public int health;
            }
            """;

        return CSharpAnalyzerVerifier<RedundantSerializeFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task SerializeFieldOnPrivateField_IsClean()
    {
        const string source = Stubs + """

            public class PlayerStats : MonoBehaviour
            {
                [SerializeField]
                protected int health;
            }
            """;

        return CSharpAnalyzerVerifier<RedundantSerializeFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PublicFieldWithoutAttribute_IsClean()
    {
        const string source = Stubs + """

            public class PlayerStats : MonoBehaviour
            {
                public int health;
            }
            """;

        return CSharpAnalyzerVerifier<RedundantSerializeFieldAnalyzer>.VerifyAsync(source);
    }
}
