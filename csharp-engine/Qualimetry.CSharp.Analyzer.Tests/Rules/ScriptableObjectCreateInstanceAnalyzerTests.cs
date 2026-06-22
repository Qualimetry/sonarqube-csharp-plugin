using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Unity;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ScriptableObjectCreateInstanceAnalyzerTests
{
    [Fact]
    public Task NewScriptableObjectDerivedType_IsReported()
    {
        const string source = """
            using UnityEngine;

            public class WeaponData : ScriptableObject { }

            public class Factory
            {
                WeaponData Make() => new {|qa_unity_scriptable_object_create_instance:WeaponData|}();
            }

            namespace UnityEngine
            {
                public class ScriptableObject { }
            }
            """;

        return CSharpAnalyzerVerifier<ScriptableObjectCreateInstanceAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NewLookAlikeScriptableObjectType_IsClean()
    {
        const string source = """
            public class ScriptableObject { }

            public class WeaponData : ScriptableObject { }

            public class Factory
            {
                WeaponData Make() => new WeaponData();
            }
            """;

        return CSharpAnalyzerVerifier<ScriptableObjectCreateInstanceAnalyzer>.VerifyAsync(source);
    }
}
