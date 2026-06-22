using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Naming;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class AbstractBaseClassSuffixAnalyzerTests
{
    [Fact]
    public Task AbstractClassWithoutSuffix_IsReported()
    {
        const string source = """
            public abstract class {|qa_naming_abstract_base_class_suffix:Repository|}
            {
            }
            """;

        return CSharpAnalyzerVerifier<AbstractBaseClassSuffixAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task AbstractClassWithSuffix_IsClean()
    {
        const string source = """
            public abstract class RepositoryBase
            {
            }
            """;

        return CSharpAnalyzerVerifier<AbstractBaseClassSuffixAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ConcreteClassWithoutSuffix_IsClean()
    {
        const string source = """
            public class Repository
            {
            }
            """;

        return CSharpAnalyzerVerifier<AbstractBaseClassSuffixAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ConfiguredSuffix_OverridesDefault()
    {
        const string source = """
            public abstract class {|qa_naming_abstract_base_class_suffix:RepositoryBase|}
            {
            }
            """;

        return CSharpAnalyzerVerifier<AbstractBaseClassSuffixAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_naming_abstract_base_class_suffix.suffix", "Component"));
    }
}
