using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class AbstractTypeConstructorAccessibilityAnalyzerTests
{
    [Fact]
    public Task InternalConstructorOnAbstractType_IsReported()
    {
        const string source = """
            public abstract class Repository
            {
                {|qa_quality_abstract_type_constructor_accessibility:internal|} Repository(string name)
                {
                    Name = name;
                }

                public string Name { get; }
            }
            """;

        return CSharpAnalyzerVerifier<AbstractTypeConstructorAccessibilityAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ProtectedConstructorOnAbstractType_IsClean()
    {
        const string source = """
            public abstract class Repository
            {
                protected Repository(string name)
                {
                    Name = name;
                }

                public string Name { get; }
            }
            """;

        return CSharpAnalyzerVerifier<AbstractTypeConstructorAccessibilityAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PublicConstructorOnConcreteType_IsClean()
    {
        const string source = """
            public class Repository
            {
                public Repository(string name)
                {
                    Name = name;
                }

                public string Name { get; }
            }
            """;

        return CSharpAnalyzerVerifier<AbstractTypeConstructorAccessibilityAnalyzer>.VerifyAsync(source);
    }
}
