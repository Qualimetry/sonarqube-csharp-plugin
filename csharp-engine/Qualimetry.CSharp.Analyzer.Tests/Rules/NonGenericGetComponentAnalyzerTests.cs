using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Unity;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class NonGenericGetComponentAnalyzerTests
{
    private const string Stubs = """
        using System;

        public class Component
        {
            public object GetComponent(Type type) => null;
            public T GetComponent<T>() => default;
        }

        public class MonoBehaviour : Component { }

        public class Rigidbody { }
        """;

    [Fact]
    public Task GetComponentWithTypeArgument_IsReported()
    {
        const string source = Stubs + """

            public class Mover : MonoBehaviour
            {
                void Awake()
                {
                    {|qa_unity_non_generic_get_component:GetComponent|}(typeof(Rigidbody));
                }
            }
            """;

        return CSharpAnalyzerVerifier<NonGenericGetComponentAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task GenericGetComponent_IsClean()
    {
        const string source = Stubs + """

            public class Mover : MonoBehaviour
            {
                void Awake()
                {
                    GetComponent<Rigidbody>();
                }
            }
            """;

        return CSharpAnalyzerVerifier<NonGenericGetComponentAnalyzer>.VerifyAsync(source);
    }
}
