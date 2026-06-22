using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class SingletonPatternAnalyzerTests
{
    [Fact]
    public Task ClassicSingleton_IsReported()
    {
        const string source = """
            public sealed class {|qa_quality_singleton_pattern:ConfigStore|}
            {
                private static readonly ConfigStore _instance = new ConfigStore();

                private ConfigStore()
                {
                }

                public static ConfigStore Instance => _instance;
            }
            """;

        return CSharpAnalyzerVerifier<SingletonPatternAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task OrdinaryClass_IsClean()
    {
        const string source = """
            public sealed class ConfigStore
            {
                public ConfigStore()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<SingletonPatternAnalyzer>.VerifyAsync(source);
    }
}
