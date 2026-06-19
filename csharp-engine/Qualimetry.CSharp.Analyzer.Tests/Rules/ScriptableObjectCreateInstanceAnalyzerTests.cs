using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Unity;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ScriptableObjectCreateInstanceAnalyzerTests
{
    private const string Stubs = """
        public class ScriptableObject
        {
            public static T CreateInstance<T>() => default;
        }

        public class GameSettings : ScriptableObject { }
        """;

    [Fact]
    public Task NewScriptableObject_IsReported()
    {
        const string source = Stubs + """

            public class SettingsFactory
            {
                public GameSettings Build()
                {
                    return new {|qa_unity_scriptable_object_create_instance:GameSettings|}();
                }
            }
            """;

        return CSharpAnalyzerVerifier<ScriptableObjectCreateInstanceAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task CreateInstance_IsClean()
    {
        const string source = Stubs + """

            public class SettingsFactory
            {
                public GameSettings Build()
                {
                    return ScriptableObject.CreateInstance<GameSettings>();
                }
            }
            """;

        return CSharpAnalyzerVerifier<ScriptableObjectCreateInstanceAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NewPlainObject_IsClean()
    {
        const string source = """
            public class PlainData { }

            public class Factory
            {
                public PlainData Build()
                {
                    return new PlainData();
                }
            }
            """;

        return CSharpAnalyzerVerifier<ScriptableObjectCreateInstanceAnalyzer>.VerifyAsync(source);
    }
}
