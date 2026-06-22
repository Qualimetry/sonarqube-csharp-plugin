using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Unity;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class NonGenericGetComponentAnalyzerTests
{
    [Fact]
    public Task NonGenericGetComponentOnComponent_IsReported()
    {
        const string source = """
            using System;
            using UnityEngine;

            public class Mover : MonoBehaviour
            {
                void Configure()
                {
                    var component = {|qa_unity_non_generic_get_component:GetComponent|}(typeof(Component));
                }
            }

            namespace UnityEngine
            {
                public class Component { public Component GetComponent(Type type) => null; }
                public class MonoBehaviour : Component { }
            }
            """;

        return CSharpAnalyzerVerifier<NonGenericGetComponentAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task GetComponentOnLookAlikeType_IsClean()
    {
        const string source = """
            using System;

            public class Toolbox
            {
                object GetComponent(Type type) => null;

                void Configure()
                {
                    var component = GetComponent(typeof(string));
                }
            }
            """;

        return CSharpAnalyzerVerifier<NonGenericGetComponentAnalyzer>.VerifyAsync(source);
    }
}
